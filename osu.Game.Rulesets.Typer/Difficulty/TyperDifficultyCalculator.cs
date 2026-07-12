// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Difficulty.Utils;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Typer.Difficulty.Preprocessing;
using osu.Game.Rulesets.Typer.Difficulty.Skills;
using osu.Game.Rulesets.Typer.Mods;
using osu.Game.Utils;

namespace osu.Game.Rulesets.Typer.Difficulty
{
    public class TyperDifficultyCalculator : DifficultyCalculator
    {
        // Skill weights — adapted from Taiko's calibration.
        // Stamina and key-switch (colour analogue) share the mechanical difficulty budget;
        // rhythm is weighted highest as it governs timing precision.
        private const double difficulty_multiplier = 0.084375;
        private const double rhythm_skill_multiplier = 0.770 * difficulty_multiplier;
        private const double keyswitch_skill_multiplier = 0.375 * difficulty_multiplier;
        private const double stamina_skill_multiplier = 0.445 * difficulty_multiplier;

        private double strainLengthBonus;
        private double patternMultiplier;

        private bool isRelax;
        private bool isConvert;

        public override int Version => 20260712;

        public TyperDifficultyCalculator(IRulesetInfo ruleset, IWorkingBeatmap beatmap)
            : base(ruleset, beatmap)
        {
        }

        protected override Skill[] CreateSkills(IBeatmap beatmap, Mod[] mods)
        {
            isConvert = beatmap.BeatmapInfo.Ruleset.OnlineID == 0;
            isRelax = mods.Any(m => m is TyperModRelax);

            return
            [
                new Rhythm(mods),
                new KeySwitch(mods),
                new Stamina(mods, false, isConvert),
                new Stamina(mods, true, isConvert),
            ];
        }

        protected override Mod[] DifficultyAdjustmentMods =>
        [
            new TyperModDoubleTime(),
            new TyperModHalfTime(),
            new TyperModEasy(),
            new TyperModHardRock(),
        ];

        protected override IEnumerable<DifficultyHitObject> CreateDifficultyHitObjects(IBeatmap beatmap, Mod[] mods)
        {
            var difficultyHitObjects = new List<DifficultyHitObject>();

            // One list per key (A–Z) for same-key tracking.
            var sameKeyObjects = new Dictionary<TyperAction, List<TyperDifficultyHitObject>>();
            for (char c = 'A'; c <= 'Z'; c++)
                sameKeyObjects[(TyperAction)c] = new List<TyperDifficultyHitObject>();

            double clockRate = ModUtils.CalculateRateWithMods(mods);

            // Start from the second hit object so that each DifficultyHitObject has a valid predecessor.
            for (int i = 1; i < beatmap.HitObjects.Count; i++)
            {
                var hitObject = beatmap.HitObjects[i];
                var key = ((Objects.TyperHitObject)hitObject).Key;

                difficultyHitObjects.Add(new TyperDifficultyHitObject(
                    hitObject,
                    beatmap.HitObjects[i - 1],
                    clockRate,
                    difficultyHitObjects,
                    sameKeyObjects[key],
                    difficultyHitObjects.Count
                ));
            }

            return difficultyHitObjects;
        }

        protected override DifficultyAttributes CreateDifficultyAttributes(IBeatmap beatmap, Mod[] mods, Skill[] skills)
        {
            if (beatmap.HitObjects.Count == 0)
                return new TyperDifficultyAttributes { Mods = mods };

            var rhythm = skills.OfType<Rhythm>().Single();
            var keySwitch = skills.OfType<KeySwitch>().Single();
            var stamina = skills.OfType<Stamina>().Single(s => !s.SingleKeyStamina);
            var singleKeyStamina = skills.OfType<Stamina>().Single(s => s.SingleKeyStamina);

            double staminaDifficultyValue = stamina.DifficultyValue();

            double rhythmSkill = rhythm.DifficultyValue() * rhythm_skill_multiplier;
            double keySwitchSkill = keySwitch.DifficultyValue() * keyswitch_skill_multiplier;
            double staminaSkill = staminaDifficultyValue * stamina_skill_multiplier;
            double monoStaminaSkill = singleKeyStamina.DifficultyValue() * stamina_skill_multiplier;
            double monoStaminaFactor = staminaSkill == 0 ? 1 : DiffUtils.Pow(monoStaminaSkill / staminaSkill, 5);

            double staminaDifficultStrains = stamina.CountTopWeightedStrains(staminaDifficultyValue);

            // Cross-skill multiplier: maps with both high stamina and high key-switch difficulty
            // are harder than the sum of their parts.
            patternMultiplier = DiffUtils.Pow(staminaSkill * keySwitchSkill, 0.10);

            // Length bonus: longer maps with many difficult strain sections get a small bonus.
            strainLengthBonus = 1 + 0.15 * DiffUtils.ReverseLerp(staminaDifficultStrains, 1000, 1555);

            double combinedRating = combinedDifficultyValue(rhythm, keySwitch, stamina, out double consistencyFactor);
            double starRating = rescale(combinedRating * 1.4);

            // Calculate proportional contribution of each skill to the combined rating.
            double skillRating = starRating / (rhythmSkill + keySwitchSkill + staminaSkill);

            double rhythmDifficulty = rhythmSkill * skillRating;
            double keySwitchDifficulty = keySwitchSkill * skillRating;
            double staminaDifficulty = staminaSkill * skillRating;
            double mechanicalDifficulty = keySwitchDifficulty + staminaDifficulty;

            return new TyperDifficultyAttributes
            {
                StarRating = starRating,
                Mods = mods,
                MechanicalDifficulty = mechanicalDifficulty,
                RhythmDifficulty = rhythmDifficulty,
                KeySwitchDifficulty = keySwitchDifficulty,
                StaminaDifficulty = staminaDifficulty,
                MonoStaminaFactor = monoStaminaFactor,
                StaminaTopStrains = staminaDifficultStrains,
                ConsistencyFactor = consistencyFactor,
                MaxCombo = beatmap.GetMaxCombo(),
            };
        }

        /// <summary>
        /// Returns the combined star rating of the beatmap, calculated using peak strains
        /// from all sections of the map.
        /// </summary>
        /// <remarks>
        /// For each section, the peak strains of all separate skills are combined into a single
        /// peak strain for the section. The resulting partial rating of the beatmap is a weighted
        /// sum of the combined peaks (higher peaks are weighted more).
        /// </remarks>
        private double combinedDifficultyValue(Rhythm rhythm, KeySwitch keySwitch, Stamina stamina, out double consistencyFactor)
        {
            List<double> peaks = combinePeaks(
                rhythm.GetCurrentStrainPeaks().ToList(),
                keySwitch.GetCurrentStrainPeaks().ToList(),
                stamina.GetCurrentStrainPeaks().ToList()
            );

            if (peaks.Count == 0)
            {
                consistencyFactor = 0;
                return 0;
            }

            double difficulty = 0;
            double weight = 1;

            foreach (double strain in peaks.OrderDescending())
            {
                difficulty += strain * weight;
                weight *= 0.9;
            }

            List<double> hitObjectStrainPeaks = combinePeaks(
                rhythm.GetObjectDifficulties(),
                keySwitch.GetObjectDifficulties(),
                stamina.GetObjectDifficulties()
            );

            if (hitObjectStrainPeaks.Count == 0)
            {
                consistencyFactor = 0;
                return 0;
            }

            // The average of the top 5% of strain peaks from hit objects.
            double topAverageHitObjectStrain = hitObjectStrainPeaks.OrderDescending().Take(1 + hitObjectStrainPeaks.Count / 20).Average();

            // Consistency factor: sum of all object strains vs if every object were as hard as the hardest.
            consistencyFactor = hitObjectStrainPeaks.Sum() / (topAverageHitObjectStrain * hitObjectStrainPeaks.Count);

            return difficulty;
        }

        /// <summary>
        /// Combines lists of peak strains from multiple skills into a list of single peak strains for each section.
        /// </summary>
        private List<double> combinePeaks(IReadOnlyList<double> rhythmPeaks, IReadOnlyList<double> keySwitchPeaks, IReadOnlyList<double> staminaPeaks)
        {
            var combinedPeaks = new List<double>();

            for (int i = 0; i < staminaPeaks.Count; i++)
            {
                double rhythmPeak = rhythmPeaks.ElementAtOrDefault(i) * rhythm_skill_multiplier * patternMultiplier;
                double keySwitchPeak = isRelax ? 0 : keySwitchPeaks.ElementAtOrDefault(i) * keyswitch_skill_multiplier;
                double staminaPeak = staminaPeaks.ElementAtOrDefault(i) * stamina_skill_multiplier * strainLengthBonus;
                staminaPeak /= isRelax ? 1.5 : 1.0;

                // Combine: inner norm merges the two mechanical skills (key-switch + stamina),
                // outer norm merges mechanical with rhythm.
                double peak = DiffUtils.Norm(2, DiffUtils.Norm(1.5, keySwitchPeak, staminaPeak), rhythmPeak);

                if (peak > 0)
                    combinedPeaks.Add(peak);
            }

            return combinedPeaks;
        }

        /// <summary>
        /// Applies a final re-scaling of the star rating.
        /// </summary>
        private static double rescale(double sr)
        {
            if (sr < 0)
                return sr;

            return 10.43 * Math.Log(sr / 8 + 1);
        }
    }
}

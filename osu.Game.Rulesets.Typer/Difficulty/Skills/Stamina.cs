// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Difficulty.Utils;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Typer.Difficulty.Evaluators;
using osu.Game.Rulesets.Typer.Difficulty.Preprocessing;

namespace osu.Game.Rulesets.Typer.Difficulty.Skills
{
    /// <summary>
    /// Calculates the stamina coefficient of typer difficulty.
    /// <para>
    /// Two instances are created: a regular stamina skill (with key-streak bonus)
    /// and a single-key stamina skill (dampened for long same-key streaks).
    /// The ratio between them (<see cref="SingleKeyStamina"/>) produces the
    /// <c>MonoStaminaFactor</c> used in performance calculation.
    /// </para>
    /// </summary>
    public class Stamina : StrainSkill
    {
        private const double skill_multiplier = 1.1;
        private const double strain_decay_base = 0.4;

        /// <summary>
        /// Whether this instance measures single-key (mono) stamina.
        /// When true, strain is dampened after the key streak index exceeds 10.
        /// </summary>
        public readonly bool SingleKeyStamina;

        private readonly bool isConvert;
        private double currentStrain;

        /// <summary>
        /// Creates a <see cref="Stamina"/> skill.
        /// </summary>
        /// <param name="mods">Mods for use in skill calculations.</param>
        /// <param name="singleKeyStamina">Whether this instance tracks single-key stamina.</param>
        /// <param name="isConvert">Whether the beatmap is a convert from another ruleset.</param>
        public Stamina(Mod[] mods, bool singleKeyStamina, bool isConvert)
            : base(mods)
        {
            SingleKeyStamina = singleKeyStamina;
            this.isConvert = isConvert;
        }

        private double strainDecay(double ms) => DiffUtils.Pow(strain_decay_base, ms / 1000);

        protected override double StrainValueAt(DifficultyHitObject current)
        {
            currentStrain *= strainDecay(current.DeltaTime);
            double staminaDifficulty = StaminaEvaluator.EvaluateDifficultyOf(current) * skill_multiplier;

            var currentObject = (TyperDifficultyHitObject)current;
            int streakIndex = currentObject.KeyStreakIndex;

            // Key-streak length bonus: longer consecutive same-key sequences within patterns
            // are rewarded (up to a cap) for regular stamina, but not for converts.
            double keyStreakBonus = isConvert ? 1.0 : 1.0 + 0.5 * DiffUtils.ReverseLerp(streakIndex, 5, 20);

            if (!SingleKeyStamina)
                staminaDifficulty *= keyStreakBonus;

            currentStrain += staminaDifficulty;

            // Single-key stamina dampens strain after index > 10 to avoid over-rewarding
            // long mono-key streams (which are common in converts).
            return SingleKeyStamina
                ? DiffUtils.Logistic(-(streakIndex - 10) / 2.0, currentStrain)
                : currentStrain;
        }

        protected override double CalculateInitialStrain(double time, DifficultyHitObject current) =>
            SingleKeyStamina
                ? 0
                : currentStrain * strainDecay(time - current.Previous().StartTime);
    }
}

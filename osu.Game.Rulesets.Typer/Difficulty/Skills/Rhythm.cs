// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Difficulty.Utils;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Typer.Difficulty.Evaluators;

namespace osu.Game.Rulesets.Typer.Difficulty.Skills
{
    /// <summary>
    /// Calculates the rhythm coefficient of typer difficulty.
    /// <para>
    /// Measures the difficulty of processing and executing changes in note timing
    /// (rhythm ratio changes). Penalises rhythm difficulty when stamina difficulty
    /// is low (long intervals), since slow rhythms don't require precise timing.
    /// </para>
    /// </summary>
    public class Rhythm : StrainDecaySkill
    {
        protected override double SkillMultiplier => 1.0;
        protected override double StrainDecayBase => 0.4;

        public Rhythm(Mod[] mods)
            : base(mods)
        {
        }

        protected override double StrainValueOf(DifficultyHitObject current)
        {
            double difficulty = RhythmEvaluator.EvaluateDifficultyOf(current);

            // Penalise rhythm difficulty when stamina difficulty is low (long intervals).
            // This prevents awkward rhythms at slow speeds from being over-rewarded.
            double staminaDifficulty = StaminaEvaluator.EvaluateDifficultyOf(current) - 0.5; // Remove base strain
            difficulty *= DiffUtils.Logistic(staminaDifficulty, 1 / 15.0, 50.0);

            return difficulty;
        }
    }
}

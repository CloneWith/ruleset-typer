// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Typer.Difficulty.Evaluators;

namespace osu.Game.Rulesets.Typer.Difficulty.Skills
{
    /// <summary>
    /// Calculates the key-switching coefficient of typer difficulty.
    /// <para>
    /// This is the Typer analogue of Taiko's Colour skill. It measures the cognitive
    /// and physical cost of switching between different letter keys (A–Z).
    /// </para>
    /// <para>
    /// Strain decays slowly (base = 0.8) because key-switch difficulty only applies
    /// at key-change points, and we want the difficulty to build up even on slower maps.
    /// </para>
    /// </summary>
    public class KeySwitch : StrainDecaySkill
    {
        protected override double SkillMultiplier => 0.12;
        protected override double StrainDecayBase => 0.8;

        public KeySwitch(Mod[] mods)
            : base(mods)
        {
        }

        protected override double StrainValueOf(DifficultyHitObject current)
        {
            return KeySwitchEvaluator.EvaluateDifficultyOf(current);
        }
    }
}

// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Typer.Difficulty.Preprocessing;

namespace osu.Game.Rulesets.Typer.Difficulty.Evaluators
{
    /// <summary>
    /// Evaluates the mechanical stamina required to play the current object.
    /// Models two sources of fatigue:
    /// <list type="bullet">
    /// <item>General hand speed — how fast notes arrive (any key).</item>
    /// <item>Same-key fatigue — how fast the same finger must repeat (same key presses).</item>
    /// </list>
    /// </summary>
    public static class StaminaEvaluator
    {
        /// <summary>
        /// Evaluates the stamina difficulty of the current hit object.
        /// </summary>
        public static double EvaluateDifficultyOf(DifficultyHitObject current)
        {
            var typerCurrent = (TyperDifficultyHitObject)current;
            var previous = current.Previous() as TyperDifficultyHitObject;

            double objectStrain = 0.5; // Base strain for every object
            if (previous == null)
                return objectStrain;

            // General speed bonus: how fast notes are arriving (half weight).
            objectStrain += 0.5 * speedBonus(current.DeltaTime);

            // Same-key fatigue: how quickly the same finger must repeat (full weight).
            // This is the primary stamina factor — rapidly pressing the same key with one finger is tiring.
            var previousSameKey = typerCurrent.PreviousSameKey;
            if (previousSameKey != null)
                objectStrain += speedBonus(typerCurrent.StartTime - previousSameKey.StartTime);

            return objectStrain;
        }

        /// <summary>
        /// Applies a speed bonus that increases as the interval between hits decreases.
        /// </summary>
        /// <param name="interval">The interval in milliseconds between two relevant hits.</param>
        private static double speedBonus(double interval)
        {
            // Cap at 1ms to prevent infinite values from simultaneous notes.
            interval = Math.Max(interval, 1);
            return 20 / interval;
        }
    }
}

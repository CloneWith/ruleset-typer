// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable enable

using System;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Utils;
using osu.Game.Rulesets.Typer.Difficulty.Preprocessing;

namespace osu.Game.Rulesets.Typer.Difficulty.Evaluators
{
    /// <summary>
    /// Evaluates the key-switching difficulty of the current object — the cognitive and
    /// physical cost of finding and pressing the correct letter key.
    /// <para>
    /// This is the Typer analogue of Taiko's Colour evaluator. The key difference is that
    /// Typer has 26 possible keys (A–Z) versus Taiko's 2 colours, making key identification
    /// and finger travel a significant difficulty factor.
    /// </para>
    /// <para>
    /// Difficulty arises from:
    /// <list type="bullet">
    /// <item><b>Key changes</b> — each switch to a different key adds difficulty proportional to the alphabetic distance between keys.</item>
    /// <item><b>Speed</b> — rapid key changes are harder than slow ones.</item>
    /// <item><b>Streak length</b> — the first note of a new key streak is the hardest; subsequent same-key notes are progressively easier (muscle memory).</item>
    /// <item><b>Pattern repetition</b> — repeating the same key-change pattern adds mild difficulty (recognition cost) but is easier than novel patterns.</item>
    /// </list>
    /// </para>
    /// </summary>
    public static class KeySwitchEvaluator
    {
        /// <summary>
        /// Maximum alphabetic distance between two keys (A↔Z = 25).
        /// </summary>
        private const int max_key_distance = 25;

        /// <summary>
        /// Evaluates the key-switching difficulty of the current hit object.
        /// Returns 0 for same-key notes (no key switch required).
        /// </summary>
        public static double EvaluateDifficultyOf(DifficultyHitObject current)
        {
            var typerCurrent = (TyperDifficultyHitObject)current;

            // Only key changes contribute to key-switch difficulty.
            if (!typerCurrent.IsKeyChange)
                return 0.0;

            if (current.Previous() is not TyperDifficultyHitObject previous)
                return 0.0;

            // Key distance: alphabetic distance as a proxy for visual/physical key separation.
            // The game positions keys vertically by alphabetical order, so this is also the visual distance.
            int keyDistance = Math.Abs(typerCurrent.KeyIndex - previous.KeyIndex);
            double distanceDifficulty = DiffUtils.ReverseLerp(keyDistance, 1, max_key_distance);

            // Base difficulty of a key change: 0.5 (fixed cost) + up to 0.5 (distance cost)
            double difficulty = 0.5 + 0.5 * distanceDifficulty;

            // Speed factor: rapid key changes are harder (less time to find the next key).
            difficulty *= 1.0 + speedBonus(current.DeltaTime);

            // Pattern repetition: if the same key-change distance repeats, apply a small bonus
            // (recognition + execution of a known pattern is slightly harder than pure repetition).
            difficulty *= patternMultiplier(typerCurrent, previous);

            return difficulty;
        }

        /// <summary>
        /// Speed bonus that increases as the interval decreases, representing
        /// the reduced time available to identify and move to the next key.
        /// </summary>
        private static double speedBonus(double interval)
        {
            interval = Math.Max(interval, 1);
            return 10 / interval;
        }

        /// <summary>
        /// Evaluates whether the current key change forms a repeating pattern with recent key changes.
        /// Repeating patterns get a small difficulty bonus (pattern recognition cost).
        /// </summary>
        private static double patternMultiplier(TyperDifficultyHitObject current, TyperDifficultyHitObject previous)
        {
            var prevKeyChange = findPreviousKeyChange(previous);
            if (prevKeyChange == null)
                return 1.0;

            // Check if the key distance is the same as the previous key change.
            int currentDistance = Math.Abs(current.KeyIndex - previous.KeyIndex);
            if (prevKeyChange.Previous() is not TyperDifficultyHitObject prevPrev)
                return 1.0;

            int prevDistance = Math.Abs(prevKeyChange.KeyIndex - prevPrev.KeyIndex);

            if (currentDistance == prevDistance)
            {
                // Same distance pattern: mild bonus for recognition cost, but capped.
                return 1.1;
            }

            return 1.0;
        }

        /// <summary>
        /// Finds the most recent <see cref="TyperDifficultyHitObject"/> that was a key change,
        /// starting from the given object and searching backwards.
        /// </summary>
        private static TyperDifficultyHitObject? findPreviousKeyChange(TyperDifficultyHitObject start)
        {
            var current = start.Previous() as TyperDifficultyHitObject;

            while (current != null)
            {
                if (current.IsKeyChange)
                    return current;

                current = current.Previous() as TyperDifficultyHitObject;
            }

            return null;
        }
    }
}

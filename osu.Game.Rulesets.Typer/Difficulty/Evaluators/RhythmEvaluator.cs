// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Utils;
using osu.Game.Rulesets.Typer.Difficulty.Preprocessing;

namespace osu.Game.Rulesets.Typer.Difficulty.Evaluators
{
    /// <summary>
    /// Evaluates the rhythm difficulty of the current object — the difficulty of
    /// processing and executing changes in note interval timing.
    /// <para>
    /// This evaluator analyses the ratio between consecutive note intervals (DeltaTimes).
    /// Ratios close to 1 (but not exactly 1) are the hardest, as they require precise
    /// timing adjustments. Speed-ups (ratio &lt; 1) are generally harder than slow-downs.
    /// </para>
    /// <para>
    /// The implementation adapts Taiko's <c>ratioDifficulty</c> Fourier-series approach,
    /// simplified for Typer's single hit-object type (no drum rolls or swells to filter).
    /// </para>
    /// </summary>
    public static class RhythmEvaluator
    {
        /// <summary>
        /// Evaluates the rhythm difficulty of the current hit object.
        /// Only contributes difficulty at the point of a rhythm change (when <see cref="TyperDifficultyHitObject.RhythmRatio"/> ≠ 1).
        /// </summary>
        public static double EvaluateDifficultyOf(DifficultyHitObject current)
        {
            var typerCurrent = (TyperDifficultyHitObject)current;

            if (current.Previous() is not TyperDifficultyHitObject previous)
                return 0;

            // Only evaluate at rhythm change points.
            // A ratio of 1 means the interval is unchanged — no rhythm difficulty.
            if (typerCurrent.RhythmRatio == 1.0)
                return 0;

            // Core ratio difficulty using the Fourier-series approach.
            double difficulty = ratioDifficulty(typerCurrent.RhythmRatio);

            // Penalise repeated rhythm patterns (too predictable → easier).
            difficulty *= repeatedIntervalPenalty(typerCurrent);

            // Penalise rhythm changes that occur right after a long gap
            // (frequent rhythm changes after long gaps are easier than expected).
            difficulty *= longGapPenalty(previous);

            return difficulty;
        }

        /// <summary>
        /// Calculates the difficulty of a given rhythm ratio using a combination of
        /// periodic penalties (Fourier terms) and bonuses.
        /// <para>
        /// Ratios near 1 (but not exactly 1) receive the highest difficulty values.
        /// Integer ratios (2, 3, 1/2, 1/3) are slightly easier due to being common in music.
        /// </para>
        /// </summary>
        private static double ratioDifficulty(double ratio, int terms = 8)
        {
            double difficulty = 0;

            ratio = double.IsNormal(ratio) ? ratio : 0;

            for (int i = 1; i <= terms; ++i)
            {
                difficulty += termPenalty(ratio, i, 4, 1);
            }

            difficulty += terms / (1 + ratio);

            // Give bonus to near-1 ratios (hard to distinguish but require timing precision).
            difficulty += DiffUtils.BellCurve(ratio, 1, 0.5);

            // Penalise ratios that are VERY near 1 (essentially unchanged rhythm).
            difficulty -= DiffUtils.BellCurve(ratio, 1, 0.3);

            difficulty = Math.Max(difficulty, 0);
            difficulty /= Math.Sqrt(8);

            return difficulty;
        }

        /// <summary>
        /// Multiplier for a given denominator term (Fourier series component).
        /// </summary>
        private static double termPenalty(double ratio, int denominator, double power, double multiplier) =>
            -multiplier * DiffUtils.Pow(Math.Cos(denominator * Math.PI * ratio), power);

        /// <summary>
        /// Penalises rhythm difficulty when the same rhythm ratio has been repeated recently.
        /// Repeated rhythms are more predictable and thus easier to play.
        /// </summary>
        private static double repeatedIntervalPenalty(TyperDifficultyHitObject current, double threshold = 0.1)
        {
            // Collect recent rhythm ratios.
            var intervals = new System.Collections.Generic.List<double>();
            var currentObj = current;

            for (int i = 0; i < 4 && currentObj != null; i++)
            {
                intervals.Add(currentObj.RhythmRatio);
                currentObj = currentObj.Previous() as TyperDifficultyHitObject;
            }

            if (intervals.Count < 3)
                return 1.0;

            // Check if any two recent ratios are similar.
            for (int i = 0; i < intervals.Count; i++)
            {
                for (int j = i + 1; j < intervals.Count; j++)
                {
                    double ratio = intervals[i] / Math.Max(intervals[j], 0.001);
                    if (Math.Abs(1 - ratio) <= threshold)
                        return 0.80;
                }
            }

            return 1.0;
        }

        /// <summary>
        /// Penalises rhythm changes that follow a long gap. Frequent rhythm changes
        /// after long rests are easier than expected because the player has time to prepare.
        /// </summary>
        private static double longGapPenalty(TyperDifficultyHitObject previous)
        {
            double gapInterval = previous.DeltaTime;

            // If the previous note was part of a rhythm group, use that group's interval.
            double rhythmInterval = previous.Previous() is TyperDifficultyHitObject prevPrev && prevPrev.DeltaTime > 0
                ? prevPrev.DeltaTime
                : gapInterval;

            // The ratio of the gap before this rhythm to the rhythm itself.
            double gapRatio = gapInterval / Math.Max(rhythmInterval, 1);

            // The gap ratio normalised to represent if the gap is long.
            double gapFactor = DiffUtils.Logistic(gapRatio, 1.75, 20);

            return 1.0 - 0.75 * gapFactor;
        }
    }
}

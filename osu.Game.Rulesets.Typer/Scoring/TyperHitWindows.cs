// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Typer.Scoring
{
    public class TyperHitWindows : HitWindows
    {
        public static readonly DifficultyRange GREAT_WINDOW_RANGE = new DifficultyRange(50, 35, 20);
        public static readonly DifficultyRange OK_WINDOW_RANGE = new DifficultyRange(120, 80, 50);
        public static readonly DifficultyRange MISS_WINDOW_RANGE = new DifficultyRange(135, 95, 70);

        private double great;
        private double ok;
        private double miss;

        public override bool IsHitResultAllowed(HitResult result) => result switch
        {
            HitResult.Great or HitResult.Ok or HitResult.Miss => true,
            _ => false,
        };

        public override void SetDifficulty(double difficulty)
        {
            great = Math.Floor(IBeatmapDifficultyInfo.DifficultyRange(difficulty, GREAT_WINDOW_RANGE)) - 0.5;
            ok = Math.Floor(IBeatmapDifficultyInfo.DifficultyRange(difficulty, OK_WINDOW_RANGE)) - 0.5;
            miss = Math.Floor(IBeatmapDifficultyInfo.DifficultyRange(difficulty, MISS_WINDOW_RANGE)) - 0.5;
        }

        public override double WindowFor(HitResult result) => result switch
        {
            HitResult.Great => great,
            HitResult.Ok => ok,
            HitResult.Miss => miss,
            _ => throw new ArgumentOutOfRangeException(nameof(result), result, null),
        };
    }
}

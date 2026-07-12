// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Typer.Objects;

namespace osu.Game.Rulesets.Typer.Difficulty.Preprocessing
{
    /// <summary>
    /// Represents a single hit object in typer difficulty calculation.
    /// Extends <see cref="DifficultyHitObject"/> with key-related data used by the
    /// stamina, key-switch, and rhythm evaluators.
    /// </summary>
    public class TyperDifficultyHitObject : DifficultyHitObject
    {
        /// <summary>
        /// The list of all <see cref="TyperDifficultyHitObject"/>s that share the same <see cref="Key"/>
        /// as this object, in temporal order.
        /// </summary>
        private readonly List<TyperDifficultyHitObject> sameKeyObjects;

        /// <summary>
        /// The letter key (A–Z) that the player must press for this object.
        /// </summary>
        public readonly TyperAction Key;

        /// <summary>
        /// Zero-based index of <see cref="Key"/> in the alphabet (0 = A, 25 = Z).
        /// Used as a proxy for visual/physical key distance.
        /// </summary>
        public readonly int KeyIndex;

        /// <summary>
        /// Whether this object requires a different key than the immediately preceding object.
        /// </summary>
        public readonly bool IsKeyChange;

        /// <summary>
        /// The index of this object within the current consecutive streak of same-key objects.
        /// 0 means this is the first note of a new key (a key change).
        /// </summary>
        public readonly int KeyStreakIndex;

        /// <summary>
        /// The position of this object in <see cref="sameKeyObjects"/>.
        /// </summary>
        public readonly int SameKeyIndex;

        /// <summary>
        /// The ratio of this object's <see cref="DifficultyHitObject.DeltaTime"/> to the previous
        /// object's DeltaTime, snapped to the closest <see cref="common_ratios"/>.
        /// A ratio of 1 means the rhythm is unchanged.
        /// </summary>
        public readonly double RhythmRatio;

        /// <summary>
        /// The previous <see cref="TyperDifficultyHitObject"/> that shares the same key, or null if none exists.
        /// </summary>
        public TyperDifficultyHitObject? PreviousSameKey => sameKeyObjects.ElementAtOrDefault(SameKeyIndex - 1);

        /// <summary>
        /// Creates a new <see cref="TyperDifficultyHitObject"/>.
        /// </summary>
        /// <param name="hitObject">The gameplay <see cref="HitObject"/> associated with this difficulty object.</param>
        /// <param name="lastObject">The gameplay <see cref="HitObject"/> preceding <paramref name="hitObject"/>.</param>
        /// <param name="clockRate">The rate of the gameplay clock. Modified by speed-changing mods.</param>
        /// <param name="objects">The list of all <see cref="DifficultyHitObject"/>s in the current beatmap.</param>
        /// <param name="sameKeyObjects">The list of <see cref="TyperDifficultyHitObject"/>s sharing the same key.</param>
        /// <param name="index">The position of this <see cref="DifficultyHitObject"/> in <paramref name="objects"/>.</param>
        public TyperDifficultyHitObject(HitObject hitObject, HitObject lastObject, double clockRate,
                                        List<DifficultyHitObject> objects,
                                        List<TyperDifficultyHitObject> sameKeyObjects,
                                        int index)
            : base(hitObject, lastObject, clockRate, objects, index)
        {
            this.sameKeyObjects = sameKeyObjects;

            Key = ((TyperHitObject)hitObject).Key;
            KeyIndex = (int)Key - (int)TyperAction.ButtonA;

            var previous = Previous() as TyperDifficultyHitObject;
            IsKeyChange = previous == null || previous.Key != Key;
            KeyStreakIndex = IsKeyChange ? 0 : previous!.KeyStreakIndex + 1;

            SameKeyIndex = sameKeyObjects.Count;
            sameKeyObjects.Add(this);

            RhythmRatio = computeRhythmRatio(previous);
        }

        private double computeRhythmRatio(TyperDifficultyHitObject? previous)
        {
            if (previous == null || previous.DeltaTime <= 0)
                return 1;

            double actualRatio = DeltaTime / previous.DeltaTime;
            return common_ratios.MinBy(r => Math.Abs(r - actualRatio));
        }

        /// <summary>
        /// Common rhythm change ratios in music. Based on how each object's interval compares to the previous object.
        /// Ratios closer to 1 (but not exactly 1) are harder to play.
        /// </summary>
        private static readonly double[] common_ratios =
        [
            1.0 / 1,
            2.0 / 1,
            1.0 / 2,
            3.0 / 1,
            1.0 / 3,
            3.0 / 2,
            2.0 / 3,
            5.0 / 4,
            4.0 / 5
        ];
    }
}

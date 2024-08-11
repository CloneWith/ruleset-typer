// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Beatmaps;
using System;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Legacy;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Typer.Judgements;
using osu.Game.Rulesets.Typer.Scoring;

namespace osu.Game.Rulesets.Typer.Objects
{
    public class TyperHitObject : HitObject
    {
        /// <summary>
        /// The radius of hit objects (ie. the radius of the border box).
        /// </summary>
        public const float OBJECT_RADIUS = 80;

        public TyperAction key;
        public override Judgement CreateJudgement() => new TyperJudgement();
        protected override HitWindows CreateHitWindows() => new TyperHitWindows();

        public float Radius => OBJECT_RADIUS * Scale;

        private HitObjectProperty<float> scale = new HitObjectProperty<float>(1);

        public virtual bool NewCombo { get; set; }

        public float Scale
        {
            get => scale.Value;
            set => scale.Value = value;
        }
        protected override void ApplyDefaultsToSelf(ControlPointInfo controlPointInfo, IBeatmapDifficultyInfo difficulty)
        {
            base.ApplyDefaultsToSelf(controlPointInfo, difficulty);

            Scale = LegacyRulesetExtensions.CalculateScaleFromCircleSize(difficulty.CircleSize, true);
        }
    }
}

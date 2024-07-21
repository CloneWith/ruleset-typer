// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Typer.Judgements;
using osu.Game.Rulesets.Typer.Scoring;

namespace osu.Game.Rulesets.Typer.Objects
{
    public class TyperHitObject : HitObject
    {
        public override Judgement CreateJudgement() => new TyperJudgement();
        protected override HitWindows CreateHitWindows() => new TyperHitWindows();
    }
}

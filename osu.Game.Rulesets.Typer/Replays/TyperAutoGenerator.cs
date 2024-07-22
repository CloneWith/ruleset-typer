// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Extensions.ObjectExtensions;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Typer.Objects;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Typer.Beatmaps;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Typer.Replays
{
    public class TyperAutoGenerator : AutoGenerator<TyperReplayFrame>
    {
        public new Beatmap<TyperHitObject> Beatmap => (Beatmap<TyperHitObject>)base.Beatmap;

        public TyperAutoGenerator(IBeatmap beatmap)
            : base(beatmap)
        {
        }

        protected override void GenerateFrames()
        {
            if (Beatmap.HitObjects.Count == 0)
                return;

            bool hitButton = true;

            Frames.Add(new TyperReplayFrame(Beatmap.HitObjects[0].StartTime - 50));

            for (int i = 0; i < Beatmap.HitObjects.Count; i++)
            {
                TyperHitObject h = Beatmap.HitObjects[i];
                double endTime = h.GetEndTime();
                Frames.Add(new TyperReplayFrame(h.StartTime, h.key));
                // todo: add required inputs and extra frames.
                double calculatedDelay = KEY_UP_DELAY;
                Frames.Add(new TyperReplayFrame(endTime + calculatedDelay));

                hitButton = !hitButton;
            }
        }
    }
}

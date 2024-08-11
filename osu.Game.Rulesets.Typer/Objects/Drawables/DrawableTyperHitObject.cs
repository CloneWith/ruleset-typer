// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Utils;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Typer.Objects.Drawables
{
    public partial class DrawableTyperHitObject : DrawableHitObject<TyperHitObject>, IKeyBindingHandler<TyperAction>
    {
        private const double allowable_error = 150;

        private bool validActionPressed;

        private bool wasCorrectKey;

        private double? lastPressHandleTime;

        private readonly Container keyContent;

        private readonly TyperAction keyToHit;

        public DrawableTyperHitObject(TyperHitObject hitObject)
            : base(hitObject)
        {
            Size = new Vector2(hitObject.Radius * 2);

            Origin = Anchor.CentreLeft;
            Anchor = Anchor.CentreLeft;

            keyToHit = hitObject.key;

            AddRangeInternal(new Drawable[]
            {
                keyContent = new Container
                {
                    Masking = true,
                    CornerRadius = 15,
                    CornerExponent = 2.5f,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    EdgeEffect = new EdgeEffectParameters
                    {
                        Radius = 8,
                        Colour = Color4Extensions.FromHex("483D8B"),
                        Type = EdgeEffectType.Shadow,
                    },
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = ColourInfo.GradientVertical(
                                Color4Extensions.FromHex("5F6A6A"),
                                Color4Extensions.FromHex("D8BFD8")
                            )
                        },
                        new OsuSpriteText
                        {
                            Font = OsuFont.Default.With(size: (hitObject.Radius * 2) * 0.6f, weight: FontWeight.Bold),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Text = TyperRuleset.ActionToString(hitObject.key),
                        }
                    }
                },
            });
        }

        protected override void OnFree()
        {
            base.OnFree();

            validActionPressed = false;
        }
        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (!userTriggered)
            {
                if (!HitObject.HitWindows.CanBeHit(timeOffset))
                    ApplyMinResult();
                return;
            }

            var result = HitObject.HitWindows.ResultFor(timeOffset);
            if (result == HitResult.None)
                return;

            if (!validActionPressed)
                ApplyMinResult();
            else
                ApplyResult(result);
        }

        public bool OnPressed(KeyBindingPressEvent<TyperAction> e)
        {
            if (lastPressHandleTime == Time.Current)
                return true;
            if (Judged)
                return false;

            if (e.Repeat) return false;

            validActionPressed = (char)e.Action == (char)keyToHit;

            if (!Result.HasResult)
            {
                wasCorrectKey = validActionPressed;

                if (wasCorrectKey)
                {
                    keyContent.ScaleTo(0.9f, 200, Easing.OutElastic);
                    keyContent.RotateTo(RNG.NextSingle(30) - 15, 500, Easing.OutElastic);
                }

                bool result = UpdateResult(true);

                return wasCorrectKey && result;
            }
            return false;
            // return base.OnPressed(e);
        }

        public void OnReleased(KeyBindingReleaseEvent<TyperAction> e)
        {
            bool correctKey = e.Action == keyToHit;

            if (State.Value != ArmedState.Hit && correctKey)
            {
                keyContent.ScaleTo(1, 300, Easing.OutQuint);
                keyContent.RotateTo(0, 300, Easing.OutQuint);
            }
            // base.OnReleased(e);
        }
        
        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            const float verticality = 80000;
                
            Y = ((char)keyToHit - 'A') / 26f * verticality - (verticality * 0.5f);
            this.MoveToY(0, InitialLifetimeOffset, Easing.OutElasticHalf);
        }

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            const double duration = 800;

            switch (state)
            {
                case ArmedState.Idle:
                    validActionPressed = false;
                    break;

                case ArmedState.Hit:
                    keyContent.ScaleTo(5f, duration, Easing.OutQuint);

                    this.FadeOut(duration, Easing.OutQuint).Expire();
                    break;

                case ArmedState.Miss:
                    keyContent.FadeColour(Color4.Red, 100);
                    keyContent.MoveToY(100, 1000, Easing.In);
                    keyContent.RotateTo(RNG.NextSingle(30) - 15, 1000, Easing.In);

                    this.FadeOut(500, Easing.InQuint).Expire();
                    break;
            }
        }
    }
}

// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Localisation;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Typer.Objects.Drawables;

namespace osu.Game.Rulesets.Typer.Mods
{
    public class TyperModRelax : ModRelax, IApplicableToDrawableHitObject
    {
        public override LocalisableString Description => @"No need to remember which key is correct anymore!";

        public void ApplyToDrawableHitObject(DrawableHitObject drawable)
        {
            var allActions = Enum.GetValues<TyperAction>();

            switch (drawable)
            {
                case DrawableTyperHitObject typerHitObject:
                    typerHitObject.AllowAnyKey = true;
                    typerHitObject.HitActions = allActions;
                    break;
            }
        }
    }
}

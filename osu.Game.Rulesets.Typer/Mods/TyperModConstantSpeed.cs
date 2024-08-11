// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Configuration;
using osu.Game.Rulesets.Typer.Objects;
using osu.Game.Rulesets.Typer.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Typer.Mods
{
    public class TyperModConstantSpeed : Mod, IApplicableToDrawableRuleset<TyperHitObject>
    {
        public override string Name => "Constant Speed";
        public override string Acronym => "CS";
        public override double ScoreMultiplier => 0.9;
        public override LocalisableString Description => "No more tricky speed changes!";
        public override IconUsage? Icon => FontAwesome.Solid.Equals;
        public override ModType Type => ModType.Conversion;

        public void ApplyToDrawableRuleset(DrawableRuleset<TyperHitObject> drawableRuleset)
        {
            var typerRuleset = (DrawableTyperRuleset)drawableRuleset;
            typerRuleset.VisualisationMethod = ScrollVisualisationMethod.Constant;
        }
    }
}

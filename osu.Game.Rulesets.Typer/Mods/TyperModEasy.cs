// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Localisation;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Typer.Mods
{
    public class TyperModEasy : ModEasyWithExtraLives
    {
        public override LocalisableString Description => @"Larger objects and font, more forgiving HP drain, less accuracy required, and three lives!";
    }
}

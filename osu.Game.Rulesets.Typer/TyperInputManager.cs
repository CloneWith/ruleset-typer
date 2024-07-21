// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.ComponentModel;
using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.UI;
using osuTK.Input;

namespace osu.Game.Rulesets.Typer
{
    public partial class TyperInputManager : RulesetInputManager<TyperAction>
    {
        public TyperInputManager(RulesetInfo ruleset)
            : base(ruleset, 0, SimultaneousBindingMode.Unique)
        {
        }
    }

    public enum TyperAction
    {
        [Description("A")] ButtonA,
        [Description("B")] ButtonB,
        [Description("C")] ButtonC,
        [Description("D")] ButtonD,
        [Description("E")] ButtonE,
        [Description("F")] ButtonF,
        [Description("G")] ButtonG,
        [Description("H")] ButtonH,
        [Description("I")] ButtonI,
        [Description("J")] ButtonJ,
        [Description("K")] ButtonK,
        [Description("L")] ButtonL,
        [Description("M")] ButtonM,
        [Description("N")] ButtonN,
        [Description("O")] ButtonO,
        [Description("P")] ButtonP,
        [Description("Q")] ButtonQ,
        [Description("R")] ButtonR,
        [Description("S")] ButtonS,
        [Description("T")] ButtonT,
        [Description("U")] ButtonU,
        [Description("V")] ButtonV,
        [Description("W")] ButtonW,
        [Description("X")] ButtonX,
        [Description("Y")] ButtonY,
        [Description("Z")] ButtonZ,
    }
}

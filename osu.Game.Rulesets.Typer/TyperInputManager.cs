// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.ComponentModel;
using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.UI;

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
        [Description("A")] ButtonA = 'A',
        [Description("B")] ButtonB = 'B',
        [Description("C")] ButtonC = 'C',
        [Description("D")] ButtonD = 'D',
        [Description("E")] ButtonE = 'E',
        [Description("F")] ButtonF = 'F',
        [Description("G")] ButtonG = 'G',
        [Description("H")] ButtonH = 'H',
        [Description("I")] ButtonI = 'I',
        [Description("J")] ButtonJ = 'J',
        [Description("K")] ButtonK = 'K',
        [Description("L")] ButtonL = 'L',
        [Description("M")] ButtonM = 'M',
        [Description("N")] ButtonN = 'N',
        [Description("O")] ButtonO = 'O',
        [Description("P")] ButtonP = 'P',
        [Description("Q")] ButtonQ = 'Q',
        [Description("R")] ButtonR = 'R',
        [Description("S")] ButtonS = 'S',
        [Description("T")] ButtonT = 'T',
        [Description("U")] ButtonU = 'U',
        [Description("V")] ButtonV = 'V',
        [Description("W")] ButtonW = 'W',
        [Description("X")] ButtonX = 'X',
        [Description("Y")] ButtonY = 'Y',
        [Description("Z")] ButtonZ = 'Z',
    }
}

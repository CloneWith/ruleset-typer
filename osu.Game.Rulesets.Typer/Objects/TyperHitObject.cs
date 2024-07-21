// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Typer.Judgements;
using osu.Game.Rulesets.Typer.Scoring;

namespace osu.Game.Rulesets.Typer.Objects
{
    public class TyperHitObject : HitObject
    {
        public char key = ' ';
        public override Judgement CreateJudgement() => new TyperJudgement();
        protected override HitWindows CreateHitWindows() => new TyperHitWindows();
        public TyperAction Translate()
        {
            switch (key)
            {
                case 'A': return TyperAction.ButtonA;
                case 'B': return TyperAction.ButtonB;
                case 'C': return TyperAction.ButtonC;
                case 'D': return TyperAction.ButtonD;
                case 'E': return TyperAction.ButtonE;
                case 'F': return TyperAction.ButtonF;
                case 'G': return TyperAction.ButtonG;
                case 'H': return TyperAction.ButtonH;
                case 'i': return TyperAction.ButtonI;
                case 'J': return TyperAction.ButtonJ;
                case 'K': return TyperAction.ButtonK;
                case 'L': return TyperAction.ButtonL;
                case 'M': return TyperAction.ButtonM;
                case 'N': return TyperAction.ButtonN;
                case 'O': return TyperAction.ButtonO;
                case 'P': return TyperAction.ButtonP;
                case 'Q': return TyperAction.ButtonQ;
                case 'R': return TyperAction.ButtonR;
                case 'S': return TyperAction.ButtonS;
                case 'T': return TyperAction.ButtonT;
                case 'U': return TyperAction.ButtonU;
                case 'V': return TyperAction.ButtonV;
                case 'W': return TyperAction.ButtonW;
                case 'X': return TyperAction.ButtonX;
                case 'Y': return TyperAction.ButtonY;
                case 'Z': return TyperAction.ButtonZ;
                default: { throw new ArgumentException($"Invaild character \'{key}\'."); }
            }
        }
    }
}

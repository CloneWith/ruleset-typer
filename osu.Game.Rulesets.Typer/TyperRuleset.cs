// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Mods;
using osu.Game.Rulesets.Typer.Beatmaps;
using osu.Game.Rulesets.Typer.Mods;
using osu.Game.Rulesets.Typer.UI;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Typer
{
    public class TyperRuleset : Ruleset
    {
        public override string Description => "a very typer ruleset";

        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod> mods = null) => new DrawableTyperRuleset(this, beatmap, mods);

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) => new TyperBeatmapConverter(beatmap, this);

        public override DifficultyCalculator CreateDifficultyCalculator(IWorkingBeatmap beatmap) => new TyperDifficultyCalculator(RulesetInfo, beatmap);

        public override IEnumerable<Mod> GetModsFor(ModType type)
        {
            switch (type)
            {
                case ModType.DifficultyReduction:
                    return new[] { new TyperModNoFail() };

                case ModType.Automation:
                    return new[] { new TyperModAutoplay() };

                default:
                    return Array.Empty<Mod>();
            }
        }

        public override string ShortName => "typer";

        public override IEnumerable<KeyBinding> GetDefaultKeyBindings(int variant = 0) => new[]
        {
            new KeyBinding(InputKey.A, TyperAction.ButtonA),
            new KeyBinding(InputKey.B, TyperAction.ButtonB),
            new KeyBinding(InputKey.C, TyperAction.ButtonC),
            new KeyBinding(InputKey.D, TyperAction.ButtonD),
            new KeyBinding(InputKey.E, TyperAction.ButtonE),
            new KeyBinding(InputKey.F, TyperAction.ButtonF),
            new KeyBinding(InputKey.G, TyperAction.ButtonG),
            new KeyBinding(InputKey.H, TyperAction.ButtonH),
            new KeyBinding(InputKey.I, TyperAction.ButtonI),
            new KeyBinding(InputKey.J, TyperAction.ButtonJ),
            new KeyBinding(InputKey.K, TyperAction.ButtonK),
            new KeyBinding(InputKey.L, TyperAction.ButtonL),
            new KeyBinding(InputKey.M, TyperAction.ButtonM),
            new KeyBinding(InputKey.N, TyperAction.ButtonN),
            new KeyBinding(InputKey.O, TyperAction.ButtonO),
            new KeyBinding(InputKey.P, TyperAction.ButtonP),
            new KeyBinding(InputKey.Q, TyperAction.ButtonQ),
            new KeyBinding(InputKey.R, TyperAction.ButtonR),
            new KeyBinding(InputKey.S, TyperAction.ButtonS),
            new KeyBinding(InputKey.T, TyperAction.ButtonT),
            new KeyBinding(InputKey.U, TyperAction.ButtonU),
            new KeyBinding(InputKey.V, TyperAction.ButtonV),
            new KeyBinding(InputKey.W, TyperAction.ButtonW),
            new KeyBinding(InputKey.X, TyperAction.ButtonX),
            new KeyBinding(InputKey.Y, TyperAction.ButtonY),
            new KeyBinding(InputKey.Z, TyperAction.ButtonZ),
        };
        public static string ActionToString(TyperAction action)
        {
            return ((char)action).ToString();
        }

        public override Drawable CreateIcon() => new SpriteText
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Text = ShortName[0].ToString(),
            Font = OsuFont.Default.With(size: 18),
        };

        // Leave this line intact. It will bake the correct version into the ruleset on each build/release.
        public override string RulesetAPIVersionSupported => CURRENT_RULESET_API_VERSION;
    }
}

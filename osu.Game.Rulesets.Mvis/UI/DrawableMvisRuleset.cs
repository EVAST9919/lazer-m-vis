﻿using System.Collections.Generic;
using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Mvis.Objects;
using osu.Game.Rulesets.Mvis.Objects.Drawables;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Mvis.UI
{
    public class DrawableMvisRuleset : DrawableRuleset<MvisHitObject>
    {
        public DrawableMvisRuleset(Ruleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
            : base(ruleset, beatmap, mods)
        {
        }

        protected override PassThroughInputManager CreateInputManager() => new MvisInputManager(Ruleset.RulesetInfo);

        protected override Playfield CreatePlayfield() => new MvisPlayfield();

        public override DrawableHitObject<MvisHitObject> CreateDrawableRepresentation(MvisHitObject h)
        {
            switch (h)
            {
                case MvisHitObject hitObject:
                    return new DrawableMvisHitObject(hitObject);
            }

            return null;
        }
    }
}

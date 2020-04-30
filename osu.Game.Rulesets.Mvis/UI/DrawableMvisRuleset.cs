// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Input.Handlers;
using osu.Game.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Mvis;
using osu.Game.Rulesets.Mvis.Objects;
using osu.Game.Rulesets.Mvis.Objects.Drawables;
using osu.Game.Rulesets.Mvis.Replays;
using osu.Game.Rulesets.Mvis.UI;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Reza.UI
{
    public class DrawableMvisRuleset : DrawableRuleset<MvisHitObject>
    {
        public DrawableMvisRuleset(Ruleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
            : base(ruleset, beatmap, mods)
        {
        }

        protected override PassThroughInputManager CreateInputManager() => new MvisInputManager(Ruleset.RulesetInfo);

        protected override Playfield CreatePlayfield() => new MvisPlayfield(Beatmap.BeatmapInfo.BaseDifficulty, CreateDrawableRepresentation);

        protected override ReplayRecorder CreateReplayRecorder(Replay replay) => new MvisReplayRecorder();

        protected override ReplayInputHandler CreateReplayInputHandler(Replay replay) => new MvisFramedReplayInputHandler(replay);

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

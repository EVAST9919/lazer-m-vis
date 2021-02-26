using System.Collections.Generic;
using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Input.Handlers;
using osu.Game.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Mvis.Objects;
using osu.Game.Rulesets.Mvis.Objects.Drawables;
using osu.Game.Rulesets.Mvis.Replays;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;
using osu.Game.Scoring;
using osu.Game.Users;

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

        protected override ReplayRecorder CreateReplayRecorder(Score score) => new MvisReplayRecorder();

        protected override ReplayInputHandler CreateReplayInputHandler(Replay replay) => new MvisFramedReplayInputHandler(replay);

        public override bool AllowGameplayOverlays => false;

        public override DrawableHitObject<MvisHitObject> CreateDrawableRepresentation(MvisHitObject h)
        {
            switch (h)
            {
                case MvisHitObject hitObject:
                    return new DrawableMvisHitObject(hitObject);
            }

            return null;
        }

        protected override void LoadComplete()
        {
            SetReplayScore(new Score
            {
                ScoreInfo = new ScoreInfo { User = new User { Username = "mvis" } },
                Replay = new MvisAutoGenerator(Beatmap).Generate(),
            });

            base.LoadComplete();
        }
    }
}

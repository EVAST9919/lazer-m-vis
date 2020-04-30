using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mvis.Objects;
using osu.Game.Rulesets.Mods;
using osu.Game.Scoring;
using osu.Game.Users;
using osu.Game.Rulesets.Mvis.Replays;

namespace osu.Game.Rulesets.Mvis.Mods
{
    public class MvisModAutoplay : ModAutoplay<MvisHitObject>
    {
        public override Score CreateReplayScore(IBeatmap beatmap) => new Score
        {
            ScoreInfo = new ScoreInfo { User = new User { Username = "bosu!" } },
            Replay = new MvisAutoGenerator(beatmap).Generate(),
        };

        public override string Description => "Use if you want to avoid auto-pause while using another window";
    }
}

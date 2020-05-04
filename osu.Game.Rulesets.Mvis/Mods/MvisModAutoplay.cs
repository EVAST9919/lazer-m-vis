using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mvis.Objects;
using osu.Game.Rulesets.Mods;
using osu.Game.Scoring;
using osu.Game.Users;
using osu.Game.Rulesets.Mvis.Replays;
using osu.Game.Screens.Play;

namespace osu.Game.Rulesets.Mvis.Mods
{
    public class MvisModAutoplay : ModAutoplay<MvisHitObject>, IApplicableToHUD, IApplicableToPlayer
    {
        public override Score CreateReplayScore(IBeatmap beatmap) => new Score
        {
            ScoreInfo = new ScoreInfo { User = new User { Username = "bosu!" } },
            Replay = new MvisAutoGenerator(beatmap).Generate(),
        };

        public void ApplyToHUD(HUDOverlay overlay)
        {
            overlay.ShowHud.Value = false;
            overlay.ShowHud.Disabled = true;
        }

        public void ApplyToPlayer(Player player)
        {
            player.BreakOverlay.Hide();
        }

        public override string Description => "Use if you want to avoid auto-pause while using another window";
    }
}

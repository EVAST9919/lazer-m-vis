using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Mvis.Scoring
{
    public class MvisScoreProcessor : ScoreProcessor
    {
        public override HitWindows CreateHitWindows() => new MvisHitWindows();
    }
}

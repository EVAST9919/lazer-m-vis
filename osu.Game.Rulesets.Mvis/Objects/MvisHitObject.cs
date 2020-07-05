using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Mvis.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Mvis.Objects
{
    public class MvisHitObject : HitObject
    {
        protected override HitWindows CreateHitWindows() => HitWindows.Empty;

        public override Judgement CreateJudgement() => new MvisJudgement();

        public double TimePreempt;

        protected override void ApplyDefaultsToSelf(ControlPointInfo controlPointInfo, BeatmapDifficulty difficulty)
        {
            base.ApplyDefaultsToSelf(controlPointInfo, difficulty);
            TimePreempt = 0;
        }
    }
}

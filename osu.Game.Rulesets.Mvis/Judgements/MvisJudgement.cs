using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Mvis.Judgements
{
    public class MvisJudgement : Judgement
    {
        public override HitResult MaxResult => HitResult.Perfect;

        protected override int NumericResultFor(HitResult result) => 300;
    }
}

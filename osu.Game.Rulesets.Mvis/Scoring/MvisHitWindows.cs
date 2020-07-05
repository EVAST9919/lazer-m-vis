using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Mvis.Scoring
{
    public class MvisHitWindows : HitWindows
    {
        public override bool IsHitResultAllowed(HitResult result)
        {
            switch (result)
            {
                case HitResult.Perfect:
                    return true;
            }

            return false;
        }
    }
}

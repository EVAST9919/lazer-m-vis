using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Mvis.Objects.Drawables
{
    public class DrawableMvisHitObject : DrawableHitObject<MvisHitObject>
    {
        public DrawableMvisHitObject(MvisHitObject h)
            : base(h)
        {
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (timeOffset > 0)
                ApplyResult(r => r.Type = HitResult.Perfect);
        }
    }
}

using osu.Game.Rulesets.Objects.Drawables;

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
                ApplyResult(r => r.Type = r.Judgement.MaxResult);
        }
    }
}

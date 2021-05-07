using osu.Framework.Graphics;
using osu.Game.Rulesets.Mvis.UI.Objects.Helpers;

namespace osu.Game.Rulesets.Mvis.UI.Objects
{
    public class Particles : CurrentRateContainer
    {
        private ParticlesDrawable particles;

        public Particles()
        {
            RelativeSizeAxes = Axes.Both;
            Add(particles = new ParticlesDrawable());
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            IsKiai.BindValueChanged(_ => particles.SetRandomDirection());
        }
    }
}

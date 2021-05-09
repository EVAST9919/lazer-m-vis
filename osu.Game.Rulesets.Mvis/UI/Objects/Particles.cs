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
            IsKiai.BindValueChanged(kiai =>
            {
                if (kiai.NewValue)
                {
                    particles.SetRandomDirection();
                }
                else
                    particles.Direction.Value = MoveDirection.Forward;
            });
        }
    }
}

using osu.Framework.Graphics;
using osu.Game.Rulesets.Mvis.UI.Objects.Helpers;

namespace osu.Game.Rulesets.Mvis.UI.Objects
{
    public class Particles : CurrentRateContainer
    {
        public Particles()
        {
            RelativeSizeAxes = Axes.Both;
            Add(new ParticlesDrawable
            {
                Backwards = { BindTarget = IsKiai }
            });
        }
    }
}

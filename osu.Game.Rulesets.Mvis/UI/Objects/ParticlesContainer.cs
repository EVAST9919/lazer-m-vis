using osu.Framework.Graphics;
using osu.Game.Rulesets.Mvis.UI.Objects.Helpers;
using System.Collections.Generic;
using System.Threading;

namespace osu.Game.Rulesets.Mvis.UI.Objects
{
    public abstract class ParticlesContainer : CurrentRateContainer
    {
        /// <summary>
        /// Maximum allowed amount of particles which can be shown at once.
        /// </summary>
        protected virtual int MaxParticlesCount => 350;

        protected ParticlesContainer()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            generateParticles();
        }

        private CancellationTokenSource cancellationToken;

        private void generateParticles()
        {
            var particles = new List<Drawable>();

            for (int i = 0; i < MaxParticlesCount; i++)
                particles.Add(CreateParticle());

            LoadComponentsAsync(particles, AddRange, (cancellationToken = new CancellationTokenSource()).Token);
        }

        protected abstract Drawable CreateParticle();

        protected override void Dispose(bool isDisposing)
        {
            cancellationToken?.Cancel();
            base.Dispose(isDisposing);
        }
    }
}

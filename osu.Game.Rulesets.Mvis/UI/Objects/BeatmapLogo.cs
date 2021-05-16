using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;
using osu.Game.Rulesets.Mvis.UI.Objects.Helpers;
using osu.Game.Screens.Ranking.Expanded.Accuracy;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Mvis.UI.Objects
{
    public class BeatmapLogo : CurrentBeatmapProvider
    {
        public new Color4 Colour
        {
            get => progress.Colour;
            set => progress.Colour = value;
        }

        private Progress progress;

        [BackgroundDependencyLoader]
        private void load()
        {
            Size = new Vector2(350);
            Origin = Anchor.Centre;

            InternalChildren = new Drawable[]
            {
                new UpdateableBeatmapBackground
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                },
                progress = new Progress
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    InnerRadius = 0.03f,
                }
            };
        }

        protected override void Update()
        {
            base.Update();

            var track = Beatmap.Value?.Track;
            progress.Current.Value = (track == null || track.Length == 0) ? 0 : (track.CurrentTime / track.Length);
        }

        private class Progress : SmoothCircularProgress
        {
            private static readonly Vector2 sigma = new Vector2(5);

            private BufferedContainer bufferedContainer => (BufferedContainer)InternalChild;

            public new Color4 Colour
            {
                get => bufferedContainer.Colour;
                set
                {
                    bufferedContainer.Colour = value;
                    bufferedContainer.EffectColour = bufferedContainer.Colour.MultiplyAlpha(4);
                }
            }

            public Progress()
            {
                bufferedContainer.BlurSigma = sigma;
                bufferedContainer.DrawOriginal = true;
                bufferedContainer.EffectBlending = BlendingParameters.Additive;
                bufferedContainer.Padding = new MarginPadding
                {
                    Horizontal = Blur.KernelSize(sigma.X),
                    Vertical = Blur.KernelSize(sigma.Y),
                };
            }
        }
    }
}

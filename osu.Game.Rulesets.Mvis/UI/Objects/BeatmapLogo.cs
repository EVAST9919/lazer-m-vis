using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Rulesets.Mvis.UI.Objects.Helpers;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Mvis.UI.Objects
{
    public class BeatmapLogo : CurrentBeatmapProvider
    {
        private Color4 colour;

        public new Color4 Colour
        {
            get => colour;
            set
            {
                colour = value;
                progress.Colour = value;
            }
        }

        private CircularProgress progress;

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
                (progress = new CircularProgress
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    InnerRadius = 0.02f,
                }).WithEffect(new GlowEffect
                {
                    Strength = 5,
                    PadExtent = true
                }),
            };
        }

        protected override void Update()
        {
            base.Update();

            var track = Beatmap.Value?.Track;
            progress.Current.Value = (track == null || track.Length == 0) ? 0 : (track.CurrentTime / track.Length);
        }
    }
}

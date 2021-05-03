using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Overlays;
using osu.Game.Rulesets.Mvis.UI.Objects;
using osu.Game.Tests.Visual;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Mvis.Tests
{
    public class TestSceneBeatmapLogo : OsuTestScene
    {
        private readonly NowPlayingOverlay nowPlayingOverlay;

        public TestSceneBeatmapLogo()
        {
            AddRange(new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black
                },
                new BeatmapLogo
                {
                    Anchor = Anchor.Centre
                },
                nowPlayingOverlay = new NowPlayingOverlay
                {
                    Origin = Anchor.TopRight,
                    Anchor = Anchor.TopRight,
                }
            });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            nowPlayingOverlay.ToggleVisibility();
        }
    }
}

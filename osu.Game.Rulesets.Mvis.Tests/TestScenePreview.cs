using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Overlays;
using osu.Game.Rulesets.Mvis.UI.Objects;
using osu.Game.Rulesets.Mvis.UI.Objects.Helpers;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Mvis.Tests
{
    public class TestScenePreview : RulesetTestScene
    {
        private readonly NowPlayingOverlay nowPlayingOverlay;

        private readonly Box box;

        public TestScenePreview()
        {
            AddRange(new Drawable[]
            {
                box = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black
                },
                new CurrentRateContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = new ParticlesDrawable()
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
            AddStep("Toggle visibility", nowPlayingOverlay.ToggleVisibility);
            AddSliderStep("Dim", 0, 1, 0.5f, v => box.Alpha = v);
        }
    }
}

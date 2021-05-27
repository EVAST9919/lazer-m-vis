using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Overlays;
using osu.Game.Rulesets.Mvis.UI;
using osu.Game.Tests.Visual;
using osuTK;

namespace osu.Game.Rulesets.Mvis.Tests
{
    public class TestSceneInnerShadowContainer : OsuTestScene
    {
        private readonly OverlayColourProvider colourProvider = new OverlayColourProvider(OverlayColourScheme.Pink);

        private TestContainer testContainer;

        public TestSceneInnerShadowContainer()
        {
            AddRange(new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = colourProvider.Background3
                },
                testContainer = new TestContainer()
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Size = new Vector2(400)
                }
            });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            AddSliderStep("Depth", 0, 100, 10, d => testContainer.Depth.Value = d);
            AddSliderStep("Corner radius", 0, 100, 10, c => testContainer.CornerRadius.Value = c);
        }

        private class TestContainer : InnerShadowContainer
        {
            protected override Container<Drawable> CreateContent() => new Container
            {
                RelativeSizeAxes = Axes.Both,
                Child = new OsuSpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Text = "Test",
                    Font = OsuFont.GetFont(size: 30)
                }
            };
        }
    }
}

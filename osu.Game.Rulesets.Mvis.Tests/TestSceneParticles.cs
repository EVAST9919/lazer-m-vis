using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Mvis.UI.Objects;
using osu.Game.Tests.Visual;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Mvis.Tests
{
    public class TestSceneParticles : OsuTestScene
    {
        protected override Ruleset CreateRuleset() => new MvisRuleset();

        private readonly Particles particles;

        public TestSceneParticles()
        {
            Add(new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(0.5f),
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0.5f,
                        Colour = Color4.Red
                    },
                    particles = new Particles(),
                }
            });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            AddSliderStep("Restart", 1, 30000, 1000, v => particles.Restart(v));
        }
    }
}

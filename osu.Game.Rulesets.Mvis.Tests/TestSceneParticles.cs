using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;
using osu.Game.Rulesets.Mvis.UI.Objects;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Mvis.Tests
{
    public class TestSceneParticles : OsuTestScene
    {
        protected override Ruleset CreateRuleset() => new MvisRuleset();

        private readonly Particles particles;
        private readonly SpriteText countText;

        public TestSceneParticles()
        {
            AddRange(new Drawable[]
            {
                particles = new Particles(),
                countText = new SpriteText
                {
                    Font = OsuFont.GetFont(size: 20)
                }
            });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            AddSliderStep("Restart", 1, 20000, 1000, v => particles.Restart(v));
        }

        protected override void Update()
        {
            base.Update();
            countText.Text = $"Alive: {particles.GetCount()}";
        }
    }
}

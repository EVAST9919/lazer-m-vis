using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics.Containers;
using osu.Game.Rulesets.Mvis.Configuration;
using osu.Game.Rulesets.Mvis.UI.Objects;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Mvis.UI
{
    public class MvisPlayfield : Playfield
    {
        [Resolved(canBeNull: true)]
        private MvisRulesetConfigManager config { get; set; }

        private readonly Bindable<bool> showParticles = new Bindable<bool>(true);

        private Container particlesPlaceholder;

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                HitObjectContainer,
                particlesPlaceholder = new Container
                {
                    RelativeSizeAxes = Axes.Both
                },
                new ParallaxContainer
                {
                    ParallaxAmount = -0.0025f,
                    Child = new BeatmapLogo
                    {
                        Anchor = Anchor.Centre,
                    }
                }
            };

            config?.BindWith(MvisRulesetSetting.ShowParticles, showParticles);
            showParticles.BindValueChanged(onParticlesVisibilityChanged, true);
        }

        private void onParticlesVisibilityChanged(ValueChangedEvent<bool> value)
        {
            if (value.NewValue)
            {
                particlesPlaceholder.Child = new SpaceParticlesContainer();
                return;
            }

            particlesPlaceholder.Clear();
        }
    }
}

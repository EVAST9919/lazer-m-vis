using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Game.Graphics.Containers;
using osu.Game.Rulesets.Mvis.Configuration;
using osu.Game.Rulesets.Mvis.UI.Objects;
using osu.Game.Rulesets.Mvis.UI.Objects.Helpers;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Mvis.UI
{
    public class MvisPlayfield : Playfield
    {
        [Resolved(canBeNull: true)]
        private MvisRulesetConfigManager config { get; set; }

        [Resolved]
        private TextureStore textures { get; set; }

        private readonly Bindable<bool> showParticles = new Bindable<bool>(true);

        private CurrentRateContainer particlesPlaceholder;

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                HitObjectContainer,
                particlesPlaceholder = new CurrentRateContainer
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
                particlesPlaceholder.Child = new Particles(textures.Get("particle"));
                return;
            }

            particlesPlaceholder.Clear();
        }
    }
}

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
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

        private readonly Bindable<bool> showParticles = new Bindable<bool>(true);
        private readonly Bindable<float> xPos = new Bindable<float>(0.5f);
        private readonly Bindable<float> yPos = new Bindable<float>(0.5f);

        private CurrentRateContainer particlesPlaceholder;
        private BeatmapLogo logo;

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
                logo = new BeatmapLogo
                {
                    RelativePositionAxes = Axes.Both
                }
            };

            config?.BindWith(MvisRulesetSetting.ShowParticles, showParticles);
            config?.BindWith(MvisRulesetSetting.LogoPositionX, xPos);
            config?.BindWith(MvisRulesetSetting.LogoPositionY, yPos);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            xPos.BindValueChanged(x => logo.X = x.NewValue, true);
            yPos.BindValueChanged(y => logo.Y = y.NewValue, true);
            showParticles.BindValueChanged(onParticlesVisibilityChanged, true);
        }

        private void onParticlesVisibilityChanged(ValueChangedEvent<bool> value)
        {
            if (value.NewValue)
            {
                particlesPlaceholder.Child = new ParticlesDrawable();
                return;
            }

            particlesPlaceholder.Clear();
        }
    }
}

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Mvis.Configuration;
using osu.Game.Rulesets.Mvis.UI.Objects;
using osu.Game.Rulesets.Mvis.UI.Objects.MusicVisualizers;
using osu.Game.Rulesets.UI;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Mvis.UI
{
    public class MvisPlayfield : Playfield
    {
        [Resolved(canBeNull: true)]
        private MvisRulesetConfigManager config { get; set; }

        private readonly Bindable<bool> showParticles = new Bindable<bool>(true);
        private readonly Bindable<float> xPos = new Bindable<float>(0.5f);
        private readonly Bindable<float> yPos = new Bindable<float>(0.5f);
        private readonly Bindable<int> radius = new Bindable<int>(350);

        private readonly Bindable<bool> useCustomColour = new Bindable<bool>();
        private readonly Bindable<int> red = new Bindable<int>(0);
        private readonly Bindable<int> green = new Bindable<int>(0);
        private readonly Bindable<int> blue = new Bindable<int>(0);

        private BeatmapLogo logo;
        private MusicVisualizer visualizer;
        private Particles particles;

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                HitObjectContainer,
                particles = new Particles(),
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = visualizer = new MusicVisualizer
                    {
                        RelativePositionAxes = Axes.Both
                    }
                },
                logo = new BeatmapLogo
                {
                    RelativePositionAxes = Axes.Both
                }
            };

            config?.BindWith(MvisRulesetSetting.ShowParticles, showParticles);
            config?.BindWith(MvisRulesetSetting.LogoPositionX, xPos);
            config?.BindWith(MvisRulesetSetting.LogoPositionY, yPos);
            config?.BindWith(MvisRulesetSetting.Radius, radius);

            config?.BindWith(MvisRulesetSetting.Red, red);
            config?.BindWith(MvisRulesetSetting.Green, green);
            config?.BindWith(MvisRulesetSetting.Blue, blue);
            config?.BindWith(MvisRulesetSetting.UseCustomColour, useCustomColour);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            radius.BindValueChanged(r =>
            {
                logo.Size = new Vector2(r.NewValue);
                visualizer.Size = new Vector2(r.NewValue - 2);
            }, true);

            xPos.BindValueChanged(x => logo.X = visualizer.X = x.NewValue, true);
            yPos.BindValueChanged(y => logo.Y = visualizer.Y = y.NewValue, true);
            showParticles.BindValueChanged(show => particles.Alpha = show.NewValue ? 1 : 0, true);

            red.BindValueChanged(_ => updateColour());
            green.BindValueChanged(_ => updateColour());
            blue.BindValueChanged(_ => updateColour());
            useCustomColour.BindValueChanged(_ => updateColour(), true);
        }

        private void updateColour()
        {
            logo.Colour = visualizer.Colour = particles.Colour
                = useCustomColour.Value ? new Color4(red.Value / 255f, green.Value / 255f, blue.Value / 255f, 1) : Color4.White;
        }
    }
}

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Rulesets.Mvis.Configuration;
using osu.Game.Rulesets.Mvis.UI.Objects.Helpers;
using osu.Game.Rulesets.Mvis.UI.Objects.MusicVisualizers;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Mvis.UI.Objects
{
    public class BeatmapLogo : CurrentBeatmapProvider
    {
        private const int radius = 350;

        [Resolved(canBeNull: true)]
        private MvisRulesetConfigManager config { get; set; }

        private readonly Bindable<int> visuals = new Bindable<int>(3);
        private readonly Bindable<double> barWidth = new Bindable<double>(2.5);
        private readonly Bindable<int> barCount = new Bindable<int>(200);
        private readonly Bindable<int> rotation = new Bindable<int>(0);

        private readonly Bindable<bool> useCustomColour = new Bindable<bool>();
        private readonly Bindable<int> red = new Bindable<int>(0);
        private readonly Bindable<int> green = new Bindable<int>(0);
        private readonly Bindable<int> blue = new Bindable<int>(0);

        private CircularProgress progressGlow;
        private GlowEffect glow;
        private Container placeholder;

        [BackgroundDependencyLoader]
        private void load()
        {
            Origin = Anchor.Centre;
            Size = new Vector2(radius);

            InternalChildren = new Drawable[]
            {
                placeholder = new Container
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
                new UpdateableBeatmapBackground
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                },
                (progressGlow = new CircularProgress
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    InnerRadius = 0.02f,
                }).WithEffect(glow = new GlowEffect
                {
                    Colour = Color4.White,
                    Strength = 5,
                    PadExtent = true
                }),
            };

            config?.BindWith(MvisRulesetSetting.VisualizerAmount, visuals);
            config?.BindWith(MvisRulesetSetting.BarWidth, barWidth);
            config?.BindWith(MvisRulesetSetting.BarsPerVisual, barCount);
            config?.BindWith(MvisRulesetSetting.Rotation, rotation);

            config?.BindWith(MvisRulesetSetting.Red, red);
            config?.BindWith(MvisRulesetSetting.Green, green);
            config?.BindWith(MvisRulesetSetting.Blue, blue);
            config?.BindWith(MvisRulesetSetting.UseCustomColour, useCustomColour);

            visuals.BindValueChanged(_ => updateVisuals(), true);
            rotation.BindValueChanged(e => placeholder.Rotation = e.NewValue, true);

            red.BindValueChanged(_ => updateColour());
            green.BindValueChanged(_ => updateColour());
            blue.BindValueChanged(_ => updateColour());
            useCustomColour.BindValueChanged(_ => updateColour(), true);
        }

        private void updateColour()
        {
            progressGlow.Colour = glow.Colour = placeholder.Colour =
                useCustomColour.Value ? new Color4(red.Value / 255f, green.Value / 255f, blue.Value / 255f, 1) : Color4.White;
        }

        private void updateVisuals()
        {
            placeholder.Clear();
            var degree = 360f / visuals.Value;

            for (int i = 0; i < visuals.Value; i++)
            {
                placeholder.Add(new MusicVisualizer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Size = new Vector2(radius - 1),
                    Rotation = i * degree,
                    DegreeValue = { Value = degree },
                    BarWidth = { BindTarget = barWidth },
                    BarCount = { BindTarget = barCount }
                });
            }
        }

        protected override void Update()
        {
            base.Update();

            var track = Beatmap.Value?.Track;
            progressGlow.Current.Value = (track == null || track.Length == 0) ? 0 : (track.CurrentTime / track.Length);
        }
    }
}

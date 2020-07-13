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
using System.Collections.Generic;
using System.Threading;

namespace osu.Game.Rulesets.Mvis.UI.Objects
{
    public class BeatmapLogo : CurrentBeatmapProvider
    {
        private const int radius = 350;

        [Resolved(canBeNull: true)]
        private MvisRulesetConfigManager config { get; set; }

        private readonly Bindable<int> visuals = new Bindable<int>(3);
        private readonly Bindable<double> barWidth = new Bindable<double>(3.0);
        private readonly Bindable<int> barCount = new Bindable<int>(120);
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

            barCount.BindValueChanged(_ => updateVisuals());
            visuals.BindValueChanged(_ => updateVisuals(), true);
            rotation.BindValueChanged(e => placeholder.Rotation = e.NewValue, true);

            red.BindValueChanged(_ => updateColour());
            green.BindValueChanged(_ => updateColour());
            blue.BindValueChanged(_ => updateColour());
            useCustomColour.BindValueChanged(_ => updateColour(), true);
        }

        private void updateColour()
        {
            if (!useCustomColour.Value)
            {
                progressGlow.Colour = Color4.White;
                glow.Colour = Color4.White;
                return;
            }

            progressGlow.FadeColour(new Colour4(red.Value / 255f, green.Value / 255f, blue.Value / 255f, 1));
            glow.Colour = new Colour4(red.Value / 255f, green.Value / 255f, blue.Value / 255f, 1);
        }

        private CancellationTokenSource cancellationTokenSource;

        private void updateVisuals()
        {
            var drawableVisuals = new List<MusicCircularVisualizer>();

            var degree = 360f / visuals.Value;

            for (int i = 0; i < visuals.Value; i++)
            {
                drawableVisuals.Add(new MusicCircularVisualizer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    DegreeValue = degree,
                    Rotation = i * degree,
                    BarWidth = (float)barWidth.Value,
                    BarsCount = barCount.Value,
                    CircleSize = radius,
                });
            }

            LoadComponentsAsync(drawableVisuals, loaded =>
            {
                placeholder.Clear();
                placeholder.AddRange(loaded);
            }, (cancellationTokenSource = new CancellationTokenSource()).Token);
        }

        protected override void Update()
        {
            base.Update();

            var track = Beatmap.Value?.Track;

            progressGlow.Current.Value = track == null ? 0 : (float)(track.CurrentTime / track.Length);
        }

        protected override void Dispose(bool isDisposing)
        {
            cancellationTokenSource?.Cancel();
            base.Dispose(isDisposing);
        }
    }
}

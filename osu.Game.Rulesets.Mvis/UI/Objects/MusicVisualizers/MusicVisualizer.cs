using osu.Game.Rulesets.Mvis.UI.Objects.Helpers;
using osu.Framework.Graphics;
using osu.Framework.Bindables;
using osu.Game.Rulesets.Mvis.Configuration;
using osu.Framework.Allocation;
using System;

namespace osu.Game.Rulesets.Mvis.UI.Objects.MusicVisualizers
{
    public class MusicVisualizer : MusicAmplitudesProvider
    {
        [Resolved(canBeNull: true)]
        private MvisRulesetConfigManager config { get; set; }

        private readonly Bindable<int> visuals = new Bindable<int>(3);
        private readonly Bindable<double> barWidth = new Bindable<double>(1.0);
        private readonly Bindable<int> barCount = new Bindable<int>(1000);
        private readonly Bindable<int> rotation = new Bindable<int>(0);

        [BackgroundDependencyLoader]
        private void load()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            config?.BindWith(MvisRulesetSetting.VisualizerAmount, visuals);
            config?.BindWith(MvisRulesetSetting.BarWidth, barWidth);
            config?.BindWith(MvisRulesetSetting.BarsPerVisual, barCount);
            config?.BindWith(MvisRulesetSetting.Rotation, rotation);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            barCount.BindValueChanged(_ => updateBarCount());
            visuals.BindValueChanged(_ => updateVisuals(), true);
            rotation.BindValueChanged(e => Rotation = e.NewValue, true);
        }

        private void updateVisuals()
        {
            Clear();

            var degree = 360f / visuals.Value;
            var barsPerVis = (int)Math.Round((float)barCount.Value / visuals.Value);

            for (int i = 0; i < visuals.Value; i++)
            {
                Add(new MusicVisualizerDrawable
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Rotation = i * degree,
                    DegreeValue = { Value = degree },
                    BarWidth = { BindTarget = barWidth },
                    BarCount = { Value = barsPerVis }
                });
            }

            updateBarCount();
        }

        private void updateBarCount()
        {
            var barsPerVis = (int)Math.Round((float)barCount.Value / visuals.Value);

            foreach (var c in Children)
                ((MusicVisualizerDrawable)c).BarCount.Value = barsPerVis;
        }

        protected override void OnAmplitudesUpdate(float[] amplitudes, double timeDifference)
        {
            foreach (var c in Children)
                ((MusicVisualizerDrawable)c).UpdateAmplitudes(amplitudes, timeDifference);
        }
    }
}

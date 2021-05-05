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
        private readonly Bindable<int> totalBarCount = new Bindable<int>(3500);
        private readonly Bindable<int> rotation = new Bindable<int>(0);
        private readonly Bindable<int> decay = new Bindable<int>(200);
        private readonly Bindable<int> multiplier = new Bindable<int>(400);
        private readonly Bindable<BarType> type = new Bindable<BarType>(BarType.Fall);

        [BackgroundDependencyLoader]
        private void load()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            config?.BindWith(MvisRulesetSetting.VisualizerAmount, visuals);
            config?.BindWith(MvisRulesetSetting.BarWidth, barWidth);
            config?.BindWith(MvisRulesetSetting.BarsPerVisual, totalBarCount);
            config?.BindWith(MvisRulesetSetting.Rotation, rotation);
            config?.BindWith(MvisRulesetSetting.BarType, type);
            config?.BindWith(MvisRulesetSetting.Decay, decay);
            config?.BindWith(MvisRulesetSetting.Multiplier, multiplier);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            totalBarCount.BindValueChanged(_ => updateBarCount());
            visuals.BindValueChanged(_ => updateVisuals());
            type.BindValueChanged(_ => updateVisuals(), true);
            rotation.BindValueChanged(e => Rotation = e.NewValue, true);
        }

        private void updateVisuals()
        {
            Clear();

            var degree = 360f / visuals.Value;

            for (int i = 0; i < visuals.Value; i++)
            {
                Add(createVisualizer().With(v =>
                {
                    v.Anchor = Anchor.Centre;
                    v.Origin = Anchor.Centre;
                    v.RelativeSizeAxes = Axes.Both;
                    v.Rotation = i * degree;
                    v.DegreeValue.Value = degree;
                    v.BarWidth.BindTo(barWidth);
                    v.Decay.BindTo(decay);
                    v.HeightMultiplier.BindTo(multiplier);
                }));
            }

            updateBarCount();
        }

        private MusicVisualizerDrawable createVisualizer()
        {
            switch (type.Value)
            {
                default:
                case BarType.Basic:
                    return new BasicMusicVisualizerDrawable();

                case BarType.Fall:
                    return new FallMusicVisualizerDrawable();
            }
        }

        private void updateBarCount()
        {
            var barsPerVis = (int)Math.Round((float)totalBarCount.Value / visuals.Value);

            foreach (var c in Children)
                ((MusicVisualizerDrawable)c).BarCount.Value = barsPerVis;
        }

        protected override void OnAmplitudesUpdate(float[] amplitudes)
        {
            foreach (var c in Children)
                ((MusicVisualizerDrawable)c).SetAmplitudes(amplitudes);
        }
    }
}

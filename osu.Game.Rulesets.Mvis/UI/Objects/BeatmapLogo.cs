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
        private readonly Bindable<double> barWidth = new Bindable<double>(3.0);
        private readonly Bindable<int> barCount = new Bindable<int>(120);

        private CircularProgress progressGlow;
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
                }).WithEffect(new GlowEffect
                {
                    Colour = Color4.White,
                    Strength = 5,
                    PadExtent = true
                }),
            };

            config?.BindWith(MvisRulesetSetting.VisualizerAmount, visuals);
            config?.BindWith(MvisRulesetSetting.BarWidth, barWidth);
            config?.BindWith(MvisRulesetSetting.BarsPerVisual, barCount);

            barWidth.BindValueChanged(_ => updateVisuals());
            barCount.BindValueChanged(_ => updateVisuals());
            visuals.BindValueChanged(_ => updateVisuals(), true);
        }

        private void updateVisuals()
        {
            placeholder.Clear();

            var degree = 360f / visuals.Value;

            for (int i = 0; i < visuals.Value; i++)
            {
                placeholder.Add(new MusicCircularVisualizer
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
        }

        protected override void Update()
        {
            base.Update();

            var track = Beatmap.Value?.Track;

            progressGlow.Current.Value = track == null ? 0 : (float)(track.CurrentTime / track.Length);
        }
    }
}

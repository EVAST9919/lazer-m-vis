using osu.Framework.Graphics;
using osuTK.Graphics;
using osu.Game.Rulesets.Mvis.UI.Objects;
using osu.Game.Rulesets.Mvis.UI.Objects.MusicVisualizers;
using osu.Game.Rulesets.Mvis.Configuration;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osuTK;
using osu.Framework.Utils;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Mvis.UI.Settings;
using osu.Game.Beatmaps;
using System.Threading;
using osu.Game.Storyboards.Drawables;
using osu.Framework.Timing;
using osu.Framework.Graphics.Shapes;
using osu.Game.Configuration;
using osu.Framework.Extensions.Color4Extensions;

namespace osu.Game.Rulesets.Mvis.UI
{
    public class VisualizerScreen : RulesetScreen
    {
        private readonly Bindable<bool> useStoryboard = new Bindable<bool>();
        private readonly BindableDouble dim = new BindableDouble();

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
        private VisualizerSettings settings;
        private Container storyboardHolder;

        [BackgroundDependencyLoader]
        private void load(OsuConfigManager osuConfig)
        {
            AddRangeInternal(new Drawable[]
            {
                storyboardHolder = new Container
                {
                    RelativeSizeAxes = Axes.Both
                },
                particles = new Particles(),
                new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    RowDimensions = new[]
                    {
                        new Dimension()
                    },
                    ColumnDimensions = new[]
                    {
                        new Dimension(),
                        new Dimension(GridSizeMode.AutoSize)
                    },
                    Content = new[]
                    {
                        new Drawable[]
                        {
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Children = new Drawable[]
                                {
                                    visualizer = new MusicVisualizer
                                    {
                                        RelativePositionAxes = Axes.Both
                                    },
                                    logo = new BeatmapLogo
                                    {
                                        RelativePositionAxes = Axes.Both
                                    }
                                }
                            },
                            settings = new VisualizerSettings()
                        }
                    }
                }
            });

            Config?.BindWith(MvisRulesetSetting.StoryboardBackground, useStoryboard);
            osuConfig?.BindWith(OsuSetting.DimLevel, dim);

            Config?.BindWith(MvisRulesetSetting.ShowParticles, showParticles);            
            Config?.BindWith(MvisRulesetSetting.LogoPositionX, xPos);
            Config?.BindWith(MvisRulesetSetting.LogoPositionY, yPos);
            Config?.BindWith(MvisRulesetSetting.Radius, radius);

            Config?.BindWith(MvisRulesetSetting.Red, red);
            Config?.BindWith(MvisRulesetSetting.Green, green);
            Config?.BindWith(MvisRulesetSetting.Blue, blue);
            Config?.BindWith(MvisRulesetSetting.UseCustomColour, useCustomColour);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            radius.BindValueChanged(r =>
            {
                logo.Size.Value = r.NewValue;
                visualizer.Size = new Vector2(r.NewValue - 2);
            }, true);

            xPos.BindValueChanged(x => logo.X = visualizer.X = x.NewValue, true);
            yPos.BindValueChanged(y => logo.Y = visualizer.Y = y.NewValue, true);
            showParticles.BindValueChanged(show => particles.Alpha = show.NewValue ? 1 : 0, true);

            red.BindValueChanged(_ => updateColour());
            green.BindValueChanged(_ => updateColour());
            blue.BindValueChanged(_ => updateColour());
            useCustomColour.BindValueChanged(_ => updateColour(), true);

            dim.BindValueChanged(_ => updateStoryboardDim());
            useStoryboard.BindValueChanged(_ => updateStoryboard(Beatmap.Value));
        }

        protected override void OnBeatmapUpdate(WorkingBeatmap beatmap)
        {
            base.OnBeatmapUpdate(beatmap);
            updateStoryboard(beatmap);
        }

        private CancellationTokenSource cancellationToken;
        private AudioContainer storyboard;

        private void updateStoryboard(WorkingBeatmap beatmap)
        {
            cancellationToken?.Cancel();
            storyboard?.FadeOut(250, Easing.OutQuint).Expire();
            storyboard = null;

            if (!useStoryboard.Value)
                return;

            if (!beatmap.Storyboard.HasDrawable)
                return;

            Drawable layer;

            if (beatmap.Storyboard.ReplacesBackground)
            {
                layer = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black
                };
            }
            else
            {
                layer = new BeatmapBackground(beatmap);
            }

            LoadComponentAsync(new AudioContainer
            {
                RelativeSizeAxes = Axes.Both,
                Volume = { Value = 0 },
                Alpha = 0,
                Colour = getStoryboardColour,
                Children = new Drawable[]
                {
                    layer,
                    new DrawableStoryboard(beatmap.Storyboard) { Clock = new InterpolatingFramedClock(beatmap.Track) }
                }
            }, loaded =>
            {
                storyboardHolder.Add(storyboard = loaded);
                loaded.FadeIn(250, Easing.OutQuint);
            }, (cancellationToken = new CancellationTokenSource()).Token);
        }

        private void updateStoryboardDim()
        {
            if (storyboard != null)
                storyboard.Colour = getStoryboardColour;
        }

        private Color4 getStoryboardColour => new Color4(1 - (float)dim.Value, 1 - (float)dim.Value, 1 - (float)dim.Value, 1);

        private void updateColour()
        {
            logo.Colour = visualizer.Colour = particles.Colour
                = useCustomColour.Value ? new Color4(red.Value / 255f, green.Value / 255f, blue.Value / 255f, 1) : Color4.White;
        }

        protected override bool OnMouseMove(MouseMoveEvent e)
        {
            base.OnMouseMove(e);

            if (settings.IsVisible.Value)
                return false;

            var cursorPosition = ToLocalSpace(e.CurrentState.Mouse.Position);

            if (Precision.AlmostEquals(cursorPosition.X, DrawWidth, 1))
            {
                settings.IsVisible.Value = true;
                return true;
            }

            return false;
        }

        protected override bool OnClick(ClickEvent e)
        {
            if (!settings.IsVisible.Value)
                return false;

            settings.IsVisible.Value = false;
            return true;
        }
    }
}

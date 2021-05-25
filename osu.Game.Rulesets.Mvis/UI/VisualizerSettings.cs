using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Configuration;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Input.Bindings;
using osu.Game.Overlays;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Mvis.Configuration;
using osuTK;
using osuTK.Graphics;
using static osu.Game.Rulesets.Mvis.UI.MvisSettingsSubsection;

namespace osu.Game.Rulesets.Mvis.UI
{
    public class VisualizerSettings : CompositeDrawable, IKeyBindingHandler<GlobalAction>
    {
        private const int width = 400;

        public readonly BindableBool IsVisible = new BindableBool();
        private readonly OverlayColourProvider colourProvider = new OverlayColourProvider(OverlayColourScheme.Pink);

        public VisualizerSettings()
        {
            Anchor = Anchor.CentreRight;
            Origin = Anchor.CentreRight;
            RelativeSizeAxes = Axes.Y;
            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = colourProvider.Background3
                },
                new SettingsContent
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            IsVisible.BindValueChanged(v =>
            {
                this.ResizeWidthTo(v.NewValue ? width : 0, 250, Easing.OutQuint);
            }, true);

            FinishTransforms(true);
        }

        public bool OnPressed(GlobalAction action)
        {
            if (!IsVisible.Value)
                return false;

            switch (action)
            {
                case GlobalAction.Back:
                    IsVisible.Value = false;
                    return true;
            }

            return false;
        }

        public void OnReleased(GlobalAction action)
        {
        }

        protected override bool OnClick(ClickEvent e) => true;

        private class SettingsContent : CompositeDrawable
        {
            public SettingsContent()
            {
                RelativeSizeAxes = Axes.Y;
                Width = width;
                InternalChildren = new Drawable[]
                {
                    new OsuScrollContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Child = new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            Padding = new MarginPadding { Horizontal = 20 },
                            AutoSizeAxes = Axes.Y,
                            Margin = new MarginPadding { Vertical = 20 },
                            Child = new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Vertical,
                                Spacing = new Vector2(0, 30),
                                Children = new Drawable[]
                                {
                                    new RulesetSection(),
                                    new GameSection()
                                }
                            }
                        }
                    }
                };
            }
        }

        private abstract class Section : Container
        {
            protected override Container<Drawable> Content => content;

            protected abstract string HeaderName { get; }

            private readonly OsuSpriteText header;
            private readonly FillFlowContainer content;

            public Section()
            {
                Masking = true;
                CornerRadius = 5;
                EdgeEffect = new EdgeEffectParameters
                {
                    Colour = Color4.Black.Opacity(0.5f),
                    Radius = 10,
                    Type = EdgeEffectType.Shadow,
                    Hollow = true
                };
                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;
                InternalChild = new Container
                {
                    AutoSizeAxes = Axes.Y,
                    RelativeSizeAxes = Axes.X,
                    Padding = new MarginPadding(10),
                    Child = new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(0, 10),
                        Children = new Drawable[]
                        {
                            header = new OsuSpriteText
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                Text = HeaderName,
                                Font = OsuFont.GetFont(size: 30)
                            },
                            content = new FillFlowContainer
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Vertical,
                                Spacing = new Vector2(0, 5)
                            }
                        }
                    }
                };
            }

            [BackgroundDependencyLoader]
            private void load(OsuColour colours)
            {
                header.Colour = colours.Yellow;
            }
        }

        private class RulesetSection : Section
        {
            protected override string HeaderName => "Ruleset settings";

            private SettingsCheckbox customColourCheckbox;
            private Container resizableContainer;

            [BackgroundDependencyLoader]
            private void load(MvisRulesetConfigManager config)
            {
                AddRange(new Drawable[]
                {
                    new SettingsCheckbox
                    {
                        LabelText = "Show particles",
                        Current = config.GetBindable<bool>(MvisRulesetSetting.ShowParticles)
                    },
                    new SettingsSlider<int>
                    {
                        LabelText = "Particle count",
                        Current = config.GetBindable<int>(MvisRulesetSetting.ParticlesCount),
                        KeyboardStep = 1,
                        TransferValueOnCommit = true
                    },
                    new SettingsEnumDropdown<BarType>
                    {
                        LabelText = "Bar type",
                        Current = config.GetBindable<BarType>(MvisRulesetSetting.BarType)
                    },
                    new SettingsCheckbox
                    {
                        LabelText = "Symmetry",
                        Current = config.GetBindable<bool>(MvisRulesetSetting.Symmetry)
                    },
                    new SettingsSlider<int>
                    {
                        LabelText = "Visualizer count",
                        Current = config.GetBindable<int>(MvisRulesetSetting.VisualizerAmount),
                        KeyboardStep = 1,
                        TransferValueOnCommit = true
                    },
                    new SettingsSlider<double>
                    {
                        LabelText = "Bar width",
                        Current = config.GetBindable<double>(MvisRulesetSetting.BarWidth),
                        KeyboardStep = 0.1f
                    },
                    new SettingsSlider<int>
                    {
                        LabelText = "Total bar count",
                        Current = config.GetBindable<int>(MvisRulesetSetting.BarsPerVisual),
                        KeyboardStep = 1
                    },
                    new SettingsSlider<int>
                    {
                        LabelText = "Decay",
                        Current = config.GetBindable<int>(MvisRulesetSetting.Decay),
                        KeyboardStep = 1
                    },
                    new SettingsSlider<int>
                    {
                        LabelText = "Height Multiplier",
                        Current = config.GetBindable<int>(MvisRulesetSetting.Multiplier),
                        KeyboardStep = 1
                    },
                    new SettingsSlider<int>
                    {
                        LabelText = "Rotation",
                        KeyboardStep = 1,
                        Current = config.GetBindable<int>(MvisRulesetSetting.Rotation)
                    },
                    new SettingsSlider<int>
                    {
                        LabelText = "Radius",
                        KeyboardStep = 1,
                        Current = config.GetBindable<int>(MvisRulesetSetting.Radius)
                    },
                    new SettingsSlider<float, PositionSlider>
                    {
                        LabelText = "X Position",
                        KeyboardStep = 0.01f,
                        Current = config.GetBindable<float>(MvisRulesetSetting.LogoPositionX)
                    },
                    new SettingsSlider<float, PositionSlider>
                    {
                        LabelText = "Y Position",
                        KeyboardStep = 0.01f,
                        Current = config.GetBindable<float>(MvisRulesetSetting.LogoPositionY)
                    },
                    customColourCheckbox = new SettingsCheckbox
                    {
                        LabelText = "Use custom accent colour",
                        Current = config.GetBindable<bool>(MvisRulesetSetting.UseCustomColour)
                    },
                    resizableContainer = new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeDuration = 200,
                        AutoSizeEasing = Easing.OutQuint,
                        Masking = true,
                        Child = new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                            Spacing = new Vector2(0, 5),
                            Children = new Drawable[]
                            {
                                new SettingsSlider<int>
                                {
                                    LabelText = "Red",
                                    KeyboardStep = 1,
                                    Current = config.GetBindable<int>(MvisRulesetSetting.Red)
                                },
                                new SettingsSlider<int>
                                {
                                    LabelText = "Green",
                                    KeyboardStep = 1,
                                    Current = config.GetBindable<int>(MvisRulesetSetting.Green)
                                },
                                new SettingsSlider<int>
                                {
                                    KeyboardStep = 1,
                                    LabelText = "Blue",
                                    Current = config.GetBindable<int>(MvisRulesetSetting.Blue)
                                }
                            }
                        }
                    }
                });
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                customColourCheckbox.Current.BindValueChanged(useCustom =>
                {
                    if (useCustom.NewValue)
                    {
                        resizableContainer.ClearTransforms();
                        resizableContainer.AutoSizeAxes = Axes.Y;
                    }
                    else
                    {
                        resizableContainer.AutoSizeAxes = Axes.None;
                        resizableContainer.ResizeHeightTo(0, 200, Easing.OutQuint);
                    }
                }, true);

                resizableContainer.FinishTransforms();
            }
        }

        private class GameSection : Section
        {
            protected override string HeaderName => "Game settings";

            [BackgroundDependencyLoader]
            private void load(OsuConfigManager config)
            {
                AddRange(new Drawable[]
                {
                    new SettingsSlider<double>
                    {
                        LabelText = "Background dim",
                        Current = config.GetBindable<double>(OsuSetting.DimLevel),
                        KeyboardStep = 0.01f,
                        DisplayAsPercentage = true
                    },
                    new SettingsSlider<double>
                    {
                        LabelText = "Background blur",
                        Current = config.GetBindable<double>(OsuSetting.BlurLevel),
                        KeyboardStep = 0.01f,
                        DisplayAsPercentage = true
                    }
                });
            }
        }
    }
}

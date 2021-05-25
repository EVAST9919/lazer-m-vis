using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Testing;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osu.Game.Overlays.Notifications;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Mvis.Configuration;
using osu.Game.Screens;
using osu.Game.Screens.Menu;
using osuTK;

namespace osu.Game.Rulesets.Mvis.UI
{
    public class MvisSettingsSubsection : RulesetSettingsSubsection
    {
        protected override string Header => "mvis";

        [Resolved]
        private OsuGame game { get; set; }

        [Resolved]
        private NotificationOverlay notifications { get; set; }

        public MvisSettingsSubsection(Ruleset ruleset)
            : base(ruleset)
        {
        }

        private SettingsCheckbox customColourCheckbox;
        private Container resizableContainer;

        [BackgroundDependencyLoader]
        private void load()
        {
            var config = (MvisRulesetConfigManager)Config;

            Children = new Drawable[]
            {
                new SettingsButton
                {
                    Text = "Open Main Screen",
                    Action = () =>
                    {
                        try
                        {
                            var screenStack = getScreenStack(game);
                            if (!(screenStack.CurrentScreen is MainMenu))
                            {
                                notifications.Post(new SimpleErrorNotification
                                {
                                    Text = "This feature can be used only in Main menu!"
                                });
                                return;
                            }

                            var settingOverlay = getSettingsOverlay(game);
                            screenStack?.Push(new VisualizerScreen());
                            settingOverlay?.Hide();
                        }
                        catch
                        {
                        }
                    }
                },
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
            };
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

        private static OsuScreenStack getScreenStack(OsuGame game) => game.ChildrenOfType<OsuScreenStack>().FirstOrDefault();

        private static SettingsOverlay getSettingsOverlay(OsuGame game) => game.ChildrenOfType<SettingsOverlay>().FirstOrDefault();

        public class PositionSlider : OsuSliderBar<float>
        {
            public override string TooltipText => Current.Value.ToString(@"0.##");
        }
    }
}

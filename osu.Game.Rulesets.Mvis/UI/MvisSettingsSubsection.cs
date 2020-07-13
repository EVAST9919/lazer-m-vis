using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Mvis.Configuration;
using osuTK;

namespace osu.Game.Rulesets.Mvis.UI
{
    public class MvisSettingsSubsection : RulesetSettingsSubsection
    {
        protected override string Header => "mvis";

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
                new SettingsCheckbox
                {
                    LabelText = "Show particles",
                    Bindable = config.GetBindable<bool>(MvisRulesetSetting.ShowParticles)
                },
                new SettingsSlider<int>
                {
                    LabelText = "Particles count",
                    Bindable = config.GetBindable<int>(MvisRulesetSetting.ParticlesCount),
                    TransferValueOnCommit = true
                },
                new SettingsEnumDropdown<BarType>
                {
                    LabelText = "Bar type",
                    Bindable = config.GetBindable<BarType>(MvisRulesetSetting.BarType)
                },
                new SettingsSlider<int>
                {
                    LabelText = "Visulizer amount",
                    Bindable = config.GetBindable<int>(MvisRulesetSetting.VisualizerAmount),
                    TransferValueOnCommit = true
                },
                new SettingsSlider<double>
                {
                    LabelText = "Bar width",
                    Bindable = config.GetBindable<double>(MvisRulesetSetting.BarWidth),
                    KeyboardStep = 0.1f
                },
                new SettingsSlider<int>
                {
                    LabelText = "Bars per visual",
                    Bindable = config.GetBindable<int>(MvisRulesetSetting.BarsPerVisual),
                    TransferValueOnCommit = true
                },
                customColourCheckbox = new SettingsCheckbox
                {
                    LabelText = "Use custom accent colour",
                    Bindable = config.GetBindable<bool>(MvisRulesetSetting.UseCustomColour)
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
                                Bindable = config.GetBindable<int>(MvisRulesetSetting.Red)
                            },
                            new SettingsSlider<int>
                            {
                                LabelText = "Green",
                                Bindable = config.GetBindable<int>(MvisRulesetSetting.Green)
                            },
                            new SettingsSlider<int>
                            {
                                LabelText = "Blue",
                                Bindable = config.GetBindable<int>(MvisRulesetSetting.Blue)
                            }
                        }
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            customColourCheckbox.Bindable.BindValueChanged(useCustom =>
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
}

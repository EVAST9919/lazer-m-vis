using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Mvis.Configuration;

namespace osu.Game.Rulesets.Mvis.UI
{
    public class MvisSettingsSubsection : RulesetSettingsSubsection
    {
        protected override string Header => "mvis";

        public MvisSettingsSubsection(Ruleset ruleset)
            : base(ruleset)
        {
        }

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
                    LabelText = "Visulizer amount",
                    Bindable = config.GetBindable<int>(MvisRulesetSetting.VisualizerAmount),
                    TransferValueOnCommit = true
                },
                new SettingsSlider<double>
                {
                    LabelText = "Bar width",
                    Bindable = config.GetBindable<double>(MvisRulesetSetting.BarWidth),
                    KeyboardStep = 0.1f,
                    TransferValueOnCommit = true
                },
                new SettingsSlider<int>
                {
                    LabelText = "Bars per visual",
                    Bindable = config.GetBindable<int>(MvisRulesetSetting.BarsPerVisual),
                    TransferValueOnCommit = true
                },
            };
        }
    }
}

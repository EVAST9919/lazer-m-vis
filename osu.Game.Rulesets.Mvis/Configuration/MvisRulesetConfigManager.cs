﻿using osu.Game.Configuration;
using osu.Game.Rulesets.Configuration;

namespace osu.Game.Rulesets.Mvis.Configuration
{
    public class MvisRulesetConfigManager : RulesetConfigManager<MvisRulesetSetting>
    {
        public MvisRulesetConfigManager(SettingsStore settings, RulesetInfo ruleset, int? variant = null)
            : base(settings, ruleset, variant)
        {
        }

        protected override void InitialiseDefaults()
        {
            base.InitialiseDefaults();
            Set(MvisRulesetSetting.ShowParticles, true);
            Set(MvisRulesetSetting.VisualizerAmount, 3, 1, 5);
            Set(MvisRulesetSetting.BarWidth, 3.0, 1, 20);
            Set(MvisRulesetSetting.BarsPerVisual, 120, 1, 200);
        }
    }

    public enum MvisRulesetSetting
    {
        ShowParticles,
        VisualizerAmount,
        BarWidth,
        BarsPerVisual
    }
}

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
            SetDefault(MvisRulesetSetting.ShowParticles, true);
            SetDefault(MvisRulesetSetting.ParticlesCount, 200, 100, 300);
            SetDefault(MvisRulesetSetting.BarType, BarType.Rounded);
            SetDefault(MvisRulesetSetting.VisualizerAmount, 3, 1, 5);
            SetDefault(MvisRulesetSetting.BarWidth, 3.0, 1, 20);
            SetDefault(MvisRulesetSetting.BarsPerVisual, 120, 1, 200);
            SetDefault(MvisRulesetSetting.Rotation, 0, 0, 359);
            SetDefault(MvisRulesetSetting.UseCustomColour, false);
            SetDefault(MvisRulesetSetting.Red, 0, 0, 255);
            SetDefault(MvisRulesetSetting.Green, 0, 0, 255);
            SetDefault(MvisRulesetSetting.Blue, 0, 0, 255);
        }
    }

    public enum MvisRulesetSetting
    {
        ShowParticles,
        ParticlesCount,
        VisualizerAmount,
        BarWidth,
        BarsPerVisual,
        BarType,
        Rotation,
        UseCustomColour,
        Red,
        Green,
        Blue
    }

    public enum BarType
    {
        Basic,
        Rounded,
        Fall
    }
}

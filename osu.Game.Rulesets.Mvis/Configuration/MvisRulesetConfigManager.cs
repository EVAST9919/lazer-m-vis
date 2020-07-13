using osu.Game.Configuration;
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
            Set(MvisRulesetSetting.ParticlesCount, 200, 100, 300);
            Set(MvisRulesetSetting.BarType, BarType.Rounded);
            Set(MvisRulesetSetting.VisualizerAmount, 3, 1, 5);
            Set(MvisRulesetSetting.BarWidth, 3.0, 1, 20);
            Set(MvisRulesetSetting.BarsPerVisual, 120, 1, 200);
            Set(MvisRulesetSetting.UseCustomColour, false);
            Set(MvisRulesetSetting.Red, 0, 0, 255);
            Set(MvisRulesetSetting.Green, 0, 0, 255);
            Set(MvisRulesetSetting.Blue, 0, 0, 255);
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

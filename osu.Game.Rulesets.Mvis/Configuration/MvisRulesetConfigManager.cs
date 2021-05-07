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
            SetDefault(MvisRulesetSetting.ShowParticles, true);
            SetDefault(MvisRulesetSetting.ParticlesCount, 300, 50, 1000);
            SetDefault(MvisRulesetSetting.BarType, BarType.Rounded);
            SetDefault(MvisRulesetSetting.VisualizerAmount, 3, 1, 5);
            SetDefault(MvisRulesetSetting.BarWidth, 3.0, 1, 20);
            SetDefault(MvisRulesetSetting.BarsPerVisual, 120, 10, 3500);
            SetDefault(MvisRulesetSetting.Rotation, 0, 0, 359);
            SetDefault(MvisRulesetSetting.UseCustomColour, false);
            SetDefault(MvisRulesetSetting.Red, 0, 0, 255);
            SetDefault(MvisRulesetSetting.Green, 0, 0, 255);
            SetDefault(MvisRulesetSetting.Blue, 0, 0, 255);
            SetDefault(MvisRulesetSetting.Decay, 200, 100, 500);
            SetDefault(MvisRulesetSetting.Multiplier, 400, 200, 500);
            SetDefault(MvisRulesetSetting.Radius, 350, 100, 450);
            SetDefault(MvisRulesetSetting.LogoPositionX, 0.5f, 0, 1);
            SetDefault(MvisRulesetSetting.LogoPositionY, 0.5f, 0, 1);
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
        Blue,
        Decay,
        Multiplier,
        Radius,
        LogoPositionX,
        LogoPositionY
    }

    public enum BarType
    {
        Basic,
        Rounded,
        Fall
    }
}

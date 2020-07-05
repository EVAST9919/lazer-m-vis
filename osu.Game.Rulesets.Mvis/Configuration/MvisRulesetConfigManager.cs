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
        }
    }

    public enum MvisRulesetSetting
    {
        ShowParticles,
    }
}

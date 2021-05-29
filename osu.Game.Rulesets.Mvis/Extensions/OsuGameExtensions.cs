using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Testing;
using osu.Game.Overlays;
using osu.Game.Screens;

namespace osu.Game.Rulesets.Mvis.Extensions
{
    public static class OsuGameExtensions
    {
        public static MvisRuleset GetRuleset(this DependencyContainer dependencies)
        {
            var rulesets = dependencies.Get<RulesetStore>().AvailableRulesets.Select(info => info.CreateInstance());
            return (MvisRuleset)rulesets.FirstOrDefault(r => r is MvisRuleset);
        }

        public static OsuScreenStack GetScreenStack(this OsuGame game) => game.ChildrenOfType<OsuScreenStack>().FirstOrDefault();

        public static SettingsOverlay GetSettingsOverlay(this OsuGame game) => game.ChildrenOfType<SettingsOverlay>().FirstOrDefault();
    }
}

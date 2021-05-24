using System.Linq;
using osu.Framework.Allocation;

namespace osu.Game.Rulesets.Mvis.Extensions
{
    public static class SyntaxExtensions
    {
        public static MvisRuleset GetRuleset(this DependencyContainer dependencies)
        {
            var rulesets = dependencies.Get<RulesetStore>().AvailableRulesets.Select(info => info.CreateInstance());
            return (MvisRuleset)rulesets.FirstOrDefault(r => r is MvisRuleset);
        }
    }
}

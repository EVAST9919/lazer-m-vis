using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Mvis
{
    public class MvisInputManager : RulesetInputManager<MvisAction>
    {
        public MvisInputManager(RulesetInfo ruleset)
            : base(ruleset, 0, SimultaneousBindingMode.Unique)
        {
        }
    }

    public enum MvisAction
    {
    }
}

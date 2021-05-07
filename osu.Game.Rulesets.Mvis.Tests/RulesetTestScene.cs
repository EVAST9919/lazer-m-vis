using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Mvis.Tests
{
    public abstract class RulesetTestScene : OsuTestScene
    {
        protected override Ruleset CreateRuleset() => new MvisRuleset();
    }
}

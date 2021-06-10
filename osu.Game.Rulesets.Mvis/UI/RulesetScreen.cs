using osu.Game.Beatmaps;
using osu.Game.Screens.Play;
using osu.Framework.Screens;
using osu.Game.Rulesets.Mvis.Configuration;
using osu.Framework.Allocation;
using osu.Game.Rulesets.Mvis.Extensions;
using osu.Framework.Input.Bindings;
using osu.Game.Input.Bindings;
using osu.Game.Rulesets.UI;
using osu.Game.Screens;

namespace osu.Game.Rulesets.Mvis.UI
{
    public class RulesetScreen : ScreenWithBeatmapBackground, IKeyBindingHandler<GlobalAction>
    {
        public override bool AllowBackButton => false;

        protected MvisRulesetConfigManager Config { get; private set; }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            var baseDependencies = new DependencyContainer(base.CreateChildDependencies(parent));
            var ruleset = baseDependencies.GetRuleset();

            Config = (MvisRulesetConfigManager)baseDependencies.Get<RulesetConfigCache>().GetConfigFor(ruleset);

            return new OsuScreenDependencies(false, new DrawableRulesetDependencies(ruleset, baseDependencies));
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            Beatmap.BindValueChanged(b => OnBeatmapUpdate(b.NewValue));
        }

        public override void OnEntering(IScreen last)
        {
            base.OnEntering(last);
            OnBeatmapUpdate(Beatmap.Value);
        }

        protected virtual void OnBeatmapUpdate(WorkingBeatmap beatmap)
        {
            ApplyToBackground(b =>
            {
                b.IgnoreUserSettings.Value = false;
                b.Beatmap = beatmap;
            });
        }

        public bool OnPressed(GlobalAction action)
        {
            switch (action)
            {
                case GlobalAction.Back:
                    this.Exit();
                    return true;
            }

            return false;
        }

        public void OnReleased(GlobalAction action)
        {
        }
    }
}

using osu.Game.Beatmaps;
using osu.Game.Screens.Play;
using osu.Framework.Screens;
using osu.Game.Rulesets.Mvis.Configuration;
using osu.Framework.Allocation;
using osu.Game.Rulesets.Mvis.Extensions;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Input.Bindings;
using osu.Game.Input.Bindings;

namespace osu.Game.Rulesets.Mvis.UI
{
    public class RulesetScreen : ScreenWithBeatmapBackground, IKeyBindingHandler<GlobalAction>
    {
        public override bool AllowBackButton => false;

        protected MvisRulesetConfigManager Config { get; private set; }

        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
            var mvisRuleset = dependencies.GetRuleset();

            // Add ruleset textures to texture store.
            dependencies.Get<TextureStore>().AddStore(new TextureLoaderStore(new NamespacedResourceStore<byte[]>(mvisRuleset.CreateResourceStore(), @"Textures")));

            // Cache ruleset config
            Config = (MvisRulesetConfigManager)dependencies.Get<RulesetConfigCache>().GetConfigFor(mvisRuleset);

            if (Config != null)
                dependencies.Cache(Config);

            return dependencies;
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

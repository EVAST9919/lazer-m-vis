using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osuTK.Graphics;
using osu.Game.Screens.Play;
using osu.Framework.Screens;
using osu.Game.Rulesets.Mvis.UI.Objects;
using osu.Game.Rulesets.Mvis.UI.Objects.MusicVisualizers;
using osu.Game.Rulesets.Mvis.Configuration;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osuTK;
using osu.Game.Rulesets.Mvis.Extensions;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Input.Bindings;
using osu.Game.Input.Bindings;

namespace osu.Game.Rulesets.Mvis.UI
{
    public class VisualizerScreen : ScreenWithBeatmapBackground, IKeyBindingHandler<GlobalAction>
    {
        public override bool AllowBackButton => false;

        private MvisRulesetConfigManager config;

        private readonly Bindable<bool> showParticles = new Bindable<bool>(true);
        private readonly Bindable<float> xPos = new Bindable<float>(0.5f);
        private readonly Bindable<float> yPos = new Bindable<float>(0.5f);
        private readonly Bindable<int> radius = new Bindable<int>(350);

        private readonly Bindable<bool> useCustomColour = new Bindable<bool>();
        private readonly Bindable<int> red = new Bindable<int>(0);
        private readonly Bindable<int> green = new Bindable<int>(0);
        private readonly Bindable<int> blue = new Bindable<int>(0);

        private BeatmapLogo logo;
        private MusicVisualizer visualizer;
        private Particles particles;

        [BackgroundDependencyLoader]
        private void load()
        {
            AddRangeInternal(new Drawable[]
            {
                particles = new Particles(),
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = visualizer = new MusicVisualizer
                    {
                        RelativePositionAxes = Axes.Both
                    }
                },
                logo = new BeatmapLogo
                {
                    RelativePositionAxes = Axes.Both
                }
            });

            config?.BindWith(MvisRulesetSetting.ShowParticles, showParticles);
            config?.BindWith(MvisRulesetSetting.LogoPositionX, xPos);
            config?.BindWith(MvisRulesetSetting.LogoPositionY, yPos);
            config?.BindWith(MvisRulesetSetting.Radius, radius);

            config?.BindWith(MvisRulesetSetting.Red, red);
            config?.BindWith(MvisRulesetSetting.Green, green);
            config?.BindWith(MvisRulesetSetting.Blue, blue);
            config?.BindWith(MvisRulesetSetting.UseCustomColour, useCustomColour);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Beatmap.BindValueChanged(b => updateComponentFromBeatmap(b.NewValue));

            radius.BindValueChanged(r =>
            {
                logo.Size.Value = r.NewValue;
                visualizer.Size = new Vector2(r.NewValue - 2);
            }, true);

            xPos.BindValueChanged(x => logo.X = visualizer.X = x.NewValue, true);
            yPos.BindValueChanged(y => logo.Y = visualizer.Y = y.NewValue, true);
            showParticles.BindValueChanged(show => particles.Alpha = show.NewValue ? 1 : 0, true);

            red.BindValueChanged(_ => updateColour());
            green.BindValueChanged(_ => updateColour());
            blue.BindValueChanged(_ => updateColour());
            useCustomColour.BindValueChanged(_ => updateColour(), true);
        }

        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
            var mvisRuleset = dependencies.GetRuleset();

            // Add ruleset textures to texture store.
            dependencies.Get<TextureStore>().AddStore(new TextureLoaderStore(new NamespacedResourceStore<byte[]>(mvisRuleset.CreateResourceStore(), @"Textures")));

            config = dependencies.Get<RulesetConfigCache>().GetConfigFor(mvisRuleset) as MvisRulesetConfigManager;
            if (config != null)
                dependencies.Cache(config);

            return dependencies;
        }

        private void updateColour()
        {
            logo.Colour = visualizer.Colour = particles.Colour
                = useCustomColour.Value ? new Color4(red.Value / 255f, green.Value / 255f, blue.Value / 255f, 1) : Color4.White;
        }

        public override void OnEntering(IScreen last)
        {
            base.OnEntering(last);
            updateComponentFromBeatmap(Beatmap.Value);
        }

        private void updateComponentFromBeatmap(WorkingBeatmap beatmap)
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

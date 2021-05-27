using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Graphics.Containers;
using osu.Game.Input.Bindings;
using osu.Game.Overlays;
using osu.Game.Rulesets.Mvis.UI.Settings.Sections;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Mvis.UI.Settings
{
    public class VisualizerSettings : CompositeDrawable, IKeyBindingHandler<GlobalAction>
    {
        private const int width = 400;
        private const float duration = 250f;

        public readonly BindableBool IsVisible = new BindableBool();

        public VisualizerSettings()
        {
            Anchor = Anchor.CentreRight;
            Origin = Anchor.CentreRight;
            RelativeSizeAxes = Axes.Y;
            InternalChild = new SettingsContent
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                IsVisible = { BindTarget = IsVisible }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            IsVisible.BindValueChanged(v =>
            {
                this.ResizeWidthTo(v.NewValue ? width : 0, duration, Easing.OutQuint);
            }, true);

            FinishTransforms(true);
        }

        public bool OnPressed(GlobalAction action)
        {
            if (!IsVisible.Value)
                return false;

            switch (action)
            {
                case GlobalAction.Back:
                    IsVisible.Value = false;
                    return true;
            }

            return false;
        }

        public void OnReleased(GlobalAction action)
        {
        }

        protected override bool OnClick(ClickEvent e) => true;

        private class SettingsContent : CompositeDrawable
        {
            public readonly BindableBool IsVisible = new BindableBool();

            private readonly OverlayColourProvider colourProvider = new OverlayColourProvider(OverlayColourScheme.Pink);

            public SettingsContent()
            {
                RelativeSizeAxes = Axes.Y;
                Width = width;
                Masking = true;
                EdgeEffect = new EdgeEffectParameters
                {
                    Colour = Color4.Black,
                    Radius = 15,
                    Type = EdgeEffectType.Shadow
                };
                InternalChildren = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = colourProvider.Background3
                    },
                    new OsuScrollContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Child = new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            Padding = new MarginPadding { Horizontal = 15 },
                            AutoSizeAxes = Axes.Y,
                            Margin = new MarginPadding { Vertical = 10 },
                            Child = new FillFlowContainer<Section>
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Vertical,
                                Spacing = new Vector2(0, 10),
                                Children = new Section[]
                                {
                                    new TrackSection(),
                                    new RulesetSection(),
                                    new GameSection()
                                }
                            }
                        }
                    }
                };
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                IsVisible.BindValueChanged(visible =>
                {
                    FadeEdgeEffectTo(visible.NewValue ? 0.6f : 0, duration, Easing.OutQuint);
                }, true);
            }
        }
    }
}

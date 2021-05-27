using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
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

            private readonly Box shadow;

            public SettingsContent()
            {
                RelativeSizeAxes = Axes.Y;
                Width = width;
                InternalChildren = new Drawable[]
                {
                    shadow = new Box
                    {
                        RelativeSizeAxes = Axes.Y,
                        Width = 10,
                        Colour = ColourInfo.GradientHorizontal(Color4.Black.Opacity(0), Color4.Black.Opacity(0.4f)),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreRight,
                        X = 1,
                    },
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
                            Child = new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Vertical,
                                Spacing = new Vector2(0, 10),
                                Children = new Drawable[]
                                {
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
                    shadow.FadeTo(visible.NewValue ? 1 : 0, duration, Easing.OutQuint);
                }, true);
            }
        }
    }
}

﻿using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Overlays;
using osu.Game.Rulesets.Mvis.UI.Objects.Helpers;
using osu.Game.Rulesets.Mvis.UI.Objects.MusicVisualizers;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Mvis.Tests
{
    public class TestSceneVisualizerTypes : RulesetTestScene
    {
        private readonly NowPlayingOverlay nowPlayingOverlay;
        private readonly Visuals visuals;

        public TestSceneVisualizerTypes()
        {
            AddRange(new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black
                },
                visuals = new Visuals(),
                nowPlayingOverlay = new NowPlayingOverlay
                {
                    Origin = Anchor.TopRight,
                    Anchor = Anchor.TopRight,
                }
            });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            AddStep("Toggle visibility", nowPlayingOverlay.ToggleVisibility);
            AddSliderStep("Bar count", 0, 3500, 3500, count => visuals.ForEach(v => ((MusicVisualizerDrawable)v).BarCount.Value = count));
            AddSliderStep("Bar width", 1f, 50f, 1f, width => visuals.ForEach(v => ((MusicVisualizerDrawable)v).BarWidth.Value = width));
            AddSliderStep("Degree value", 0f, 360f, 360f, degree => visuals.ForEach(v =>
            {
                if (v is CircularMusicVisualizerDrawable c)
                    c.DegreeValue.Value = degree;
            }));
            AddToggleStep("Reversed", r => visuals.ForEach(v => ((MusicVisualizerDrawable)v).Reversed.Value = r));
        }

        private class Visuals : MusicAmplitudesProvider
        {
            public Visuals()
            {
                RelativeSizeAxes = Axes.Both;
                Children = new Drawable[]
                {
                    new LinearMusicVisualizerDrawable
                    {
                        BarAnchor = { Value = BarAnchor.Centre }
                    },
                    new LinearMusicVisualizerDrawable
                    {
                        BarAnchor = { Value = BarAnchor.Top }
                    },
                    new LinearMusicVisualizerDrawable
                    {
                        BarAnchor = { Value = BarAnchor.Bottom }
                    },
                    new BasicMusicVisualizerDrawable
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(200)
                    }
                };
            }

            protected override void OnAmplitudesUpdate(float[] amplitudes)
            {
                foreach (var c in Children)
                    ((MusicVisualizerDrawable)c).SetAmplitudes(amplitudes);
            }
        }
    }
}

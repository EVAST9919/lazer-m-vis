﻿using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.OpenGL.Vertices;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Utils;
using osu.Game.Rulesets.Mvis.Configuration;
using osu.Game.Rulesets.Mvis.Extensions;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Mvis.UI.Objects
{
    public class Particles : Sprite
    {
        private const float min_depth = 1f;
        private const float max_depth = 1000f;
        private const float particle_max_size = 4;
        private const float particle_min_size = 1;

        [Resolved(canBeNull: true)]
        private MvisRulesetConfigManager config { get; set; }

        private readonly Bindable<int> count = new Bindable<int>(1000);
        private readonly Bindable<bool> useCustomColour = new Bindable<bool>();
        private readonly Bindable<int> red = new Bindable<int>(0);
        private readonly Bindable<int> green = new Bindable<int>(0);
        private readonly Bindable<int> blue = new Bindable<int>(0);

        private readonly List<Particle> parts = new List<Particle>();

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            RelativeSizeAxes = Axes.Both;
            Texture = textures.Get("particle");
            Blending = BlendingParameters.Additive;

            config?.BindWith(MvisRulesetSetting.ParticlesCount, count);
            config?.BindWith(MvisRulesetSetting.Red, red);
            config?.BindWith(MvisRulesetSetting.Green, green);
            config?.BindWith(MvisRulesetSetting.Blue, blue);
            config?.BindWith(MvisRulesetSetting.UseCustomColour, useCustomColour);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            count.BindValueChanged(c => Restart(c.NewValue), true);

            red.BindValueChanged(_ => updateColour());
            green.BindValueChanged(_ => updateColour());
            blue.BindValueChanged(_ => updateColour());
            useCustomColour.BindValueChanged(_ => updateColour(), true);
        }

        public void Restart(int particleCount)
        {
            parts.Clear();

            for (int i = 0; i < particleCount; i++)
                parts.Add(new Particle());
        }

        private void updateColour()
        {
            Colour = useCustomColour.Value ? new Color4(red.Value / 255f, green.Value / 255f, blue.Value / 255f, 1) : Color4.White;
        }

        protected override void Update()
        {
            base.Update();

            var timeDiff = (float)Clock.ElapsedFrameTime * 0.05f;

            foreach (var p in parts)
                p.UpdateCurrentPosition(timeDiff);

            Invalidate(Invalidation.DrawNode);
        }

        protected override DrawNode CreateDrawNode() => new ParticleDrawNode(this);

        protected override void Dispose(bool isDisposing)
        {
            parts.Clear();
            base.Dispose(isDisposing);
        }

        private class ParticleDrawNode : SpriteDrawNode
        {
            private readonly List<Particle> parts = new List<Particle>();

            private Particles source => (Particles)Source;

            private Vector2 sourceSize;

            public ParticleDrawNode(Sprite source)
                : base(source)
            {
            }

            public override void ApplyState()
            {
                base.ApplyState();

                parts.Clear();
                parts.AddRange(source.parts);

                sourceSize = source.DrawSize;
            }

            protected override void Blit(Action<TexturedVertex2D> vertexAction)
            {
                foreach (var p in parts)
                {
                    var rect = getPartRectangle(p.CurrentPosition, p.CurrentSize);
                    var quad = getQuad(rect);

                    drawPart(quad, p.CurrentAlpha, vertexAction);
                }
            }

            private void drawPart(Quad quad, float alpha, Action<TexturedVertex2D> vertexAction)
            {
                DrawQuad(Texture, quad, DrawColourInfo.Colour.MultiplyAlpha(alpha), null, vertexAction,
                        new Vector2(InflationAmount.X / DrawRectangle.Width, InflationAmount.Y / DrawRectangle.Height),
                        null, TextureCoords);
            }

            private Quad getQuad(RectangleF rect) => new Quad(
                        Vector2Extensions.Transform(rect.TopLeft, DrawInfo.Matrix),
                        Vector2Extensions.Transform(rect.TopRight, DrawInfo.Matrix),
                        Vector2Extensions.Transform(rect.BottomLeft, DrawInfo.Matrix),
                        Vector2Extensions.Transform(rect.BottomRight, DrawInfo.Matrix)
                    );

            private RectangleF getPartRectangle(Vector2 pos, float size) => new RectangleF(
                        pos.X * sourceSize.X + sourceSize.X / 2 - size / 2,
                        pos.Y * sourceSize.Y + sourceSize.Y / 2 - size / 2,
                        size,
                        size);
        }

        private class Particle
        {
            private Vector2 initialPosition;
            private float initialDepth;
            private bool useFadeIn;
            private float currentDepth;

            public Vector2 CurrentPosition { get; private set; }

            public float CurrentSize { get; private set; }

            public float CurrentAlpha { get; private set; }

            public Particle()
            {
                reset(true, false);
            }

            private void reuse() => reset(false, true);

            private void reset(bool randomDepth, bool useFadeIn)
            {
                this.useFadeIn = useFadeIn;

                initialPosition = new Vector2(RNG.NextSingle(-0.5f, 0.5f) * max_depth, RNG.NextSingle(-0.5f, 0.5f) * max_depth);
                currentDepth = initialDepth = randomDepth ? RNG.NextSingle(min_depth, max_depth) : max_depth;

                CurrentPosition = getCurrentPosition();

                if (outOfBounds)
                {
                    reset(randomDepth, useFadeIn);
                    return;
                }

                updateProperties();
            }

            public void UpdateCurrentPosition(float timeDifference)
            {
                currentDepth -= timeDifference;

                if (currentDepth < min_depth)
                {
                    reuse();
                    return;
                }

                CurrentPosition = getCurrentPosition();

                if (outOfBounds)
                {
                    reuse();
                    return;
                }

                updateProperties();
            }

            private bool outOfBounds => CurrentPosition.X > 0.5f || CurrentPosition.X < -0.5f || CurrentPosition.Y > 0.5f || CurrentPosition.Y < -0.5f;

            private Vector2 getCurrentPosition() => Vector2.Divide(initialPosition, currentDepth);

            private void updateProperties()
            {
                CurrentSize = MathExtensions.Map(currentDepth, max_depth, min_depth, particle_min_size, particle_max_size);

                float newAlpha;

                if (useFadeIn)
                {
                    if (currentDepth <= initialDepth - max_depth / 10)
                        newAlpha = 1;
                    else
                        newAlpha = MathExtensions.Map(currentDepth, initialDepth, initialDepth - max_depth / 10, 0, 1);
                }
                else
                    newAlpha = 1;

                CurrentAlpha = newAlpha;
            }
        }
    }
}

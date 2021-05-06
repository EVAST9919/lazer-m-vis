using System;
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
        private const int min_depth = 1;
        private const int max_depth = 1500;
        private const float particle_max_size = 4;
        private const float particle_min_size = 1;

        [Resolved(canBeNull: true)]
        private MvisRulesetConfigManager config { get; set; }

        private readonly Bindable<int> count = new Bindable<int>(500);
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

            var timeDiff = (float)Clock.ElapsedFrameTime * 0.1f;

            for (int i = 0; i < parts.Count; i++)
            {
                parts[i].UpdateCurrentPosition(timeDiff);

                if (parts[i].CurrentDepth < min_depth)
                {
                    parts[i].Reset(false);
                    continue;
                }

                var position = parts[i].CurrentPosition;

                if (position.X > 0.5f || position.X < -0.5f || position.Y > 0.5f || position.Y < -0.5f)
                    parts[i].Reset(false);

            }

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
                    Vector2 pos = p.CurrentPosition;
                    var size = p.CurrentSize;

                    var rect = new RectangleF(
                        pos.X * sourceSize.X + sourceSize.X / 2 - size / 2,
                        pos.Y * sourceSize.Y + sourceSize.Y / 2 - size / 2,
                        size,
                        size);

                    // convert to screen space.
                    var quad = new Quad(
                        Vector2Extensions.Transform(rect.TopLeft, DrawInfo.Matrix),
                        Vector2Extensions.Transform(rect.TopRight, DrawInfo.Matrix),
                        Vector2Extensions.Transform(rect.BottomLeft, DrawInfo.Matrix),
                        Vector2Extensions.Transform(rect.BottomRight, DrawInfo.Matrix)
                    );

                    DrawQuad(Texture, quad, DrawColourInfo.Colour.MultiplyAlpha(p.CurrentAlpha), null, vertexAction,
                        new Vector2(InflationAmount.X / DrawRectangle.Width, InflationAmount.Y / DrawRectangle.Height),
                        null, TextureCoords);
                }
            }
        }

        private class Particle
        {
            private Vector2 initialPosition;
            private int initialDepth;

            public Vector2 CurrentPosition { get; private set; }

            public float CurrentDepth { get; private set; }

            public float CurrentSize { get; private set; }

            public float CurrentAlpha { get; private set; }

            public Particle()
            {
                Reset(true);
            }

            public void Reset(bool randomDepth)
            {
                CurrentPosition = initialPosition = new Vector2(RNG.NextSingle(-0.5f, 0.5f), RNG.NextSingle(-0.5f, 0.5f));
                CurrentDepth = initialDepth = randomDepth ? RNG.Next(min_depth, max_depth) : max_depth;
                updateSize();
                updateAlpha();
            }

            public void UpdateCurrentPosition(float timeDifference)
            {
                CurrentDepth -= timeDifference;
                CurrentPosition = Vector2.Divide(initialPosition, CurrentDepth / max_depth);
                updateSize();
                updateAlpha();
            }

            private void updateSize()
            {
                CurrentSize = MathExtensions.Map(CurrentDepth, max_depth, min_depth, particle_min_size, particle_max_size);
            }

            private void updateAlpha()
            {
                float newAlpha;

                if (CurrentDepth < min_depth + 10)
                {
                    newAlpha = MathExtensions.Map(CurrentDepth, min_depth, min_depth + 10, 0, 1);
                }
                else
                {
                    if (CurrentDepth <= initialDepth - max_depth / 10)
                        newAlpha = 1;
                    else
                        newAlpha = MathExtensions.Map(CurrentDepth, initialDepth, initialDepth - max_depth / 10, 0, 1);
                }

                CurrentAlpha = newAlpha;
            }
        }
    }
}

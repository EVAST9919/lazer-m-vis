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
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Mvis.UI.Objects
{
    public class Particles : Sprite
    {
        /// <summary>
        /// Adjusts the speed of all the particles.
        /// </summary>
        private const int absolute_time = 5000;

        /// <summary>
        /// The maximum scale of a single particle.
        /// </summary>
        private const float particle_max_scale = 3;

        /// <summary>
        /// Base particle size.
        /// </summary>
        private const float particle_size = 2;

        [Resolved(canBeNull: true)]
        private MvisRulesetConfigManager config { get; set; }

        private readonly Bindable<int> count = new Bindable<int>(500);
        private readonly Bindable<bool> useCustomColour = new Bindable<bool>();
        private readonly Bindable<int> red = new Bindable<int>(0);
        private readonly Bindable<int> green = new Bindable<int>(0);
        private readonly Bindable<int> blue = new Bindable<int>(0);

        private readonly List<ParticlePart> parts = new List<ParticlePart>();

        public Particles(Texture texture)
        {
            RelativeSizeAxes = Axes.Both;
            Texture = texture;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
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

        private void updateColour()
        {
            Colour = useCustomColour.Value ? new Color4(red.Value / 255f, green.Value / 255f, blue.Value / 255f, 1) : Color4.White;
        }

        public void Restart(int particleCount)
        {
            Scheduler.CancelDelayedTasks();
            parts.Clear();

            for (int i = 0; i < particleCount; i++)
                createParticle();
        }

        private void createParticle()
        {
            var initialPosition = new Vector2(RNG.NextSingle(-0.5f, 0.5f), RNG.NextSingle(-0.5f, 0.5f));
            var depth = RNG.NextSingle(0.25f, 1);

            float finalX;
            float finalY;
            float ratio;

            if (Math.Abs(initialPosition.X) > Math.Abs(initialPosition.Y))
            {
                ratio = Math.Abs(initialPosition.X) / 0.51f;
                finalX = initialPosition.X > 0 ? 0.51f : -0.51f;
                finalY = initialPosition.Y / ratio;
            }
            else
            {
                ratio = Math.Abs(initialPosition.Y) / 0.51f;
                finalY = initialPosition.Y > 0 ? 0.51f : -0.51f;
                finalX = initialPosition.X / ratio;
            }

            var finalPosition = new Vector2(finalX, finalY);
            var lifeTime = absolute_time * (1 - ratio) / depth;

            var particleToAdd = new ParticlePart(initialPosition, finalPosition, depth, ratio, lifeTime, Time.Current);
            parts.Add(particleToAdd);

            var hash = particleToAdd.GetHash();

            Scheduler.AddDelayed(() =>
            {
                // Removes particle which exceeded it's lifetime. Works a lot faster than "parts.Remove(particleToAdd)".
                for (int i = 0; i < parts.Count; i++)
                {
                    if (parts[i].GetHash() == hash)
                        parts.RemoveAt(i);
                }

                createParticle();
            }, lifeTime);
        }

        protected override void Update()
        {
            base.Update();
            Invalidate(Invalidation.DrawNode);
        }

        protected override DrawNode CreateDrawNode() => new ParticleDrawNode(this);

        protected override void Dispose(bool isDisposing)
        {
            Scheduler.CancelDelayedTasks();
            parts.Clear();
            base.Dispose(isDisposing);
        }

        private class ParticleDrawNode : SpriteDrawNode
        {
            private readonly List<ParticlePart> parts = new List<ParticlePart>();

            private Particles source => (Particles)Source;

            private double currentTime;
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
                currentTime = source.Time.Current;
            }

            protected override void Blit(Action<TexturedVertex2D> vertexAction)
            {
                foreach (var p in parts)
                {
                    var time = currentTime - p.GetStartTime();

                    Vector2 pos = p.PositionAtTime(time);
                    float alpha = p.AlphaAtTime(time);
                    var scale = p.ScaleAtTime(time);

                    var rect = new RectangleF(
                        pos.X * sourceSize.X + sourceSize.X / 2 - particle_size * scale.X / 2,
                        pos.Y * sourceSize.Y + sourceSize.Y / 2 - particle_size * scale.Y / 2,
                        particle_size * scale.X,
                        particle_size * scale.Y);

                    // convert to screen space.
                    var quad = new Quad(
                        Vector2Extensions.Transform(rect.TopLeft, DrawInfo.Matrix),
                        Vector2Extensions.Transform(rect.TopRight, DrawInfo.Matrix),
                        Vector2Extensions.Transform(rect.BottomLeft, DrawInfo.Matrix),
                        Vector2Extensions.Transform(rect.BottomRight, DrawInfo.Matrix)
                    );

                    DrawQuad(Texture, quad, DrawColourInfo.Colour.MultiplyAlpha(alpha), null, vertexAction,
                        new Vector2(InflationAmount.X / DrawRectangle.Width, InflationAmount.Y / DrawRectangle.Height),
                        null, TextureCoords);
                }
            }
        }

        private readonly struct ParticlePart
        {
            private readonly Vector2 initialPosition;
            private readonly Vector2 finalPosition;
            private readonly Vector2 initialScale;
            private readonly Vector2 finalScale;
            private readonly double lifeTime;
            private readonly double startTime;

            public ParticlePart(Vector2 initialPosition, Vector2 finalPosition, float depth, float ratio, double lifeTime, double startTime)
            {
                this.initialPosition = initialPosition;
                this.finalPosition = finalPosition;
                this.lifeTime = lifeTime;
                this.startTime = startTime;

                initialScale = new Vector2(depth);
                finalScale = new Vector2(1 + ((particle_max_scale - 1) * depth * (1 - ratio)));
            }

            public float AlphaAtTime(double time) => (float)Math.Clamp(time / Math.Min(lifeTime, 500), 0, 1);

            public Vector2 PositionAtTime(double time) => Vector2.Lerp(initialPosition, finalPosition, (float)Math.Clamp(time / lifeTime, 0, 1));

            public Vector2 ScaleAtTime(double time) => Vector2.Lerp(initialScale, finalScale, (float)Math.Clamp(time / lifeTime, 0, 1));

            public double GetStartTime() => startTime;

            public double GetLifeTime() => lifeTime;

            public int GetHash() => (int)Math.Ceiling(startTime * lifeTime * finalScale.X);
        }
    }
}

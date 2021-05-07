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
    public class ParticlesDrawable : Sprite
    {
        private const float min_depth = 1f;
        private const float max_depth = 1000f;
        private const float particle_max_size = 4;
        private const float particle_min_size = 0.5f;
        private const float speed_multiplier = 0.05f;

        public readonly Bindable<Direction> Direction = new Bindable<Direction>();

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

        public void SetRandomDirection()
        {
            var count = Enum.GetValues(typeof(Direction)).Length;
            var newDirection = (Direction)RNG.Next(count);

            if (Direction.Value == newDirection)
            {
                SetRandomDirection();
                return;
            }

            Direction.Value = newDirection;
        }

        private void updateColour()
        {
            Colour = useCustomColour.Value ? new Color4(red.Value / 255f, green.Value / 255f, blue.Value / 255f, 1) : Color4.White;
        }

        protected override void Update()
        {
            base.Update();

            var timeDiff = (float)Clock.ElapsedFrameTime * speed_multiplier;

            foreach (var p in parts)
                p.UpdateCurrentPosition(timeDiff, Direction.Value);

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

            private ParticlesDrawable source => (ParticlesDrawable)Source;

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
            private float currentDepth;

            public Vector2 CurrentPosition { get; private set; }

            public float CurrentSize { get; private set; }

            public float CurrentAlpha { get; private set; }

            public bool Backwards { get; set; }

            public Particle()
            {
                reset(false, Objects.Direction.Forward);
            }

            private void reset(bool maxDepth, Direction direction)
            {
                switch (direction)
                {
                    default:
                    case Objects.Direction.Backwards:
                        CurrentPosition = getRandomPositionOnTheEdge();
                        currentDepth = RNG.NextSingle(max_depth / 10, max_depth);
                        initialPosition = CurrentPosition * currentDepth;
                        break;

                    case Objects.Direction.Forward:
                        currentDepth = maxDepth ? max_depth : RNG.NextSingle(min_depth, max_depth);
                        initialPosition = new Vector2(RNG.NextSingle(-0.5f, 0.5f), RNG.NextSingle(-0.5f, 0.5f)) * max_depth;
                        CurrentPosition = getCurrentPosition();

                        if (outOfBounds)
                        {
                            reset(maxDepth, direction);
                            return;
                        }

                        break;
                }

                updateProperties();
            }

            public void UpdateCurrentPosition(float timeDifference, Direction direction)
            {
                switch (direction)
                {
                    default:
                    case Objects.Direction.Forward:
                        currentDepth -= timeDifference;

                        if (currentDepth < min_depth)
                        {
                            reset(true, direction);
                            return;
                        }

                        CurrentPosition = getCurrentPosition();

                        if (outOfBounds)
                        {
                            reset(true, direction);
                            return;
                        }

                        break;

                    case Objects.Direction.Backwards:
                        currentDepth += timeDifference;

                        if (currentDepth > max_depth)
                        {
                            reset(false, direction);
                            return;
                        }

                        CurrentPosition = getCurrentPosition();

                        break;
                }

                updateProperties();
            }

            private void updateProperties()
            {
                CurrentSize = MathExtensions.Map(currentDepth, max_depth, min_depth, particle_min_size, particle_max_size);
                CurrentAlpha = CurrentSize < 1 ? MathExtensions.Map(CurrentSize, particle_min_size, 1, 0, 1) : 1;
            }

            private Vector2 getCurrentPosition() => Vector2.Divide(initialPosition, currentDepth);

            private static Vector2 getRandomPositionOnTheEdge()
            {
                float x = 0;
                float y = 0;

                var side = RNG.Next(4);

                if (side == 0)
                {
                    x = -0.5f;
                    y = RNG.NextSingle(-0.5f, 0.5f);
                }
                else if (side == 1)
                {
                    y = -0.5f;
                    x = RNG.NextSingle(-0.5f, 0.5f);
                }
                else if (side == 2)
                {
                    x = 0.5f;
                    y = RNG.NextSingle(-0.5f, 0.5f);
                }
                else if (side == 3)
                {
                    y = 0.5f;
                    x = RNG.NextSingle(-0.5f, 0.5f);
                }

                return new Vector2(x, y);
            }

            private bool outOfBounds => CurrentPosition.X > 0.5f || CurrentPosition.X < -0.5f || CurrentPosition.Y > 0.5f || CurrentPosition.Y < -0.5f;
        }
    }

    public enum Direction
    {
        Forward,
        Backwards
    }
}

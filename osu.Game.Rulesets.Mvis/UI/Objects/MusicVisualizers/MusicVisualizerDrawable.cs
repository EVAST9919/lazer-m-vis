﻿using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Batches;
using osu.Framework.Graphics.OpenGL.Vertices;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Mvis.Extensions;
using osuTK;

namespace osu.Game.Rulesets.Mvis.UI.Objects.MusicVisualizers
{
    public class MusicVisualizerDrawable : Drawable
    {
        // Total amplitude count is 256, however in most cases some of them are empty, let's not use them.
        private const int used_amplitude_count = 200;

        public readonly Bindable<float> DegreeValue = new Bindable<float>();
        public readonly Bindable<double> BarWidth = new Bindable<double>();
        public readonly Bindable<int> BarCount = new Bindable<int>();

        private IShader shader;
        private readonly Texture texture;

        public MusicVisualizerDrawable()
        {
            texture = Texture.WhitePixel;
        }

        [BackgroundDependencyLoader]
        private void load(ShaderManager shaders)
        {
            shader = shaders.Load(VertexShaderDescriptor.TEXTURE_2, FragmentShaderDescriptor.TEXTURE_ROUNDED);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            BarCount.BindValueChanged(c =>
            {
                barCount = c.NewValue;
                Restart();
            }, true);
        }

        private readonly List<float> audioData = new List<float>();
        private readonly List<float> decays = new List<float>();

        private int barCount;

        public void Restart()
        {
            audioData.Clear();
            decays.Clear();

            for (int i = 0; i < barCount; i++)
            {
                audioData.Add(0);
                decays.Add(0);
            }
        }

        public void SetAmplitudes(float[] amplitudes, double timeDifference)
        {
            var amps = getConvertedAmplitudes(amplitudes);

            amps.Smooth(Math.Max((int)Math.Round(barCount * 0.005f * 360f / DegreeValue.Value), 1));

            for (int i = 0; i < barCount; i++)
                audioData[i] = getNewHeight(i, amps[i], 400, 200, timeDifference);

            Invalidate(Invalidation.DrawNode);
        }

        private float[] getConvertedAmplitudes(float[] amplitudes)
        {
            var amps = new float[barCount];

            for (int i = 0; i < barCount; i++)
                amps[i] = amplitudes[getAmpIndexForBar(i)];

            return amps;
        }

        private int getAmpIndexForBar(int barIndex) => (int)Math.Round((float)used_amplitude_count / barCount * barIndex);

        private float getNewHeight(int index, float amplitudeValue, float valueMultiplier, float smootheness, double timeDifference)
        {
            var oldHeight = audioData[index];
            var newHeight = amplitudeValue * valueMultiplier;

            if (newHeight >= oldHeight)
                decays[index] = newHeight / smootheness;
            else
                newHeight = oldHeight - decays[index] * (float)timeDifference;

            return newHeight;
        }

        protected override DrawNode CreateDrawNode() => new VisualizerDrawNode(this);

        private class VisualizerDrawNode : DrawNode
        {
            protected new MusicVisualizerDrawable Source => (MusicVisualizerDrawable)base.Source;

            private IShader shader;
            private Texture texture;
            private float size;
            private float degreeValue;
            private double barWidth;
            private List<float> audioData;

            private readonly QuadBatch<TexturedVertex2D> vertexBatch = new QuadBatch<TexturedVertex2D>(100, 10);

            public VisualizerDrawNode(MusicVisualizerDrawable source)
                : base(source)
            {
            }

            public override void ApplyState()
            {
                base.ApplyState();

                shader = Source.shader;
                texture = Source.texture;
                size = Source.DrawSize.X;
                degreeValue = Source.DegreeValue.Value;
                barWidth = Source.BarWidth.Value;
                audioData = Source.audioData;
            }

            public override void Draw(Action<TexturedVertex2D> vertexAction)
            {
                base.Draw(vertexAction);

                shader.Bind();
                Vector2 inflation = DrawInfo.MatrixInverse.ExtractScale().Xy;

                if (audioData != null)
                {
                    float spacing = degreeValue / audioData.Count;

                    for (int i = 0; i < audioData.Count; i++)
                    {
                        float rotation = MathHelper.DegreesToRadians(i * spacing - 90);
                        float rotationCos = MathF.Cos(rotation);
                        float rotationSin = MathF.Sin(rotation);

                        // taking the cos and sin to the 0..1 range
                        var barPosition = new Vector2(rotationCos / 2 + 0.5f, rotationSin / 2 + 0.5f) * size;

                        var barSize = new Vector2((float)barWidth, 2 + audioData[i]);

                        // The distance between the position and the sides of the bar.
                        var bottomOffset = new Vector2(-rotationSin * barSize.X / 2, rotationCos * barSize.X / 2);
                        // The distance between the bottom side of the bar and the top side.
                        var amplitudeOffset = new Vector2(rotationCos * barSize.Y, rotationSin * barSize.Y);

                        var rectangle = new Quad(
                                Vector2Extensions.Transform(barPosition - bottomOffset, DrawInfo.Matrix),
                                Vector2Extensions.Transform(barPosition - bottomOffset + amplitudeOffset, DrawInfo.Matrix),
                                Vector2Extensions.Transform(barPosition + bottomOffset, DrawInfo.Matrix),
                                Vector2Extensions.Transform(barPosition + bottomOffset + amplitudeOffset, DrawInfo.Matrix)
                            );

                        DrawQuad(
                            texture,
                            rectangle,
                            DrawColourInfo.Colour,
                            null,
                            vertexBatch.AddAction,
                            // barSize by itself will make it smooth more in the X axis than in the Y axis, this reverts that.
                            Vector2.Divide(inflation, barSize.Yx));
                    }
                }
            }

            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);
                vertexBatch.Dispose();
            }
        }
    }
}

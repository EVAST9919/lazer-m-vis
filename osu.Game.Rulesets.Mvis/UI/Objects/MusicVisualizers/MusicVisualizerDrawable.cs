﻿using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Batches;
using osu.Framework.Graphics.OpenGL.Vertices;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Mvis.Extensions;

namespace osu.Game.Rulesets.Mvis.UI.Objects.MusicVisualizers
{
    public abstract class MusicVisualizerDrawable : Drawable
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
                restart();
            }, true);
        }

        private readonly List<float> audioData = new List<float>();
        private readonly List<float> decays = new List<float>();

        private int barCount;

        private void restart()
        {
            ClearData();

            for (int i = 0; i < barCount; i++)
                AddEmptyDataValue();
        }

        protected virtual void ClearData()
        {
            audioData.Clear();
            decays.Clear();
        }

        protected virtual void AddEmptyDataValue()
        {
            audioData.Add(0);
            decays.Add(0);
        }

        public void SetAmplitudes(float[] amplitudes, double timeDifference)
        {
            var amps = getConvertedAmplitudes(amplitudes);
            amps.Smooth(Math.Max((int)Math.Round(barCount * 0.005f * 360f / DegreeValue.Value), 1));

            for (int i = 0; i < barCount; i++)
                ApplyData(i, amps[i], timeDifference);

            Invalidate(Invalidation.DrawNode);
        }

        protected virtual void ApplyData(int index, float data, double timeDifference)
        {
            audioData[index] = getNewHeight(index, data, 400, 200, timeDifference);
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

        protected override DrawNode CreateDrawNode() => CreateVisualizerDrawNode();

        protected abstract VisualizerDrawNode CreateVisualizerDrawNode();

        protected abstract class VisualizerDrawNode<T> : VisualizerDrawNode
            where T : MusicVisualizerDrawable
        {
            protected new T Source => (T)base.Source;

            private IShader shader;
            protected Texture Texture;
            protected float Size;
            protected float DegreeValue;
            protected double BarWidth;

            protected readonly List<float> AudioData = new List<float>();
            protected readonly QuadBatch<TexturedVertex2D> VertexBatch = new QuadBatch<TexturedVertex2D>(100, 10);

            public VisualizerDrawNode(T source)
                : base(source)
            {
            }

            public override void ApplyState()
            {
                base.ApplyState();

                shader = Source.shader;
                Texture = Source.texture;
                Size = Source.DrawSize.X;
                DegreeValue = Source.DegreeValue.Value;
                BarWidth = Source.BarWidth.Value;

                AudioData.Clear();
                AudioData.AddRange(Source.audioData);
            }

            public override void Draw(Action<TexturedVertex2D> vertexAction)
            {
                base.Draw(vertexAction);

                shader.Bind();
                DrawNode(vertexAction);
                shader.Unbind();
            }

            protected abstract void DrawNode(Action<TexturedVertex2D> vertexAction);

            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);
                VertexBatch.Dispose();
            }
        }

        protected abstract class VisualizerDrawNode : DrawNode
        {
            public VisualizerDrawNode(MusicVisualizerDrawable source)
                : base(source)
            {
            }
        }
    }
}

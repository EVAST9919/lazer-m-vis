using System;
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
                ResetArrays(barCount);
            }, true);
        }

        private float[] currentRawAudioData;
        private float[] maxBarValues;
        private float[] smoothAudioData;

        private int barCount;

        protected virtual void ResetArrays(int barCount)
        {
            currentRawAudioData = new float[barCount];
            maxBarValues = new float[barCount];
            smoothAudioData = new float[barCount];
        }

        public void SetAmplitudes(float[] amplitudes)
        {
            var newRawAudioData = getConvertedAmplitudes(amplitudes);

            for (int i = 0; i < barCount; i++)
                ApplyData(i, newRawAudioData[i]);
        }

        protected virtual void ApplyData(int index, float newRawAudioDataAtIndex)
        {
            if (newRawAudioDataAtIndex > currentRawAudioData[index])
            {
                currentRawAudioData[index] = newRawAudioDataAtIndex;
                maxBarValues[index] = currentRawAudioData[index];
            }
        }

        protected override void Update()
        {
            base.Update();

            var diff = (float)Clock.ElapsedFrameTime;

            for (int i = 0; i < barCount; i++)
                UpdateData(i, diff);

            PostUpdate();

            Invalidate(Invalidation.DrawNode);
        }

        protected virtual void UpdateData(int index, float timeDifference)
        {
            currentRawAudioData[index] -= maxBarValues[index] / 200 * timeDifference;
            smoothAudioData[index] = currentRawAudioData[index] * 400;
        }

        protected virtual void PostUpdate()
        {
            smoothAudioData.Smooth(Math.Max((int)Math.Round(barCount * 0.003f * 360f / DegreeValue.Value), 1));
        }

        private float[] getConvertedAmplitudes(float[] amplitudes)
        {
            var amps = new float[barCount];

            for (int i = 0; i < barCount; i++)
                amps[i] = amplitudes[getAmpIndexForBar(i)];

            return amps;
        }

        private int getAmpIndexForBar(int barIndex) => (int)Math.Round((float)used_amplitude_count / barCount * barIndex);

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
            protected readonly QuadBatch<TexturedVertex2D> VertexBatch = new QuadBatch<TexturedVertex2D>(200, 5);

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
                AudioData.AddRange(Source.smoothAudioData);
            }

            public override void Draw(Action<TexturedVertex2D> vertexAction)
            {
                base.Draw(vertexAction);

                shader.Bind();
                DrawNode();
                shader.Unbind();
            }

            protected abstract void DrawNode();

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

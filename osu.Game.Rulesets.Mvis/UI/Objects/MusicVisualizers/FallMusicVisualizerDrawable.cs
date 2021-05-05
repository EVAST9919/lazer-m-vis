using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.OpenGL.Vertices;
using osu.Framework.Graphics.Primitives;
using osuTK;

namespace osu.Game.Rulesets.Mvis.UI.Objects.MusicVisualizers
{
    public class FallMusicVisualizerDrawable : MusicVisualizerDrawable
    {
        private readonly List<float> fallAudioData = new List<float>();
        private readonly List<float> fallDecays = new List<float>();

        protected override void ClearData()
        {
            base.ClearData();
            fallAudioData.Clear();
            fallDecays.Clear();
        }

        protected override void AddEmptyDataValue()
        {
            base.AddEmptyDataValue();
            fallAudioData.Add(0);
            fallDecays.Add(0);
        }

        protected override void ApplyData(int index, float data, double timeDifference)
        {
            base.ApplyData(index, data, timeDifference);
            fallAudioData[index] = getNewY(index, data, 400, 1200, timeDifference);
        }

        private float getNewY(int index, float amplitudeValue, float valueMultiplier, float smootheness, double timeDifference)
        {
            var oldY = fallAudioData[index];
            var newY = amplitudeValue * valueMultiplier;

            if (newY >= oldY)
                fallDecays[index] = newY / smootheness;
            else
                newY = oldY - fallDecays[index] * (float)timeDifference;

            return newY;
        }

        protected override VisualizerDrawNode CreateVisualizerDrawNode() => new FallVisualizerDrawNode(this);

        private class FallVisualizerDrawNode : VisualizerDrawNode<FallMusicVisualizerDrawable>
        {
            public FallVisualizerDrawNode(FallMusicVisualizerDrawable source)
                : base(source)
            {
            }

            private readonly List<float> fallAudioData = new List<float>();

            public override void ApplyState()
            {
                base.ApplyState();

                fallAudioData.Clear();
                fallAudioData.AddRange(Source.fallAudioData);
            }

            protected override void DrawNode(Action<TexturedVertex2D> vertexAction)
            {
                Vector2 inflation = DrawInfo.MatrixInverse.ExtractScale().Xy;

                if (AudioData != null)
                {
                    float spacing = DegreeValue / AudioData.Count;

                    for (int i = 0; i < AudioData.Count; i++)
                    {
                        float rotation = MathHelper.DegreesToRadians(i * spacing - 90);
                        float rotationCos = MathF.Cos(rotation);
                        float rotationSin = MathF.Sin(rotation);

                        var barPosition = new Vector2(rotationCos / 2 + 0.5f, rotationSin / 2 + 0.5f) * Size;
                        var barSize = new Vector2((float)BarWidth, 2 + AudioData[i]);

                        var bottomOffset = new Vector2(-rotationSin * barSize.X / 2, rotationCos * barSize.X / 2);
                        var amplitudeOffset = new Vector2(rotationCos * barSize.Y, rotationSin * barSize.Y);

                        var rectangle = new Quad(
                                Vector2Extensions.Transform(barPosition - bottomOffset, DrawInfo.Matrix),
                                Vector2Extensions.Transform(barPosition - bottomOffset + amplitudeOffset, DrawInfo.Matrix),
                                Vector2Extensions.Transform(barPosition + bottomOffset, DrawInfo.Matrix),
                                Vector2Extensions.Transform(barPosition + bottomOffset + amplitudeOffset, DrawInfo.Matrix)
                            );

                        DrawQuad(
                            Texture,
                            rectangle,
                            DrawColourInfo.Colour,
                            null,
                            VertexBatch.AddAction,
                            Vector2.Divide(inflation, barSize.Yx));

                        // Fall bar

                        var scale = (fallAudioData[i] * 2 + Size) / Size;
                        var multiplier = 1f / (scale * 2);

                        var fallBarPosition = new Vector2(rotationCos / 2 + multiplier, rotationSin / 2 + multiplier) * Size * scale;
                        var fallBarSize = new Vector2((float)BarWidth, 2);

                        var fallBarBottomOffset = new Vector2(-rotationSin * fallBarSize.X / 2, rotationCos * fallBarSize.X / 2);
                        var fallBarAmplitudeOffset = new Vector2(rotationCos * fallBarSize.Y, rotationSin * fallBarSize.Y);

                        var fallBarRectangle = new Quad(
                                Vector2Extensions.Transform(fallBarPosition - fallBarBottomOffset, DrawInfo.Matrix),
                                Vector2Extensions.Transform(fallBarPosition - fallBarBottomOffset + fallBarAmplitudeOffset, DrawInfo.Matrix),
                                Vector2Extensions.Transform(fallBarPosition + fallBarBottomOffset, DrawInfo.Matrix),
                                Vector2Extensions.Transform(fallBarPosition + fallBarBottomOffset + fallBarAmplitudeOffset, DrawInfo.Matrix)
                            );

                        DrawQuad(
                            Texture,
                            fallBarRectangle,
                            DrawColourInfo.Colour,
                            null,
                            VertexBatch.AddAction,
                            Vector2.Divide(inflation, fallBarSize.Yx));
                    }
                }
            }
        }
    }
}

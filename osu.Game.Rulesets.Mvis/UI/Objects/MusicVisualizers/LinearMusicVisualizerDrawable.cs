using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osuTK;

namespace osu.Game.Rulesets.Mvis.UI.Objects.MusicVisualizers
{
    public class LinearMusicVisualizerDrawable : MusicVisualizerDrawable
    {
        public LinearMusicVisualizerDrawable()
        {
            RelativeSizeAxes = Axes.Both;
        }

        protected override VisualizerDrawNode CreateVisualizerDrawNode() => new LinearVisualizerDrawNode(this);

        private class LinearVisualizerDrawNode : VisualizerDrawNode
        {
            public LinearVisualizerDrawNode(LinearMusicVisualizerDrawable source)
                : base(source)
            {
            }

            protected override float Spacing => Size.X / AudioData.Count;

            protected override void DrawBar(int index, float spacing, Vector2 inflation)
            {
                var barPosition = new Vector2(index * spacing, Size.Y);
                var barSize = new Vector2((float)BarWidth, 2 + AudioData[index]);

                var rectangle = new Quad(
                        Vector2Extensions.Transform(barPosition, DrawInfo.Matrix),
                        Vector2Extensions.Transform(barPosition + new Vector2(0, -barSize.Y), DrawInfo.Matrix),
                        Vector2Extensions.Transform(barPosition + new Vector2(barSize.X, 0), DrawInfo.Matrix),
                        Vector2Extensions.Transform(barPosition + new Vector2(barSize.X, -barSize.Y), DrawInfo.Matrix)
                    );

                DrawQuad(
                    Texture,
                    rectangle,
                    DrawColourInfo.Colour,
                    null,
                    VertexBatch.AddAction,
                    Vector2.Divide(inflation, barSize.Yx));
            }
        }
    }
}

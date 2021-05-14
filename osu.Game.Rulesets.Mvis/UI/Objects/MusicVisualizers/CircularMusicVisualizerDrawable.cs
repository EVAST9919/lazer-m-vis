using osu.Framework.Bindables;
using osuTK;

namespace osu.Game.Rulesets.Mvis.UI.Objects.MusicVisualizers
{
    public abstract class CircularMusicVisualizerDrawable : MusicVisualizerDrawable
    {
        public readonly Bindable<float> DegreeValue = new Bindable<float>();

        protected override float SmoothMultiplier => 360f / DegreeValue.Value;

        protected override VisualizerDrawNode CreateVisualizerDrawNode() => CreateCircularVisualizerDrawNode();

        protected abstract CircularVisualizerDrawNode CreateCircularVisualizerDrawNode();

        protected abstract class CircularVisualizerDrawNode : VisualizerDrawNode
        {
            protected new CircularMusicVisualizerDrawable Source => (CircularMusicVisualizerDrawable)base.Source;

            protected float DegreeValue;

            public CircularVisualizerDrawNode(CircularMusicVisualizerDrawable source)
                : base(source)
            {
            }

            public override void ApplyState()
            {
                base.ApplyState();
                DegreeValue = Source.DegreeValue.Value;
            }

            protected override float Spacing => DegreeValue / AudioData.Count;

            protected override Vector2 Inflation => DrawInfo.MatrixInverse.ExtractScale().Xy;
        }
    }
}

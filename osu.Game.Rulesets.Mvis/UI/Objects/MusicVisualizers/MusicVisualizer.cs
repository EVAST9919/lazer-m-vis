using osu.Game.Rulesets.Mvis.UI.Objects.Helpers;
using osu.Framework.Graphics;
using osu.Framework.Bindables;

namespace osu.Game.Rulesets.Mvis.UI.Objects.MusicVisualizers
{
    public class MusicVisualizer : MusicAmplitudesProvider
    {
        public readonly Bindable<float> DegreeValue = new Bindable<float>(100);
        public readonly Bindable<double> BarWidth = new Bindable<double>(3.0);
        public readonly Bindable<int> BarCount = new Bindable<int>(20);

        private readonly MusicVisualizerDrawable drawable;

        public MusicVisualizer()
        {
            Add(drawable = new MusicVisualizerDrawable()
            {
                RelativeSizeAxes = Axes.Both,
                DegreeValue = { BindTarget = DegreeValue },
                BarWidth = { BindTarget = BarWidth },
                BarCount = { BindTarget = BarCount }
            });
        }

        protected override void OnAmplitudesUpdate(float[] amplitudes) => drawable.UpdateAmplitudes(amplitudes);
    }
}

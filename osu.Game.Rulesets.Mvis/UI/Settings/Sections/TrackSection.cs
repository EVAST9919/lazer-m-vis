using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Overlays.Settings;

namespace osu.Game.Rulesets.Mvis.UI.Settings.Sections
{
    public class TrackSection : Section
    {
        protected override string HeaderName => "Track Settings";

        [Resolved]
        private Bindable<WorkingBeatmap> working { get; set; }

        private readonly BindableBool loopCurrent = new BindableBool();

        [BackgroundDependencyLoader]
        private void load()
        {
            AddRange(new Drawable[]
            {
                new SettingsCheckbox
                {
                    LabelText = "Loop Current Track",
                    Current = loopCurrent
                }
            });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            loopCurrent.BindValueChanged(loop => updateLooping(working.Value, loop.NewValue));
            working.BindValueChanged(w => updateLooping(w.NewValue, loopCurrent.Value), true);
        }

        private static void updateLooping(WorkingBeatmap beatmap, bool isLooping)
        {
            if (beatmap != null && beatmap.Track != null)
                beatmap.Track.Looping = isLooping;
        }

        protected override void Dispose(bool isDisposing)
        {
            updateLooping(working.Value, false);
            base.Dispose(isDisposing);
        }
    }
}

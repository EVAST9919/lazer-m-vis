using osu.Game.Beatmaps;
using osu.Game.Replays.Legacy;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Replays.Types;

namespace osu.Game.Rulesets.Mvis.Replays
{
    public class MvisReplayFrame : ReplayFrame, IConvertibleReplayFrame
    {
        public MvisReplayFrame()
        {
        }

        public MvisReplayFrame(double time)
            : base(time)
        {
        }

        public void FromLegacy(LegacyReplayFrame currentFrame, IBeatmap beatmap, ReplayFrame lastFrame = null)
        {
        }

        public LegacyReplayFrame ToLegacy(IBeatmap beatmap)
        {
            ReplayButtonState state = ReplayButtonState.None;
            return new LegacyReplayFrame(Time, null, null, state);
        }
    }
}

using osu.Game.Replays;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.Mvis.Replays
{
    public class MvisFramedReplayInputHandler : FramedReplayInputHandler<MvisReplayFrame>
    {
        public MvisFramedReplayInputHandler(Replay replay)
            : base(replay)
        {
        }
    }
}

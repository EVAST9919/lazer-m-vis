using osu.Game.Beatmaps;
using osu.Game.Replays;
using osu.Game.Rulesets.Mvis.Beatmaps;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.Mvis.Replays
{
    internal class MvisAutoGenerator : AutoGenerator
    {
        public new MvisBeatmap Beatmap => (MvisBeatmap)base.Beatmap;

        public MvisAutoGenerator(IBeatmap beatmap)
            : base(beatmap)
        {
            Replay = new Replay();
        }

        protected Replay Replay;

        public override Replay Generate()
        {
            Replay.Frames.Add(new MvisReplayFrame(0));
            return Replay;
        }
    }
}

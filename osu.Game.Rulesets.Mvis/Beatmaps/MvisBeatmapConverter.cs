using System.Collections.Generic;
using System.Threading;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mvis.Objects;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Mvis.Beatmaps
{
    public class MvisBeatmapConverter : BeatmapConverter<MvisHitObject>
    {
        public MvisBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
            : base(beatmap, ruleset)
        {
        }

        public override bool CanConvert() => true;

        protected override Beatmap<MvisHitObject> CreateBeatmap() => new MvisBeatmap();

        protected override IEnumerable<MvisHitObject> ConvertHitObject(HitObject obj, IBeatmap beatmap, CancellationToken token)
            => new MvisHitObject
            {
                StartTime = obj.StartTime,
                Samples = obj.Samples
            }.Yield();
    }
}

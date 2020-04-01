// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
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

        protected override IEnumerable<MvisHitObject> ConvertHitObject(HitObject obj, IBeatmap beatmap)
        {
            switch (obj)
            {
                default:
                    return new MvisHitObject().Yield();
            }
        }
    }
}

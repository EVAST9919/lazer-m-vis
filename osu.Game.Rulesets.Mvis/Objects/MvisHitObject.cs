// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Mvis.Objects
{
    public class MvisHitObject : HitObject
    {
        protected override HitWindows CreateHitWindows() => HitWindows.Empty;
    }
}

// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mvis.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Mvis.UI
{
    public class MvisPlayfield : Playfield
    {
        public MvisPlayfield(BeatmapDifficulty difficulty, Func<MvisHitObject, DrawableHitObject<MvisHitObject>> createDrawableRepresentation)
        {
            InternalChildren = new Drawable[]
            {
                HitObjectContainer
            };
        }
    }
}

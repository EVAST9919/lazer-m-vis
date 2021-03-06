﻿using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Mvis.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Mvis.Objects
{
    public class MvisHitObject : HitObject
    {
        protected override HitWindows CreateHitWindows() => HitWindows.Empty;

        public override Judgement CreateJudgement() => new MvisJudgement();
    }
}

// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Mvis.Judgements
{
    public class MvisJudgement : Judgement
    {
        public override HitResult MaxResult => HitResult.Perfect;
    }
}

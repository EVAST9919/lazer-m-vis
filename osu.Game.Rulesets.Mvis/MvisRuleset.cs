using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI;
using System;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Reza.UI;
using osu.Game.Rulesets.Mvis.Scoring;
using osu.Game.Rulesets.Mvis.Beatmaps;
using osu.Game.Rulesets.Mvis.Difficulty;
using osu.Game.Rulesets.Mvis.Mods;
using osu.Game.Rulesets.Configuration;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mvis.Configuration;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Mvis.UI;

namespace osu.Game.Rulesets.Mvis
{
    public class MvisRuleset : Ruleset
    {
        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod> mods = null) => new DrawableMvisRuleset(this, beatmap, mods);

        public override ScoreProcessor CreateScoreProcessor() => new MvisScoreProcessor();

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) => new MvisBeatmapConverter(beatmap, this);

        public override HealthProcessor CreateHealthProcessor(double drainStartTime) => new MvisHealthProcessor();

        public override IRulesetConfigManager CreateConfig(SettingsStore settings) => new MvisRulesetConfigManager(settings, RulesetInfo);

        public override RulesetSettingsSubsection CreateSettings() => new MvisSettingsSubsection(this);

        public override string Description => "mvis";

        public override string ShortName => "mvis";

        public override string PlayingVerb => "Enjoying the music";

        public override Drawable CreateIcon() => new Sprite
        {
            Texture = new TextureStore(new TextureLoaderStore(CreateResourceStore()), false).Get("Textures/mvis"),
        };

        public override DifficultyCalculator CreateDifficultyCalculator(WorkingBeatmap beatmap) => new MvisDifficultyCalculator(this, beatmap);

        public override IEnumerable<Mod> GetModsFor(ModType type)
        {
            switch (type)
            {
                case ModType.Fun:
                    return new Mod[]
                    {
                        new MultiMod(new MvisModHalfTime(), new MvisModDaycore()),
                        new MultiMod(new MvisModDoubleTime(), new MvisModNightcore()),
                    };

                default:
                    return Array.Empty<Mod>();
            }
        }
    }
}

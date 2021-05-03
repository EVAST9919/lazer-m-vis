﻿using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps;

namespace osu.Game.Rulesets.Mvis.UI.Objects
{
    public class BeatmapBackground : BufferedContainer
    {
        private readonly Sprite sprite;
        private readonly WorkingBeatmap beatmap;

        public BeatmapBackground(WorkingBeatmap beatmap = null)
        {
            this.beatmap = beatmap;

            CacheDrawnFrameBuffer = true;
            RelativeSizeAxes = Axes.Both;
            Child = sprite = new Sprite
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                FillMode = FillMode.Fill,
            };
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            sprite.Texture = beatmap?.Background ?? textures.Get(@"Backgrounds/bg4");
        }
    }
}

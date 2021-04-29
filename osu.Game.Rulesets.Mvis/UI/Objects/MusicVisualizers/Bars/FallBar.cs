using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Mvis.UI.Objects.MusicVisualizers.Bars
{
    public class FallBar : BasicBar
    {
        protected override IEnumerable<Drawable> ColourReceptors => new[] { main, piece };

        private Box main;
        private Box piece;

        protected override Drawable CreateContent() => new Container
        {
            AutoSizeAxes = Axes.Y,
            RelativeSizeAxes = Axes.X,
            Children = new Drawable[]
            {
                main = new Box
                {
                    Origin = Anchor.BottomCentre,
                    RelativeSizeAxes = Axes.X,
                    Colour = Color4.White,
                    EdgeSmoothness = Vector2.One,
                },
                piece = new Box
                {
                    Origin = Anchor.BottomCentre,
                    RelativeSizeAxes = Axes.X,
                    Height = 1,
                    Colour = Color4.White,
                    EdgeSmoothness = Vector2.One,
                }
            },
        };

        public override void SetValue(float amplitudeValue, float valueMultiplier, int smoothness)
        {
            var newValue = ValueFormula(amplitudeValue, valueMultiplier);

            if (newValue > main.Height)
            {
                main.ResizeHeightTo(newValue)
                    .Then()
                    .ResizeHeightTo(0, smoothness);
            }

            if (main.Height > -piece.Y)
            {
                piece.MoveToY(-newValue)
                    .Then()
                    .MoveToY(0, smoothness * 6);
            }
        }
    }
}

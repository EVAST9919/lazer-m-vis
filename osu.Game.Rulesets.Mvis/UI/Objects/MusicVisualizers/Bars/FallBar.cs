using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Mvis.UI.Objects.MusicVisualizers.Bars
{
    public class FallBar : BasicBar
    {
        private Container mainBar;
        private Container fallingPiece;

        protected override Drawable CreateContent() => new Container
        {
            AutoSizeAxes = Axes.Y,
            RelativeSizeAxes = Axes.X,
            Children = new Drawable[]
            {
                mainBar = new Container
                {
                    Origin = Anchor.BottomCentre,
                    RelativeSizeAxes = Axes.X,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.White,
                        EdgeSmoothness = Vector2.One,
                    }
                },
                fallingPiece = new Container
                {
                    Origin = Anchor.BottomCentre,
                    RelativeSizeAxes = Axes.X,
                    Height = 2,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.White,
                        EdgeSmoothness = Vector2.One,
                    }
                }
            },
        };

        public override void SetValue(float amplitudeValue, float valueMultiplier, int smoothness)
        {
            var newValue = ValueFormula(amplitudeValue, valueMultiplier);

            if (newValue > mainBar.Height)
            {
                mainBar.ResizeHeightTo(newValue)
                    .Then()
                    .ResizeHeightTo(0, smoothness);
            }

            if (mainBar.Height > -fallingPiece.Y)
            {
                fallingPiece.MoveToY(-newValue)
                    .Then()
                    .MoveToY(0, smoothness * 6);
            }
        }
    }
}

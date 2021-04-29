using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Mvis.Configuration;
using osuTK;
using osuTK.Graphics;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Mvis.UI.Objects.MusicVisualizers.Bars
{
    public class BasicBar : CompositeDrawable
    {
        protected virtual IEnumerable<Drawable> ColourReceptors => new[] { box };

        [Resolved(canBeNull: true)]
        private MvisRulesetConfigManager config { get; set; }

        private readonly Bindable<bool> useCustomColour = new Bindable<bool>();
        private readonly Bindable<int> red = new Bindable<int>(0);
        private readonly Bindable<int> green = new Bindable<int>(0);
        private readonly Bindable<int> blue = new Bindable<int>(0);

        public BasicBar()
        {
            InternalChild = CreateContent();
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            config?.BindWith(MvisRulesetSetting.Red, red);
            config?.BindWith(MvisRulesetSetting.Green, green);
            config?.BindWith(MvisRulesetSetting.Blue, blue);
            config?.BindWith(MvisRulesetSetting.UseCustomColour, useCustomColour);

            red.BindValueChanged(_ => updateColour());
            green.BindValueChanged(_ => updateColour());
            blue.BindValueChanged(_ => updateColour());
            useCustomColour.BindValueChanged(_ => updateColour(), true);
        }

        private void updateColour()
        {
            foreach (var r in ColourReceptors)
                r.Colour = useCustomColour.Value ? new Color4(red.Value / 255f, green.Value / 255f, blue.Value / 255f, 1) : Color4.White;
        }

        private Box box;

        protected virtual Drawable CreateContent() => box = new Box
        {
            EdgeSmoothness = Vector2.One,
            RelativeSizeAxes = Axes.Both,
            Colour = Color4.White,
        };

        public virtual void SetValue(float amplitudeValue, float valueMultiplier, int softness)
        {
            var newHeight = ValueFormula(amplitudeValue, valueMultiplier);

            // Don't allow resize if new height less than current
            if (newHeight <= Height)
                return;

            ClearTransforms();
            Height = newHeight;
            this.ResizeHeightTo(0, softness);
        }

        protected virtual float ValueFormula(float amplitudeValue, float valueMultiplier) => amplitudeValue * valueMultiplier;
    }
}

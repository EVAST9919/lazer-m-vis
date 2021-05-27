using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Configuration;
using osu.Game.Overlays.Settings;

namespace osu.Game.Rulesets.Mvis.UI.Settings.Sections
{
    public class GameSection : Section
    {
        protected override string HeaderName => "Game settings";

        [BackgroundDependencyLoader]
        private void load(OsuConfigManager config)
        {
            AddRange(new Drawable[]
            {
                new SettingsSlider<double>
                {
                    LabelText = "Background dim",
                    Current = config.GetBindable<double>(OsuSetting.DimLevel),
                    KeyboardStep = 0.01f,
                    DisplayAsPercentage = true
                },
                new SettingsSlider<double>
                {
                    LabelText = "Background blur",
                    Current = config.GetBindable<double>(OsuSetting.BlurLevel),
                    KeyboardStep = 0.01f,
                    DisplayAsPercentage = true
                }
            });
        }
    }
}

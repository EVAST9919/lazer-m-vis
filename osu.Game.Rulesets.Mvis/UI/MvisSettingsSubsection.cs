using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Testing;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osu.Game.Overlays.Notifications;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Mvis.Configuration;
using osu.Game.Screens;
using osu.Game.Screens.Menu;

namespace osu.Game.Rulesets.Mvis.UI
{
    public class MvisSettingsSubsection : RulesetSettingsSubsection
    {
        protected override string Header => "mvis";

        [Resolved]
        private OsuGame game { get; set; }

        [Resolved]
        private NotificationOverlay notifications { get; set; }

        public MvisSettingsSubsection(Ruleset ruleset)
            : base(ruleset)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            var config = (MvisRulesetConfigManager)Config;

            Children = new Drawable[]
            {
                new SettingsButton
                {
                    Text = "Open Main Screen",
                    Action = () =>
                    {
                        try
                        {
                            var screenStack = getScreenStack(game);
                            if (!(screenStack.CurrentScreen is MainMenu))
                            {
                                notifications.Post(new SimpleErrorNotification
                                {
                                    Text = "This feature can be used only in Main menu!"
                                });
                                return;
                            }

                            var settingOverlay = getSettingsOverlay(game);
                            screenStack?.Push(new VisualizerScreen());
                            settingOverlay?.Hide();
                        }
                        catch
                        {
                        }
                    }
                }
            };
        }

        private static OsuScreenStack getScreenStack(OsuGame game) => game.ChildrenOfType<OsuScreenStack>().FirstOrDefault();

        private static SettingsOverlay getSettingsOverlay(OsuGame game) => game.ChildrenOfType<SettingsOverlay>().FirstOrDefault();

        public class PositionSlider : OsuSliderBar<float>
        {
            public override string TooltipText => Current.Value.ToString(@"0.##");
        }
    }
}

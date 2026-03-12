using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace PlayerHUD
{
    public class Config : IConfig
    {
        [Description("Activating plugin")]
        public bool IsEnabled { get; set; } = true;

        [Description("Activating debug mode")]
        public bool Debug { get; set; } = false;

        [Description("Makes text gradient")]
        public bool UseGradient { get; set; } = false;

        [Description("Allows you changing main HUD gradient text color by using HEX format")]
        public string GradientFirstColor { get; set; } = null;
        public string GradientSecondColor { get; set; } = null;

        [Description("Allows you add role groups to HUD with their custom color")]
        public Dictionary<string, RoleStyle> RoleStyles { get; set; } = new()
        {
            ["example1"] = new RoleStyle { Color = "#C50000", DonateRole = false },
            ["example2"] = new RoleStyle { Color = "#DC143C", DonateRole = false },
            ["donaterole"] = new RoleStyle { Color = "#FFD700", DonateRole = true },
            ["example3"] = new RoleStyle { Color = "default", DonateRole = false }
        };

        [Description("Allows hide or show some information in HUD")]
        public bool ShowAdminEffects { get; set; } = true;
        public bool ShowMuteStatus { get; set; } = true;
        public bool ShowRemoteAccess { get; set; } = true;
        public bool ShowCreatorBadge { get; set; } = true;
        public bool ShowOwnerBadge { get; set; } = true;
        public bool ShowWarheadStatus { get; set; } = true;
        public bool ShowGenStatus { get; set; } = true;
        public bool ShowFriendlyFire { get; set; } = true;
        public bool ShowPlayersCount { get; set; } = true;
        public bool ShowHp { get; set; } = true;
        public bool ShowTps { get; set; } = true;
        public bool ShowClock { get; set; } = true;

        [Description("Allows to change HUD color text by using HEX format")]
        public string HudColor { get; set; } = "#e91e63";

        [Description("The server name that will be shown at the bottom of the HUD")]
        public string ServerName { get; set; } = "changeme";
    }

    public class RoleStyle
    {
        public string Color { get; set; } = "default";
        public bool DonateRole { get; set; } = false;
    }
}

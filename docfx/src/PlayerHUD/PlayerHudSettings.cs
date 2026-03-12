namespace PlayerHUD
{
    public class PlayerHudSettings
    {
        public string CustomHudColor = null;
        public bool ShowHud { get; set; } = true;
        public bool UseGradient { get; set; } = false;
        public string GradientFirstColor = null;
        public string GradientSecondColor = null;
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
        public bool SettingsRegistered { get; set; } = false;
    }
}

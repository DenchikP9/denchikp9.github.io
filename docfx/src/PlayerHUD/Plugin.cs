using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using Exiled.Events.EventArgs.Player;
using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Utilities;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using ExiledPlayer = Exiled.API.Features.Player;
using Hint = HintServiceMeow.Core.Models.Hints.Hint;
using PlayerHandlers = Exiled.Events.Handlers.Player;
using Scp079Role = Exiled.API.Features.Roles.Scp079Role;

namespace PlayerHUD
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; private set; }
        public override string Author => "DenchikP";
        public override string Name => "PlayerHUD";
        public override string Prefix => "playerhud";
        public string CreatorSteamId => "76561199188306998@steam";
        public string CreatorSteamUsername => "DenchikP_Fun";
        public override Version Version => new(2, 0, 0);
        public override Version RequiredExiledVersion => new(9, 13, 1);

        private readonly Dictionary<string, PlayerHudSettings> PlayerSettings = new();

        public static readonly Dictionary<string, string> ScpColorToHex = new()
        {
            { "pink", "#FF96DE" },
            { "red", "#C50000" },
            { "brown", "#944710" },
            { "silver", "#A0A0A0" },
            { "light_green", "#32CD32" },
            { "crimson", "#DC143C" },
            { "cyan", "#00B7EB" },
            { "aqua", "#00FFFF" },
            { "deep_pink", "#FF1493" },
            { "tomato", "#FF6448" },
            { "yellow", "#FAFF86" },
            { "magenta", "#FF0090" },
            { "blue_green", "#4DFFB8" },
            { "orange", "#FF9966" },
            { "lime", "#BFFF00" },
            { "green", "#228B22" },
            { "emerald", "#50C878" },
            { "carmine", "#960018" },
            { "nickel", "#727472" },
            { "mint", "#98FF98" },
            { "army_green", "#4B5320" },
            { "pumpkin", "#EE7600" },
            { "gold", "#FFC01A" },
            { "teal", "#008080" },
            { "blue", "#005EBC" },
            { "purple", "#8137CE" },
            { "light_red", "#FD8272" },
            { "silver_blue", "#666699" },
            { "police_blue", "#002DB3" }
        };

        public override void OnEnabled()
        {
            base.OnEnabled();

            Instance = this;

            string banner = @"
██████╗ ██╗      █████╗ ██╗   ██╗███████╗██████╗ ██╗  ██╗██╗   ██╗██████╗ 
██╔══██╗██║     ██╔══██╗╚██╗ ██╔╝██╔════╝██╔══██╗██║  ██║██║   ██║██╔══██╗
██████╔╝██║     ███████║ ╚████╔╝ █████╗  ██████╔╝███████║██║   ██║██║  ██║
██╔═══╝ ██║     ██╔══██║  ╚██╔╝  ██╔══╝  ██╔══██╗██╔══██║██║   ██║██║  ██║
██║     ███████╗██║  ██║   ██║   ███████╗██║  ██║██║  ██║╚██████╔╝██████╔╝
╚═╝     ╚══════╝╚═╝  ╚═╝   ╚═╝   ╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝ ╚═════╝ ╚═════╝ ";

            Log.Info(banner);

            RegisterSettings();

            PlayerHandlers.Verified += OnVerified;
        }

        public override void OnDisabled()
        {
            PlayerHandlers.Verified -= OnVerified;
            base.OnDisabled();
        }

        private bool IsValidHex(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                return false;

            return System.Text.RegularExpressions.Regex
                .IsMatch(hex, "^#([0-9A-Fa-f]{6})$");
        }

        private void OnColorChanged(ExiledPlayer pl, SettingBase setting)
        {
            var input = setting as UserTextInputSetting;
            string text = input?.Text?.Trim();

            if (!PlayerSettings.TryGetValue(pl.UserId, out var hud))
                return;

            if (!string.IsNullOrWhiteSpace(text))
            {
                if (!text.StartsWith("#"))
                    text = "#" + text;

                hud.CustomHudColor = IsValidHex(text) ? text : "";
            }
            else
            {
                hud.CustomHudColor = "";
            }

            PlayerDisplay.Get(pl)?.ForceUpdate();
        }

        private void OnGradientChanged(ExiledPlayer pl, SettingBase setting)
        {
            var two = setting as TwoButtonsSetting;

            if (!PlayerSettings.TryGetValue(pl.UserId, out var hud))
                return;

            hud.UseGradient = two.IsSecond;
            PlayerDisplay.Get(pl)?.ForceUpdate();
        }

        private void OnGradientColorChanged(ExiledPlayer pl, SettingBase setting, bool first)
        {
            var input = setting as UserTextInputSetting;
            string text = input?.Text?.Trim();

            if (!PlayerSettings.TryGetValue(pl.UserId, out var hud))
                return;

            string result = "";

            if (!string.IsNullOrWhiteSpace(text))
            {
                if (!text.StartsWith("#"))
                    text = "#" + text;

                if (IsValidHex(text))
                    result = text;
            }

            if (first)
                hud.GradientFirstColor = result;
            else
                hud.GradientSecondColor = result;

            PlayerDisplay.Get(pl)?.ForceUpdate();
        }

        private void OnFirstGradientColorChanged(ExiledPlayer pl, SettingBase setting)
        {
            OnGradientColorChanged(pl, setting, true);
        }

        private void OnSecondGradientColorChanged(ExiledPlayer pl, SettingBase setting)
        {
            OnGradientColorChanged(pl, setting, false);
        }

        private void OnHudChanged(ExiledPlayer pl, SettingBase setting)
        {
            var two = setting as TwoButtonsSetting;

            if (!PlayerSettings.TryGetValue(pl.UserId, out var hud))
                return;

            hud.ShowHud = !two.IsSecond;
            PlayerDisplay.Get(pl)?.ForceUpdate();
        }

        private void OnAdminEffectsChanged(ExiledPlayer pl, SettingBase setting)
        {
            var two = setting as TwoButtonsSetting;

            if (!PlayerSettings.TryGetValue(pl.UserId, out var hud))
                return;

            hud.ShowAdminEffects = !two.IsSecond;
            PlayerDisplay.Get(pl)?.ForceUpdate();
        }

        private void OnMuteChanged(ExiledPlayer pl, SettingBase setting)
        {
            var two = setting as TwoButtonsSetting;

            if (!PlayerSettings.TryGetValue(pl.UserId, out var hud))
                return;

            hud.ShowMuteStatus = !two.IsSecond;
            PlayerDisplay.Get(pl)?.ForceUpdate();
        }

        private void OnWarheadChanged(ExiledPlayer pl, SettingBase setting)
        {
            var two = setting as TwoButtonsSetting;

            if (!PlayerSettings.TryGetValue(pl.UserId, out var hud))
                return;

            hud.ShowWarheadStatus = !two.IsSecond;
            PlayerDisplay.Get(pl)?.ForceUpdate();
        }

        private void OnGeneratorsChanged(ExiledPlayer pl, SettingBase setting)
        {
            var two = setting as TwoButtonsSetting;

            if (!PlayerSettings.TryGetValue(pl.UserId, out var hud))
                return;

            hud.ShowGenStatus = !two.IsSecond;
            PlayerDisplay.Get(pl)?.ForceUpdate();
        }

        private void OnFriendlyFireChanged(ExiledPlayer pl, SettingBase setting)
        {
            var two = setting as TwoButtonsSetting;

            if (!PlayerSettings.TryGetValue(pl.UserId, out var hud))
                return;

            hud.ShowFriendlyFire = !two.IsSecond;
            PlayerDisplay.Get(pl)?.ForceUpdate();
        }

        private void OnPlayersCountChanged(ExiledPlayer pl, SettingBase setting)
        {
            var two = setting as TwoButtonsSetting;

            if (!PlayerSettings.TryGetValue(pl.UserId, out var hud))
                return;

            hud.ShowPlayersCount = !two.IsSecond;
            PlayerDisplay.Get(pl)?.ForceUpdate();
        }

        private void OnHpChanged(ExiledPlayer pl, SettingBase setting)
        {
            var two = setting as TwoButtonsSetting;

            if (!PlayerSettings.TryGetValue(pl.UserId, out var hud))
                return;

            hud.ShowHp = !two.IsSecond;
            PlayerDisplay.Get(pl)?.ForceUpdate();
        }

        private void OnTpsChanged(ExiledPlayer pl, SettingBase setting)
        {
            var two = setting as TwoButtonsSetting;

            if (!PlayerSettings.TryGetValue(pl.UserId, out var hud))
                return;

            hud.ShowTps = !two.IsSecond;
            PlayerDisplay.Get(pl)?.ForceUpdate();
        }

        private void OnClockChanged(ExiledPlayer pl, SettingBase setting)
        {
            var two = setting as TwoButtonsSetting;

            if (!PlayerSettings.TryGetValue(pl.UserId, out var hud))
                return;

            hud.ShowClock = !two.IsSecond;
            PlayerDisplay.Get(pl)?.ForceUpdate();
        }

        private void OnRemoteAccessChanged(ExiledPlayer pl, SettingBase setting)
        {
            var two = setting as TwoButtonsSetting;

            if (!PlayerSettings.TryGetValue(pl.UserId, out var hud))
                return;

            hud.ShowRemoteAccess = !two.IsSecond;
            PlayerDisplay.Get(pl)?.ForceUpdate();
        }

        private void OnOwnerBadgeChanged(ExiledPlayer pl, SettingBase setting)
        {
            var two = setting as TwoButtonsSetting;

            if (!PlayerSettings.TryGetValue(pl.UserId, out var hud))
                return;

            hud.ShowOwnerBadge = !two.IsSecond;
            PlayerDisplay.Get(pl)?.ForceUpdate();
        }

        private void OnCreatorBadgeChanged(ExiledPlayer pl, SettingBase setting)
        {
            var two = setting as TwoButtonsSetting;

            if (!PlayerSettings.TryGetValue(pl.UserId, out var hud))
                return;

            hud.ShowCreatorBadge = !two.IsSecond;
            PlayerDisplay.Get(pl)?.ForceUpdate();
        }

        private string Gradient(string text, string startHex, string endHex)
        {
            Color start = ColorUtility.TryParseHtmlString(startHex, out var s) ? s : Color.white;
            Color end = ColorUtility.TryParseHtmlString(endHex, out var e) ? e : Color.white;

            var result = new System.Text.StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                float t = text.Length == 1 ? 0f : (float)i / (text.Length - 1);
                Color c = Color.Lerp(start, end, t);
                string hex = ColorUtility.ToHtmlStringRGB(c);
                result.Append($"<color=#{hex}>{text[i]}</color>");
            }

            return result.ToString();
        }

        private bool _settingsRegistered = false;

        private void RegisterSettings()
        {
            if (_settingsRegistered)
                return;

            string color = Config.HudColor;

            var settingsList = new List<SettingBase>
            {
                new HeaderSetting(1000, $"<color={color}>═══════【 PlayerHUD - Общие настройки 】═══════</color>"),

                new TwoButtonsSetting(1, $"[<color={color}>ℹ️</color>] | HUD",
                    "Показать", "Скрыть",
                    false,
                    onChanged: OnHudChanged),

                new UserTextInputSetting(
                    2,
                    $"[<color={color}>🎨</color>] | Пользовательский цвет HUD (HEX)",
                    "#ffffff",
                    7,
                    TMP_InputField.ContentType.Standard,
                    "Введите HEX цвет формата #RRGGBB",
                    255,
                    false,
                    null,
                    OnColorChanged
                ),

                new TwoButtonsSetting(3, "[<color=#00FBFF>🎨</color>] | Градиент",
                    "Отключить", "Включить",
                    false,
                    onChanged: OnGradientChanged),

                new UserTextInputSetting(
                    4,
                    "[<color=#00FBFF>🎨</color>] | Польз. начальный цвет градиента HUD (HEX)",
                    "#ffffff",
                    7,
                    TMP_InputField.ContentType.Standard,
                    "Введите HEX цвет формата #RRGGBB",
                    255,
                    false,
                    null,
                    OnFirstGradientColorChanged
                ),

                new UserTextInputSetting(
                    5,
                    "[<color=#00FBFF>🎨</color>] | Польз. конечный цвет градиента HUD (HEX)",
                    "#ffffff",
                    7,
                    TMP_InputField.ContentType.Standard,
                    "Введите HEX цвет формата #RRGGBB",
                    255,
                    false,
                    null,
                    OnSecondGradientColorChanged
                ),

                new HeaderSetting(999, $"<color={color}>═══════【 PlayerHUD - Настройки видимости 】═══════</color>"),

                new TwoButtonsSetting(6, "[<color=#7cc0f5>🏃</color>, <color=#91f57c>❤️</color>, <color=#d3d26a>💳</color>] | Админ эффекты",
                    "Показать", "Скрыть",
                    false,
                    onChanged: OnAdminEffectsChanged),

                new TwoButtonsSetting(7, "[<color=#d9092c>🔇</color>] | Статус отключения звука",
                    "Показать", "Скрыть",
                    false,
                    onChanged: OnMuteChanged),

                new TwoButtonsSetting(8, "[<color=#dd2108>\uf7ba</color>] | Статус боеголовки \"Альфа\"",
                    "Показать", "Скрыть",
                    false,
                    onChanged: OnWarheadChanged),

                new TwoButtonsSetting(9, "[<color=#fcc01e>⚡</color>] | Статус генераторов",
                    "Показать", "Скрыть",
                    false,
                    onChanged: OnGeneratorsChanged),

                new TwoButtonsSetting(10, "[<color=orange>🔫</color>] | Индикатор дружественного огня",
                    "Показать", "Скрыть",
                    false,
                    onChanged: OnFriendlyFireChanged),

                new TwoButtonsSetting(11, "[👤] | Счетчик игроков",
                    "Показать", "Скрыть",
                    false,
                    onChanged: OnPlayersCountChanged),

                new TwoButtonsSetting(12, "[<color=green>➕</color>] | HP",
                    "Показать", "Скрыть",
                    false,
                    onChanged: OnHpChanged),

                new TwoButtonsSetting(13, "[<color=green>📶</color>] | TPS",
                    "Показать", "Скрыть",
                    false,
                    onChanged: OnTpsChanged),

                new TwoButtonsSetting(14, "[<color=#00BFF0>🕒</color>] | Часы",
                    "Показать", "Скрыть",
                    false,
                    onChanged: OnClockChanged),

                new HeaderSetting(998, $"<color={color}>═══════【 PlayerHUD - Админские настройки 】═══════</color>"),

                new TwoButtonsSetting(15, "[<color=#ffffff>👤</color>] | Значок доступа к панели администратора",
                    "Показать", "Скрыть",
                    false,
                    onChanged: OnRemoteAccessChanged),

                new TwoButtonsSetting(16, "[<color=#faa61a>👑</color>] | Значок владельца сервера",
                    "Показать", "Скрыть",
                    false,
                    onChanged: OnOwnerBadgeChanged),

                new TwoButtonsSetting(17, "[<color=#e91e63>🔧</color>] | Значок создателя плагина",
                    "Показать", "Скрыть",
                    false,
                    onChanged: OnCreatorBadgeChanged)
            };

            SettingBase.Register(settingsList);

            _settingsRegistered = true;
        }

        private void OnVerified(VerifiedEventArgs ev)
        {
            ExiledPlayer player = ev.Player;
            var panelManager = new SidePanelManager();
            PlayerDisplay display = PlayerDisplay.Get(player);

            string userId = ev.Player.UserId;

            if (!PlayerSettings.ContainsKey(userId))
                PlayerSettings.Add(userId, new PlayerHudSettings());

            if (display == null) return;

            display.AddHint(new Hint
            {
                AutoText = _ => $"<b>{panelManager.BuildScpList(player)}</b>",
                XCoordinate = 1200,
                YCoordinate = 300,
                Alignment = HintAlignment.Right,
                SyncSpeed = HintSyncSpeed.Fastest
            });

            display.AddHint(new Hint
            {
                AutoText = _ => panelManager.BuildChaosList(player),
                XCoordinate = 1200,
                YCoordinate = 300,
                Alignment = HintAlignment.Right,
                SyncSpeed = HintSyncSpeed.Fastest
            });

            display.AddHint(new Hint
            {
                AutoText = _ => panelManager.BuildMtfList(player),
                XCoordinate = 1200,
                YCoordinate = 300,
                Alignment = HintAlignment.Right,
                SyncSpeed = HintSyncSpeed.Fastest
            });

            display.AddHint(new Hint
            {
                AutoText = _ => panelManager.BuildEffectsString(player),
                XCoordinate = 1200,
                YCoordinate = 750,
                Alignment = HintAlignment.Right,
                SyncSpeed = HintSyncSpeed.Fastest
            });

            display.AddHint(new Hint
            {
                AutoText = _ => panelManager.BuildSpectatorsList(player),
                XCoordinate = 1200,
                YCoordinate = 500,
                Alignment = HintAlignment.Right,
                SyncSpeed = HintSyncSpeed.Fastest
            });

            display.AddHint(new Hint
            {
                AutoText = _ => GetHudText(player),
                XCoordinate = 65,
                YCoordinate = 1022,
                Alignment = HintAlignment.Left,
                SyncSpeed = HintSyncSpeed.Fastest
            });
        }

        private (string, string) GetFinalGradient(ExiledPlayer player, PlayerHudSettings settings)
        {
            string baseDefault = player.UserId == CreatorSteamId
                ? "#f92e3b"
                : Config.HudColor;

            bool hasCustomFirst = !string.IsNullOrWhiteSpace(settings.GradientFirstColor);
            bool hasCustomSecond = !string.IsNullOrWhiteSpace(settings.GradientSecondColor);

            if (hasCustomFirst || hasCustomSecond)
            {
                return (
                    hasCustomFirst ? settings.GradientFirstColor : baseDefault,
                    hasCustomSecond ? settings.GradientSecondColor : "#ffffff"
                );
            }

            if (Config.UseGradient)
            {
                string serverFirst = !string.IsNullOrWhiteSpace(Config.GradientFirstColor)
                    ? Config.GradientFirstColor
                    : baseDefault;

                string serverSecond = !string.IsNullOrWhiteSpace(Config.GradientSecondColor)
                    ? Config.GradientSecondColor
                    : "#ffffff";

                return (serverFirst, serverSecond);
            }

            return (baseDefault, "#ffffff");
        }

        private string GetHudText(ExiledPlayer player)
        {
            var settings = PlayerSettings.ContainsKey(player.UserId)
                ? PlayerSettings[player.UserId]
                : new PlayerHudSettings();

            string defaultColor = player.UserId == CreatorSteamId
                ? "#f92e3b"
                : Config.HudColor;

            string customColor = settings.CustomHudColor;

            string finalColor = !string.IsNullOrWhiteSpace(customColor)
                ? customColor
                : defaultColor;

            var (start, end) = GetFinalGradient(player, settings);

            TimeSpan t = Round.ElapsedTime;
            string roundDuration = t.TotalHours >= 1
                ? $"{(int)t.TotalHours:00}:{t.Minutes:00}:{t.Seconds:00}"
                : $"{t.Minutes:00}:{t.Seconds:00}";
            if (t.TotalHours >= 1)
            {
                roundDuration = $"{(int)t.TotalHours} ч. {t.Minutes} мин. {t.Seconds} сек.";
            }
            else
            {
                roundDuration = $"{t.Minutes} мин. {t.Seconds} сек.";
            }
            int scp079EngagedGen = Generator.List.Count(g => g.IsEngaged);
            int scp079MaxGen = Generator.List.Count;
            string scp079GenStatus;
            if (Round.IsLobby || Round.IsEnded)
            {
                scp079GenStatus = "";
            }
            else
            {
                if (scp079EngagedGen == 3)
                {
                    scp079GenStatus = (Config.UseGradient && settings.UseGradient) ?
                        $"|  [{Gradient("⚡: \u2714 Все генераторы активированы", "#20A82F", "#75EA73")}]" :
                        $"|  [<color=#06cd13>⚡: \u2714 Все генераторы активированы</color>]";
                }
                else
                {
                    scp079GenStatus = (Config.UseGradient && settings.UseGradient) ?
                        $"|  [{Gradient($"⚡: {scp079EngagedGen}/{scp079MaxGen} генераторов", "#DB9D00", "#FED236")}]" :
                        $"|  [<color=#fcc01e>⚡: {scp079EngagedGen}/{scp079MaxGen} генераторов</color>]";
                }
            }
            string formatted;
            string warheadStatus;
            bool deadManSwitchStatus = DeadmanSwitch.IsSequenceActive;
            string deadManSwitchText = deadManSwitchStatus
                ? ((Config.UseGradient && settings.UseGradient)
                    ? $" [{Gradient("\u26a0: Активирован протокол \"Мертвая Рука\"!", "#EB2323", "#940505")}]"
                    : " [<color=#dd2108>\u26a0: Активирован протокол \"Мертвая Рука\"!</color>]")
                : "";

            if (Round.IsLobby || Round.IsEnded)
            {
                warheadStatus = "";
            }
            else
            {
                if (Warhead.Status == WarheadStatus.Detonated)
                {
                    warheadStatus = (Config.UseGradient && settings.UseGradient) ?
                        $"|  [{Gradient("\uf7ba: Сдетонировала", "#474747", "#7F7F7F")}]" :
                        "|  [<color=#808080>\uf7ba: Сдетонировала</color>]";
                }
                else if (Warhead.Status == WarheadStatus.InProgress)
                {
                    warheadStatus = (int)Warhead.DetonationTimer < 11 ?
                        (Config.UseGradient && settings.UseGradient) ?
                        $"|  [{Gradient("\uf7ba: Детонация неизбежна!", "#EB2323", "#AA0101")}]{deadManSwitchText}" :
                        $"|  [<color=#e52107>\uf7ba: Детонация неизбежна!</color>]{deadManSwitchText}" :
                        (Config.UseGradient && settings.UseGradient) ?
                        $"|  [{Gradient($"\uf7ba: Детонация через {(int)Warhead.DetonationTimer} секунд", "#EB2323", "#AA0101")}]{deadManSwitchText}" :
                        $"|  [<color=#c87701>\uf7ba: Детонация через {(int)Warhead.DetonationTimer} секунд</color>]{deadManSwitchText}";
                }
                else if (Warhead.Status == WarheadStatus.Armed)
                {
                    warheadStatus = (Config.UseGradient && settings.UseGradient) ?
                        $"|  [{Gradient("\uf7ba: Готова", "#20A82F", "#70E56F")}]" :
                        "|  [<color=#06cd13>\uf7ba: Готова</color>]";
                }
                else if (Warhead.Status == WarheadStatus.NotArmed)
                {
                    warheadStatus = (Config.UseGradient && settings.UseGradient) ?
                        $"|  [{Gradient("\uf7ba: Отключена", "#EB2323", "#B40505")}]" :
                        "|  [<color=#dd2108>\uf7ba: Отключена</color>]";
                }
                else
                {
                    warheadStatus = "";
                }
            }

            if (Round.IsEnded)
            {
                formatted = (Config.UseGradient && settings.UseGradient)
                    ? $"|  <color={start}>🕑</color> {Gradient("Раунд окончен, спасибо за игру!", start, end)}"
                    : $"|  <color={finalColor}>🕑 Раунд окончен, спасибо за игру!</color>";
            }
            else if (Round.IsStarted)
            {
                formatted = Round.IsLocked
                    ? (Config.UseGradient && settings.UseGradient)
                        ? $"|  <color={start}>🕑</color> {Gradient("Раунд длится:", start, end)} [<color=#f5bc1d>🔒</color>] {roundDuration}"
                        : $"|  <color={finalColor}>🕑 Раунд длится:</color> [<color=#f5bc1d>🔒</color>] {roundDuration}"
                    : (Config.UseGradient && settings.UseGradient)
                        ? $"|  <color={start}>🕑</color> {Gradient("Раунд длится:", start, end)} {roundDuration}"
                        : $"|  <color={finalColor}>🕑 Раунд длится:</color> {roundDuration}";
            }
            else
            {
                if (Round.IsLocked)
                {
                    formatted = (Config.UseGradient && settings.UseGradient)
                        ? $"|  <color={start}>🕑</color> {Gradient("Раунд не начался", start, end)} [<color=#f5bc1d>🔒</color>]"
                        : $"|  <color={finalColor}>🕑 Раунд не начался [<color=#f5bc1d>🔒</color>]</color>";
                }
                else
                {
                    formatted = (Config.UseGradient && settings.UseGradient)
                        ? $"|  <color={start}>🕑</color> {Gradient("Раунд не начался", start, end)}"
                        : $"|  <color={finalColor}>🕑 Раунд не начался</color>";
                }
            }

            string nicknameWithBadge = GetNicknameWithBadge(player);
            string groupNameWithColor = GetColoredBadge(player);
            string roleName;
            string playersCount;
            string friendlyFire = (Config.ShowFriendlyFire && settings.ShowFriendlyFire) ? Server.FriendlyFire ? (Config.UseGradient && settings.UseGradient) ? "[<color=#FC3421>🔫</color> <color=#F77017>F</color><color=#F48E12>F</color>] " : "[<color=orange>🔫 FF</color>] " : "" : "";
            int lobbyJoinedPlayers = Server.PlayerCount;
            short lobbyTimeLeft = Round.LobbyWaitingTime;
            string serverName = Config.ServerName;

            if (Round.IsLobby)
            {
                if (Round.IsLobbyLocked)
                {
                    roleName = (Config.UseGradient && settings.UseGradient)
                        ? $"|  <color={start}>🏠</color> {Gradient("Лобби:", start, end)} [<color=#FF8C00>🔒</color> {Gradient("Заблокирован", "#FD9002", "#F2A90D")}] [<color=#00BFF0>👤</color> {Gradient($"Игроков: {lobbyJoinedPlayers}", "#00BFF0", "#00D5EB")}]"
                        : $"|  <color={finalColor}>🏠 Лобби:</color> [<color=#f5bc1d>🔒 Заблокирован</color>] [Игроков: {lobbyJoinedPlayers}]";
                    playersCount = $"";
                }
                else if (lobbyTimeLeft == -2)
                {
                    roleName = (Config.UseGradient && settings.UseGradient)
                        ? $"|  <color={start}>🏠</color> {Gradient("Лобби:", start, end)} [<color=#B19F9F>🕑</color><color=#B59A99>:</color> {Gradient("Отсчет не начался", "#BE928F", "#F04626")}] [<color=#00BFF0>👤</color> {Gradient($"Игроков: {lobbyJoinedPlayers}", "#00BFF0", "#00D5EB")}]"
                        : $"|  <color={finalColor}>🏠 Лобби:</color> [🕑: Отсчет не начался] [👤 Игроков: {lobbyJoinedPlayers}]";
                    playersCount = $"";
                }
                else if (lobbyTimeLeft == -1)
                {
                    roleName = (Config.UseGradient && settings.UseGradient)
                        ? $"|  <color={start}>🏠</color> {Gradient("Лобби:", start, end)} [<color=#B19F9F>🕑</color><color=#B59A99>:</color> {Gradient("Раунд начинается!", "#BE928F", "#F04626")}] [<color=#00BFF0>👤</color> {Gradient($"Игроков: {lobbyJoinedPlayers}", "#00BFF0", "#00D5EB")}]"
                        : $"|  <color={finalColor}>🏠 Лобби:</color> [🕑: Раунд начинается!] [👤 Игроков: {lobbyJoinedPlayers}]";
                    playersCount = $"";
                }
                else
                {
                    roleName = (Config.UseGradient && settings.UseGradient)
                        ? $"|  <color={start}>🏠</color> {Gradient("Лобби:", start, end)} [<color=#B19F9F>🕑</color><color=#B59A99>:</color> {Gradient($"{lobbyTimeLeft} сек", "#BE928F", "#F04626")}] [<color=#00BFF0>👤</color> {Gradient($"Игроков: {lobbyJoinedPlayers}", "#00BFF0", "#00D5EB")}]"
                        : $"|  <color={finalColor}>🏠 Лобби:</color> [🕑: {lobbyTimeLeft} сек] [👤 Игроков: {lobbyJoinedPlayers}]";
                    playersCount = $"";
                }
            }
            else
            {
                if (Config.UseGradient && settings.UseGradient)
                {
                    roleName = $"|  <color={start}>🎮</color> {Gradient("Класс:", start, end)} {GetFriendlyRoleName(player.Role, player)}";
                    playersCount = $" [👤: {Server.PlayerCount}/{Server.MaxPlayerCount}]";
                }
                else
                {
                    roleName = $"|  <color={finalColor}>🎮 Класс:</color> {GetFriendlyRoleName(player.Role, player)}";
                    playersCount = $" [👤: {Server.PlayerCount}/{Server.MaxPlayerCount}]";
                }
            }
            string playersCountToggle = (Config.ShowPlayersCount && settings.ShowPlayersCount) ? playersCount : "";

            int tps = (int)Math.Round(Server.Tps);
            string tpsStatus;
            if (Config.ShowTps && settings.ShowTps)
            {
                tpsStatus = tps >= 46 ? $"[<color=green>📶 {tps} TPS</color>] " :
                                tps >= 31 ? $"[<color=yellow>📶 {tps} TPS</color>] " :
                                tps >= 16 ? $"[<color=orange>⚠️ 📶 {tps} TPS ⚠️</color>] " :
                                           $"[<color=red>⚠️ 📶 {tps} TPS ⚠️</color>] ";
            }
            else
            {
                tpsStatus = "";
            }

            string groupName = (Config.UseGradient && settings.UseGradient)
                ? $"|  <color={start}>🔧</color> {Gradient("Роль:", start, end)} {groupNameWithColor}"
                : $"|  <color={finalColor}>🔧 Роль:</color> {groupNameWithColor}";

            string clock = DateTime.Now.ToString("HH:mm");
            string clockResult = (Config.ShowClock && settings.ShowClock) ? $" [<color=#00BFF0>🕒</color> <color=#00C1EF>{clock}</color>]" : "";

            string warheadStatusToggle = (Config.ShowWarheadStatus && settings.ShowWarheadStatus) ? warheadStatus : "\u200B";
            string scp079GenStatusToggle = (Config.ShowGenStatus && settings.ShowGenStatus) ? scp079GenStatus : "\u200B";

            if (!settings.ShowHud || player.Role.Type == RoleTypeId.Filmmaker)
                return null;
            else
                return $"<b><size=14>" +
                    $"{warheadStatusToggle}\n" +
                    $"{scp079GenStatusToggle}\n" +
                    $"{formatted}\n" +
                    $"{nicknameWithBadge}\n" +
                    $"{roleName}\n" +
                    $"{groupName}\n" +
                    $"|  {tpsStatus}{friendlyFire}{serverName}{playersCountToggle}{clockResult}" +
                    $"</size></b>";
        }

        private string ResolveColor(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "#808080";

            input = input.ToLower().Trim();

            if (System.Text.RegularExpressions.Regex.IsMatch(input, "^#([0-9A-Fa-f]{6})$"))
                return input;

            if (ScpColorToHex.TryGetValue(input, out string hex))
                return hex;

            return "#808080";
        }

        public string GetColoredBadge(ExiledPlayer player)
        {
            if (player?.Group == null)
                return "<color=#808080>Игрок</color>";

            string badgeText = player.Group.BadgeText;
            string groupName = player.GroupName;

            if (!string.IsNullOrEmpty(groupName) &&
                Config.RoleStyles.TryGetValue(groupName, out RoleStyle style))
            {
                string finalColor = ResolveColor(
                    style.Color.Equals("default", StringComparison.OrdinalIgnoreCase)
                        ? player.Group.BadgeColor
                        : style.Color
                );

                string moneyPrefix = style.DonateRole
                    ? "<color=#FFFFFF>[</color>💰<color=#FFFFFF>]</color> "
                    : "";

                return $"<color={finalColor}>{moneyPrefix}{badgeText}</color>";
            }

            return $"<color={ResolveColor(player.Group.BadgeColor)}>{badgeText}</color>";
        }

        private string GetNicknameWithBadge(ExiledPlayer player)
        {
            var settings = PlayerSettings.ContainsKey(player.UserId)
                ? PlayerSettings[player.UserId]
                : new PlayerHudSettings();

            string defaultColor = player.UserId == CreatorSteamId
                ? "#f92e3b"
                : Config.HudColor;

            string customColor = settings.CustomHudColor;

            string finalColor = !string.IsNullOrWhiteSpace(customColor)
                ? customColor
                : defaultColor;

            var (start, end) = GetFinalGradient(player, settings);

            List<string> hpParts = new();

            if (player.Role.Type == RoleTypeId.Scp079)
            {
                var scp079 = player.Role.As<Scp079Role>();

                int tier = scp079.Level;
                int generators = Generator.List.Count(g => g.IsEngaged);
                int totalGenerators = Generator.List.Count;
                string scp079WarningStatus;

                if (generators == 3)
                {
                    scp079WarningStatus = $"<color=red>⚠️⚠️⚠️</color>";
                }
                else if (generators == 2)
                {
                    scp079WarningStatus = $"<color=orange>⚠️⚠️―</color>";

                }
                else if (generators == 1)
                {
                    scp079WarningStatus = $"<color=yellow>⚠️――</color>";
                }
                else
                {
                    scp079WarningStatus = $"<color=green>―――</color>";
                }

                hpParts.Add($"<color=#fcc01e>⚡ Генераторы: {generators}/{totalGenerators}</color>");
                hpParts.Add(scp079WarningStatus);
                hpParts.Add($"<color=#00bfff>💳 Уровень доступа {tier}</color>");
            }
            else
            {
                int health = (int)Math.Round(player.Health);
                int maxHealth = (int)Math.Round(player.MaxHealth);

                int artHealth = (int)Math.Round(player.ArtificialHealth);
                int humeShield = (int)Math.Round(player.HumeShield);

                if (health <= 0)
                {
                    if (!(Round.IsLobby ||
                          player.Role.Type == RoleTypeId.Overwatch ||
                          player.Role.Type == RoleTypeId.Filmmaker))
                    {
                        hpParts.Add("<color=#808080>👻 Вы мертвы</color>");
                    }
                }
                else
                {
                    double percent = health / (double)maxHealth;

                    string color;

                    if (percent <= 0.25)
                        color = "red";
                    else if (percent <= 0.50)
                        color = "orange";
                    else if (percent <= 0.75)
                        color = "yellow";
                    else
                        color = "green";

                    hpParts.Add($"<color={color}>➕ {health} HP</color>");
                }

                if (artHealth > 0)
                    hpParts.Add($"<color=#2f7a0d>{artHealth} AHP</color>");

                if (humeShield > 0)
                    hpParts.Add($"<color=#6d68da>{humeShield} HS</color>");
            }

            string hpStatus = (Config.ShowHp && settings.ShowHp) && hpParts.Count > 0
                ? $" [{string.Join(" | ", hpParts)}]"
                : "";

            string muteStatus;
            if (player.IsMuted && player.IsIntercomMuted)
            {
                muteStatus = "<color=white>[<color=#d9092c>🔇 Голосовой чат + Интерком</color>]</color> ";
            }
            else if (player.IsMuted)
            {
                muteStatus = "<color=white>[<color=#d9092c>🔇 Голосовой чат</color>]</color> ";
            }
            else if (player.IsIntercomMuted)
            {
                muteStatus = "<color=white>[<color=#d9092c>🔇 Интерком</color>]</color> ";
            }
            else
            {
                muteStatus = "";
            }

            string adminEffects;
            if (player.IsNoclipPermitted || player.IsGodModeEnabled || player.IsBypassModeEnabled)
            {
                List<string> statuses = new();

                if (player.IsNoclipPermitted)
                    statuses.Add("<color=#7cc0f5>🏃</color>");

                if (player.IsGodModeEnabled)
                    statuses.Add("<color=#91f57c>❤️</color>");

                if (player.IsBypassModeEnabled)
                    statuses.Add("<color=#d3d26a>💳</color>");

                adminEffects = $"[{string.Join(", ", statuses)}] ";
            }
            else
            {
                adminEffects = "";
            }

            string creatorBadge = player.UserId == CreatorSteamId ? "[<color=#e91e63>🔧</color>] " : "";
            string ownerBadge = player.GroupName == "owner" ? "[<color=#faa61a>👑</color>] " : "";
            string remoteAdmin = player.RemoteAdminAccess ? "[<color=#ffffff>👤</color>] " : "";
            string adminEffectsToggle = (Config.ShowAdminEffects && settings.ShowAdminEffects) ? adminEffects : "";
            string muteStatusToggle = (Config.ShowMuteStatus && settings.ShowMuteStatus) ? muteStatus : "";
            string remoteAdminToggle = (Config.ShowRemoteAccess && settings.ShowRemoteAccess) ? remoteAdmin : "";
            string creatorBadgeToggle = (Config.ShowCreatorBadge && settings.ShowCreatorBadge) ? creatorBadge : "";
            string ownerBadgeToggle = (Config.ShowOwnerBadge && settings.ShowOwnerBadge) ? ownerBadge : "";
            return (Config.UseGradient && settings.UseGradient)
                ? $"|  <color={start}>👤</color> {Gradient("Игрок:", start, end)} {adminEffectsToggle}{muteStatusToggle}{remoteAdminToggle}{creatorBadgeToggle}{ownerBadgeToggle}{player.DisplayNickname}{hpStatus}"
                : $"|  <color={finalColor}>👤 Игрок:</color> {adminEffectsToggle}{muteStatusToggle}{remoteAdminToggle}{creatorBadgeToggle}{ownerBadgeToggle}{player.DisplayNickname}{hpStatus}";
        }

        private string GetFriendlyRoleName(RoleTypeId role, ExiledPlayer player)
        {
            string unitName = string.IsNullOrEmpty(player.UnitName) ? "" : $" | {player.UnitName}";
            return role switch
            {
                RoleTypeId.ClassD => "<color=#FF8E00>👤 Персонал Класса D</color>",
                RoleTypeId.Scientist => "<color=#FFFF7C>👤 Научный Сотрудник</color>",
                RoleTypeId.FacilityGuard => $"<color=#5B6370>🔫 Охранник Комплекса{unitName}</color>",
                RoleTypeId.NtfPrivate => $"<color=#70C3FF>🔫 Рядовой МОГ{unitName}</color>",
                RoleTypeId.NtfSergeant => $"<color=#0096FF>🔫 Сержант МОГ{unitName}</color>",
                RoleTypeId.NtfCaptain => $"<color=#003DCA>🔫 Капитан МОГ{unitName}</color>",
                RoleTypeId.NtfSpecialist => $"<color=#00D9FF>🔫 Специалист МОГ{unitName}</color>",
                RoleTypeId.ChaosRifleman => "<color=#008F1C>🔫 Стрелок Хаоса</color>",
                RoleTypeId.ChaosRepressor => "<color=#15853D>🔫 Усмиритель Хаоса</color>",
                RoleTypeId.ChaosMarauder => "<color=#066328>🔫 Мародёр Хаоса</color>",
                RoleTypeId.ChaosConscript => "<color=#559101>🔫 Новобранец Хаоса</color>",
                RoleTypeId.Scp173 => "<color=#EC2222>💥 Скульптура (SCP-173)</color>",
                RoleTypeId.Scp049 => "<color=#EC2222>💉 Чумной доктор (SCP-049)</color>",
                RoleTypeId.Scp939 => "<color=#EC2222>🔊 Со множеством голосов (SCP-939)</color>",
                RoleTypeId.Scp096 => "<color=#EC2222>💀 «Скромник» (SCP-096)</color>",
                RoleTypeId.Scp106 => "<color=#EC2222>👤 Старик (SCP-106)</color>",
                RoleTypeId.Scp079 => "<color=#EC2222>💻 Старый ИИ (SCP-079)</color>",
                RoleTypeId.Scp0492 => "<color=#EC2222>💀 Зомби (SCP-049-2)</color>",
                RoleTypeId.Scp3114 => "<color=#EC2222>💀 Разве это не было бы Чилли? (SCP-3114)</color>",
                RoleTypeId.Tutorial => "<color=#F700FD>👤 Обучение</color>",
                RoleTypeId.Spectator => "<color=#FFFFFF>👻 Наблюдатель</color>",
                RoleTypeId.Overwatch => "<color=#00FFFF>👤 Надзиратель</color>",
                RoleTypeId.Filmmaker => "<color=#3D3D3D>📹 Режиссёр</color>",
                _ => "Неизвестная роль"
            };
        }
    }
}

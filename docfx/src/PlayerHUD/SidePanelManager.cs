using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.API.Extensions;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlayerHUD
{
    public class SidePanelManager
    {
        private const string White = "#FFFFFF";

        private static readonly Dictionary<RoleTypeId, (string Name, string Color)> ChaosRoles = new()
        {
            { RoleTypeId.ChaosRepressor, ("Усмиритель", "#15853D") },
            { RoleTypeId.ChaosMarauder,  ("Мародёр",    "#066328") },
            { RoleTypeId.ChaosRifleman,  ("Стрелок",    "#008F1C") },
            { RoleTypeId.ChaosConscript, ("Новобранец", "#559101") }
        };

        private static readonly Dictionary<RoleTypeId, (string Name, string Color)> MtfRoles = new()
        {
            { RoleTypeId.NtfCaptain,    ("Командир",     "#003DCA") },
            { RoleTypeId.NtfSergeant,   ("Сержант",      "#0096FF") },
            { RoleTypeId.NtfSpecialist, ("Специалист",   "#00D9FF") },
            { RoleTypeId.NtfPrivate,    ("Рядовой",      "#70C3FF") },
            { RoleTypeId.FacilityGuard, ("Охрана",       "#5B6370") }
        };

        private static readonly Dictionary<EffectType, string> EffectColors = new()
        {
            { EffectType.AmnesiaItems, "#ff7a7a" },
            { EffectType.AmnesiaVision, "#ff7a7a" },
            { EffectType.Asphyxiated, "#ff7a7a" },
            { EffectType.Bleeding, "#ff7a7a" },
            { EffectType.Blinded, "#ff7a7a" },
            { EffectType.Blurred, "#ff7a7a" },
            { EffectType.Burned, "#ff7a7a" },
            { EffectType.Concussed, "#ff7a7a" },
            { EffectType.Corroding, "#ff7a7a" },
            { EffectType.Deafened, "#ff7a7a" },
            { EffectType.Decontaminating, "#ff7a7a" },
            { EffectType.Disabled, "#ff7a7a" },
            { EffectType.Ensnared, "#ff7a7a" },
            { EffectType.Exhausted, "#ff7a7a" },
            { EffectType.Flashed, "#ff7a7a" },
            { EffectType.Hemorrhage, "#ff7a7a" },
            { EffectType.Poisoned, "#ff7a7a" },
            { EffectType.SeveredHands, "#ff7a7a" },
            { EffectType.SeveredEyes, "#ff7a7a" },
            { EffectType.Stained, "#ff7a7a" },
            { EffectType.Hypothermia, "#ff7a7a" },
            { EffectType.CardiacArrest, "#ff7a7a" },
            { EffectType.PocketCorroding, "#ff7a7a" },
            { EffectType.Strangled, "#ff7a7a" },
            { EffectType.PitDeath, "#ff7a7a" },
            { EffectType.Traumatized, "#ff7a7a" },
            { EffectType.AnomalousTarget, "#ff7a7a"},
        
            { EffectType.Invigorated, "#b4ffb4" },
            { EffectType.MovementBoost, "#b4ffb4" },
            { EffectType.Vitality, "#b4ffb4" },
            { EffectType.DamageReduction, "#b4ffb4" },
            { EffectType.BodyshotReduction, "#b4ffb4" },
            { EffectType.Scp1853, "#b4ffb4" },
            { EffectType.AntiScp207, "#b4ffb4" },
            { EffectType.FocusedVision, "#b4ffb4" },
            { EffectType.AnomalousRegeneration, "#b4ffb4" },
            { EffectType.RainbowTaste, "#b4ffb4" },
            { EffectType.SpawnProtected, "#b4ffb4" },
            { EffectType.SilentWalk, "#b4ffb4" },
            { EffectType.Fade, "#b4ffb4"},
        
            { EffectType.Scp207, "#ffb4ff" },
            { EffectType.Scp1344, "#ffb4ff" },
        
            { EffectType.Scanned, "#f5de5d" },
            { EffectType.SoundtrackMute, "#f5de5d" },
            { EffectType.FogControl, "#f5de5d" },
            { EffectType.Scp1576, "#f5de5d" },
            { EffectType.Lightweight, "#f5de5d" },
            { EffectType.HeavyFooted, "#f5de5d" },
            { EffectType.Scp1509Resurrected, "#f5de5d" },
            { EffectType.NightVision, "#f5de5d" },
        };

        public string BuildScpList(Player viewer)
        {
            if (!viewer.IsScp)
                return string.Empty;

            var scps = Player.List
                .Where(p => p.IsAlive && p.IsScp)
                .OrderBy(p => p.Role.Type);

            if (!scps.Any())
                return string.Empty;

            StringBuilder sb = new(256);
            sb.AppendLine("\u200B<color=#EC2222><b>Список SCP:</b></color>");

            int zombies = Player.List.Count(p =>
                p.IsAlive && p.Role.Type == RoleTypeId.Scp0492);

            foreach (var scp in scps)
            {
                int hp = Mathf.CeilToInt(scp.Health);
                int hs = Mathf.CeilToInt(scp.HumeShield);

                string roleName = scp.Role.Type switch
                {
                    RoleTypeId.Scp173 => "<color=#EC2222>SCP-173</color>",
                    RoleTypeId.Scp049 => "<color=#EC2222>SCP-049</color>",
                    RoleTypeId.Scp939 => "<color=#EC2222>SCP-939</color>",
                    RoleTypeId.Scp096 => "<color=#EC2222>SCP-096</color>",
                    RoleTypeId.Scp106 => "<color=#EC2222>SCP-106</color>",
                    RoleTypeId.Scp079 => "<color=#EC2222>SCP-079</color>",
                    RoleTypeId.Scp0492 => "<color=#EC2222>SCP-049-2</color>",
                    RoleTypeId.Scp3114 => "<color=#EC2222>SCP-3114</color>",
                    _ => $"<color=#EC2222>{scp.Role.Type}</color>"
                };

                switch (scp.Role.Type)
                {
                    case RoleTypeId.Scp049:
                        sb.AppendLine(
                            $"\u200B{roleName} <color=white>({zombies} зомби | {hp} HP | {hs} HS)</color>");
                        break;

                    case RoleTypeId.Scp079:
                        var role079 = scp.Role.As<Scp079Role>();
                        int gens = Generator.List.Count(g => g.IsEngaged);
                        int totalGens = Generator.List.Count;

                        sb.AppendLine(
                            $"\u200B{roleName} <color=white>({gens}/{totalGens} | T{role079.Level})</color>");
                        break;

                    default:
                        sb.AppendLine(
                            $"\u200B{roleName} <color=white>({hp} HP | {hs} HS)</color>");
                        break;
                }
            }

            return sb.ToString().TrimEnd();
        }

        public string BuildChaosList(Player viewer)
        {
            if (viewer.Role.Team != Team.ChaosInsurgency)
                return string.Empty;

            var chaos = Player.List.Where(p =>
                p.IsAlive && p.Role.Team == Team.ChaosInsurgency);

            if (!chaos.Any())
                return string.Empty;

            var grouped = chaos.GroupBy(p => p.Role.Type)
                               .ToDictionary(g => g.Key, g => g.Count());

            StringBuilder sb = new(128);
            sb.AppendLine("\u200B<color=#15853D>Список ПХ:</color>");

            foreach (var role in ChaosRoles)
            {
                grouped.TryGetValue(role.Key, out int count);

                sb.AppendLine(
                    $"\u200B<color={role.Value.Color}>{role.Value.Name}</color> " +
                    $"<color={White}>({count})</color>");
            }

            return sb.ToString().TrimEnd();
        }

        public string BuildMtfList(Player viewer)
        {
            if (viewer.Role.Team != Team.FoundationForces)
                return string.Empty;

            var mtf = Player.List.Where(p =>
                p.IsAlive && p.Role.Team == Team.FoundationForces);

            if (!mtf.Any())
                return string.Empty;

            var grouped = mtf.GroupBy(p => p.Role.Type)
                             .ToDictionary(g => g.Key, g => g.Count());

            StringBuilder sb = new(128);
            sb.AppendLine("\u200B<color=#00BFFF>Список МОГ:</color>");

            foreach (var role in MtfRoles)
            {
                grouped.TryGetValue(role.Key, out int count);

                sb.AppendLine(
                    $"\u200B<color={role.Value.Color}>{role.Value.Name}</color> " +
                    $"<color={White}>({count})</color>");
            }

            return sb.ToString().TrimEnd();
        }

        public string BuildEffectsString(Player player)
        {
            if (player == null ||
                !player.IsAlive ||
                player.Role.Team == Team.Dead ||
                player.Role.Type == RoleTypeId.Spectator)
                return string.Empty;

            if (!player.ActiveEffects.Any())
                return string.Empty;

            StringBuilder sb = new(128);
            sb.AppendLine($"\u200B<color={White}>Эффекты:</color>");

            foreach (var effect in player.ActiveEffects)
            {
                if (!effect.TryGetEffectType(out EffectType type))
                    continue;

                string color = EffectColors.TryGetValue(type, out string custom)
                    ? custom
                    : type.IsNegative() ? "#ff7a7a"
                    : type.IsPositive() ? "#b4ffb4"
                    : "#ffb4ff";

                bool hasDuration = effect.Duration > 0.1f;
                bool hasIntensity = effect.Intensity > 0;

                StringBuilder effectInfo = new();

                if (hasIntensity)
                    effectInfo.Append($"x{effect.Intensity}");

                if (hasDuration)
                {
                    if (hasIntensity)
                        effectInfo.Append(" |");

                    effectInfo.Append($" {effect.Duration:0}s");
                }

                if (effectInfo.Length > 0)
                {
                    sb.AppendLine(
                        $"\u200B<color={color}>{type}</color>" +
                        $"\u200B<color={White}> ({effectInfo})</color>");
                }
                else
                {
                    sb.AppendLine(
                        $"\u200B<color={color}>{type}</color>");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public string BuildSpectatorsList(Player player)
        {
            if (player == null || !player.IsAlive)
                return string.Empty;

            var spectators = player.CurrentSpectatingPlayers;

            if (spectators == null || !spectators.Any())
                return string.Empty;

            StringBuilder sb = new StringBuilder(128);
            sb.AppendLine($"\u200B<color=#FFD700>Наблюдатели:</color>");

            foreach (var spec in spectators)
            {
                string specName = spec.Nickname ?? spec.UserId;
                sb.AppendLine($"\u200B<color=#FFFFFF>{specName}</color>");
            }

            return sb.ToString().TrimEnd();
        }

        public string BuildFullPanel(Player player)
        {
            StringBuilder sb = new(512);

            string scp = BuildScpList(player);
            string chaos = BuildChaosList(player);
            string mtf = BuildMtfList(player);
            string effects = BuildEffectsString(player);
            string spectators = BuildSpectatorsList(player);

            if (player.IsScp)
            {
                if (!string.IsNullOrEmpty(scp))
                    sb.AppendLine(scp);
            }
            else if (player.Role.Team == Team.ChaosInsurgency)
            {
                if (!string.IsNullOrEmpty(chaos))
                    sb.AppendLine(chaos);
            }
            else if (player.Role.Team == Team.FoundationForces)
            {
                if (!string.IsNullOrEmpty(mtf))
                    sb.AppendLine(mtf);
            }

            if (!string.IsNullOrEmpty(effects))
                sb.AppendLine(effects);

            if (player.IsScp && !string.IsNullOrEmpty(spectators))
                sb.AppendLine(spectators);

            return sb.ToString().TrimEnd();
        }
    }
}
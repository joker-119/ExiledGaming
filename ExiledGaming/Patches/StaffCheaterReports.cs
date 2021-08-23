namespace ExiledGaming.Patches
{
    using System;
    using DiscordIntegration.API.Commands;
    using Exiled.API.Features;
    using HarmonyLib;

    [HarmonyPatch(typeof(CheaterReport), nameof(CheaterReport.IssueReport))]
    public class CheaterReportOverride
    {
        public static bool Prefix(CheaterReport __instance, GameConsoleTransmission reporter,
            string reporterUserId,
            string reportedUserId,
            string reportedAuth,
            string reportedIp,
            string reporterAuth,
            string reporterIp,
            ref string reason,
            ref byte[] signature,
            string reporterPublicKey,
            int reportedId)
        {
            if (reportedAuth == reporterAuth)
            {
                reporter.SendToClient(__instance.connectionToClient, "You can't report yourself!" + Environment.NewLine, "yellow");
                return false;
            }

            DiscordIntegration.DiscordIntegration.Network.SendAsync(new RemoteCommand("sendMessage", 520510914374664192, $"{Player.Get(reportedId)?.Nickname} has been reported by {Player.Get(reporterUserId)?.Nickname} for: {reason}"));
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerMovementSync), nameof(PlayerMovementSync.AntiCheatKillPlayer))]
    public class Cheaty
    {
        public static bool Prefix(PlayerMovementSync __instance, string message)
        {
            Player player = Player.Get(__instance._hub.gameObject);
            if (player == null)
                return true;
			
            if (player.Role == RoleType.Spectator || player.Role == RoleType.Tutorial)
                return false;
            DiscordIntegration.DiscordIntegration.Network.SendAsync(new RemoteCommand("sendMessage", 520510914374664192,
                $"Player {player.Nickname} - {player.UserId} ({player.Role}) was killed by Anti-cheat: {message}"));

            return true;
        }
    }
}
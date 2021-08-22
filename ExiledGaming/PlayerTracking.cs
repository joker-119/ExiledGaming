using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using NorthwoodLib.Pools;

namespace ExiledGaming
{
    public class PlayerTracking
    {
        public static Dictionary<string, Tuple<RoleType, int>> TrackedPlayers =
            new Dictionary<string, Tuple<RoleType, int>>();

        public static List<RoleType> ValidRoles = new List<RoleType>
        {
            RoleType.Scientist,
            RoleType.ClassD,
            RoleType.Scp93953,
            RoleType.FacilityGuard,
            RoleType.ChaosConscript,
        };

        public static void TrackAllRoles()
        {
            List<string> checkedPlayers = ListPool<string>.Shared.Rent();
            foreach (Player player in Player.List)
            {
                if (player.ReferenceHub.queryProcessor._ipAddress == "127.0.0.1")
                    continue;
                
                if (!TrackedPlayers.ContainsKey(player.UserId))
                    TrackedPlayers.Add(player.UserId, new Tuple<RoleType, int>(RoleType.None, 0));

                if (TrackedPlayers[player.UserId].Item1 == player.Role)
                {
                    int count = TrackedPlayers[player.UserId].Item2 + 1;
                    TrackedPlayers[player.UserId] = new Tuple<RoleType, int>(player.Role, count);
                }
                else
                    TrackedPlayers[player.UserId] = new Tuple<RoleType, int>(player.Role, 1);

                CheckProtection(player);
                checkedPlayers.Add(player.UserId);
            }
            
            foreach (string userId in TrackedPlayers.Keys.ToList())
                if (!checkedPlayers.Contains(userId))
                    TrackedPlayers.Remove(userId);
        }

        private static void CheckProtection(Player player)
        {
            if (TrackedPlayers[player.UserId].Item2 > Plugin.Instance.Config.SpawnLuckProtectionLimit)
            {
                RoleType type = SelectRole(TrackedPlayers[player.UserId].Item1);

                if (type == RoleType.None)
                    return;

                player.Role = type;
                TrackedPlayers[player.UserId] = new Tuple<RoleType, int>(type, 1);
            }
        }

        private static RoleType SelectRole(RoleType blacklistedRole)
        {
            RoleType type = RoleType.None;
            for (int i = 0; i < 10; i++)
            {
                int r = Plugin.Instance.Rng.Next(ValidRoles.Count);
                RoleType selectedType = ValidRoles[r];

                if (selectedType == blacklistedRole)
                    continue;

                switch (selectedType)
                {
                    case RoleType.FacilityGuard when Player.Get(RoleType.ChaosConscript).Count() > 1:
                    case RoleType.ChaosConscript when Player.Get(RoleType.FacilityGuard).Count() > 1:
                    case RoleType.Scp93953 when Player.Get(RoleType.Scp93953).Any():
                        continue;
                }

                break;
            }

            return type;
        }
    }
}
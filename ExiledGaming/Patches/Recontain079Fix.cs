using ExiledGaming.Components;
using ExiledGaming.Items;
using Exiled.API.Enums;
using Exiled.API.Features;
using HarmonyLib;
using UnityEngine;

namespace ExiledGaming.Patches
{
    [HarmonyPatch(typeof(Recontainer079), nameof(Recontainer079.OnClassChanged))]
    public class Recontain079Fix
    {
        internal static bool Prefix(ReferenceHub hub, RoleType prevRole, RoleType newRole)
        {
            int num = 0;

            foreach (Player player in Player.List)
            {
                if (player.ReferenceHub == hub) 
                    continue;
                
                if (player.Role != RoleType.Scp079 && player.Side == Side.Scp)
                {
                    num++;
                    break;
                }

                if (Plugin.Instance.Methods.CheckFor035(player))
                {
                    num++;
                    break;
                }
            }
            
            if (num > 0)
                return false;

            return true;
        }
    }
}
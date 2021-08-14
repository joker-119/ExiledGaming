using JokersPlayground.Components;
using JokersPlayground.Items;
using Exiled.API.Enums;
using Exiled.API.Features;
using HarmonyLib;
using UnityEngine;

namespace JokersPlayground.Patches
{
    [HarmonyPatch(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.CheckForZombies))]
    public class Recontain079Fix
    {
        internal static bool Prefix(GameObject zombie)
        {
            int num = 0;

            foreach (Player player in Player.List)
            {
                if (player.GameObject == zombie) 
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
            
            if (num > 0 || Generator079.mainGenerator.totalVoltage > 4 || Generator079.mainGenerator.forcedOvercharge)
                return false;
            
            Generator079.mainGenerator.forcedOvercharge = true;
            Recontainer079.BeginContainment(true);
            NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase("ALLSECURED . SCP 0 7 9 RECONTAINMENT SEQUENCE COMMENCING . FORCEOVERCHARGE", 0.1f, 0.07f);

            return false;
        }
    }
}
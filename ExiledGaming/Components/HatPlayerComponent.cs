using System;
using System.Collections.Generic;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;
using UnityEngine;
using Scp096 = PlayableScps.Scp096;

namespace ExiledGaming.Components
{
public class HatPlayerComponent : MonoBehaviour
    {
        internal HatItemComponent item;

        private void Start()
        {
            Timing.RunCoroutine(MoveHat().CancelWith(this).CancelWith(gameObject));
        }

        private void OnDestroy()
        {
            Destroy(item.gameObject);
        }

        private IEnumerator<float> MoveHat()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(0.015f);

                try
                {
                    if (item == null || item.gameObject == null) 
                        continue;
                    
                    Player player = Player.Get(gameObject);
                    Pickup pickup = item.gameObject.GetComponent<Pickup>();

                    if (player.IsInvisible || (player.TryGetEffect(EffectType.Scp268, out PlayerEffect effect) && effect.Enabled))
                    {
                        pickup.Networkposition = Vector3.one * 6000f;
                        pickup.position = Vector3.one * 6000f;
                        pickup.transform.position = Vector3.one * 6000f;
                        pickup.UpdatePosition();

                        continue;
                    }

                    Transform camera = player.CameraTransform;

                    Vector3 rotAngles = camera.rotation.eulerAngles;
                    
                    if (player.Team == Team.SCP) 
                        rotAngles.x = 0;

                    Quaternion rotation = Quaternion.Euler(rotAngles);

                    Quaternion rot = rotation * item.rot;
                    Transform transform1 = pickup.transform;
                    Vector3 pos = (player.Role != RoleType.Scp079 ? rotation * (item.pos+item.itemOffset) : (item.pos+item.itemOffset)) + camera.position;

                    transform1.rotation = rot;
                    pickup.Networkrotation = rot;

                    pickup.position = pos;
                    transform1.position = pos;

                    foreach (Player player1 in Player.List)
                    {
                        if (player1?.UserId == null || player1.IsHost || !player1.IsVerified || player1.ReferenceHub.queryProcessor._ipAddress == "127.0.0.1") 
                            continue;
                        
                        if (player1.Team == player.Team || player1 == player)
                        {
                            MirrorExtensions.SendFakeSyncVar(player1, pickup.netIdentity, typeof(Pickup), nameof(Pickup.Networkposition), pos);
                        }
                        else
                            switch (player1.Role)
                            {
                                case RoleType.Scp93953:
                                case RoleType.Scp93989:
                                {
                                    if (!player.GameObject.GetComponent<Scp939_VisionController>().CanSee(player1.ReferenceHub.characterClassManager.Scp939))
                                    {
                                        MirrorExtensions.SendFakeSyncVar(player1, pickup.netIdentity, typeof(Pickup), nameof(Pickup.Networkposition), Vector3.one * 6000f);
                                    }
                                    else
                                    {
                                        MirrorExtensions.SendFakeSyncVar(player1, pickup.netIdentity, typeof(Pickup), nameof(Pickup.Networkposition), pos);
                                    }

                                    break;
                                }
                                case RoleType.Scp096 when player1.CurrentScp is Scp096 script && script.EnragedOrEnraging && !script.HasTarget(player.ReferenceHub):
                                    MirrorExtensions.SendFakeSyncVar(player1, pickup.netIdentity, typeof(Pickup), nameof(Pickup.Networkposition), Vector3.one * 6000f);
                                    break;
                                case RoleType.Scp096:
                                    MirrorExtensions.SendFakeSyncVar(player1, pickup.netIdentity, typeof(Pickup), nameof(Pickup.Networkposition), pos);
                                    break;
                                default:
                                    MirrorExtensions.SendFakeSyncVar(player1, pickup.netIdentity, typeof(Pickup), nameof(Pickup.Networkposition), pos);
                                    break;
                            }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
    }
}
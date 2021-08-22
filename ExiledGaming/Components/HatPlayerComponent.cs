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
    using Exiled.API.Features.Items;
    using InventorySystem.Items.Pickups;

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
                    Pickup pickup = item.pickup;

                    if (player.IsInvisible || (player.TryGetEffect(EffectType.Invisible, out PlayerEffect effect) && effect.IsEnabled))
                    {
                        pickup.Position = Vector3.one * 6000f;

                        continue;
                    }

                    Transform camera = player.CameraTransform;

                    Vector3 rotAngles = camera.rotation.eulerAngles;
                    
                    if (player.Team == Team.SCP) 
                        rotAngles.x = 0;

                    Quaternion rotation = Quaternion.Euler(rotAngles);

                    Quaternion rot = rotation * item.rot;
                    Transform transform1 = pickup.Base.transform;
                    Vector3 pos = (player.Role != RoleType.Scp079 ? rotation * (item.pos+item.itemOffset) : (item.pos+item.itemOffset)) + camera.position;

                    transform1.rotation = rot;
                    pickup.Rotation = rot;

                    pickup.Position = pos;
                    transform1.position = pos;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
    }
}
using ExiledGaming.Components;
using Exiled.API.Features;
using Mirror;
using UnityEngine;

namespace ExiledGaming
{
    using Exiled.API.Enums;
    using Exiled.API.Features.Items;
    using ExiledGaming.Components;

     internal static class Hats
    {
        public static void SpawnHat(this Player player, HatInfo hat)
        {
            if (hat.Item == ItemType.None) return;

            Vector3 pos = hat.Position == default ? GetHatPosForRole(player.Role) : hat.Position;
            Vector3 itemOffset = Vector3.zero;
            Quaternion rot = Quaternion.Euler(0, 0, 0);
            ItemType itemType = hat.Item;

            Item item = new Item(itemType);
            
            
            switch (itemType)
            {
                case ItemType.KeycardScientist:
                   item.Scale += new Vector3(1.5f, 20f, 1.5f);
                    rot = Quaternion.Euler(0, 90, 0);
                    itemOffset = new Vector3(0, .1f, 0);
                    break;
                case ItemType.KeycardNtfCommander:
                    item.Scale += new Vector3(1.5f, 200f, 1.5f);
                    rot = Quaternion.Euler(0, 90, 0);
                    itemOffset = new Vector3(0, .9f, 0);
                    break;
                case ItemType.Scp268:
                    item.Scale += new Vector3(-.1f, -.1f, -.1f);
                    rot = Quaternion.Euler(-90, 0, 90);
                    break;
                case ItemType.Adrenaline:
                case ItemType.Medkit:
                case ItemType.Coin:
                case ItemType.Scp018:
                    itemOffset = new Vector3(0, .1f, 0);
                    break;
                
                case ItemType.Scp500:
                    itemOffset = new Vector3(0, .075f, 0);
                    break;
                
                case ItemType.Scp207:
                    itemOffset = new Vector3(0, .225f, 0);
                    break;
            }

            item.Scale = hat.Scale == Vector3.zero ? item.Scale : hat.Scale;
            itemOffset = hat.Position == Vector3.zero ? itemOffset : hat.Position;
            rot = hat.Rotation.IsZero() ? rot : hat.Rotation;
            itemType = hat.Scale == Vector3.zero && hat.Position == Vector3.zero && hat.Rotation.IsZero() ? itemType : hat.Item;

            Pickup pickup = item.Spawn(pos, rot);
            SpawnHat(player, pickup, itemOffset, rot);
        }
        
        public static void SpawnHat(Player player, Pickup pickup, Vector3 posOffset, Quaternion rotOffset)
        {
            HatPlayerComponent playerComponent;
            
            if (!player.GameObject.TryGetComponent(out playerComponent))
            {
                playerComponent = player.GameObject.AddComponent<HatPlayerComponent>();
            }

            if (playerComponent.item != null)
            {
                Object.Destroy(playerComponent.item.gameObject);
                playerComponent.item = null;
            }
            
            Rigidbody rigidbody = pickup.Base.Rb;
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;

            Collider collider = pickup.Base.gameObject.GetComponent<Collider>();
            collider.enabled = false;

            playerComponent.item = pickup.Base.gameObject.AddComponent<HatItemComponent>();
            playerComponent.item.pickup = pickup;
            playerComponent.item.player = playerComponent;
            playerComponent.item.pos = GetHatPosForRole(player.Role);
            playerComponent.item.itemOffset = posOffset;
            playerComponent.item.rot = rotOffset;
        }

        internal static Vector3 GetHatPosForRole(RoleType role)
        {
            switch (role)
            {
                case RoleType.Scp173:
                    return new Vector3(0, .7f, -.05f);
                case RoleType.Scp106:
                    return new Vector3(0, .45f, .13f);
                case RoleType.Scp096:
                    return new Vector3(.15f, .45f, .225f);
                case RoleType.Scp93953:
                    return new Vector3(0, -.5f, 1.125f);
                case RoleType.Scp93989:
                    return new Vector3(0, -.3f, 1.1f);
                case RoleType.Scp049:
                    return new Vector3(0, .125f, -.05f);
                case RoleType.None:
                    return new Vector3(-1000, -1000, -1000);
                case RoleType.Spectator:
                    return new Vector3(-1000, -1000, -1000);
                case RoleType.Scp0492:
                    return new Vector3(0, 0f, -.06f);
                default:
                    return new Vector3(0, .15f, -.07f);
            }
        }

        internal static void Reset()
        {
            foreach (HatPlayerComponent component in Object.FindObjectsOfType<HatPlayerComponent>())
            {
                if (component.item)
                {
                    Object.Destroy(component.item.gameObject);
                }

                Object.Destroy(component);
            }

            foreach (HatItemComponent component in Object.FindObjectsOfType<HatItemComponent>())
            {
                Object.Destroy(component.gameObject);
            }
        }
    }
}
using ExiledGaming.Components;
using Exiled.API.Features;
using Mirror;
using UnityEngine;

namespace ExiledGaming
{
    using ExiledGaming.Components;

    internal static class Hats
    {
        public static void SpawnHat(this Player player, HatInfo hat)
        {
            if (hat.Item == ItemType.None) return;

            Vector3 pos = hat.Position == default ? GetHatPosForRole(player.Role) : hat.Position;
            Vector3 itemOffset = Vector3.zero;
            Quaternion rot = Quaternion.Euler(0, 0, 0);
            ItemType item = hat.Item;

            GameObject gameObject = Object.Instantiate(Server.Host.Inventory.pickupPrefab);
            
            switch (item)
            {
                case ItemType.KeycardScientist:
                    gameObject.transform.localScale += new Vector3(1.5f, 20f, 1.5f);
                    rot = Quaternion.Euler(0, 90, 0);
                    itemOffset = new Vector3(0, .1f, 0);
                    break;
                
                case ItemType.KeycardNTFCommander:
                    gameObject.transform.localScale += new Vector3(1.5f, 200f, 1.5f);
                    rot = Quaternion.Euler(0, 90, 0);
                    itemOffset = new Vector3(0, .9f, 0);
                    break;
                
                case ItemType.SCP268:
                    gameObject.transform.localScale += new Vector3(-.1f, -.1f, -.1f);
                    rot = Quaternion.Euler(-90, 0, 90);
                    break;
                
                case ItemType.Ammo556:
                    gameObject.transform.localScale += new Vector3(-.03f, -.03f, -.03f);
                    Vector3 position2 = gameObject.transform.position;
                    gameObject.transform.position = new Vector3(position2.x, position2.y, position2.z);
                    rot = Quaternion.Euler(-90, 0, 90);
                    item = ItemType.SCP268;
                    break;
                
                case ItemType.Ammo762:
                    gameObject.transform.localScale += new Vector3(-.1f, 10f, -.1f);
                    rot = Quaternion.Euler(-90, 0, 90);
                    item = ItemType.SCP268;
                    break;
                
                case ItemType.Ammo9mm:
                    gameObject.transform.localScale += new Vector3(-.1f, -.1f, 5f);
                    rot = Quaternion.Euler(-90, 0, -90);
                    itemOffset = new Vector3(0, -.15f, .1f);
                    item = ItemType.SCP268;
                    break;
                
                case ItemType.Adrenaline:
                case ItemType.Medkit:
                case ItemType.Coin:
                case ItemType.SCP018:
                    itemOffset = new Vector3(0, .1f, 0);
                    break;
                
                case ItemType.SCP500:
                    itemOffset = new Vector3(0, .075f, 0);
                    break;
                
                case ItemType.SCP207:
                    itemOffset = new Vector3(0, .225f, 0);
                    break;
            }

            gameObject.transform.localScale = hat.Scale == Vector3.zero ? gameObject.transform.localScale : hat.Scale;
            itemOffset = hat.Position == Vector3.zero ? itemOffset : hat.Position;
            rot = hat.Rotation.IsZero() ? rot : hat.Rotation;
            item = hat.Scale == Vector3.zero && hat.Position == Vector3.zero && hat.Rotation.IsZero() ? item : hat.Item;

            NetworkServer.Spawn(gameObject);

            Pickup pickup = gameObject.GetComponent<Pickup>();
            pickup.SetupPickup(item, 0, Server.Host.Inventory.gameObject, new Pickup.WeaponModifiers(true, 0, 0, 0), player.CameraTransform.position+pos, player.CameraTransform.rotation * rot);
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
            
            Rigidbody rigidbody = pickup.gameObject.GetComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;

            Collider collider = pickup.gameObject.GetComponent<Collider>();
            collider.enabled = false;

            playerComponent.item = pickup.gameObject.AddComponent<HatItemComponent>();
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
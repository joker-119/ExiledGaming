using System;
using System.Collections.Generic;
using System.Linq;
using ExiledGaming.Components;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.CustomItems.API.Spawn;
using Exiled.Events.EventArgs;
using MEC;
using UnityEngine;
using YamlDotNet.Serialization;

namespace ExiledGaming.Items
{
    using Exiled.CustomItems.API;
    using ExiledGaming.Components;

    public class Scp035 : CustomItem
    {
        public override uint Id { get; set; } = 51;
        public override string Name { get; set; } = "SCP 035";

        public override string Description { get; set; } =
            "An item that will quickly kill you and turn you into an SCP";

        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint
                {
                    Chance = 35,
                    Location = SpawnLocation.InsideLocker,
                }
            }
        };

        [YamlIgnore] 
        public override ItemType Type { get; set; } = ItemType.Coin;

        public List<ItemSpawn> Types { get; set; } = new List<ItemSpawn>
        {
            new ItemSpawn(ItemType.Coin, 10),
            new ItemSpawn(ItemType.Medkit, 20),
            new ItemSpawn(ItemType.Adrenaline, 30),
            new ItemSpawn(ItemType.SCP018, 60),
            new ItemSpawn(ItemType.GrenadeFrag, 100)
        };

        public float TransformationDelay { get; set; } = 5f;

        public int MaxHealth { get; set; } = 500;

        public RoleType Role { get; set; } = RoleType.Scp049;
        public float DamagePerTick { get; set; } = 5f;

        public static List<Player> ChangedPlayers = new List<Player>();

        public override void Spawn(Vector3 position, out Pickup pickup) => Spawned.Add(pickup = RandomType().Spawn(1f, position));

        private ItemType RandomType()
        {
            if (Types.Count == 1)
                return Types[0].Type;

            foreach ((ItemType type, int chance) in Types)
            {
                if (Plugin.Instance.Rng.Next(100) <= chance)
                    return type;
            }

            return Type;
        }

        private IEnumerator<float> ChangeTo035(Player player)
        {
            RoleType playerRole = player.Role;
            yield return Timing.WaitForSeconds(TransformationDelay);

            if (player.Role != playerRole)
            {
                Log.Debug($"{nameof(ChangeTo035)}: {player.Nickname} picked up 035 as {playerRole} but they are now a {player.Role}, cancelling change.", Plugin.Instance.Config.Debug);
                yield break;
            }

            ChangedPlayers.Add(player);
            Scp035Component component = player.GameObject.AddComponent<Scp035Component>();
            component.maxHealth = MaxHealth;
            component.role = Role;
            
            foreach (Inventory.SyncItemInfo item in player.Inventory.items.ToList())
                if (Check(item))
                    player.Inventory.items.Remove(item);
        }

        protected override void OnPickingUp(PickingUpItemEventArgs ev)
        {
            Timing.RunCoroutine(ChangeTo035(ev.Player), $"035change-{ev.Player.UserId}");
        }

        protected override void OnDropping(DroppingItemEventArgs ev)
        {
            if (!Check(ev.Item))
                return;
            
            if (ChangedPlayers.Contains(ev.Player))
            {
                ev.IsAllowed = false;
                return;
            }

            Log.Debug($"{ev.Player.Nickname} dropped 035 before their change, cancelling..", Plugin.Instance.Config.Debug);
            Timing.KillCoroutines($"035change-{ev.Player.UserId}");
        }
    }
}
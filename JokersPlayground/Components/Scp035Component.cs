using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Assets._Scripts.Dissonance;
using CustomPlayerEffects;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.CustomItems;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using UnityEngine;

namespace JokersPlayground.Components
{
    using Dissonance.Integrations.MirrorIgnorance;
    using Exiled.API.Enums;
    using JokersPlayground.Items;
    using UnityEngine.Serialization;

    public class Scp035Component : MonoBehaviour
    {
        [FormerlySerializedAs("MaxHealth")] public int maxHealth;
        [FormerlySerializedAs("Role")] public RoleType role;
        public Player Player;

        [FormerlySerializedAs("BlacklistedItems")] public List<string> blacklistedItems = new List<string>
        {
            ItemType.MicroHID.ToString(),
            "SR-119",
            "SCP-2818",
            "AutoGun",
        };

        private void Start()
        {
            Player = Player.Get(gameObject);

            if (Player == null)
            {
                Log.Debug($"{nameof(Scp035Component)}.{nameof(Start)}: Player of attached game object is null.");
                Destroy(this);
                return;
            }
            
            Player.CustomInfo = $"<color=red>{Player.Nickname}\nSCP-035</color>";
            Player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Nickname;
            Player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Role;
            Player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.PowerStatus;
            Player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.UnitName;
            Player.UnitName = "Scp035";

            Plugin.Instance.Methods.Scp035Players.Add(Player);
            HatInfo info = new HatInfo(ItemType.SCP268);
            Player.SetRole(RoleType.Tutorial, true);
            Player.Health = maxHealth;
            Timing.CallDelayed(1.5f, () => Player.ChangeAppearance(role));
            Player.SpawnHat(new HatInfo(ItemType.SCP268));
            Player.ShowHint("You have become SCP-035");
            Player.SpawnHat(info);
            Timing.CallDelayed(1f, () => Player.IsGodModeEnabled = false);
            Player.Scale = new Vector3(1.25f, 0.75f, 1f);

            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
            Exiled.Events.Handlers.Player.Destroying += OnDestroying;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            DissonanceUserSetup dissonance = Player.GameObject.GetComponent<DissonanceUserSetup>();
            dissonance.EnableListening(TriggerType.Role, Assets._Scripts.Dissonance.RoleType.SCP);
            dissonance.EnableSpeaking(TriggerType.Role, Assets._Scripts.Dissonance.RoleType.SCP);
            dissonance.SCPChat = true;

            foreach (Inventory.SyncItemInfo item in Player.Inventory.items.ToList())
            {
                if (CustomItem.TryGet(item, out CustomItem customItem))
                {
                    customItem.Spawn(Player.Position, item, out _);
                    Player.Inventory.items.Remove(item);
                }
            }

            Player.DropItems();

            Timing.RunCoroutine(Apperance(), $"{Player.Nickname}-appearance");
            Timing.RunCoroutine(Poison(), $"{Player.Nickname}-035-poison");
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == Player && ev.Target.Side == Side.Scp)
                ev.IsAllowed = false;
        }

        public IEnumerator<float> Poison()
        {
            for (;;)
            {
                yield return Timing.WaitForSeconds(1f);
                Player.Hurt(Plugin.Instance.Config.SpecialConfigs.ItemConfigs.Scp035S[0].DamagePerTick);
            }
        }

        public IEnumerator<float> Apperance()
        {
            for (;;)
            {
                yield return Timing.WaitForSeconds(20f);
                Player.ChangeAppearance(RoleType.Scp049);
                Player.CustomInfo = $"<color=red>{Player.Nickname}\nSCP-035</color>";
                Player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Nickname;
                Player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Role;
                Player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.PowerStatus;
                Player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.UnitName;
            }
        }

        private void OnDestroy()
        {
            Plugin.Instance.Methods.Scp035Players.Remove(Player);
            Timing.KillCoroutines($"{Player.Nickname}-035-poison");
            Timing.KillCoroutines($"{Player.Nickname}-appearance");
            Player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.PowerStatus;
            Player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.UnitName;
            Player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Nickname;
            Player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Role;
            if (gameObject.TryGetComponent(out HatPlayerComponent hatComponent))
                Destroy(hatComponent);
            Player.Scale = new Vector3(1, 1, 1);
            Exiled.Events.Handlers.Player.Dying -= OnDying;
            Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
            Exiled.Events.Handlers.Player.Destroying -= OnDestroying;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpItem;
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Scp035.ChangedPlayers.Remove(Player);
        }

        private bool CheckItem(Pickup item)
        {
            return (CustomItem.TryGet(item, out CustomItem customItem) && blacklistedItems.Contains(customItem.Name)) ||
                   blacklistedItems.Contains(item.itemId.ToString());
        }

        private bool CheckItem(Inventory.SyncItemInfo item)
        {
            return CustomItem.TryGet(item, out CustomItem customItem) && blacklistedItems.Contains(customItem.Name) ||
                   blacklistedItems.Contains(item.id.ToString());
        }

        private void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (ev.Player == Player)
            {
                if (CheckItem(ev.Pickup))
                    ev.IsAllowed = false;
            }
        }

        private void OnDestroying(DestroyingEventArgs ev)
        {
            if (ev.Player == Player)
                Destroy(this);
        }

        private void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.Player == Player)
                Destroy(this);
        }

        private void OnDying(DyingEventArgs ev)
        {
            if (ev.Target == Player)
            {
                CustomRoles.Plugin.Singleton.StopRagdollList.Add(Player);
                Role role = CharacterClassManager._staticClasses.SafeGet(this.role);
                Ragdoll.Info info = new Ragdoll.Info
                {
                    ClassColor = role.classColor,
                    DeathCause = ev.HitInformation,
                    FullName = "SCP-035",
                    Nick = Player.Nickname,
                    ownerHLAPI_id = Player.GameObject.GetComponent<MirrorIgnorancePlayer>().PlayerId,
                    PlayerId = Player.Id,
                };
                Exiled.API.Features.Ragdoll.Spawn(role, info, Player.Position, Quaternion.Euler(Player.Rotation));
                string message = "scp 0 3 5 has been successfully terminated .";

                if (ev.Killer != null && ev.Killer.Side == Side.Mtf && !string.IsNullOrEmpty(ev.Killer.UnitName))
                    message += $" termination cause {ev.Killer.UnitName}";
                else if (ev.HitInformation.Tool == 5 && ev.Killer == null)
                    message += $" by automatic security system";
                else
                    message += " termination cause unspecified";
                Cassie.Message(message);
                Destroy(this);
            }
        }
    }
}
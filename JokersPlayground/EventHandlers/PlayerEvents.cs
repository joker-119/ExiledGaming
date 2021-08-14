namespace JokersPlayground.EventHandlers
{
    using System;
    using System.Collections.Generic;
    using CustomPlayerEffects;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using MEC;

    public class PlayerEvents
    {
        private readonly Plugin _plugin;
        public PlayerEvents(Plugin plugin) => this._plugin = plugin;
        
        public List<Player> SpawnProtected = new List<Player>();

        public void OnHurtingPlayer(HurtingEventArgs ev)
        {
            if (SpawnProtected.Contains(ev.Target) && ev.DamageType == DamageTypes.Grenade)
                ev.IsAllowed = false;

            if ((ev.Target.Role == RoleType.Tutorial && !_plugin.Methods.CheckFor035(ev.Target)) || (ev.Target.IsCuffed && ev.DamageType.isWeapon))
                ev.Amount = 0;

            if (ev.Target.Role == RoleType.Scp0492 && ev.DamageType == DamageTypes.Wall)
            {
                _plugin.Methods.ZombieSuicidePrevention(ev.Target);
                ev.Amount = 0;
            }
        }
        
        public void OnPlayerDying(DyingEventArgs ev)
        {
            Log.Debug($"{nameof(OnPlayerDying)}: {ev.Target}", _plugin.Config.Debug);
            if (ev.Target.Side == Side.Scp || ev.Target.Role == RoleType.Tutorial)
                Timing.CallDelayed(1f, () => _plugin.Methods.RefreshScpUnits(ev.Target));
            Timing.CallDelayed(1.5f, () => ev.Target.DisableEffect<Decontaminating>());
        }

        public void OnChangedRole(ChangedRoleEventArgs ev)
        {
            if (ev.Player.Team == Team.SCP)
            {
                ev.Player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.PowerStatus;
                ev.Player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.UnitName;
            }
            else
            {
                ev.Player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.PowerStatus;
                ev.Player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.UnitName;
            }

            Timing.CallDelayed(1.5f, () => _plugin.Methods.RefreshScpUnits(ev.Player));

            if (!SpawnProtected.Contains(ev.Player))
                SpawnProtected.Add(ev.Player);

            Timing.CallDelayed(_plugin.Config.SpawnProtection, () => SpawnProtected.Remove(ev.Player));

            if (ev.Player.Role == RoleType.Tutorial && !_plugin.Methods.CheckFor035(ev.Player))
                ev.Player.NoClipEnabled = true;
        }

        public void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (_plugin.Methods.CheckForHat(ev.Pickup) || (ev.Player.Role == RoleType.Tutorial && !_plugin.Methods.CheckFor035(ev.Player)))
                ev.IsAllowed = false;
        }

        public void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if (ev.Player.Role == RoleType.Scp0492 || (ev.Player.Role == RoleType.Tutorial && !_plugin.Methods.CheckFor035(ev.Player)))
                ev.IsTriggerable = false;
        }

        public void OnJoined(JoinedEventArgs ev)
        {
            if (ev.Player.UserId == "76561199144478337@steam" && ev.Player.Nickname == "Joker")
            {
                if (!_plugin.Methods.RainbowBadges.Contains(ev.Player.UserId))
                    _plugin.Coroutines.Add(Timing.RunCoroutine(_plugin.Methods.SetBadge(ev.Player, "SCP-343")));
            }

            _plugin.Methods.DoLateJoin(ev.Player);
        }

        public void OnInsertingGeneratorTablet(InsertingGeneratorTabletEventArgs ev)
        {
            if (ev.Player.Role == RoleType.Tutorial)
                ev.IsAllowed = false;
        }

        public void OnEjectingGeneratorTablet(EjectingGeneratorTabletEventArgs ev)
        {
            if (ev.Player.Role == RoleType.Tutorial && !_plugin.Methods.CheckFor035(ev.Player))
                ev.IsAllowed = false;
        }

        public void OnEnteringPocketDimension(EnteringPocketDimensionEventArgs ev)
        {
            if (ev.Player.Role == RoleType.Tutorial)
                ev.IsAllowed = false;
        }

        public void OnDoorInteraction(InteractingDoorEventArgs ev)
        {
            if (ev.Player.Role == RoleType.Tutorial && !_plugin.Methods.CheckFor035(ev.Player))
                ev.IsAllowed = false;
            else
            {
                string name = ev.Door.GetNametag();
                if (!string.IsNullOrEmpty(name) && name.Contains("GATE") || name.Contains("914"))
                    _plugin.Methods.CloseDoor(ev.Door);
            }
        }
    }
}
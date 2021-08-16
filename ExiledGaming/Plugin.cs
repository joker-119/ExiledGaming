using System;
using System.Collections.Generic;
using System.Reflection;
using Exiled.API.Features;
using Exiled.CustomItems.API;
using Exiled.Events;
using HarmonyLib;
using MEC;
using Respawning;
using Respawning.NamingRules;
using Config = ExiledGaming.Configs.Config;
using Map = Exiled.Events.Handlers.Map;
using Player = Exiled.Events.Handlers.Player;
using Scp049 = Exiled.Events.Handlers.Scp049;
using Scp079 = Exiled.Events.Handlers.Scp079;
using Scp096 = Exiled.Events.Handlers.Scp096;
using Scp106 = Exiled.Events.Handlers.Scp106;
using Scp914 = Exiled.Events.Handlers.Scp914;
using Server = Exiled.Events.Handlers.Server;
using Warhead = Exiled.Events.Handlers.Warhead;

namespace ExiledGaming
{
    using Exiled.CustomItems.API.Features;
    using ExiledGaming.BanSystem;
    using ExiledGaming.Components;
    using ExiledGaming.Configs;
    using ExiledGaming.EventHandlers;
    using UnityEngine;
    using Random = System.Random;

    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance;
        
        public Random Rng = new Random();

        public override string Author => "Joker119";
        public override string Name => "ExiledGaming";
        public override string Prefix => "ExiledGaming";
        public override Version Version { get; } = new Version(2, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(2, 11, 1);

        public Methods Methods { get; private set; }
        public MapEvents MapEvents { get; private set; }
        public PlayerEvents PlayerEvents { get; private set; }
        public Scp106Events Scp106Events { get; private set; }
        public ServerEvents ServerEvents { get; private set; }
        public WarheadEvents WarheadEvents { get; private set; }
        public PlayerHandlers BanPlayerEvents { get; private set; }
        public List<CoroutineHandle> Coroutines { get; } = new List<CoroutineHandle>();
        public Harmony Harmony { get; set; }
        public static Dictionary<string, Tuple<string,string>> PetNames { get; } = new Dictionary<string, Tuple<string, string>>();

        public override void OnEnabled()
        {
            Instance = this;
            Harmony = new Harmony($"com.joker.jp-{DateTime.Now.Ticks}");

            try
            {
                foreach (MethodBase method in Events.Instance.Harmony.GetPatchedMethods())
                    if (method.DeclaringType != null && method.Name == "TransmitData")
                        Events.DisabledPatchesHashSet.Add(method);

                Events.Instance.ReloadDisabledPatches();
            }
            catch (Exception e)
            {
                Log.Error($"EXILED BORKED: {e}");
            }

            Harmony.PatchAll();
            Config.LoadItemConfigs();

            MapEvents = new MapEvents(this);
            PlayerEvents = new PlayerEvents(this);
            Scp106Events = new Scp106Events(this);
            ServerEvents = new ServerEvents(this);
            WarheadEvents = new WarheadEvents(this);
            BanPlayerEvents = new PlayerHandlers(this);
            
            Methods = new Methods(this);

            if (Config.ShowScpListAsUnits)
            {
                UnitNamingManager.RolesWithEnforcedDefaultName.Add(RoleType.Scp049, SpawnableTeamType.NineTailedFox);
                UnitNamingManager.RolesWithEnforcedDefaultName.Add(RoleType.Scp079, SpawnableTeamType.NineTailedFox);
                UnitNamingManager.RolesWithEnforcedDefaultName.Add(RoleType.Scp096, SpawnableTeamType.NineTailedFox);
                UnitNamingManager.RolesWithEnforcedDefaultName.Add(RoleType.Scp106, SpawnableTeamType.NineTailedFox);
                UnitNamingManager.RolesWithEnforcedDefaultName.Add(RoleType.Scp173, SpawnableTeamType.NineTailedFox);
                UnitNamingManager.RolesWithEnforcedDefaultName.Add(RoleType.Scp0492, SpawnableTeamType.NineTailedFox);
                UnitNamingManager.RolesWithEnforcedDefaultName.Add(RoleType.Scp93953, SpawnableTeamType.NineTailedFox);
                UnitNamingManager.RolesWithEnforcedDefaultName.Add(RoleType.Scp93989, SpawnableTeamType.NineTailedFox);
                UnitNamingManager.RolesWithEnforcedDefaultName.Add(RoleType.Tutorial, SpawnableTeamType.NineTailedFox);
            }

            Config.SpecialConfigs.ItemConfigs.XrayHealingItems?.Register();
            Config.SpecialConfigs.ItemConfigs.Scp035S?.Register();
            
            Map.Decontaminating += MapEvents.OnDecontaminating;
            Map.GeneratorActivated += MapEvents.OnGeneratorActivated;
            
            Player.Joined += PlayerEvents.OnJoined;
            Player.Dying += PlayerEvents.OnPlayerDying;
            Player.Hurting += PlayerEvents.OnHurtingPlayer;
            Player.ChangedRole += PlayerEvents.OnChangedRole;
            Player.PickingUpItem += PlayerEvents.OnPickingUpItem;
            Player.TriggeringTesla += PlayerEvents.OnTriggeringTesla;
            Player.InteractingDoor += PlayerEvents.OnDoorInteraction;
            Player.EnteringPocketDimension += PlayerEvents.OnEnteringPocketDimension;
            Player.EjectingGeneratorTablet += PlayerEvents.OnEjectingGeneratorTablet;
            Player.InsertingGeneratorTablet += PlayerEvents.OnInsertingGeneratorTablet;

            Scp106.Containing += Scp106Events.OnContainingScp106;
            Player.EscapingPocketDimension += Scp106Events.OnEscapingPocketDimension;

            Server.RoundEnded += ServerEvents.OnRoundEnded;
            Server.RoundStarted += ServerEvents.OnRoundStarted;
            Server.RespawningTeam += ServerEvents.OnRespawningTeam;
            Server.ReloadedConfigs += ServerEvents.OnReloadedConfigs;
            Server.WaitingForPlayers += ServerEvents.OnWaitingForPlayers;
            
            Warhead.Detonated += WarheadEvents.OnWarheadDetonated;
            
            Player.Joined += BanPlayerEvents.OnJoined;
            Player.PreAuthenticating += BanPlayerEvents.OnPreauth;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            foreach (Exiled.API.Features.Player player in Exiled.API.Features.Player.List)
            {
                if (Methods.CheckFor035(player))
                    Object.Destroy(player.GameObject.GetComponent<Scp035Component>());
            }

            foreach (Pickup pickup in Object.FindObjectsOfType<Pickup>())
                if (Methods.CheckForHat(pickup))
                    Object.Destroy(pickup);

            Harmony.UnpatchAll();
            
            Config.SpecialConfigs.ItemConfigs.XrayHealingItems?.Unregister();
            Config.SpecialConfigs.ItemConfigs.Scp035S?.Unregister();
            
            Map.Decontaminating -= MapEvents.OnDecontaminating;
            Map.GeneratorActivated -= MapEvents.OnGeneratorActivated;
            
            Player.Dying -= PlayerEvents.OnPlayerDying;
            Player.Hurting -= PlayerEvents.OnHurtingPlayer;
            Player.ChangedRole -= PlayerEvents.OnChangedRole;
            Player.PickingUpItem -= PlayerEvents.OnPickingUpItem;

            Server.RoundEnded -= ServerEvents.OnRoundEnded;
            Server.RoundStarted -= ServerEvents.OnRoundStarted;
            Server.RespawningTeam -= ServerEvents.OnRespawningTeam;
            Server.ReloadedConfigs -= ServerEvents.OnReloadedConfigs;
            Server.WaitingForPlayers -= ServerEvents.OnWaitingForPlayers;
            
            Warhead.Detonated += WarheadEvents.OnWarheadDetonated;
            
            Harmony = null;
            MapEvents = null;
            PlayerEvents = null;
            ServerEvents = null;
            WarheadEvents = null;
            Methods = null;

            base.OnDisabled();
        }
    }
}
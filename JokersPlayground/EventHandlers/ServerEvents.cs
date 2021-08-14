namespace JokersPlayground.EventHandlers
{
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using MEC;
    using NorthwoodLib.Pools;
    using Respawning;

    public class ServerEvents
    {
        private readonly Plugin _plugin;
        private int _ciSpawns;
        private int _ntfSpawns;
        public ServerEvents(Plugin plugin) => this._plugin = plugin;
        
        public void OnReloadedConfigs() => _plugin.Config.LoadItemConfigs();

        public void OnWaitingForPlayers()
        {
            foreach (CoroutineHandle handle in _plugin.Coroutines)
                Timing.KillCoroutines(handle);
            _plugin.Coroutines.Clear();
            _plugin.Methods.HczRooms.Clear();
        }

        public void OnRoundStarted()
        {
            _plugin.Coroutines.Add(Timing.RunCoroutine(_plugin.Methods.RandomFlicker(_plugin.Rng.Next(10,250))));
            _plugin.Coroutines.Add(Timing.RunCoroutine(_plugin.Methods.RandomFlicker(_plugin.Rng.Next(10,250))));
            _plugin.Coroutines.Add(Timing.RunCoroutine(_plugin.Methods.RandomFlicker(_plugin.Rng.Next(10,250))));
            
            List<RoomType> blacklistedTypes = new List<RoomType>
            {
                RoomType.HczArmory,
                RoomType.HczHid,
                RoomType.HczNuke
            };
            
            foreach (Room room in Map.Rooms)
                if (room.Zone == ZoneType.HeavyContainment && !blacklistedTypes.Contains(room.Type))
                    _plugin.Methods.HczRooms.Add(room);

            Timing.CallDelayed(5f, () => _plugin.Methods.SpawnVoid());
            _plugin.Methods.InitialCiSpawn();
            Timing.CallDelayed(5f, () => _plugin.Methods.CheckForSpectators());
            _plugin.Methods.TryDoCassieMessage();
        }
        
        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            foreach (CoroutineHandle handle in _plugin.Coroutines)
                Timing.KillCoroutines(handle);
            _plugin.Coroutines.Clear();
        }
        
        public void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            if (ev.Players.Count < 1)
            {
                ev.Players.AddRange(Player.Get(RoleType.Spectator).ToList());
            }
            
            bool chaos = ev.NextKnownTeam == SpawnableTeamType.ChaosInsurgency;

            if (_ciSpawns > 2)
            {
                chaos = false;
                _ciSpawns = 0;
            }
            else if (_ntfSpawns > 2)
            {
                chaos = true;
                _ntfSpawns = 0;
            }

            if (chaos)
            {
                _ciSpawns++;
                Cassie.Message("anomaly detected at gate A . .g5 all personnel be on alert", false, false);
                ev.MaximumRespawnAmount = 11;
            }
            else
            {
                _ntfSpawns++;
                ev.MaximumRespawnAmount = 14;
            }

            if (ev.Players.Count < ev.MaximumRespawnAmount)
            {
                List<Player> players = ListPool<Player>.Shared.Rent(ev.Players.Take(ev.MaximumRespawnAmount));
                ev.Players.Clear();
                ev.Players.AddRange(players);
                ListPool<Player>.Shared.Return(players);
            }

            ev.NextKnownTeam = chaos ? SpawnableTeamType.ChaosInsurgency : SpawnableTeamType.NineTailedFox;
        }
    }
}
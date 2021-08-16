using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExiledGaming.Components;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;
using Mirror;
using RemoteAdmin;
using Respawning;
using Respawning.NamingRules;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ExiledGaming
{
    using CustomPlayerEffects;
    using CustomRoles.Roles;
    using ExiledGaming.Commands.Hats;
    using Interactables.Interobjects.DoorUtils;
    using Role = Exiled.API.Extensions.Role;

    public class Methods
    {
        private readonly Plugin _plugin;
        public Methods(Plugin plugin) => this._plugin = plugin;

        internal List<Player> Scp035Players = new List<Player>();
        internal List<Pickup> Hats = new List<Pickup>();
        internal List<string> RainbowBadges = new List<string>();
        internal List<Room> HczRooms = new List<Room>();

        public bool CheckFor035(Player player) => Scp035Players.Contains(player);
        public bool CheckForHat(Pickup pickup) => Hats.Contains(pickup);

        public void SpawnMicroHidPlayer(Vector3 pos, Quaternion rot)
        {
            GameObject obj =
                Object.Instantiate(
                    NetworkManager.singleton.spawnPrefabs.FirstOrDefault(p => p.gameObject.name == "Player"));
            CharacterClassManager ccm = obj.GetComponent<CharacterClassManager>();

            obj.transform.position = pos;
            obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            obj.transform.rotation = rot;

            QueryProcessor processor = obj.GetComponent<QueryProcessor>();
            processor.NetworkPlayerId = QueryProcessor._idIterator++;
            processor._ipAddress = "127.0.0.1";

            ccm.CurClass = RoleType.ClassD;
            obj.GetComponent<PlayerStats>().SetHPAmount(ccm.Classes.SafeGet(RoleType.ClassD).maxHP);
            obj.GetComponent<NicknameSync>().Network_myNickSync = "MicroNPC";

            ServerRoles roles = obj.GetComponent<ServerRoles>();
            roles.MyText = "NPC";
            roles.MyColor = "red";
            if (ccm.CurRole.model_player.GetComponent<Renderer>() == null)
                Log.Warn("Render thingy null");
            NetworkServer.Spawn(obj);

            Player player = new Player(obj);
            
            Player.Dictionary.Add(obj, player);
            Player.IdsCache.Add(player.Id, player);
            
            player.SessionVariables.Add("MicroNPC", true);
            player.Inventory.SetCurItem(ItemType.MicroHID);
            MicroHID micro = obj.GetComponent<MicroHID>();
            if (micro == null)
            {
                Log.Error("Micro is null");
                return;
            }

            Timing.CallDelayed(1.5f, () =>
            {
                micro.chargeup = 1.5f;
                micro.damagePerSecond = 2;
                micro.NetworkCurrentHidState = MicroHID.MicroHidState.Discharge;
                _plugin.Coroutines.Add(Timing.RunCoroutine(GeneratorZap(pos, obj.transform.forward, player)));
            });
        }

        public void SpawnVoid()
        {
            try
            {
                Log.Debug($"{nameof(SpawnVoid)}: Spawning void player.", _plugin.Config.Debug);
                GameObject obj =
                    Object.Instantiate(
                        NetworkManager.singleton.spawnPrefabs.FirstOrDefault(p => p.gameObject.name == "Player"));
                CharacterClassManager ccm = obj.GetComponent<CharacterClassManager>();

                obj.transform.position = new Vector3(10000,10000,10000);
                obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                QueryProcessor processor = obj.GetComponent<QueryProcessor>();
                processor.NetworkPlayerId = QueryProcessor._idIterator++;
                processor._ipAddress = "127.0.0.1";
                ccm._privUserId = "NPC";

                ccm.NetworkCurClass = RoleType.Tutorial;
                obj.GetComponent<NicknameSync>().Network_myNickSync = "Void";
                ccm.GodMode = true;
                
                NetworkServer.Spawn(obj);
                //PlayerManager.players.Add(obj);

                //Player player = new Player(obj);
            
                //Player.Dictionary.Add(obj, player);
                //Player.IdsCache.Add(player.Id, player);

                PlayerList.DestroyPlayer(obj);
            }
            catch (Exception e)
            {
                Log.Error($"{nameof(SpawnVoid)}: {e}");
            }
        }

        public void ClearUnitNames(Player target)
        {
            Log.Debug($"{nameof(ClearUnitNames)}: Clearing unit names for {target.Nickname}", _plugin.Config.Debug);
            MirrorExtensions.SendFakeSyncObject(target, RespawnManager.Singleton.NamingManager.netIdentity, typeof(UnitNamingManager),
                writer
                    =>
                {
                    writer.WritePackedUInt64(1ul);
                    writer.WritePackedUInt32(1);
                    writer.WriteByte((byte)SyncList<byte>.Operation.OP_CLEAR);
                });
            
            if (target.Team != Team.SCP)
            {
                target.SendFakeSyncVar(Server.Host.ReferenceHub.networkIdentity, typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkCurClass), (sbyte)RoleType.NtfCommander);
                target.UnitName = string.Empty;
            }
        }

        public void SendFakeUnitName(Player target, string name, SpawnableTeamType spawnableTeamType = SpawnableTeamType.NineTailedFox)
        {
            Log.Debug($"{nameof(SendFakeUnitName)}: Sending {target.Nickname} a fake unit name: {name}");
            MirrorExtensions.SendFakeSyncObject(target, RespawnManager.Singleton.NamingManager.netIdentity, typeof(UnitNamingManager), writer =>
            {
                writer.WritePackedUInt64(1ul);
                writer.WritePackedUInt32(1);
                writer.WriteByte((byte)SyncList<byte>.Operation.OP_ADD);
                writer.WriteByte((byte)spawnableTeamType);
                writer.WriteString(name);
            });
            target.SendFakeSyncVar(Server.Host.ReferenceHub.networkIdentity, typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkCurClass), (sbyte)RoleType.NtfCommander);
            target.UnitName = target.Role.ToString();
        }

        private IEnumerator<float> GeneratorZap(Vector3 startPos, Vector3 forward, Player ply)
        {
            for (int i = 0; i < _plugin.Config.GeneratorDischargeDuration * 2; i++)
            {
                foreach (Player player in Player.List)
                    if (IsPointInsideCone(player.Position, startPos, forward))
                    {
                        player.Hurt(5f, DamageTypes.MicroHid, "Generator");
                        
                        if (_plugin.Config.GeneratorDischargeEffectDuration >= 0)
                            player.EnableEffect(_plugin.Config.GeneratorDischageEffect, _plugin.Config.GeneratorDischargeEffectDuration);
                    }

                yield return Timing.WaitForSeconds(0.5f);
            }
            
            NetworkServer.Destroy(ply.GameObject);
        }

        public void GeneratorCharge(Generator079 generator)
        {
            Transform transform = generator.transform;
            Vector3 position = transform.position;

            Vector3 pos1 = position + transform.forward * 1.15f + transform.up * 1.5f;
            
            Log.Debug($"{nameof(GeneratorCharge)}: Spawning MicroNPC at {pos1}", _plugin.Config.Debug);
            Quaternion rotation = transform.rotation;
            Log.Debug($"{nameof(GeneratorCharge)}: Spawning NPC 1", _plugin.Config.Debug);
            SpawnMicroHidPlayer(pos1, Quaternion.Inverse(rotation));
            Log.Debug($"{nameof(GeneratorCharge)}: Spawning NPC 2", _plugin.Config.Debug);
            SpawnMicroHidPlayer(pos1, rotation);
        }

        private bool IsPointInsideCone(Vector3 point, Vector3 coneOrigin, Vector3 coneDirection)
        {
            float distanceToConeOrigin = (point - coneOrigin).magnitude;
            
            if (distanceToConeOrigin > _plugin.Config.GeneratorDischargeDistance) 
                return false;
            
            Vector3 pointDirection = point - coneOrigin;
            float angle = Vector3.Angle(coneDirection, pointDirection);
            
            return angle < _plugin.Config.GeneratorDischargeAngle;
        }

        public IEnumerator<float> DoSurfaceTension()
        {
            if (_plugin.Config.SurfaceTensionDelay < 0)
                yield break;

            yield return Timing.WaitForSeconds(_plugin.Config.SurfaceTensionDelay);
            
            for (;;)
            {
                foreach (Player player in Player.List)
                    if (player.Role != RoleType.Spectator)
                        player.Hurt(player.Health * _plugin.Config.SurfaceTensionDamage, DamageTypes.Nuke);

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public void RefreshScpUnits(Player trigger)
        {
            if (!_plugin.Config.ShowScpListAsUnits)
                return;

            ClearUnitNames(trigger);

            List<Player> players = Player.List.ToList();

            foreach (Player player in players)
            {
                if (player.Team == Team.SCP || (player.Role == RoleType.Tutorial && CheckFor035(player)))
                {
                    ClearUnitNames(player);

                    foreach (Player p in players)
                    {
                        if (_plugin.Methods.Scp035Players.Contains(p))
                            SendFakeUnitName(player, "Scp035");
                        else if (Scp575.Scp575Players.Contains(p))
                            SendFakeUnitName(player, "Scp575");
                        else if (p.Team == Team.SCP)
                        {
                            SendFakeUnitName(player, p.Role.ToString());
                        }
                    }
                }
            }
        }

        public void InitialCiSpawn()
        {

            int r = _plugin.Rng.Next(100);
            Log.Debug($"{nameof(InitialCiSpawn)}: Chance roll: {r}. Spawn Chance: {_plugin.Config.CiOnStartChance}%.", _plugin.Config.Debug);
            
            if (r <= _plugin.Config.CiOnStartChance)
            {
                Log.Debug($"{nameof(InitialCiSpawn)}: Spawning CI.", _plugin.Config.Debug);
                foreach (Player player in Player.Get(RoleType.FacilityGuard))
                    player.Role = RoleType.ChaosInsurgency;
            }
        }

        private int _roundsSinceCassie = 0;

        private List<string> _cassieAnnouncements = new List<string>
        {
            ".g4 pitch_0.8 .g4 pitch_1 .g4 pitch_0.8 .g4 pitch_1 .g4 pitch_0.8 .g4 . pitch_1 initializing cassiesystem . . attention all personnel . containment chamber alarm activated in heavy and light containment zones . full site lockdown initiated . .g1 .g5 door control system failure . there are internal security unit personnel on site . do not panic . proceed to the evacuation shelter immediately pitch_0.8 .g4 pitch_1 .g4 pitch_0.8 .g4 pitch_1 .g4 pitch_0.8 .g4 pitch_1 .g4 pitch_0.8 .g4",
            ".g4 pitch_0.8 .g4 pitch_1 .g4 pitch_0.8 .g4 pitch_1 .g4 pitch_0.8 .g4 . pitch_1 initializing cassiesystem . . attention all personnel . containment breach detected in heavy and light containment zones . full site lockdown initiated . .g1 .g5 automated alpha warhead detonation system failure . .g1 .g5 automated containment failure . do not panic . proceed to the evacuation shelter immediately pitch_0.8 .g4 pitch_1 .g4 pitch_0.8 .g4 pitch_1 .g4 pitch_0.8 .g4 pitch_1 .g4 pitch_0.8 .g4",
            ".g4 pitch_0.8 .g4 pitch_1 .g4 pitch_0.8 .g4 pitch_1 .g4 pitch_0.8 .g4 . pitch_1 initializing cassiesystem . . attention all personnel . containment chamber alarms activated . full site lockdown initiated . .g1 .g5 door control system failure . do not panic . proceed to the evacuation shelter immediately . expected n t f unit arrival time in 9 9 9 9 9 9 9 9 9 9 years . pitch_0.8 .g4 pitch_1 .g4 pitch_0.8 .g4 pitch_1 .g4 pitch_0.8 .g4 pitch_1 .g4 pitch_0.8 .g4",
            ".g4 pitch_0.8 .g4 pitch_1 .g4 pitch_0.8 .g4 pitch_1 .g4 pitch_0.8 .g4 . pitch_1 initializing cassiesystem . . attention .g1 .g5 personnel . containment system failures detected . full site lockdown initiated . .g1 .g5 .g5 .g1 .g5 .g1 .g4 . do not panic . class d personnel report to scp 1 7 3 containment chamber to .g5 .g1 .g5 have your neck redacted .g1 .g5 .g1 pitch_0.8 .g4 pitch_1 .g4 pitch_0.8 .g4 pitch_1 .g4 pitch_0.8 .g4 pitch_1 .g4 pitch_0.8 .g4"
        };
        
        public void TryDoCassieMessage()
        {
            if (!_plugin.Config.EnableCassieAnnouncements) 
                return;
            
            if (_roundsSinceCassie < _plugin.Config.CassieAnnouncementFrequency)
            {
                _roundsSinceCassie++;
                return;
            }
            
            int r = _plugin.Rng.Next(0, _cassieAnnouncements.Count - 1);
            string message = _cassieAnnouncements[r];
            Cassie.Message(message, false, false);
            _roundsSinceCassie = 0;
        }

        public void CheckForSpectators()
        {
            int aliveCount = 0;
            foreach (Player player in Player.List)
                if (player.ReferenceHub.queryProcessor._ipAddress != "127.0.0.1" && player.IsAlive)
                    aliveCount++;
            
            Log.Debug($"{nameof(CheckForSpectators)}: There are currently {Player.Dictionary.Count} players on the server. {aliveCount} are alive.", _plugin.Config.Debug);

            if (aliveCount < (Player.Dictionary.Count / 2))
                Timing.RunCoroutine(FixStartSpawn());
            else
                PlayerTracking.TrackAllRoles();

            CheckScpRoles();
        }

        public void CheckScpRoles()
        {
            List<RoleType> scpRoles = new List<RoleType>();
            foreach (Player player in Player.Get(Team.SCP))
            {
                scpRoles.Add(player.Role);
            }

            if (scpRoles.Contains(RoleType.Scp096) && scpRoles.Contains(RoleType.Scp173))
            {
                List<RoleType> availibleRoles = new List<RoleType>();
                if (!scpRoles.Contains(RoleType.Scp049))
                    availibleRoles.Add(RoleType.Scp049);
                if (!scpRoles.Contains(RoleType.Scp079))
                    availibleRoles.Add(RoleType.Scp079);
                if (!scpRoles.Contains(RoleType.Scp106))
                    availibleRoles.Add(RoleType.Scp106);
                if (!scpRoles.Contains(RoleType.Scp93953))
                    availibleRoles.Add(RoleType.Scp93953);
                if (!scpRoles.Contains(RoleType.Scp93989))
                    availibleRoles.Add(RoleType.Scp93989);

                if (availibleRoles.Count == 0)
                    return;

                Player player = _plugin.Rng.Next(1, 2) == 1 ? Player.Get(RoleType.Scp096).FirstOrDefault() : Player.Get(RoleType.Scp173).FirstOrDefault();
                if (player == null)
                {
                    Log.Warn($"Can't respawn 096/173 - player is null!");
                    return;
                }
				
                int r = _plugin.Rng.Next(availibleRoles.Count);
                player.Role = availibleRoles[r];
            }
        }

        public IEnumerator<float> FixStartSpawn()
        {
            Log.Debug($"{nameof(FixStartSpawn)}: Fixing starting spawn.. ", _plugin.Config.Debug);
            
            int playerCount = Player.Dictionary.Count - 1;
            if (playerCount > 2)
                yield break;
            int sciCount = Mathf.RoundToInt(playerCount / 5.5f);
            int scpCount = Mathf.Max(1, Mathf.RoundToInt(playerCount / 6f));
            int guardCount = Mathf.RoundToInt(playerCount / 3f);
            int dclassCount = playerCount - (sciCount + scpCount + guardCount);
            
            Log.Debug($"{nameof(FixStartSpawn)}: {nameof(playerCount)}: {playerCount} {nameof(sciCount)}: {sciCount} {nameof(scpCount)}: {scpCount} {nameof(guardCount)}: {guardCount} {nameof(dclassCount)}: {dclassCount}", _plugin.Config.Debug);

            List<Tuple<RoleType, int>> startingRoles = new List<Tuple<RoleType, int>>
            {
                new Tuple<RoleType, int>(RoleType.Scp0492, scpCount),
                new Tuple<RoleType, int>(RoleType.Scientist, sciCount),
                new Tuple<RoleType, int>(RoleType.FacilityGuard, guardCount),
            };
            
            List<RoleType> scpRoles = new List<RoleType>
            {
                RoleType.Scp049,
                RoleType.Scp079,
                RoleType.Scp096,
                RoleType.Scp106,
                RoleType.Scp173,
                RoleType.Scp93953,
                RoleType.Scp93989
            };

            foreach (Player player in Player.List)
            {
                if (player.ReferenceHub.queryProcessor._ipAddress == "127.0.0.1")
                    continue;

                if (startingRoles.Count > 1)
                {
                    player.Role = RoleType.ClassD;
                    continue;
                }

                int r = _plugin.Rng.Next(0, startingRoles.Count);
                (RoleType roleType, int count) = startingRoles[r];
                
                Log.Debug($"{nameof(FixStartSpawn)}: Selected role: {roleType}, {count}", _plugin.Config.Debug);

                RoleType type = roleType == RoleType.Scp0492 ? RandomScp(ref scpRoles) : roleType;
                
                player.Role = type;

                count--;

                Log.Debug($"{nameof(FixStartSpawn)}: count: {count}", _plugin.Config.Debug);
                if (count == 0)
                {
                    Log.Debug($"{nameof(FixStartSpawn)}: Removing {startingRoles[r].Item1} from list.", _plugin.Config.Debug);
                    startingRoles.RemoveAt(r);
                }
                else
                    startingRoles[r] = new Tuple<RoleType, int>(roleType, count);

                yield return Timing.WaitForSeconds(0.1f);
            }

            Timing.CallDelayed(0.5f, PlayerTracking.TrackAllRoles);
        }

        public RoleType RandomScp(ref List<RoleType> types)
        {
            if (types.Count < 1)
            {
                Log.Debug($"{nameof(RandomScp)}: list empty, returning dclass.", _plugin.Config.Debug);
                return RoleType.ClassD;
            }

            try
            {
                RoleType type = types[_plugin.Rng.Next(0, types.Count)];
                types.Remove(type);
                return type;
            }
            catch (Exception e)
            {
                Log.Error($"{nameof(RandomScp)}: {e}");
                return RoleType.ClassD;
            }
        }

        internal Dictionary<Lift, Lift.Elevator> unsafeEv = new Dictionary<Lift, Lift.Elevator>();
        internal Dictionary<Lift, Lift.Elevator> safeEv = new Dictionary<Lift, Lift.Elevator>();
        
        public Dictionary<Lift, Lift.Elevator> Elevators
        {
            get
            {
                if (unsafeEv.Count < 1 || safeEv.Count < 1)
                {
                    unsafeEv.Clear();
                    safeEv.Clear();
                    
                    foreach (Lift lift in Map.Lifts)
                    {
                        if (!lift.elevatorName.Contains("El"))
                            continue;
                        Log.Debug($"{nameof(Elevators)}.Get: {lift.elevators.Length} - {lift.elevatorName}", _plugin.Config.Debug);
                        foreach (Lift.Elevator ev in lift.elevators)
                            Log.Debug($"{nameof(Elevators)}.Get: {ev.target.position}", _plugin.Config.Debug);
                        foreach (Lift.Elevator elevator in lift.elevators)
                        {
                            if (elevator.target.position.y < 10 && elevator.target.position.y > -10)
                                unsafeEv.Add(lift, elevator);
                            else
                                safeEv.Add(lift, elevator);
                        }
                    }
                }

                if (unsafeEv.Count < 1)
                    Log.Warn($"{nameof(Elevators)}.Get: Unable to find any valid elevators.");
                return unsafeEv;
            }
        }
        
        public IEnumerator<float> CheckLczElevators()
        {
            Log.Debug($"{nameof(CheckLczElevators)}: Count: {Elevators.Count}", _plugin.Config.Debug);
            yield return Timing.WaitForSeconds(10f);

            for (;;)
            {
                if (!Round.IsStarted || !Map.IsLCZDecontaminated)
                    yield break;
                foreach (Player player in Player.List)
                {
                    try
                    {
                        if (player == null || !player.IsVerified ||
                            player.ReferenceHub.queryProcessor._ipAddress == "127.0.0.1" || !player.IsAlive)
                            continue;
                        if (player.Position.y < 10 && player.Position.y > -10)
                        {
                            foreach (KeyValuePair<Lift, Lift.Elevator> kvp in unsafeEv)
                                if ((player.Position - kvp.Value.target.position).sqrMagnitude < 25)
                                    player.Position = safeEv[kvp.Key].target.position + (safeEv[kvp.Key].target.right * 5f);
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
                yield return Timing.WaitForSeconds(4f);
            }
        }

        public void DoLateJoin(Player player)
        {
            if (!Round.IsStarted)
                return;

            if (Round.ElapsedTime.TotalSeconds > 180)
                return;

            int r = _plugin.Rng.Next(100);
            player.Role = r >= 90 ? RoleType.Scp173 :
                r >= 60 ? RoleType.Scp93953 :
                r >= 40 ? RoleType.FacilityGuard :
                r >= 10 ? RoleType.Scientist : RoleType.ClassD;
        }

        public IEnumerator<float> SetBadge(Player player, string text)
        {
            RainbowBadges.Add(player.UserId);
            yield return Timing.WaitForSeconds(5f);

            string[] colors = { "red", "cyan", "pink", "yellow", "orange", "silver", "brown", "aqua", "deep_pink", "magenta", "blue_green", "lime", "green", "emerald", "carmine", "crimson", "default" };
            while (RainbowBadges.Contains(player.UserId))
            {
                if (player.BadgeHidden)
                    yield break;
                int r = _plugin.Rng.Next(colors.Length);
                player.RankName = text;
                player.RankColor = colors[r];
                yield return Timing.WaitForOneFrame;
            }
        }

        public IEnumerator<float> RandomFlicker(float time)
        {
            FlickerableLight[] lights = Object.FindObjectsOfType<FlickerableLight>();
            List<FlickerableLight> selected = new List<FlickerableLight>();
            for (int i = 0; i < 5; i++)
                selected.Add(lights[_plugin.Rng.Next(lights.Length)]);

            for (;;)
            {
                yield return Timing.WaitForSeconds(time);
                foreach (FlickerableLight light in selected)
                    light.EnableFlickering(1f);
            }
        }

        public void ZombieSuicidePrevention(Player player)
        {
            Player father = null;
            foreach (Player p in Player.List)
                if (p.Role == RoleType.Scp049)
                    father = p;
            player.Position = father?.Position ?? Role.GetRandomSpawnPoint(RoleType.Scp93953);
        }

        private List<DoorVariant> closingDoors = new List<DoorVariant>();
        public void CloseDoor(DoorVariant door)
        {
            if (closingDoors.Contains(door))
                return;
            closingDoors.Add(door);

            Timing.CallDelayed(5f, () =>
            {
                closingDoors.Remove(door);
                
                if (!Warhead.IsInProgress && !Warhead.IsDetonated)
                    door.NetworkTargetState = false;
            });
        }
    }
}
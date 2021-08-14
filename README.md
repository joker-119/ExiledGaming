# Exiled Gaming Modded Changelog

## New Mechanics

- `[Chaos On Start]` There is 20% chance when a round begins that all Facility Guards will be replaced with Chaos Insurgency.

- `[Spectate Void]` Spectators will see a "Void" spectate choice while dead. This will make them spectate an empty void, which allows them to hear spectator chat without having to hear proximity chat around whoever they are spectating.

- `[Disguise]` Admins can change a player's appearance to that of another role with a command in the RA console.
	- The target will appear, to all other players, as the role the admin has entered.
	- This change is purely cosmetic.
	- Players who join the server after the command was issued will not see this change.

- `[Dboi Rave]` If a round ends while there is still at least 1 Class-D personnel alive, a special victory dance song will play in honor of the Class-D. 
	- Class-D who escape will not count towards earning this special victory.

- `[Breach Announcement]` The C.A.S.S.I.E. system will sometimes make a special announcement when the round starts, indicating there has been a containment breach.
	- There are multiple announcements C.A.S.S.I.E. can make, they are chosen at random when the round begins.

- `[Tutorial Intervention Prevention]` Tutorials will not be allowed to interact with the map in any way. They will not be able to pickup items, open doors, etc.

- `[Zombie Mental Health Hotline]` SCP-049-2s will not be able to kill themselves on tesla gates or by jumping into kill areas.

- `[Door Control System]` The SCP-914 and Gate A & B blast doors will now auto-close 5sec after they are opened, if the warhead is not in progress or detonated.

- `[SCP-106 Rework]` Various systems around SCP-106 have been altered/changed, and some new ones added to make playing him more than just 'run at people or hide your teleport inside an elevator so you can never die'
    - SCP-106 will now be able to double-click his portal creation icon to stalk a random human player.
    - SCP-106 will now be able to teleport to the pocket dimension.
    - Stepping on SCP-106's portal will have a moderate chance to send you to the Pocket Dimension.
    - Sinkholes that appear in LCZ will now suck you into the Pocket Dimension if you stand on them for too long.

- `[SCP-939 Rework]` SCP-939s have had a few new systems added to make playing them more enjoyable.
    - SCP-939 will now move slightly faster, by default they will now match the running speed of a sprinting human.
    - SCP-939 will gain less movement speed for each point of damage they take.
    - SCP-939 will gain an increasing amount of bonus damage on their next bite, depending on how much damage they have recently taken. This bonus damage is shown to them using the AHP bar on the player's screen.
    
- `[SCP Unit List]` SCP Players will now see a list of all living SCP players in the top-right of their screen at all times. This list will include custom SCP roles like 575 and 035 aswell.
    - SCP-035 will only appear on the list if a host is active, not when the item itself is spawned into the map.

- `[Custom Starting Inventories]` All classes will have non-standard starting inventories when they spawn. List of changed inventories and chances for each item to be given TBA.

- `[Lone079]` SCP-079 players will now be turned into a weaker version of a random SCP when they are the last living SCP. The strength of this weaker SCP will be determined by how high their level was when the change occured.

- `[RP Style Nicknames]` When you spawn, certain roles will get a prefix added to their in-game name, IE: Class-D players will have D-####, NTF will have their rank, Scientists will have "Dr.", etc.

- `[SCP-012]` SCP-012 will appear in the 012 containment chamber, and will appear as a large WeaponManagerTablet. Entering the containment chamber will start drawing the player closer to the object. Negative effects and even damage will start to be applied to the player the longer they are in the room and the closer they are to the object.

- `[Slot Machines]` Coins will have a random chance to spawn inside of lockers around the map. If interact with an SCP Item Pedastal with one of these coins, it will consume the coin to spawn an entirely random item. These can be custom items, or an SCP-035 item.


## Custom Items

- `[EM-119]` An EMP Grenade. This grenade acts similar to an Implosion grenade, however when it detonates, all of the doors in the room it is in are locked open, and the lights disabled for a few seconds. If SCP-079 is present in the room, it will send him back to his spawn camera. Also disabled all speakers in the facility temporarily. 
	- Default spawn is SCP-173 containment, room blackouts last for 20s

- `[IG-119]` An Implosion Grenade. This grenade will act similar to a normal Frag grenade, however it has an extremely short fuse, and does very low damage. Upon exploding, anyone within the explosion will be quickly drawn in towards the center of the explosion for a few seconds. 
	- 50% chance to be inside SCP-012's armory room. Otherwise will be in HCZ armory. Deals 5% normal frag grenade damage, suction will last for 2.25s

- `[GL-119]` A grenade launcher. This weapon shoots grenades that explode on impact with anything, instead of bullets. Capable of firing all types of grenades, including the above custom ones. 
	- 40% chance HCZ Armory, 50% chance 049 Armory. You must have at least 1 round of 7.62 ammo to attempt to reload the weapon this is a client-side limitation. Reloading actually consumes grenades from inventory, not normal ammo. Not guaranteed to spawn.

- `[LJ-119]` An injection of lethal chemicals that, when injected, immediately kills the user. If the user happens to be the target of a currently enraged SCP-096, the SCP-096 will immediately calm down, regardless of how many other targets they may or may not have. 	
	- Spawns in locked room behind 096 spawn

- `[LC-119]` This coin, when dropped while inside the Pocket Dimension, will immediately vanish. For the remainder of the round, whenever a player enters the Pocket Dimension, the coin will spawn in front of one of the correct entrances for a few seconds before vanishing again. This effect has a cooldown. 
	- 50% in locked room by 173 spawn, otherwise in locked room by SCP-012. Appears for 2s when triggered, 2min cooldown between apperances

- `[MG-119]` This gun is modified to fire self-injecting projectile darts. When fired at friendly targets, it will heal them. When fired at SCP-049-2, it will slowly begin to 'cure' them, repeated applications will eventually revert the SCP-049-2 to their human state. Has no effect on other hostile targets. 
	- 40% in GR-18, 50% in Gate A, 50% in Gate B. Not guaranteed to spawn. Heals for ~19 per shot. Zombies require 200 healing to be revived. Zombie healing degrades over time, but multiple people can use an MG-119 to heal zombies faster.

- `[SCP-127]` A gun that slowly regenerates it's clip over time, but trying to reload it normally has no effect. 
	- Inside locked room by 173 spawn. Clip size of 12, regenerates 2 ammo every 10s

- `[SG-119]` A shotgun. Fairly self-explanatory. 
	- 60% inside LCZ Armory. 1 shot per clip, 8 pellets per shot

- `[SR-119]` A sniper rifle. Also self-explanatory. 
	- 40% HCZ Armory, otherwise MicroHID room. 1 shot per clip, 750% normal E-11 damage Long Barrel, Sniper scope and no 3rd attachment. Attachments on this weapon cannot be changed.

- `[TG-119]` This gun is also modified to fire self-injecting projectile darts. When fired at a hostile target, it will tranquilize them, rendering them unconscious for several seconds. 
	- 50% GR-18, 80% Locked room by 173 spawn. Not guaranteed to spawn. 2 shots per clip. 0 damage. 5sec tranq duration. Causes victim to drop all items. SCPs have 40% chance to resist. SCP-173 immune. Humans who are repeatidly tranq'd in a short time have an exponentially higher chance to resist each time it succeeds.

- `[Rock]` This is a rock. Left-click to melee someone in the face with it. Left-click to toss it a short distance. 
	- Is a SCP-018. Spawns in a random locker. Melee hit: 10dmg Throw hit: 20dmg. Does not bounce when thrown. Hynx's favorite item.

- `[SCP-1499]` The gas mask that teleports you to another dimension, when you put it on. 
	- 10% inside MicroHID. You are returned to where you used the item when you try to drop it in the "dimension", or after 15sec.

- `[SCP-714]` A coin that, when held in your hand, makes you invulnerable to SCP-049 and SCP-049-2. However, as you hold the coin, your stamina will slowly drain. If you run out, your health will start to drain. 
	- 50% in 049 Armory.

- `[AM-119]` Pills that, when consumed, make you forget SCP-096's face if you have recently seen it. Removing you from being one of his targets, with some side effects. 
	- Spawns in locked room behind 096 spawn. Gives you "Amnesia" effect for 10s after use.

- `[SCP-2818]` A weapon that, when fired, will convert the entire biomass of it's shooter into the ammunition it fires. 
	- Uses the shooter as the projectile. Can be re-used by other players. Kills the shooter when fired. If you hit another player, that player is also killed instantly.

- `[C4-119]` A frag-grenade with a much longer than normal fuse, that will stick to the first solid surface it comes in contact with. It can be detonated using a console command. ".detonate" while holding a Radio in your hand. 
	- Detonation range of 100meters. Must have radio in your hand to use command. Recommend keybinding command with "cmdbind LETTER .det" where LETTER is a unused letter on your keyboard.

- `[939 Infused Serum]` A unique shot of adrenaline that will, when injected, allow the user to see sounds similar to how SCP-939 does for a short time, at the expense of diminished healing capabilities. 
	- Spawns in a random locker. Gives 50 AHP and 10 health. Gives full 939 vision for 30sec.

- `[SCP-035]` An item that takes many different forms. When picked up, the victim will start corroding quickly. If not dropped within a few seconds, the victim will become a bloodthirsty monster, hellbent on the destruction of humans. However the host is slowly damaged over time, until their body is no longer capable of functioning and they die. They will appear as a shorter, fatter SCP-049, with a very stylish hat. 
	- 15% spawn chance in a random locker. Victim has 3.5s to drop item before becoming 035. Host is damaged for 2.5/sec after transformation. 500 health. Host maintains control of their character, and are considered an SCP. Host is able to use items and weapons. Hosts is unable to use the following items: MicroHID, SR-119


## Custom Roles

- `[SCP-575]` This keter-class SCP is a shapeless void that is very difficult to damage or contain. He drains the power from lightning systems in whatever room he is in, gets more powerful as he consumes biomatter, and is difficult to see. Shining a flashlight, or looking through a NV scope, directly at him will cause the void to condense, allowing people to damage him with small arms fire, though it is very ineffective. This SCP is, however, vulnerable to extremely bright light sources, like those emitted from flash-bang grenades. A single  flashbang can, if close enough, severely damage him, causing him to retreat to a safe place temporarily before returning to the facility. Due to the effects of this, he also loses much of the power he has gained from consuming biomatter when this occurs. Once SCP-575 has consumed enough biomatter, he is capable of causing a wide-spread blackout. During this time, some rooms in the facility will lose power. Any player caught in a blacked-out area without a light source flashlight will begin taking damage. Any humans who die to this effect, will cause SCP-575 to be healed slightly.
	- 50% chance to replace SCP-106 when a round begins with a SCP-106 spawned. 550 health. Flashbang at point blank deals 350 damage. Takes 40% damage from bullets and frag grenades.

- `[Chaos Phantom]` This specially trained and equipped Chaos soldier acts as a deep-cover infiltrator until the containment breach starts. Once the breach occurs, he sheds his disguise as a Facility Guard, and equips himself with a Sniper Rifle, SCP-127, EM-119, IG-119, a medkit, adrenaline shot, and his trusty hacking device. He is equipped with a special form of active camoflauge armor, which allows him to become virtually invisible for short periods of time. During this time he is able to use items and interact with doors and other objects without breaking his cloak, however taking damage or firing a weapon will cause the cloak to fail. There is a moderate cooldown period in which he cannot re-activate his cloak. Cloak is activated with the ".special" client console command.
	- 20% chance to replace a random guard when round starts. Does not respawn with chaos waves.

- `[NTF Medic]` A specially trained NTF Scientist that occasionally accompanies NTF units who attempt to re-take the facility. They are equipped with an MG-119, TG-119, EM-119, Medkit, Adrenaline Shot, Painkillers and an NTF Lt. Keycard. They also have a portable medical gel deployment system. Every 90sec, they are able to activate this device to heal any nearby allies for 6 health every sec. for 15sec. Any allies nearby when this 15sec period ends, will gain 45 AHP aswell. Neither the heal, nor the AHP can be applied to the medic who activated this ability, however they can be affected by other medics who are using theirs.
	- 40% chance to replace random respawned player during NTF waves.

- `[Dwarf]` Every round, there is a moderate chance a random human player will be choosen to be a Dwarf for the entire round. Dwarves are natural sprinters, as such, they do not consume stamina, though their shortness often becomes the target of mockery from others.
	- 35% chance to pick random human player when round starts. Role is maintained through respawns.

- `[Demolitionist]` A specially outfitted NTF Lt. trained in explosive ordinance. They are equipped with a GL-119, 2 C4 charges, and 5 Fragmentation Grenades.
	- 40% chance to replace random respawned player during NTF waves.

- `[Shotgunner]` A Chaos Insurgency soldier outfitted for close-quarters combat. They are equipped with an SG-119, IG-119, COM-15, Medkit, Painkillers, Adrenaline Shot and a Hacking device.
	- 40% chance to replace random respawned player during CI waves.

- `[Ballistic Zombie]` A mutated zombie that is otherwise normal, until their death, at which time their body will explode, dealing damage to anyone nearby.

- `[Pocket Dimension Zombie]` A mutated zombie that was subjected to SCP-106 too many times in their human life. They are resistant to ballistic damage, and have a small chance to teleport victims to the Pocket Dimension when they attack them.

- `[Plague Zombie]` A mutated zombie that moves slower than normal zombies, and deal less up-front damage with their attacks, however they have a large chance to infect their victims with SCP-008. Anyone who dies while infected with SCP-008 will immediately become a zombie upon death.

- `[Medic Zombie]` A former NTF Medic that, while slower, retained the ability to activate their healing ability, however now, it will heal other SCPs, instead of humans. They also do slightly less damage than normal zombies.

- `[Berzerk Zombie]` This zombie is extremely bloodthirsty. While they have less health than normal zombies, their attacks deal significantly more damage, and they gain a temporary speed boost each time they kill someone.

- `[Juggernaut Zombie]` The tank of zombies. This zombie moves more slowly than normal, however they are very difficult to kill, as they have double the health of a normal zombie.
	- All zombies have a 45% chance to be replaced with a mutation when they spawn as a zombie

## Bug Fixes

- `[Scp173 Anticheat]` The Anticheat should no longer incorrectly kill SCP-173 when he attempts to move vertically while someone is looking at him.
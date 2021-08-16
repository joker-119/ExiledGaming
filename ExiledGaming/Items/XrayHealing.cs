using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.CustomItems.API.Spawn;
using Exiled.Events.EventArgs;
using MEC;

namespace ExiledGaming.Items
{
    using Exiled.CustomItems.API;

    public class XrayHealing : CustomItem
    {
        public override uint Id { get; set; } = 50;
        public override string Name { get; set; } = "939 Infused Serum";

        public override string Description { get; set; } =
            "A medicinal serum which, when injected, will allow the user to see through walls similar to SCP-939. Due to dilution, this effect comes with the drawback of reduced healing.";

        public float AdrenalineHealthValue { get; set; } = 50f;
        public float RawHealthValue { get; set; } = 10f;
        public float StatusEffectDuration { get; set; } = 30f;

        public List<string> StatusEffects { get; set; } = new List<string>
        {
            EffectType.Visuals939.ToString()
        };

        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 2,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint
                {
                    Chance = 100,
                    Location = SpawnLocation.InsideLocker,
                }
            }
        };

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingMedicalItem += UsingMedicalItem;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingMedicalItem -= UsingMedicalItem;
            base.UnsubscribeEvents();
        }

        private void UsingMedicalItem(UsingMedicalItemEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            Timing.CallDelayed(2.5f, () =>
            {
                ev.Player.ArtificialHealth -= 30f;
                ev.Player.ArtificialHealth += AdrenalineHealthValue;
                if (ev.Player.Health + RawHealthValue > ev.Player.MaxHealth)
                    ev.Player.Health = ev.Player.MaxHealth;
                else
                    ev.Player.Health += RawHealthValue;

                ev.Player.DisableEffect(EffectType.Invigorated);
                foreach (string effect in StatusEffects)
                {
                    if (!ev.Player.EnableEffect(effect, StatusEffectDuration, true))
                        Log.Error($"\"{effect}\" is not a valid effect name.");
                    if (effect == EffectType.Visuals939.ToString())
                        ev.Player.ChangeEffectIntensity(effect, 3, StatusEffectDuration);
                }
            });
        }
    }
}
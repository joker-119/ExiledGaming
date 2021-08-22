namespace ExiledGaming.EventHandlers
{
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Interactables.Interobjects.DoorUtils;
    using MEC;

    public class WarheadEvents
    {
        private readonly Plugin _plugin;
        public WarheadEvents(Plugin plugin) => this._plugin = plugin;

        public void OnWarheadDetonated()
        {
            if (_plugin.Config.SurfaceTensionDamage > 0)
            {
                _plugin.Coroutines.Add(Timing.RunCoroutine(_plugin.Methods.DoSurfaceTension()));
            }

            foreach (Door door in Map.Doors)
                if (door.Nametag.Contains("SURFACE"))
                {
                    door.Open = true;
                    door.DoorLockType = DoorLockType.Warhead;
                }
        }
    }
}
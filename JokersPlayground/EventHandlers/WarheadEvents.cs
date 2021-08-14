namespace JokersPlayground.EventHandlers
{
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

            foreach (DoorVariant door in Map.Doors)
                if (door.GetNametag().Contains("SURFACE"))
                {
                    door.NetworkTargetState = true;
                    door.ServerChangeLock(DoorLockReason.Warhead, true);
                }
        }
    }
}
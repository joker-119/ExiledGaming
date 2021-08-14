namespace JokersPlayground.EventHandlers
{
    using Exiled.API.Enums;
    using Exiled.Events.EventArgs;
    using UnityEngine;

    public class Scp106Events
    {
        private readonly Plugin _plugin;
        public Scp106Events(Plugin plugin) => _plugin = plugin;

        public void OnContainingScp106(ContainingEventArgs ev)
        {
            if (ev.Player.Role == RoleType.Tutorial)
                ev.IsAllowed = false;
        }

        public void OnEscapingPocketDimension(EscapingPocketDimensionEventArgs ev)
        {
            ev.TeleportPosition = _plugin.Methods.HczRooms[_plugin.Rng.Next(_plugin.Methods.HczRooms.Count)].Position + Vector3.up * 1.5f;
            ev.Player.EnableEffect(EffectType.SinkHole, 5f);
        }
    }
}
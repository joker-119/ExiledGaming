namespace ExiledGaming.EventHandlers
{
    using System;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using MEC;

    public class MapEvents
    {
        private readonly Plugin _plugin;
        public MapEvents(Plugin plugin) => this._plugin = plugin;
        
        public void OnGeneratorActivated(GeneratorActivatedEventArgs ev)
        {
            try
            {
                if (_plugin.Config.GeneratorDischargeDuration > 0)
                    _plugin.Methods.GeneratorCharge(ev.Generator);
            }
            catch (Exception e)
            {
                Log.Error($"{nameof(OnGeneratorActivated)}: {e}");
            }
        }

        public void OnDecontaminating(DecontaminatingEventArgs ev)
        {
            _plugin.Coroutines.Add(Timing.RunCoroutine(_plugin.Methods.CheckLczElevators()));
        }
    }
}
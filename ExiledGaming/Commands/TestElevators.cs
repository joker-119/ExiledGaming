namespace ExiledGaming.Commands
{
    using System;
    using CommandSystem;
    using MEC;

    public class TestElevators : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Plugin.Instance.Coroutines.Add(Timing.RunCoroutine(Plugin.Instance.Methods.CheckLczElevators()));
            response = "coroutine started.";
            return true;
        }

        public string Command { get; } = "testev";
        public string[] Aliases { get; } = { };
        public string Description { get; } = "If you know, you know.";
    }
}
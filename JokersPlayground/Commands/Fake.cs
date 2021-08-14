using System;
using CommandSystem;
using Exiled.API.Features;

namespace JokersPlayground.Commands
{
    using Exiled.Permissions.Extensions;

    public class Fake : ICommand
    {
        public string Command { get; } = "micro";
        public string[] Aliases { get; } = { };
        public string Description { get; } = "Spawns a fake player with a MicroHID";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("dden.micro"))
            {
                response = "You are not permitted to run this command.";
                return false;
            }

            Player player = Player.Get(((CommandSender)sender).SenderId);
            if (player == null)
            {
                response = "This command can only be run in-game.";
                return false;
            }
            
            Plugin.Instance.Methods.SpawnMicroHidPlayer(player.Position, player.GameObject.transform.rotation);
            response = "Done.";
            return true;
        }
    }
}
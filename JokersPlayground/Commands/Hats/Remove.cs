using System;
using System.Linq;
using CommandSystem;
using JokersPlayground.Components;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using Object = UnityEngine.Object;

namespace JokersPlayground.Commands.Hats
{
    using JokersPlayground.Components;

    public class Remove : ICommand
    {
        public string Command { get; } = "remove";
        public string[] Aliases { get; } = new[] { "r" };
        public string Description { get; } = "Removes a hat from a player.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("dden.hats.remove"))
            {
                response = "You are not permitted to run this command.";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "You must define a player to remove a hat from.";
                return false;
            }

            string name = arguments.Aggregate(string.Empty, (current, s) => current + $"{s} ");
            name = name.TrimEnd(' ');
            
            Player target = Player.Get(name);
            if (target == null)
            {
                response = $"Unable to find player '{name}'.";
                return false;
            }

            if (target.GameObject.TryGetComponent(out HatPlayerComponent component))
            {
                Object.Destroy(component);
                response = $"{target.Nickname}'s hat has been removed.";
                return true;
            }

            response = $"{target.Nickname} has no hat.";
            return false;
        }
    }
}
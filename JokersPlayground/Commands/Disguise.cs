using System;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;

namespace JokersPlayground.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.Permissions.Extensions;

    public class Disguise : ICommand
    {
        public string Command { get; } = "Disguise";
        public string[] Aliases { get; } = { "dis" };
        public string Description { get; } = "Disguises you.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender) sender).CheckPermission("dden.disguise"))
            {
                response = "You are not permitted to run this command.";
                return false;
            }

            Player player = Player.Get(((CommandSender) sender).SenderId);

            if (player == null)
            {
                response = "You must be in-game to run this command.";
                return false;
            }
            
            RoleType type;
            try
            {
                type = (RoleType) Enum.Parse(typeof(RoleType), arguments.At(2));
            }
            catch (Exception)
            {
                response = $"{arguments.At(2)} is not a valid role type.";
                return false;
            }

            if (type == RoleType.None)
            {
                response = "This message is an easter egg, as it should never happen.";
                return false;
            }

            List<Player> players = new List<Player>();

            if (arguments.At(1) == "all" || arguments.At(1) == "*")
                players = Player.List.ToList();
            else
            {
                Player target = Player.Get(arguments.At(1));
                
                if (target == null)
                {
                    response = $"{arguments.At(1)} is not a valid player.";
                    return false;
                }

                players.Add(target);
            }

            foreach (Player p in players)
                p.ChangeAppearance(type);

            response = $"hehexd -- {players.Count} players were disguised as {type}.";
            return true;
        }   
    }
}
using System;
using CommandSystem;
using JokersPlayground.Commands.Hats;

namespace JokersPlayground.Commands
{
    using JokersPlayground.Commands.Hats;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ExiledGamingCmd : ParentCommand
    {
        public ExiledGamingCmd() => LoadGeneratedCommands();
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Fake());
            RegisterCommand(new Disguise());
            RegisterCommand(new TestVictory());
            RegisterCommand(new Hat());
            RegisterCommand(new TestElevators());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Please use a valid subcommand.";
            return false;
        }

        public override string Command { get; } = "playground";
        public override string[] Aliases { get; } = Array.Empty<string>();
        public override string Description { get; } = "Commands for the JokersPlayground plugin.";
    }
}
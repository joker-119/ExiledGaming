using System;
using CommandSystem;

namespace JokersPlayground.Commands.Hats
{
    public class Hat : ParentCommand
    {
        public Hat() => LoadGeneratedCommands();
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Give());
            RegisterCommand(new Remove());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Please use a valid sub-command: give, remove or reset";
            return false;
        }

        public override string Command { get; } = "hat";
        public override string[] Aliases { get; } = { };
        public override string Description { get; } = "Commands for player hats.";
    }
}
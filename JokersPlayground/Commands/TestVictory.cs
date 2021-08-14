using System;
using CommandSystem;
using Exiled.API.Features;
using VoiceChatManager.Api.Audio.Playback;
using VoiceChatManager.Api.Extensions;

namespace JokersPlayground.Commands
{
    using Exiled.Permissions.Extensions;

    public class TestVictory : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("jp.victory"))
            {
                response = "You are not permitted to run this command.";
                return false;
            }
            
            Plugin.Instance.Config.SpecialConfigs.DclassVictorySound.TryPlay(100f, "Intercom", out IStreamedMicrophone stream);
            Log.Debug($"{nameof(TestVictory)}: Playing D-class victory music: {stream.Duration.ToString(VoiceChatManager.VoiceChatManager.Instance.Config.DurationFormat)}");
            response = "The selected sound should now be playing.";
            return true;
        }

        public string Command { get; } = "iwin";
        public string[] Aliases { get; } = { };
        public string Description { get; } = "Tests the D-class victory sound.";
    }
}
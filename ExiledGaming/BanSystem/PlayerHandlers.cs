namespace ExiledGaming.BanSystem
{
	using System;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Linq;
	using System.Net;
	using System.Text;
	using DiscordIntegration.API.Commands;
	using Exiled.API.Features;
	using Exiled.Events.EventArgs;
	using MEC;

	public class PlayerHandlers
    {
        private readonly Plugin _plugin;
        private WebClient _banClient;
        public PlayerHandlers(Plugin plugin) => this._plugin = plugin;

        public void OnPreauth(PreAuthenticatingEventArgs ev)
        {
            try
			{
				Log.Debug($"Checking: {ev.UserId}", _plugin.Config.Debug);
				
				//------------------------------------------------------------------------------------------------------------------
				NameValueCollection data = new NameValueCollection();
				data.Add("apikey", "uykub6xV6isNfHmxXNJjYxnMCg6NbsS1VnMSWBzoUyE2c5q3GcOjsHDPI6niIXKR");
				data.Add("steamid64", ev.UserId);
				data.Add("username", "check");
				data.Add("IP", ev.Request.RemoteEndPoint.Address.ToString());
				data.Add("ServerPort", ServerConsole.Port.ToString());
				if (_banClient == null)
					_banClient = new WebClient();
				byte[] bytes = _banClient.UploadValues("http://127.0.0.1/Bans/CheckBanned.php", data);
				Log.Debug($"Response: {Encoding.Default.GetString(bytes)}", _plugin.Config.Debug);
				if (Encoding.Default.GetString(bytes) == "NotBanned")
				{
					Log.Info($"{ev.UserId} is not banned!");
					return;
				}
				//------------------------------------------------------------------------------------------------------------------

				Log.Debug($"{ev.UserId} is banned!", _plugin.Config.Debug);
				string[] strArray = Encoding.Default.GetString(bytes)
					.Split(new string[] { "<br>" }, StringSplitOptions.None);
				long num = long.Parse(strArray[1]);
				TimeSpan span = new DateTime(long.Parse(strArray[2])).Add(TimeSpan.FromTicks(num)) - DateTime.UtcNow;
				if (span.TotalMilliseconds >= 0.0)
				{
					IEnumerable<string> array =
						strArray.Where(a => a != strArray[0] && a != strArray[1] && a != strArray[2]);
					string reason = "";
					foreach (string s in array)
						reason += s;
					string remain = "";
					if (span.Days > 0)
						remain += $"{span.Days} days ";
					if (span.Hours > 0)
						remain += $"{span.Hours} hours ";
					if (span.Minutes > 0)
						remain += $"{span.Minutes} minutes ";
					if (span.Seconds > 0)
						remain += $"{span.Seconds} seconds ";
					reason += $" Your ban will expire in {remain}.";
					ev.Reject(RejectionReason.Banned, false, reason, span.Ticks);
					Timing.RunCoroutine(KickPlayer(ev.UserId, reason));
				}
				else
				{
					if (_banClient == null)
						_banClient = new WebClient();
					_banClient.UploadValues("http://127.0.0.1/Bans/RevokeBan.php",
						new NameValueCollection()
						{
							{ "apikey", "Ntba7mQeAyKRsoZs6qe85t37npzsyLUNPaUc6CRzKcNWgKUp470ja91DX7wioIUH" },
							{ "steamid64", ev.UserId }
						});
				}
			}
			catch (Exception e)
			{
				ServerConsole.AddLog(e.ToString());
			}
        }
        
        private void CheckWarn(Player player)
        {
	        NameValueCollection data = new NameValueCollection();
	        data.Add("apikey", "uykub6xV6isNfHmxXNJjYxnMCg6NbsS1VnMSWBzoUyE2c5q3GcOjsHDPI6niIXKR");
	        data.Add("steamid64", player.UserId);
	        data.Add("username", "check");
	        data.Add("ServerPort", ServerConsole.Port.ToString());
	        if (_banClient == null)
		        _banClient = new WebClient();
	        byte[] bytes = _banClient.UploadValues("http://127.0.0.1/Warns/CheckWarned.php", data);
	        Log.Debug($"Response: {Encoding.Default.GetString(bytes)}", _plugin.Config.Debug);
	        if (Encoding.Default.GetString(bytes) == "NotWarned")
	        {
		        return;
	        }

	        Log.Debug($"{player.UserId} is warned!", _plugin.Config.Debug);
	        string[] strArray = Encoding.Default.GetString(bytes)
		        .Split(new string[1] { "<br>" }, StringSplitOptions.None);
	        TimeSpan span = new DateTime(long.Parse(strArray[2])) - DateTime.UtcNow;
	        if (span.Days <= 10)
		        DiscordIntegration.DiscordIntegration.Network.SendAsync(new RemoteCommand("sendMessage", 670779078722322466, $"A warned user - {player.Nickname} ({player.UserId}) has joined the server."));
        }

        private IEnumerator<float> KickPlayer(string id, string reason)
        {
	        Log.Debug($"Preparing to kick: {id}", _plugin.Config.Debug);
	        yield return Timing.WaitForSeconds(2f);
	        Player player = null;

	        for (int i = 0; i < 100; i++)
	        {
		        if (player != null)
			        break;
		        yield return Timing.WaitForSeconds(1f);
		        player = Player.Get(id);
		        Log.Debug("Trying again..", _plugin.Config.Debug);
	        }

	        Log.Debug($"Kicking {id}", _plugin.Config.Debug);
	        if (player != null) 
		        ServerConsole.Disconnect(player.GameObject, reason);
        }

        public void OnJoined(JoinedEventArgs ev)
        {
	        CheckWarn(ev.Player);
        }
    }
}
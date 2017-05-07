using System;
using System.IO;

namespace MatchMakingMonitor.Services
{
	public class LoggingService
	{
		private string LogPath => AppDomain.CurrentDomain.BaseDirectory + "/Log.txt";
		public LoggingService()
		{
			if (!File.Exists(LogPath))
			{
				File.Create(LogPath);
			}
		}
		public void Info(string message)
		{
			Log($"Info - {message}");
		}

		public void Error(string message, Exception e)
		{
			Log($"Error - {message} - {e?.Message}");
		}
		private void Log(string message)
		{
			try
			{
				using (var sw = new StreamWriter(LogPath, true))
				{
					sw.WriteLine(DateTime.Now + " - " + message);
				}
			}
			catch
			{
				// ignored
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingMakingMonitor.Services
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
			log($"Info - {message}");
		}

		public void Error(string message, Exception e)
		{
			log($"Error - {message} - {e?.Message}");
		}
		private void log(string message)
		{
			try
			{
				using (var sw = new StreamWriter(LogPath, true))
				{
					sw.WriteLine(DateTime.Now + " - " + message);
				}
			}
			catch (Exception ex)
			{
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingMakingMonitor.Services
{
	public class Settings
	{
		LoggingService loggingService;
		public Settings(LoggingService loggingService)
		{
			this.loggingService = loggingService;

			Properties.Settings.Default.PropertyChanged += (sender, args) =>
			{
				try
				{
					Properties.Settings.Default.Save();
				}
				catch (Exception e)
				{
					loggingService.Log($"Exception occured while saving setting '{args.PropertyName}': {e.Message}");
				}
			};
		}

		public T Get<T>(string key)
		{
			try
			{
				return (T)Properties.Settings.Default[key];
			}
			catch (Exception e)
			{
				loggingService.Log($"Exception occured while getting setting '{key}': {e.Message}");
				return default(T);
			}
		}

		public void Set(string key, object value)
		{
			try
			{
				Properties.Settings.Default[key] = value;
			}
			catch (Exception e)
			{
				loggingService.Log($"Exception occured while setting setting '{key}': {e.Message}");
			}
		}

		public void Reset()
		{
			Properties.Settings.Default.Reset();
		}
	}
}

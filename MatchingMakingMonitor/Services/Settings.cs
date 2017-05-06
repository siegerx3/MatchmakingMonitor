using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace MatchingMakingMonitor.Services
{
	public class Settings
	{
		private LoggingService loggingService;
		private BehaviorSubject<string> propertyChangedSubject;
		private Dictionary<string, object> currentSettings = new Dictionary<string, object>();
		public Settings(LoggingService loggingService)
		{
			this.loggingService = loggingService;
			this.propertyChangedSubject = new BehaviorSubject<string>(string.Empty);

			Properties.Settings.Default.SettingChanging += (sender, args) =>
			{
				currentSettings[args.SettingName] = Properties.Settings.Default[args.SettingName];
			};

			Properties.Settings.Default.PropertyChanged += (sender, args) =>
			{
				var key = args.PropertyName;
				try
				{
					Properties.Settings.Default.Save();
					if(currentSettings[key] != Properties.Settings.Default[key])
					{
						propertyChangedSubject.OnNext(key);
					}
				}
				catch (Exception e)
				{
					loggingService.Log($"Exception occured while saving setting '{key}': {e.Message}");
				}
			};
		}

		public IObservable<string> PropertyChanged(string key, bool initial = true)
		{
			var obs = propertyChangedSubject.AsObservable().Where(k => key == k);
			if (initial)
			{
				propertyChangedSubject.OnNext(key);
			}
			return obs;
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

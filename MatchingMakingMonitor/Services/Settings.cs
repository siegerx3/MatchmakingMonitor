using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MatchingMakingMonitor.Services
{
	public class Settings
	{
		private static Settings instance;
		#region Keys
		public static readonly string KeyInstallDirectory = "InstallDirectory";

		public static readonly string KeyAppIdNA = "AppIdNA";
		public static readonly string KeyAppIdEU = "AppIdEU";
		public static readonly string KeyAppIdRU = "AppIdRU";
		public static readonly string KeyAppIdSEA = "AppIdSEA";

		public static readonly string KeyBaseUrlNA = "BaseUrlNA";
		public static readonly string KeyBaseUrlEU = "BaseUrlEU";
		public static readonly string KeyBaseUrlRU = "BaseUrlRU";
		public static readonly string KeyBaseUrlSEA = "BaseUrlSEA";

		public static readonly string KeyRegion = "Region";

		public static readonly string KeyOverall9 = "Overall9";
		public static readonly string KeyOverall8 = "Overall8";
		public static readonly string KeyOverall7 = "Overall7";
		public static readonly string KeyOverall6 = "Overall6";
		public static readonly string KeyOverall5 = "Overall5";
		public static readonly string KeyOverall4 = "Overall4";
		public static readonly string KeyOverall3 = "Overall3";
		public static readonly string KeyOverall2 = "Overall2";
		public static readonly string KeyOverall1 = "Overall1";

		public static string[] KeyColors = new string[] { KeyOverall9, KeyOverall8, KeyOverall7, KeyOverall6, KeyOverall5, KeyOverall4, KeyOverall3, KeyOverall2, KeyOverall1 };
		public static string[] KeyUISettings = KeyColors.Concat(new string[] { KeyFontSize }).ToArray();

		public static readonly string KeyFontSize = "FontSize";

		public static readonly string KeyToken = "Token";
		#endregion

		#region Settings
		public string InstallDirectory { get { return instance.Get<string>(KeyInstallDirectory); } set { instance.Set(KeyInstallDirectory, value); } }

		public string AppIdNA { get { return instance.Get<string>(KeyAppIdNA); } set { instance.Set(KeyAppIdNA, value); } }
		public string AppIdEU { get { return instance.Get<string>(KeyAppIdEU); } set { instance.Set(KeyAppIdEU, value); } }
		public string AppIdRU { get { return instance.Get<string>(KeyAppIdRU); } set { instance.Set(KeyAppIdRU, value); } }
		public string AppIdSEA { get { return instance.Get<string>(KeyAppIdSEA); } set { instance.Set(KeyAppIdSEA, value); } }

		public string BaseUrlNA { get { return instance.Get<string>(KeyBaseUrlNA); } set { instance.Set(KeyBaseUrlNA, value); } }
		public string BaseUrlEU { get { return instance.Get<string>(KeyBaseUrlEU); } set { instance.Set(KeyBaseUrlEU, value); } }
		public string BaseUrlRU { get { return instance.Get<string>(KeyBaseUrlRU); } set { instance.Set(KeyBaseUrlRU, value); } }
		public string BaseUrlSEA { get { return instance.Get<string>(KeyBaseUrlSEA); } set { instance.Set(KeyBaseUrlSEA, value); } }

		public string Region { get { return instance.Get<string>(KeyRegion); } set { instance.Set(KeyRegion, value); } }

		public string Overall9 { get { return instance.Get<string>(KeyOverall9); } set { instance.Set(KeyOverall9, value); } }
		public string Overall8 { get { return instance.Get<string>(KeyOverall8); } set { instance.Set(KeyOverall8, value); } }
		public string Overall7 { get { return instance.Get<string>(KeyOverall7); } set { instance.Set(KeyOverall7, value); } }
		public string Overall6 { get { return instance.Get<string>(KeyOverall6); } set { instance.Set(KeyOverall6, value); } }
		public string Overall5 { get { return instance.Get<string>(KeyOverall5); } set { instance.Set(KeyOverall5, value); } }
		public string Overall4 { get { return instance.Get<string>(KeyOverall4); } set { instance.Set(KeyOverall4, value); } }
		public string Overall3 { get { return instance.Get<string>(KeyOverall3); } set { instance.Set(KeyOverall3, value); } }
		public string Overall2 { get { return instance.Get<string>(KeyOverall2); } set { instance.Set(KeyOverall2, value); } }
		public string Overall1 { get { return instance.Get<string>(KeyOverall1); } set { instance.Set(KeyOverall1, value); } }

		public int FontSize { get { return instance.Get<int>(KeyFontSize); } set { instance.Set(KeyFontSize, value); } }

		public string Token { get { return instance.Get<string>(KeyToken); } set { instance.Set(KeyToken, value); } }

		#endregion

		private LoggingService loggingService;
		private BehaviorSubject<string> propertyChangedSubject;
		private Dictionary<string, object> oldSettings = new Dictionary<string, object>();
		private bool resetting;
		public Settings(LoggingService loggingService)
		{
			instance = this;
			this.loggingService = loggingService;
			this.propertyChangedSubject = new BehaviorSubject<string>(string.Empty);

			foreach (SettingsProperty prop in Properties.Settings.Default.Properties)
			{
				oldSettings[prop.Name] = Properties.Settings.Default[prop.Name];
			}

			Properties.Settings.Default.SettingChanging += (sender, args) =>
			{
				oldSettings[args.SettingName] = Properties.Settings.Default[args.SettingName];
			};

			Properties.Settings.Default.PropertyChanged += (sender, args) =>
			{
				var key = args.PropertyName;
				try
				{
					if (!resetting) Properties.Settings.Default.Save();
					if (oldSettings[key] != Properties.Settings.Default[key])
					{
						propertyChangedSubject.OnNext(key);
					}
				}
				catch (Exception e)
				{
					loggingService.Error($"Exception occured while saving setting '{key}'", e);
				}
			};

			UiPropertyChanged = Observable.Merge(new IObservable<string>[] {
				PropertyChanged(KeyOverall1, false),
				PropertyChanged(KeyOverall2, false),
				PropertyChanged(KeyOverall3, false),
				PropertyChanged(KeyOverall4, false),
				PropertyChanged(KeyOverall5, false),
				PropertyChanged(KeyOverall6, false),
				PropertyChanged(KeyOverall7, false),
				PropertyChanged(KeyOverall8, false),
				PropertyChanged(KeyOverall9, false),
				PropertyChanged(KeyFontSize, false)
			});

			UiPropertyChanged.Subscribe(key =>
			{
				setBrushes();
			});
			setBrushes();
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
				loggingService.Error($"Exception occured while getting setting '{key}'", e);
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
				loggingService.Error($"Exception occured while setting setting '{key}'", e);
			}
		}

		public async void ResetUI()
		{
			resetting = true;
			await Task.Run(() =>
			{
				foreach (SettingsProperty prop in Properties.Settings.Default.Properties)
				{
					if (KeyUISettings.Contains(prop.Name))
					{
						Properties.Settings.Default[prop.Name] = prop.DefaultValue;
					}
				}
			});
			resetting = false;
		}

		public IObservable<string> UiPropertyChanged { get; private set; }

		public SolidColorBrush[] Brushes { get; private set; }

		private void setBrushes()
		{
			Brushes = KeyColors.Select(name =>
			{
				var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Get<string>(name)));
				brush.Freeze();
				return brush;
			}).ToArray();
		}
	}
}

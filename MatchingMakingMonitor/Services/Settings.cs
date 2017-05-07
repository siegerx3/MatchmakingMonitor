using MatchingMakingMonitor.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MatchingMakingMonitor.Services
{
	public class Settings : BaseViewBinding
	{
		private static Settings instance;
		#region Keys
		public const string KeyInstallDirectory = "InstallDirectory";

		public const string KeyAppIdNA = "AppIdNA";
		public const string KeyAppIdEU = "AppIdEU";
		public const string KeyAppIdRU = "AppIdRU";
		public const string KeyAppIdSEA = "AppIdSEA";

		public const string KeyBaseUrlNA = "BaseUrlNA";
		public const string KeyBaseUrlEU = "BaseUrlEU";
		public const string KeyBaseUrlRU = "BaseUrlRU";
		public const string KeyBaseUrlSEA = "BaseUrlSEA";

		public const string KeyRegion = "Region";

		public const string KeyColor9 = "Color9";
		public const string KeyColor8 = "Color8";
		public const string KeyColor7 = "Color7";
		public const string KeyColor6 = "Color6";
		public const string KeyColor5 = "Color5";
		public const string KeyColor4 = "Color4";
		public const string KeyColor3 = "Color3";
		public const string KeyColor2 = "Color2";
		public const string KeyColor1 = "Color1";

		public const string KeyBattleLimits = "BattleLimits";
		public const string KeyWinLimits = "WinLimits";
		public const string KeyFragsLimits = "FragsLimits";
		public const string KeyXpLimits = "XpLimits";
		public const string KeyDmgLimits = "DmgLimits";

		public const string KeyFontSize = "FontSize";

		public static string[] KeysColors = new string[] { KeyColor9, KeyColor8, KeyColor7, KeyColor6, KeyColor5, KeyColor4, KeyColor3, KeyColor2, KeyColor1 };
		public static string[] KeysLimits = new string[] { KeyBattleLimits, KeyWinLimits, KeyFragsLimits, KeyXpLimits, KeyDmgLimits };
		public static string[] KeysOthers = new string[] { KeyFontSize };
		public static string[] KeysUISettings = KeysColors.Concat(KeysLimits).ToArray();

		public const string KeyToken = "Token";
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

		public string Color9 { get { return instance.Get<string>(KeyColor9); } set { instance.Set(KeyColor9, value); FirePropertyChanged(); } }
		public string Color8 { get { return instance.Get<string>(KeyColor8); } set { instance.Set(KeyColor8, value); FirePropertyChanged(); } }
		public string Color7 { get { return instance.Get<string>(KeyColor7); } set { instance.Set(KeyColor7, value); FirePropertyChanged(); } }
		public string Color6 { get { return instance.Get<string>(KeyColor6); } set { instance.Set(KeyColor6, value); FirePropertyChanged(); } }
		public string Color5 { get { return instance.Get<string>(KeyColor5); } set { instance.Set(KeyColor5, value); FirePropertyChanged(); } }
		public string Color4 { get { return instance.Get<string>(KeyColor4); } set { instance.Set(KeyColor4, value); FirePropertyChanged(); } }
		public string Color3 { get { return instance.Get<string>(KeyColor3); } set { instance.Set(KeyColor3, value); FirePropertyChanged(); } }
		public string Color2 { get { return instance.Get<string>(KeyColor2); } set { instance.Set(KeyColor2, value); FirePropertyChanged(); } }
		public string Color1 { get { return instance.Get<string>(KeyColor1); } set { instance.Set(KeyColor1, value); FirePropertyChanged(); } }

		public int FontSize { get { return instance.Get<int>(KeyFontSize); } set { instance.Set(KeyFontSize, value); FirePropertyChanged(); } }

		#region Limits
		private ObservableCollection<double> battleLimits;
		public ObservableCollection<double> BattleLimits
		{
			get
			{
				if (battleLimits == null) battleLimits = new ObservableCollection<double>(instance.Get<string>(KeyBattleLimits).Split(';').Select(s => double.Parse(s)).ToArray());
				return battleLimits;
			}
			set
			{
				battleLimits = null;
				instance.Set(KeyBattleLimits, string.Join(";", value));
			}
		}

		public void BattleLimitsChanged()
		{
			instance.Set(KeyWinLimits, string.Join(";", battleLimits));
		}

		private ObservableCollection<double> winLimits;
		public ObservableCollection<double> WinLimits
		{
			get
			{
				if (winLimits == null) winLimits = new ObservableCollection<double>(instance.Get<string>(KeyWinLimits).Split(';').Select(s => double.Parse(s)).ToArray());
				return winLimits;
			}
			set
			{
				winLimits = null;
				instance.Set(KeyWinLimits, string.Join(";", value));
			}
		}

		public void WinLimitsChanged()
		{
			instance.Set(KeyWinLimits, string.Join(";", winLimits));
		}

		private ObservableCollection<double> fragsLimits;
		public ObservableCollection<double> FragsLimits
		{
			get
			{
				if (fragsLimits == null) fragsLimits = new ObservableCollection<double>(instance.Get<string>(KeyFragsLimits).Split(';').Select(s => double.Parse(s)).ToArray());
				return fragsLimits;
			}
			set
			{
				fragsLimits = null;
				instance.Set(KeyFragsLimits, string.Join(";", value));
			}
		}

		public void FragsLimitsChanged()
		{
			instance.Set(KeyFragsLimits, string.Join(";", fragsLimits));
		}

		private ObservableCollection<double> xpLimits;
		public ObservableCollection<double> XpLimits
		{
			get
			{
				if (xpLimits == null) xpLimits = new ObservableCollection<double>(instance.Get<string>(KeyXpLimits).Split(';').Select(s => double.Parse(s)).ToArray());
				return xpLimits;
			}
			set
			{
				xpLimits = null;
				instance.Set(KeyXpLimits, string.Join(";", value));
			}
		}

		public void XpLimitsChanged()
		{
			instance.Set(KeyXpLimits, string.Join(";", xpLimits));
		}

		private ObservableCollection<double> dmgLimits;
		public ObservableCollection<double> DmgLimits
		{
			get
			{
				if (dmgLimits == null) dmgLimits = new ObservableCollection<double>(instance.Get<string>(KeyDmgLimits).Split(';').Select(s => double.Parse(s)).ToArray());
				return dmgLimits;
			}
			set
			{
				dmgLimits = null;
				instance.Set(KeyDmgLimits, string.Join(";", value));
			}
		}

		public void DmgLimitsChanged()
		{
			instance.Set(KeyDmgLimits, string.Join(";", dmgLimits));
		}
		#endregion

		public string Token { get { return instance.Get<string>(KeyToken); } set { instance.Set(KeyToken, value); } }

		#endregion

		private Subject<string> uiPropertiesChanged;
		public IObservable<string> UiPropertiesChanged { get { return uiPropertiesChanged.AsObservable(); } }

		private IObservable<string> uiPropertyChangedInternal;

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

			uiPropertiesChanged = new Subject<string>();

			uiPropertyChangedInternal = Observable.Merge(KeysUISettings.Select(key => PropertyChanged(key, false))).Throttle(TimeSpan.FromMilliseconds(500));

			uiPropertyChangedInternal.Subscribe(key =>
			{
				setBrushes();
				uiPropertiesChanged.OnNext(key);
			});
			setBrushes();

			Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(BattleLimits, "CollectionChanged").Throttle(TimeSpan.FromMilliseconds(1500)).Subscribe(e => BattleLimitsChanged());
			Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(WinLimits, "CollectionChanged").Throttle(TimeSpan.FromMilliseconds(1500)).Subscribe(e => WinLimitsChanged());
			Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(XpLimits, "CollectionChanged").Throttle(TimeSpan.FromMilliseconds(1500)).Subscribe(e => XpLimitsChanged());
			Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(DmgLimits, "CollectionChanged").Throttle(TimeSpan.FromMilliseconds(1500)).Subscribe(e => DmgLimitsChanged());
			Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(FragsLimits, "CollectionChanged").Throttle(TimeSpan.FromMilliseconds(1500)).Subscribe(e => FragsLimitsChanged());
		}

		public void Save()
		{
			Properties.Settings.Default.Save();
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
					if (KeysColors.Contains(prop.Name))
					{
						Properties.Settings.Default[prop.Name] = Convert.ChangeType(prop.DefaultValue, prop.PropertyType);
						FirePropertyChanged(prop.Name);
					}
					else if (KeysLimits.Contains(prop.Name))
					{
						var ints = ((string)prop.DefaultValue).Split(';').Select(s => double.Parse(s)).ToArray();
						var collection = getLimitCollectionByKey(prop.Name);
						if (collection != null)
						{
							for (int i = 0; i < ints.Length; i++)
							{
								collection[i] = ints[i];
							}
						}
					}
				}
				Properties.Settings.Default.Save();
			});
			resetting = false;
		}

		private ObservableCollection<double> getLimitCollectionByKey(string key)
		{
			switch (key)
			{
				case KeyBattleLimits:
					return battleLimits;
				case KeyWinLimits:
					return winLimits;
				case KeyFragsLimits:
					return fragsLimits;
				case KeyXpLimits:
					return xpLimits;
				case KeyDmgLimits:
					return dmgLimits;
				default:
					return null;
			}
		}

		public SolidColorBrush[] Brushes { get; private set; }

		private void setBrushes()
		{
			Brushes = KeysColors.Select(name =>
			{
				var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Get<string>(name)));
				brush.Freeze();
				return brush;
			}).ToArray();
		}
	}
}

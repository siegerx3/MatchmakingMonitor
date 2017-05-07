using MatchMakingMonitor.config;
using MatchMakingMonitor.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MatchMakingMonitor.Services
{
	public class Settings : BaseViewBinding
	{
		private static Settings instance;

		private static string defaultSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "settings.default.json");
		private static string settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "settings.json");

		private SettingsJson currentSettings;

		#region Keys
		public const string KeyInstallDirectory = nameof(SettingsJson.installDirectory);

		public const string KeyAppIdNA = nameof(SettingsJson.appIdNA);
		public const string KeyAppIdEU = nameof(SettingsJson.appIdEU);
		public const string KeyAppIdRU = nameof(SettingsJson.appIdRU);
		public const string KeyAppIdSEA = nameof(SettingsJson.appIdSEA);

		public const string KeyBaseUrlNA = nameof(SettingsJson.baseUrlNA);
		public const string KeyBaseUrlEU = nameof(SettingsJson.baseUrlEU);
		public const string KeyBaseUrlRU = nameof(SettingsJson.baseUrlRU);
		public const string KeyBaseUrlSEA = nameof(SettingsJson.baseUrlSEA);

		public const string KeyRegion = nameof(SettingsJson.region);

		public const string KeyToken = nameof(SettingsJson.token);

		public const string KeyColor9 = nameof(SettingsJson.color9);
		public const string KeyColor8 = nameof(SettingsJson.color8);
		public const string KeyColor7 = nameof(SettingsJson.color7);
		public const string KeyColor6 = nameof(SettingsJson.color6);
		public const string KeyColor5 = nameof(SettingsJson.color5);
		public const string KeyColor4 = nameof(SettingsJson.color4);
		public const string KeyColor3 = nameof(SettingsJson.color3);
		public const string KeyColor2 = nameof(SettingsJson.color2);
		public const string KeyColor1 = nameof(SettingsJson.color1);

		public const string KeyBattleLimits = nameof(SettingsJson.battleLimits);
		public const string KeyWinLimits = nameof(SettingsJson.winLimits);
		public const string KeyFragsLimits = nameof(SettingsJson.fragsLimits);
		public const string KeyXpLimits = nameof(SettingsJson.xpLimits);
		public const string KeyDmgLimits = nameof(SettingsJson.dmgLimits);

		public const string KeyFontSize = nameof(SettingsJson.fontSize);

		public const string KeyBattleWeight = nameof(SettingsJson.battleWeight);
		public const string KeyWinWeight = nameof(SettingsJson.winWeight);
		public const string KeyFragsWeight = nameof(SettingsJson.fragsWeight);
		public const string KeyXpWeight = nameof(SettingsJson.xpWeight);
		public const string KeyDmgWeight = nameof(SettingsJson.dmgWeight);

		public static string[] KeysColors = new string[] { KeyColor9, KeyColor8, KeyColor7, KeyColor6, KeyColor5, KeyColor4, KeyColor3, KeyColor2, KeyColor1 };
		public static string[] KeysLimits = new string[] { KeyBattleLimits, KeyWinLimits, KeyFragsLimits, KeyXpLimits, KeyDmgLimits };
		public static string[] KeysWeights = new string[] { KeyBattleWeight, KeyWinWeight, KeyFragsWeight, KeyXpWeight, KeyDmgWeight };
		public static string[] KeysOthers = new string[] { KeyFontSize };
		public static string[] KeysUISettings = KeysColors.Concat(KeysLimits).Concat(KeysWeights).ToArray();

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
				if (battleLimits == null) battleLimits = new ObservableCollection<double>(instance.Get<double[]>(KeyBattleLimits));
				return battleLimits;
			}
			set
			{
				battleLimits = null;
				instance.Set(KeyBattleLimits, value);
			}
		}

		public void BattleLimitsChanged()
		{
			instance.Set(KeyBattleLimits, battleLimits.ToArray(), true);
		}

		private ObservableCollection<double> winLimits;
		public ObservableCollection<double> WinLimits
		{
			get
			{
				if (winLimits == null) winLimits = new ObservableCollection<double>(instance.Get<double[]>(KeyWinLimits));
				return winLimits;
			}
			set
			{
				winLimits = null;
				instance.Set(KeyWinLimits, value);
			}
		}

		public void WinLimitsChanged()
		{
			instance.Set(KeyWinLimits, winLimits.ToArray(), true);
		}

		private ObservableCollection<double> fragsLimits;
		public ObservableCollection<double> FragsLimits
		{
			get
			{
				if (fragsLimits == null) fragsLimits = new ObservableCollection<double>(instance.Get<double[]>(KeyFragsLimits));
				return fragsLimits;
			}
			set
			{
				fragsLimits = null;
				instance.Set(KeyFragsLimits, value);
			}
		}

		public void FragsLimitsChanged()
		{
			instance.Set(KeyFragsLimits, fragsLimits.ToArray(), true);
		}

		private ObservableCollection<double> xpLimits;
		public ObservableCollection<double> XpLimits
		{
			get
			{
				if (xpLimits == null) xpLimits = new ObservableCollection<double>(instance.Get<double[]>(KeyXpLimits));
				return xpLimits;
			}
			set
			{
				xpLimits = null;
				instance.Set(KeyXpLimits, value);
			}
		}

		public void XpLimitsChanged()
		{
			instance.Set(KeyXpLimits, xpLimits.ToArray(), true);
		}

		private ObservableCollection<double> dmgLimits;
		public ObservableCollection<double> DmgLimits
		{
			get
			{
				if (dmgLimits == null) dmgLimits = new ObservableCollection<double>(instance.Get<double[]>(KeyDmgLimits));
				return dmgLimits;
			}
			set
			{
				dmgLimits = null;
				instance.Set(KeyDmgLimits, value);
			}
		}

		public void DmgLimitsChanged()
		{
			instance.Set(KeyDmgLimits, dmgLimits.ToArray(), true);
		}

		public double BattleWeight { get { return instance.Get<double>(KeyBattleWeight); } set { instance.Set(KeyBattleWeight, value); } }
		public double FragsWeight { get { return instance.Get<double>(KeyFragsWeight); } set { instance.Set(KeyFragsWeight, value); } }
		public double XpWeight { get { return instance.Get<double>(KeyXpWeight); } set { instance.Set(KeyXpWeight, value); } }
		public double DmgWeight { get { return instance.Get<double>(KeyDmgWeight); } set { instance.Set(KeyDmgWeight, value); } }
		public double WinWeight { get { return instance.Get<double>(KeyWinWeight); } set { instance.Set(KeyWinWeight, value); } }
		#endregion

		public string Token { get { return instance.Get<string>(KeyToken); } set { instance.Set(KeyToken, value); } }

		#endregion

		private Subject<string> uiSettingsChangedSubject;
		public IObservable<string> UiSettingsChanged { get { return uiSettingsChangedSubject.AsObservable(); } }

		private IObservable<string> uiSettingsChangedInternal;

		private LoggingService loggingService;
		private BehaviorSubject<string> settingChangedSubject;
		public Settings(LoggingService loggingService)
		{
			instance = this;
			this.loggingService = loggingService;

			init();

			settingChangedSubject = new BehaviorSubject<string>(string.Empty);
			settingChangedSubject.Where(key => key != null).Throttle(TimeSpan.FromSeconds(2)).Subscribe(key =>
			{
				Save();
			});

			uiSettingsChangedSubject = new Subject<string>();

			uiSettingsChangedInternal = Observable.Merge(KeysUISettings.Select(key => SettingChanged(key, false))).Throttle(TimeSpan.FromMilliseconds(500));

			uiSettingsChangedInternal.Subscribe(key =>
			{
				setBrushes();
				uiSettingsChangedSubject.OnNext(key);
			});
			setBrushes();

			Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(BattleLimits, "CollectionChanged").Throttle(TimeSpan.FromMilliseconds(500)).Subscribe(e => BattleLimitsChanged());
			Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(WinLimits, "CollectionChanged").Throttle(TimeSpan.FromMilliseconds(500)).Subscribe(e => WinLimitsChanged());
			Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(XpLimits, "CollectionChanged").Throttle(TimeSpan.FromMilliseconds(500)).Subscribe(e => XpLimitsChanged());
			Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(DmgLimits, "CollectionChanged").Throttle(TimeSpan.FromMilliseconds(500)).Subscribe(e => DmgLimitsChanged());
			Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(FragsLimits, "CollectionChanged").Throttle(TimeSpan.FromMilliseconds(500)).Subscribe(e => FragsLimitsChanged());
		}

		private void init()
		{
			if (!File.Exists(settingsPath))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(settingsPath));
				using (var f = File.CreateText(settingsPath))
				{
					f.Write(defaults());
				}
			}
			fromJson();
		}

		private string defaults()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MatchMakingMonitor.config.settings.default.json");
			var result = string.Empty;
			using (var sr = new StreamReader(stream))
			{
				result = sr.ReadToEnd();
			}
			return result;
		}

		private void fromJson()
		{
			currentSettings = JsonConvert.DeserializeObject<SettingsJson>(File.ReadAllText(settingsPath));
		}

		public async Task ExportUISettings(string path)
		{
			var export = new Dictionary<string, Object>();

			foreach (var key in KeysUISettings.Concat(KeysOthers))
			{
				export.Add(key, currentSettings.Get(key));
			}

			var exportJson = await Task.Run(() => { return JsonConvert.SerializeObject(export); });

			using (var f = File.CreateText(path))
			{
				await f.WriteAsync(exportJson);
			}
		}

		public async Task ImportUISettings(string path)
		{
			if (File.Exists(path))
			{
				var importJson = File.ReadAllText(path);
				var import = await Task.Run(() => { return JsonConvert.DeserializeObject<SettingsJson>(importJson); });
				await ResetUISettings(import);
			}
		}

		public async Task Save()
		{
			try
			{
				await Task.Run(() => { File.WriteAllText(settingsPath, JsonConvert.SerializeObject(currentSettings)); });
			}
			catch (Exception e)
			{
				loggingService.Error("Exception occured while saving settings to file", e);
			}
		}

		public IObservable<string> SettingChanged(string key, bool initial = true)
		{
			var obs = settingChangedSubject.AsObservable().Where(k => key == k);
			if (initial)
			{
				settingChangedSubject.OnNext(key);
			}
			return obs;
		}

		public T Get<T>(string key)
		{
			try
			{
				return currentSettings.Get<T>(key);
			}
			catch (Exception e)
			{
				loggingService.Error($"Exception occured while getting setting '{key}'", e);
				return default(T);
			}
		}

		public void Set<T>(string key, T value, bool forceChanged = false)
		{
			try
			{
				var changed = forceChanged || !value.Equals(Get<T>(key));
				currentSettings.Set(key, value);
				if (changed) fireSettingChanged(key);
			}
			catch (Exception e)
			{
				loggingService.Error($"Exception occured while setting setting '{key}'", e);
			}
		}

		public async Task ResetUISettings(SettingsJson sourceSettings = null)
		{
			if (sourceSettings == null) sourceSettings = JsonConvert.DeserializeObject<SettingsJson>(this.defaults());
			await Task.Run(() =>
			{
				foreach (var key in KeysColors.Concat(KeysOthers).Concat(KeysWeights))
				{
					currentSettings.Set(key, sourceSettings.Get(key));
					FirePropertyChanged(firstLetterToUpper(key));
				}
				foreach (var key in KeysLimits)
				{
					currentSettings.Set(key, sourceSettings.Get(key));
					var collection = getLimitCollectionByKey(key);
					if (collection != null)
					{
						var defaultLimit = sourceSettings.Get<double[]>(key);
						var limit = currentSettings.Get<double[]>(key);
						for (int i = 0; i < defaultLimit.Length; i++)
						{
							collection[i] = limit[i];
						}
					}
				}
			});
			await Save();
		}

		private void fireSettingChanged(string key)
		{
			settingChangedSubject.OnNext(key);
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

		private string firstLetterToUpper(string str)
		{
			if (str == null)
				return null;

			if (str.Length > 1)
				return char.ToUpper(str[0]) + str.Substring(1);

			return str.ToUpper();
		}
	}
}

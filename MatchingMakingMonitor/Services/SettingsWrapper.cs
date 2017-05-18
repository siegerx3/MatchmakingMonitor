using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Media;
using MatchMakingMonitor.config;
using MatchMakingMonitor.config.Reflection;
using MatchMakingMonitor.View.Util;
using Newtonsoft.Json;

namespace MatchMakingMonitor.Services
{
	public class SettingsWrapper : ViewModelBase
	{
		private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
		{
			NullValueHandling = NullValueHandling.Ignore
		};

		private static readonly string SettingsPath =
			Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "settings.json");

		private static readonly Type SettingsType = typeof(SettingsJson);

		private readonly ILogger _logger;
		public readonly BehaviorSubject<ChangedSetting> SettingChangedSubject;

		private readonly Subject<ChangedSetting> _uiSettingsChangedSubject;

		private readonly Subject<object> _saveQueueSubject;

		public SettingsWrapper(ILogger logger)
		{
			_logger = logger;

			Init();

			_saveQueueSubject = new Subject<object>();

			_saveQueueSubject.Throttle(TimeSpan.FromSeconds(30)).Subscribe(async _ => { await InternalSave(); });

			SettingChangedSubject = new BehaviorSubject<ChangedSetting>(null);
			SettingChangedSubject.Where(setting => setting?.Key != null && setting.HasChanged)
				.Do(setting => _logger.Info($"Setting ({setting.Key}) changed from '{setting.OldValue}' to '{setting.NewValue}'"))
				.Throttle(TimeSpan.FromSeconds(2))
				.Subscribe(key => Save());

			_uiSettingsChangedSubject = new Subject<ChangedSetting>();

			var uiSettingsChangedInternal = SettingChanged("UISetting")
				.Throttle(TimeSpan.FromMilliseconds(500));

			uiSettingsChangedInternal.Subscribe(changedSetting =>
			{
				SetBrushes();
				_uiSettingsChangedSubject.OnNext(changedSetting);
			});
			SetBrushes();
		}

		public SettingsJson CurrentSettings { get; private set; }

		public string AppId => CurrentSettings.AppIds.Single(appId => appId.Region == CurrentSettings.Region).Id;
		public string BaseUrl => CurrentSettings.BaseUrls.Single(baseUrl => baseUrl.Region == CurrentSettings.Region).Url;

		public IObservable<ChangedSetting> UiSettingsChanged => _uiSettingsChangedSubject.AsObservable();

		public SolidColorBrush[] Brushes { get; private set; }

		private void Init()
		{
			if (!File.Exists(SettingsPath))
			{
				// ReSharper disable once AssignNullToNotNullAttribute
				Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));
				using (var f = File.CreateText(SettingsPath))
				{
					f.Write(Defaults());
				}
			}
			FromJson();
		}

		private static string Defaults()
		{
			var stream = Assembly.GetExecutingAssembly()
				.GetManifestResourceStream("MatchMakingMonitor.config.settings.default.json");
			var result = string.Empty;
			if (stream == null) return result;
			using (var sr = new StreamReader(stream))
			{
				result = sr.ReadToEnd();
			}
			return result;
		}

		private void FromJson()
		{
			CurrentSettings = JsonConvert.DeserializeObject<SettingsJson>(File.ReadAllText(SettingsPath), JsonSerializerSettings);
		}

		public async Task ExportUiSettings(string path)
		{
			var export = new Dictionary<string, object>();
			foreach (var field in GetExportSettings(SettingsType))
			{
				export.Add(FirstLetterToLower(field.Name), field.GetValue(CurrentSettings));
			}

			var exportJson = await Task.Run(() => JsonConvert.SerializeObject(export, JsonSerializerSettings));

			using (var f = File.CreateText(path))
			{
				await f.WriteAsync(exportJson);
			}
		}

		public async Task ImportUiSettings(string path)
		{
			if (File.Exists(path))
				try
				{
					var importJson = File.ReadAllText(path);
					var import = await Task.Run(() => JsonConvert.DeserializeObject<SettingsJson>(importJson, JsonSerializerSettings));
					await Task.Run(() => ResetUiSettings(CurrentSettings, import));
					await InternalSave();
				}
				catch (Exception e)
				{
					_logger.Error("Error during import", e);
				}
		}

		public void Save()
		{
			_saveQueueSubject.Next();
		}

		private async Task InternalSave()
		{
			try
			{
				await Task.Run(() => { File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(CurrentSettings, JsonSerializerSettings)); });
			}
			catch (Exception e)
			{
				_logger.Error("Exception occured while saving settings to file", e);
			}
		}

		public IObservable<ChangedSetting> SettingChanged(string key, bool initial = true)
		{
			var obs = SettingChangedSubject.AsObservable().Where(s => s != null && (s.Key == key && s.HasChanged || s.Initial));
			if (initial)
				SettingChangedSubject.OnNext(new ChangedSetting(null, null, key) { Initial = true });
			return obs;
		}

		[SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
		public static void ResetUiSettings(SettingsJson targetSettings, SettingsJson sourceSettings = null)
		{
			if (sourceSettings == null) sourceSettings = JsonConvert.DeserializeObject<SettingsJson>(Defaults(), JsonSerializerSettings);

			targetSettings.FontSize = sourceSettings.FontSize;

			for (var i = 0; i < sourceSettings.Colors.Length; i++)
			{
				targetSettings.Colors[i] = sourceSettings.Colors[i];
			}

			for (var i = 0; i < sourceSettings.BattleLimits.Length; i++)
			{
				targetSettings.BattleLimits[i] = sourceSettings.BattleLimits[i];
			}

			for (var i = 0; i < sourceSettings.AvgFragsLimits.Length; i++)
			{
				targetSettings.AvgFragsLimits[i] = sourceSettings.AvgFragsLimits[i];
			}

			for (var i = 0; i < sourceSettings.WinRateLimits.Length; i++)
			{
				targetSettings.WinRateLimits[i] = sourceSettings.WinRateLimits[i];
			}

			for (var i = 0; i < sourceSettings.AvgXpLimits.Length; i++)
			{
				for (var x = 0; x < sourceSettings.AvgXpLimits[i].Values.Length; x++)
				{
					targetSettings.AvgXpLimits[i].Values[x] = sourceSettings.AvgXpLimits[i].Values[x];
				}
			}

			for (var i = 0; i < sourceSettings.AvgDmgLimits.Battleship.Length; i++)
			{
				for (var x = 0; x < sourceSettings.AvgDmgLimits.Battleship[i].Values.Length; x++)
				{
					targetSettings.AvgDmgLimits.Battleship[i].Values[x] = sourceSettings.AvgDmgLimits.Battleship[i].Values[x];
				}
			}
			for (var i = 0; i < sourceSettings.AvgDmgLimits.Cruiser.Length; i++)
			{
				for (var x = 0; x < sourceSettings.AvgDmgLimits.Cruiser[i].Values.Length; x++)
				{
					targetSettings.AvgDmgLimits.Cruiser[i].Values[x] = sourceSettings.AvgDmgLimits.Cruiser[i].Values[x];
				}
			}
			for (var i = 0; i < sourceSettings.AvgDmgLimits.Destroyer.Length; i++)
			{
				for (var x = 0; x < sourceSettings.AvgDmgLimits.Destroyer[i].Values.Length; x++)
				{
					targetSettings.AvgDmgLimits.Destroyer[i].Values[x] = sourceSettings.AvgDmgLimits.Destroyer[i].Values[x];
				}
			}
			for (var i = 0; i < sourceSettings.AvgDmgLimits.AirCarrier.Length; i++)
			{
				for (var x = 0; x < sourceSettings.AvgDmgLimits.AirCarrier[i].Values.Length; x++)
				{
					targetSettings.AvgDmgLimits.AirCarrier[i].Values[x] = sourceSettings.AvgDmgLimits.AirCarrier[i].Values[x];
				}
			}

			targetSettings.BattleWeight = sourceSettings.BattleWeight;
			targetSettings.AvgDmgWeight = sourceSettings.AvgDmgWeight;
			targetSettings.AvgFragsWeight = sourceSettings.AvgFragsWeight;
			targetSettings.AvgXpWeight = sourceSettings.AvgXpWeight;
			targetSettings.AvgFragsWeight = sourceSettings.AvgFragsWeight;
		}

		private static IEnumerable<FieldInfo> GetExportSettings(Type type)
		{
			return type.GetFields().Where(field => field.GetCustomAttribute(typeof(ExportSettingAttribute)) != null);
		}

		private void SetBrushes()
		{
			Brushes = CurrentSettings.Colors.Select(colorValue =>
			{
				var convertFromString = ColorConverter.ConvertFromString(colorValue);
				if (convertFromString == null) return System.Windows.Media.Brushes.Black;
				var brush = new SolidColorBrush((Color)convertFromString);
				brush.Freeze();
				return brush;
			}).ToArray();
		}

		private static string FirstLetterToLower(string str)
		{
			if (str == null)
				return null;

			if (str.Length > 1)
				return char.ToLower(str[0]) + str.Substring(1);

			return str.ToLower();
		}
	}
}
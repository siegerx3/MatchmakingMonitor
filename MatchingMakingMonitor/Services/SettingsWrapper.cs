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
		private readonly List<PropertyInfo> _colorSettings;

		private readonly ILogger _logger;
		public readonly BehaviorSubject<ChangedSetting> SettingChangedSubject;
		private readonly List<PropertyInfo> _uiSettings;

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

			_colorSettings = GetColorSettings(SettingsType, new List<PropertyInfo>(10)).ToList();

			_uiSettings = GetUiSettings(SettingsType, new List<PropertyInfo>(10)).ToList();

			_uiSettingsChangedSubject = new Subject<ChangedSetting>();

			var uiSettingsChangedInternal = _uiSettings.Select(prop => SettingChanged(prop.Name, false)).Merge()
				.Throttle(TimeSpan.FromMilliseconds(500));

			uiSettingsChangedInternal.Subscribe(changedSetting =>
			{
				SetBrushes();
				_uiSettingsChangedSubject.OnNext(changedSetting);
			});
			SetBrushes();
		}

		public IReadOnlyList<PropertyInfo> UiSettings => _uiSettings.AsReadOnly();
		public IReadOnlyList<PropertyInfo> ColorSettings => _colorSettings.AsReadOnly();

		[NestedSetting]
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
			var props = GetUiSettingsFromType(SettingsType);
			foreach (var prop in props)
			{
				if (prop.GetCustomAttribute(typeof(UiSettingAttribute)) != null ||
						(prop.PropertyType.IsArray && prop.GetCustomAttribute(typeof(NestedSettingAttribute)) != null))
				{
					export.Add(FirstLetterToLower(prop.Name), prop.GetValue(CurrentSettings));
				}
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

		private static IEnumerable<PropertyInfo> GetUiSettings(Type type, ICollection<PropertyInfo> settings)
		{
			var props = GetUiSettingsFromType(type);
			foreach (var prop in props)
			{
				if (prop.GetCustomAttribute(typeof(UiSettingAttribute)) != null)
					settings.Add(prop);
				if (prop.PropertyType.IsArray)
					GetUiSettings(prop.PropertyType.GetElementType(), settings);
				if (typeof(NestedSetting).IsAssignableFrom(prop.PropertyType))
					GetUiSettings(prop.PropertyType, settings);
			}
			return settings;
		}

		private static IEnumerable<PropertyInfo> GetColorSettings(Type type, ICollection<PropertyInfo> settings)
		{
			var props = type.GetProperties().Where(p => p.GetCustomAttributes()
				.Any(a => a is ColorSettingAttribute));
			foreach (var prop in props)
				if (prop.GetCustomAttribute(typeof(UiSettingAttribute)) != null)
					settings.Add(prop);
			return settings;
		}

		[SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
		public static void ResetUiSettings(object targetSettings, object sourceSettings = null, Type type = null)
		{
			if (type == null) type = SettingsType;
			if (sourceSettings == null) sourceSettings = JsonConvert.DeserializeObject<SettingsJson>(Defaults(), JsonSerializerSettings);

			var props = GetUiSettingsFromType(type);
			foreach (var prop in props)
			{
				if (prop.GetCustomAttribute(typeof(UiSettingAttribute)) != null || (prop.PropertyType.IsArray && prop.GetCustomAttribute(typeof(NestedSettingAttribute)) != null)) { }
				prop.SetValue(targetSettings, prop.GetValue(sourceSettings));
				if (typeof(NestedSetting).IsAssignableFrom(prop.PropertyType))
					ResetUiSettings(prop.GetValue(targetSettings), prop.GetValue(sourceSettings), prop.PropertyType);
			}
		}

		private static IEnumerable<PropertyInfo> GetUiSettingsFromType(Type type)
		{
			return type.GetProperties().Where(p => p.GetCustomAttributes()
				.Any(a => a is UiSettingAttribute || a is NestedSettingAttribute));
		}

		private void SetBrushes()
		{
			Brushes = _colorSettings.Select(prop =>
			{
				var convertFromString = ColorConverter.ConvertFromString((string)prop.GetValue(CurrentSettings));
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
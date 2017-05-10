using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	public class SettingsWrapper : BaseViewBinding
	{
		private static SettingsWrapper _instance;

		private static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
		{
			NullValueHandling = NullValueHandling.Ignore
		};

		private static readonly string SettingsPath =
			Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "settings.json");

		private readonly ILogger _logger;
		private readonly List<PropertyInfo> _uiSettings;
		public IReadOnlyList<PropertyInfo> UiSettings => _uiSettings.AsReadOnly();
		private readonly List<PropertyInfo> _colorSettings;
		public IReadOnlyList<PropertyInfo> ColorSettings => _colorSettings.AsReadOnly();
		private readonly BehaviorSubject<string> _settingChangedSubject;

		private readonly Subject<string> _uiSettingsChangedSubject;

		private SettingsJson _currentSettings;
		[NestedSetting]
		public SettingsJson CurrentSettings => _currentSettings;
		private static readonly Type SettingsType = typeof(SettingsJson);

		public string AppId => _currentSettings.AppIds.Single(appId => appId.Region == _currentSettings.Region).Id;
		public string BaseUrl => _currentSettings.BaseUrls.Single(baseUrl => baseUrl.Region == _currentSettings.Region).Url;

		public SettingsWrapper(ILogger logger)
		{
			_instance = this;
			_logger = logger;

			Init();

			_settingChangedSubject = new BehaviorSubject<string>(string.Empty);
			_settingChangedSubject.Where(key => key != null)
				.Do(key => _logger.Info($"Setting ({key}) changed"))
				.Throttle(TimeSpan.FromSeconds(2))
				.Subscribe(async key => { await Save(); });

			NestedSetting.AttachListenerStatic(GetType(), this, _settingChangedSubject);

			_colorSettings = GetColorSettings(SettingsType, new List<PropertyInfo>(10)).ToList();

			_uiSettings = GetUiSettings(SettingsType, new List<PropertyInfo>(10)).ToList();

			_uiSettingsChangedSubject = new Subject<string>();

			var uiSettingsChangedInternal = _uiSettings.Select(prop => SettingChanged(prop.Name, false)).Merge()
				.Throttle(TimeSpan.FromMilliseconds(500));

			uiSettingsChangedInternal.Subscribe(key =>
			{
				SetBrushes();
				_uiSettingsChangedSubject.OnNext(key);
			});
			SetBrushes();
		}

		public IObservable<string> UiSettingsChanged => _uiSettingsChangedSubject.AsObservable();

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
			_currentSettings = JsonConvert.DeserializeObject<SettingsJson>(File.ReadAllText(SettingsPath));
		}

		public async Task ExportUiSettings(string path)
		{
			// TODO
			//var export = KeysUiSettings.Concat(KeysOthers).ToDictionary(key => key, key => _currentSettings.Get(key));

			//var exportJson = await Task.Run(() => JsonConvert.SerializeObject(export));

			//using (var f = File.CreateText(path))
			//{
			//	await f.WriteAsync(exportJson);
			//}
		}

		public async Task ImportUiSettings(string path)
		{
			if (File.Exists(path))
				try
				{
					var importJson = File.ReadAllText(path);
					var import = await Task.Run(() => JsonConvert.DeserializeObject<SettingsJson>(importJson));
					await ResetUiSettings(import);
				}
				catch (Exception e)
				{
					_logger.Error("Error during import", e);
				}
		}

		public async Task Save()
		{
			try
			{
				await Task.Run(() => { File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(_currentSettings)); });
			}
			catch (Exception e)
			{
				_logger.Error("Exception occured while saving settings to file", e);
			}
		}

		public IObservable<string> SettingChanged(string key, bool initial = true)
		{
			var obs = _settingChangedSubject.AsObservable().Where(k => key == k);
			if (initial)
				_settingChangedSubject.OnNext(key);
			return obs;
		}

		private static IEnumerable<PropertyInfo> GetUiSettings(Type type, ICollection<PropertyInfo> settings)
		{
			var props = type.GetProperties().Where(p => p.GetCustomAttributes()
				.Any(a => a is UiSettingAttribute || a is NestedSettingAttribute));
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
			{
				if (prop.GetCustomAttribute(typeof(UiSettingAttribute)) != null)
					settings.Add(prop);
			}
			return settings;
		}

		[SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
		public async Task ResetUiSettings(SettingsJson sourceSettings = null)
		{
			if (sourceSettings == null) sourceSettings = JsonConvert.DeserializeObject<SettingsJson>(Defaults());
			await Task.Run(() =>
			{
				// TODO
			});
			await Save();
		}

		private void FireSettingChanged(string key)
		{
			_settingChangedSubject.OnNext(key);
		}

		private void SetBrushes()
		{
			Brushes = _colorSettings.Select(prop =>
			{
				var convertFromString = ColorConverter.ConvertFromString((string)prop.GetValue(_currentSettings));
				if (convertFromString == null) return System.Windows.Media.Brushes.Black;
				var brush = new SolidColorBrush((Color)convertFromString);
				brush.Freeze();
				return brush;
			}).ToArray();
		}
	}
}
using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Xml;
using MatchmakingMonitor.config;

namespace MatchmakingMonitor.Services
{
  public class WatcherService
  {
    private readonly ILogger _logger;

    private readonly BehaviorSubject<string> _matchFoundSubject;

    private readonly SettingsWrapper _settingsWrapper;
    private bool _initialCheckDone;

    public WatcherService(ILogger logger, SettingsWrapper settingsWrapper)
    {
      _settingsWrapper = settingsWrapper;
      _logger = logger;

      _matchFoundSubject = new BehaviorSubject<string>(null);

      var fileSystemWatcher = new FileSystemWatcher
      {
        Filter = "tempArenaInfo.json",
        NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.Attributes |
                       NotifyFilters.LastAccess
      };
      fileSystemWatcher.Created += (obj, args) => { CallMatchFound(args.FullPath); };
      fileSystemWatcher.Changed += (obj, args) => { CallMatchFound(args.FullPath); };


      settingsWrapper.SettingChanged(nameof(SettingsJson.InstallDirectory)).Subscribe(key =>
      {

        var path = GetPath();

        if (Directory.Exists(path))
        {
          fileSystemWatcher.Path = path;
          fileSystemWatcher.EnableRaisingEvents = true;
          if (_initialCheckDone) return;
          _initialCheckDone = true;
          CheckStatic();
        }
        else
        {
          fileSystemWatcher.EnableRaisingEvents = false;
        }
      });

      settingsWrapper.SettingChanged(nameof(SettingsJson.Region), false).Subscribe(change =>
      {
        if (!change.Initial)
          CheckStatic();
      });
    }

    public IObservable<string> MatchFound => _matchFoundSubject.Throttle(TimeSpan.FromMilliseconds(150)).AsObservable();

    private string GetPath()
    {
      var directory = _settingsWrapper.CurrentSettings.InstallDirectory;

      if (!Directory.Exists(directory) || !File.Exists(Path.Combine(directory, "res", "engine_config.xml")) || !File.Exists(Path.Combine(directory, "WorldOfWarships.exe")))
        return string.Empty;


      var version = FileVersionInfo.GetVersionInfo(Path.Combine(directory, "WorldOfWarships.exe")).FileVersion
      .Replace(" ", string.Empty)
      .Replace(',', '.')
      .Trim();

      var engine_configXml = new XmlDocument();
      if (File.Exists(Path.Combine(directory, "res_mods", version, "engine_config.xml")))
      {
        engine_configXml.LoadXml(File.ReadAllText(Path.Combine(directory, "res_mods", version, "engine_config.xml")));
      }
      else
      {
        engine_configXml.LoadXml(File.ReadAllText(Path.Combine(directory, "res", "engine_config.xml")));
      }

      var versioned = bool.Parse(engine_configXml.SelectSingleNode("engine_config.xml/replays/versioned").InnerText);
      var replaysPath = engine_configXml.SelectSingleNode("engine_config.xml/replays/dirPath").InnerText;

      if (versioned)
      {
        return Path.Combine(directory, replaysPath, version);
      }
      return Path.Combine(directory, replaysPath);
    }

    private void CheckStatic()
    {
      var gamePath = GetPath();
      var path = Path.Combine(gamePath, "tempArenaInfo.json");
      _logger.Info("Checking for match in path " + path);
      if (File.Exists(path))
        CallMatchFound(path);
    }

    private void CallMatchFound(string path)
    {
      _logger.Info("Match found");
      _matchFoundSubject.OnNext(path);
    }
  }
}
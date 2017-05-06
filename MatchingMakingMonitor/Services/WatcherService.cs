using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MatchingMakingMonitor.Services
{
	public class WatcherService
	{
		private FileSystemWatcher fileSystemWatcher;
		private bool initialCheckDone = false;

		private Settings settings;

		private BehaviorSubject<string> matchFoundSubject;
		public IObservable<string> MatchFound => matchFoundSubject.AsObservable();
		public WatcherService(LoggingService loggingService, Settings settings)
		{
			this.settings = settings;

			matchFoundSubject = new BehaviorSubject<string>(null);

			fileSystemWatcher = new FileSystemWatcher();
			fileSystemWatcher.Filter = "tempArenaInfo.json";
			fileSystemWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.Attributes | NotifyFilters.LastAccess;
			fileSystemWatcher.Created += (obj, args) =>
			{
				matchFound(args.FullPath);
			};
			fileSystemWatcher.Changed += (obj, args) =>
			{
				matchFound(args.FullPath);
			};


			settings.PropertyChanged("InstallDirectory").Subscribe(key =>
			{
				var directory = settings.Get<string>(key);
				if (Directory.Exists(Path.Combine(directory, "replays")) && File.Exists(Path.Combine(directory, "WorldOfWarships.exe")))
				{
					fileSystemWatcher.Path = Path.Combine(directory, "replays");
					fileSystemWatcher.EnableRaisingEvents = true;
					if (!initialCheckDone)
					{
						initialCheckDone = true;
						checkStatic();
					}
				}
				else
				{
					fileSystemWatcher.EnableRaisingEvents = false;
				}
			});

			settings.PropertyChanged("Region", false).Subscribe(key => {
				checkStatic();
			});
		}

		private void checkStatic()
		{
			var directory = this.settings.Get<string>("InstallDirectory");
			var path = Path.Combine(directory, "replays", "tempArenaInfo.json");
			if (File.Exists(path))
			{
				matchFound(path);
			}
		}

		private void matchFound(string path)
		{
			matchFoundSubject.OnNext(path);
		}
	}
}

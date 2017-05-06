using MatchingMakingMonitor.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace MatchingMakingMonitor.Services
{
	public class StatsService
	{
		private ReplayModel currentReplay;
		private string currentRegion;

		private LoggingService loggingService;
		private WatcherService watcherService;
		private ApiService apiService;
		private Settings settings;

		private BehaviorSubject<StatsStatus> statsStatusChangedSubject;
		public IObservable<StatsStatus> StatsStatusChanged => statsStatusChangedSubject.AsObservable();

		public StatsService(LoggingService loggingService, Settings settings, WatcherService watcherService, ApiService apiService)
		{
			this.loggingService = loggingService;
			this.watcherService = watcherService;
			this.apiService = apiService;
			this.settings = settings;

			this.statsStatusChangedSubject = new BehaviorSubject<StatsStatus>(StatsStatus.Waiting);

			this.watcherService.MatchFound.Subscribe(path =>
			{
				Task.Run(async () =>
				{
					await statsFound(path);
				});
			});
		}

		private async Task statsFound(string path)
		{
			ReplayModel replay = null;
			using (var sr = new StreamReader(path))
			{
				try
				{
					replay = await Task.Run(async () => { return JsonConvert.DeserializeObject<ReplayModel>(await sr.ReadToEndAsync()); });
				}
				catch (Exception e)
				{
					loggingService.Log("Error while reading replay file. " + e.Message);
				}
			}
			if (replay != null)
			{
				var region = this.settings.Get<string>("Region");
				if (currentReplay == null || region != currentRegion || (currentReplay != null && replay.dateTime > currentReplay.dateTime))
				{
					currentReplay = replay;
					currentRegion = region;
					statsStatusChangedSubject.OnNext(StatsStatus.Fetching);
					var stats = await apiService.Stats(currentReplay);
					if (stats.Count() > 6)
					{
						statsStatusChangedSubject.OnNext(StatsStatus.Fetched);
					}
					else
					{
						statsStatusChangedSubject.OnNext(StatsStatus.WrongRegion);
					}
				}
			}
		}
	}


	public enum StatsStatus
	{
		Waiting,
		Fetching,
		Fetched,
		WrongRegion
	}
}

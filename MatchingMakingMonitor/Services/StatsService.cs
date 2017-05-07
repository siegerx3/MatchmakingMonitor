using MatchingMakingMonitor.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MatchingMakingMonitor.Services
{
	public class StatsService
	{
		private Replay currentReplay;
		private string currentRegion;

		private LoggingService loggingService;
		private WatcherService watcherService;
		private ApiService apiService;
		private Settings settings;

		private BehaviorSubject<StatsStatus> statsStatusChangedSubject;
		public IObservable<StatsStatus> StatsStatusChanged => statsStatusChangedSubject.AsObservable();

		private BehaviorSubject<List<DisplayPlayerStats>> statsSubject;
		public IObservable<List<DisplayPlayerStats>> Stats => statsSubject.Where(s => s != null).AsObservable();

		public StatsService(LoggingService loggingService, Settings settings, WatcherService watcherService, ApiService apiService)
		{
			this.loggingService = loggingService;
			this.watcherService = watcherService;
			this.apiService = apiService;
			this.settings = settings;

			this.statsStatusChangedSubject = new BehaviorSubject<StatsStatus>(StatsStatus.Waiting);
			this.statsSubject = new BehaviorSubject<List<DisplayPlayerStats>>(null);

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
			Replay replay = null;
			using (var sr = new StreamReader(path))
			{
				try
				{
					replay = await Task.Run(async () => { return JsonConvert.DeserializeObject<Replay>(await sr.ReadToEndAsync(), new IsoDateTimeConverter()); });
				}
				catch (Exception e)
				{
					loggingService.Error("Error while reading replay file", e);
				}
			}
			if (replay != null)
			{
				var region = settings.Region;
				if (currentReplay == null || region != currentRegion || (currentReplay != null && replay.dateTime > currentReplay.dateTime))
				{
					currentReplay = replay;
					currentRegion = region;
					statsStatusChangedSubject.OnNext(StatsStatus.Fetching);
					var players = await apiService.Players(currentReplay);
					if (players.Count() > 6)
					{
						try
						{
							var stats = await computeDisplayPlayer(players);
							statsStatusChangedSubject.OnNext(StatsStatus.Fetched);
							statsSubject.OnNext(stats);
						}
						catch (Exception e)
						{
							loggingService.Error("Exception occured when computing player for display", e);
							statsStatusChangedSubject.OnNext(StatsStatus.Waiting);
						}
					}
					else
					{
						statsStatusChangedSubject.OnNext(StatsStatus.WrongRegion);
					}
				}
			}
		}

		private async Task<List<DisplayPlayerStats>> computeDisplayPlayer(IEnumerable<PlayerShip> players)
		{
			return await Task.Run(() => players.Select(p => new DisplayPlayerStats(settings, p)).ToList());
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

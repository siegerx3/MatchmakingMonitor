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
using System.Windows.Media;

namespace MatchingMakingMonitor.Services
{
	public class StatsService
	{
		private Replay currentReplay;
		private string currentRegion;
		private static string[] colors = new string[9] { "Overall9", "Overall8", "Overall7", "Overall6", "Overall5", "Overall4", "Overall3", "Overall2", "Overall1" };

		private LoggingService loggingService;
		private WatcherService watcherService;
		private ApiService apiService;
		private Settings settings;

		private BehaviorSubject<StatsStatus> statsStatusChangedSubject;
		public IObservable<StatsStatus> StatsStatusChanged => statsStatusChangedSubject.AsObservable();

		private BehaviorSubject<List<DisplayPlayer>> statsSubject;
		public IObservable<List<DisplayPlayer>> Stats => statsSubject.Where(s => s != null).AsObservable();

		public StatsService(LoggingService loggingService, Settings settings, WatcherService watcherService, ApiService apiService)
		{
			this.loggingService = loggingService;
			this.watcherService = watcherService;
			this.apiService = apiService;
			this.settings = settings;

			this.statsStatusChangedSubject = new BehaviorSubject<StatsStatus>(StatsStatus.Waiting);
			this.statsSubject = new BehaviorSubject<List<DisplayPlayer>>(null);

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
					replay = await Task.Run(async () => { return JsonConvert.DeserializeObject<Replay>(await sr.ReadToEndAsync()); });
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
							loggingService.Log("Exception occured when computing display player. " + e.Message);
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

		private async Task<List<DisplayPlayer>> computeDisplayPlayer(IEnumerable<PlayerShip> players)
		{
			var brushes = colors.Select(name =>
			{
				var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settings.Get<string>(name)));
				brush.Freeze();
				return brush;
			}).ToArray();
			return await Task.Run(() => players.Select(p => new DisplayPlayer(p, brushes)).ToList());
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

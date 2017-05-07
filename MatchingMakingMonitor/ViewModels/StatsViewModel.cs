using MatchingMakingMonitor.Models;
using MatchingMakingMonitor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Windows;
using System.Threading;

namespace MatchingMakingMonitor.ViewModels
{
	public class StatsViewModel : BaseViewBinding
	{
		public RelayCommand DetailCommand { get; set; }

		private ObservableCollection<DisplayPlayerStats> friendlyPlayers;
		public ObservableCollection<DisplayPlayerStats> FriendlyPlayers
		{
			get { return friendlyPlayers; }
			set
			{
				friendlyPlayers = value;
				FirePropertyChanged();
			}
		}

		private ObservableCollection<DisplayPlayerStats> enemyPlayers;
		public ObservableCollection<DisplayPlayerStats> EnemyPlayers
		{
			get { return enemyPlayers; }
			set
			{
				enemyPlayers = value;
				FirePropertyChanged();
			}
		}

		private int fontSize;

		public int FontSize
		{
			get { return fontSize; }
			set
			{
				fontSize = value;
				FirePropertyChanged();
			}
		}


		private Visibility listVisibility = Visibility.Collapsed;

		public Visibility ListVisibility
		{
			get { return listVisibility; }
			set
			{
				listVisibility = value;
				FirePropertyChanged();
			}
		}

		private Settings settings;
		private List<DisplayPlayerStats> stats;
		public StatsViewModel(LoggingService loggingService, StatsService statsService, Settings settings)
		{
			this.settings = settings;

			DetailCommand = new RelayCommand(param => openPlayerDetail((string[])param));

			statsService.Stats.Subscribe(async stats =>
			{
				await Task.Run(() =>
				{
					this.stats = stats;
					FriendlyPlayers = new ObservableCollection<DisplayPlayerStats>(stats.Where(p => p.Player.Relation != 2));
					EnemyPlayers = new ObservableCollection<DisplayPlayerStats>(stats.Where(p => p.Player.Relation == 2));
					ListVisibility = Visibility.Visible;
				});
			});

			this.settings.UiPropertiesChanged.Subscribe(async key =>
			{
				if (stats != null)
				{
					FontSize = this.settings.FontSize;
					foreach (var player in stats)
					{
						await Task.Run(() =>
						{
							player.ComputeUi();
						});
					}
				}
			});
		}

		public StatsViewModel()
		{
			FriendlyPlayers = new ObservableCollection<DisplayPlayerStats>(new List<DisplayPlayerStats>() { DisplayPlayerStats.MockPlayer(1), DisplayPlayerStats.MockPlayer(0) });
			EnemyPlayers = new ObservableCollection<DisplayPlayerStats>(new List<DisplayPlayerStats>() { DisplayPlayerStats.MockPlayer(2), DisplayPlayerStats.MockPlayer(2, true) });
			ListVisibility = Visibility.Visible;
		}

		private void openPlayerDetail(string[] param)
		{
			if (param[0] != "0")
			{
				System.Diagnostics.Process.Start($"https://{settings.Region}.warships.today/player/{param[0]}/{param[1]}");
			}
		}
	}
}

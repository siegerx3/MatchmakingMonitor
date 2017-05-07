using MatchingMakingMonitor.Models;
using MatchingMakingMonitor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MatchingMakingMonitor.ViewModels
{
	public class StatsViewModel : BaseViewModel
	{
		public RelayCommand DetailCommand { get; set; }

		private ObservableCollection<DisplayPlayer> friendlyPlayers;
		public ObservableCollection<DisplayPlayer> FriendlyPlayers
		{
			get { return friendlyPlayers; }
			set
			{
				friendlyPlayers = value;
				FirePropertyChanged();
			}
		}

		private ObservableCollection<DisplayPlayer> enemyPlayers;
		public ObservableCollection<DisplayPlayer> EnemyPlayers
		{
			get { return enemyPlayers; }
			set
			{
				enemyPlayers = value;
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
		public StatsViewModel(StatsService statsService, Settings settings)
		{
			this.settings = settings;

			DetailCommand = new RelayCommand(param => openPlayerDetail((string[])param));

			statsService.Stats.Subscribe(stats =>
			{
				FriendlyPlayers = new ObservableCollection<DisplayPlayer>(stats.Where(p => p.Player.Relation != 2));
				EnemyPlayers = new ObservableCollection<DisplayPlayer>(stats.Where(p => p.Player.Relation == 2));
				ListVisibility = Visibility.Visible;
			});
		}

		public StatsViewModel()
		{
			FriendlyPlayers = new ObservableCollection<DisplayPlayer>(new List<DisplayPlayer>() { DisplayPlayer.MockPlayer(1), DisplayPlayer.MockPlayer(0) });
			EnemyPlayers = new ObservableCollection<DisplayPlayer>(new List<DisplayPlayer>() { DisplayPlayer.MockPlayer(2), DisplayPlayer.MockPlayer(2, true) });
			ListVisibility = Visibility.Visible;
		}

		private void openPlayerDetail(string[] param)
		{
			if (param[0] != "0")
			{
				System.Diagnostics.Process.Start($"https://{settings.Get<string>("Region")}.warships.today/player/{param[0]}/{param[1]}");
			}
		}
	}
}

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


		public StatsViewModel(StatsService statsService)
		{
			statsService.Stats.Subscribe(stats =>
			{
				FriendlyPlayers = new ObservableCollection<DisplayPlayer>(stats.Where(p => p.Player.Relation != 2));
				EnemyPlayers = new ObservableCollection<DisplayPlayer>(stats.Where(p => p.Player.Relation == 2));
				ListVisibility = Visibility.Visible;
			});
		}

		public StatsViewModel()
		{
			FriendlyPlayers = new ObservableCollection<DisplayPlayer>(new List<DisplayPlayer>() { DisplayPlayer.MockPlayer(), DisplayPlayer.MockPlayer(true) });
			EnemyPlayers = new ObservableCollection<DisplayPlayer>(new List<DisplayPlayer>() { DisplayPlayer.MockPlayer(), DisplayPlayer.MockPlayer(true) });
			ListVisibility = Visibility.Visible;
		}
	}
}

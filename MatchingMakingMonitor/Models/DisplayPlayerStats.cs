using MatchingMakingMonitor.Services;
using MatchingMakingMonitor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MatchingMakingMonitor.Models
{
	public class DisplayPlayerStats: BaseViewBinding
	{
		public PlayerShip Player { get; set; }

		private SolidColorBrush background;
		public SolidColorBrush Background
		{
			get { return background; }
			set
			{
				background = value;
				FirePropertyChanged();
			}
		}

		private SolidColorBrush colorName = Brushes.Black;
		public SolidColorBrush ColorName
		{
			get { return colorName; }
			set
			{
				colorName = value;
				FirePropertyChanged();
			}
		}

		private SolidColorBrush colorWinRate = Brushes.Black;
		public SolidColorBrush ColorWinRate
		{
			get { return colorWinRate; }
			set
			{
				colorWinRate = value;
				FirePropertyChanged();
			}
		}

		private SolidColorBrush colorAvgXp = Brushes.Black;
		public SolidColorBrush ColorAvgXp
		{
			get { return colorAvgXp; }
			set
			{
				colorAvgXp = value;
				FirePropertyChanged();
			}
		}

		private SolidColorBrush colorAvgFrags = Brushes.Black;
		public SolidColorBrush ColorAvgFrags
		{
			get { return colorAvgFrags; }
			set
			{
				colorAvgFrags = value;
				FirePropertyChanged();
			}
		}

		private SolidColorBrush colorAvgDamage = Brushes.Black;
		public SolidColorBrush ColorAvgDamage
		{
			get { return colorAvgDamage; }
			set
			{
				colorAvgDamage = value;
				FirePropertyChanged();
			}
		}

		private SolidColorBrush colorBattles = Brushes.Black;
		public SolidColorBrush ColorBattles
		{
			get { return colorBattles; }
			set
			{
				colorBattles = value;
				FirePropertyChanged();
			}
		}

		private SolidColorBrush colorBorder = Brushes.Transparent;
		public SolidColorBrush ColorBorder
		{
			get { return colorBorder; }
			set
			{
				colorBorder = value;
				FirePropertyChanged();
			}
		}
		
		public string TextBattles => $"Battles: {Player.Battles}";
		public string TextWins => $"Wins: {Player.Wins}";
		public string TextWinRate => $"WinRate: {WinRate}%";
		public string TextAvgXp => $"Avg XP: {AvgXp}";
		public string TextAvgFrags => $"Avg Frags: {AvgFrags}";
		public string TextAvgDamage => $"Avg Damage: {AvgDamage}";
		public string TextName => $"{Player?.Nickname} | {AccountId}";

		public int FontSize => settings.FontSize;

		public string[] CommandParams => new string[] { AccountId, Player.Nickname };

		public string ShipName => Player?.ShipName;
		public string AccountId => Player?.AccountId.ToString();

		public double WinRate { get; private set; }
		public double AvgXp { get; private set; }
		public double AvgFrags { get; private set; }
		public double AvgDamage { get; private set; }

		public Visibility StatsVisibility
		{
			get
			{
				return Player.IsPrivateOrHidden ? Visibility.Collapsed : Visibility.Visible;
			}
		}

		public Visibility TextVisibility
		{
			get
			{
				return !Player.IsPrivateOrHidden ? Visibility.Collapsed : Visibility.Visible;
			}
		}

		private Settings settings;
		public DisplayPlayerStats(Settings settings, PlayerShip player) : this()
		{
			Player = player;
			this.settings = settings;
			WinRate = Math.Round(player.Wins / player.Battles * 100, 2);
			AvgFrags = Math.Round(player.Frags / player.Battles, 2);
			AvgXp = Math.Round(player.XpEarned / player.Battles, 0);
			AvgDamage = Math.Round(player.DamageDealt / player.Battles, 0);

			ComputeUi();
		}

		public DisplayPlayerStats()
		{

		}

		public static DisplayPlayerStats MockPlayer(int relation = 1, bool privateOrHidden = false)
		{
			return new DisplayPlayerStats()
			{
				Player = new PlayerShip() { Nickname = "Test", AccountId = 12323325, ShipName = "ShipName", IsPrivateOrHidden = privateOrHidden, Relation = relation },
				WinRate = 40,
				AvgFrags = 5,
				AvgXp = 1234,
				AvgDamage = 123242
			};
		}

		public void ComputeUi()
		{
			double totalRating = 0;
			if (!Player.IsPrivateOrHidden)
			{
				ColorWinRate = getColor(WinRate, winBoundaries, totalRating, 1, out totalRating);
				ColorAvgFrags = getColor(AvgFrags, fragsBoundaries, totalRating, 1.1, out totalRating);
				ColorAvgXp = getColor(AvgXp, xpBoundaries, totalRating, 0.8, out totalRating);
				ColorAvgDamage = getColor(AvgDamage, dmgBoundaries, totalRating, 0.9, out totalRating);
				ColorBattles = getColor(Player.Battles, battleBoundaries, totalRating, 1.2, out totalRating);

				var totalRatingInt = (int)Math.Floor((double)(totalRating / 5));
				ColorName = settings.Brushes[totalRatingInt - 1];
			}

			var color = ColorName.CloneCurrentValue();
			color.Opacity = 0.08;
			color.Freeze();
			Background = color;

			ColorBorder = Player.Relation == 0 ? ColorName : Brushes.Transparent;
		}

		#region Colors
		private static double[] dmgBoundaries = new double[9] { 75000, 65000, 55000, 45000, 35000, 25000, 15000, 10000, 0 };
		private static double[] xpBoundaries = new double[9] { 1500, 1200, 1000, 900, 800, 600, 500, 400, 0 };
		private static double[] fragsBoundaries = new double[9] { 1.5, 1.3, 1.1, 1.0, 0.8, 0.6, 0.4, 0.2, 0 };
		private static double[] winBoundaries = new double[9] { 90, 80, 70, 60, 50, 40, 30, 20, 0 };
		private static double[] battleBoundaries = new double[9] { 150, 100, 80, 60, 40, 30, 20, 10, 0 };

		private SolidColorBrush getColor(double value, double[] boundaries)
		{
			double dummy = 0;
			return getColor(value, boundaries, dummy, 1, out dummy);
		}

		private SolidColorBrush getColor(double value, double[] boundaries, double oldTotal, double multiplier, out double total)
		{
			for (int i = 0; i < boundaries.Length; i++)
			{
				if (value >= boundaries[i])
				{
					total = oldTotal + ((i + 1) * multiplier);
					return settings.Brushes[i];
				}
			}
			total = oldTotal + (9 * multiplier);
			return Brushes.Black;
		}
		#endregion
	}
}

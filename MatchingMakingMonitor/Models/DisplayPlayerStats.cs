using MatchMakingMonitor.Services;
using MatchMakingMonitor.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MatchMakingMonitor.Models
{
	public class DisplayPlayerStats : BaseViewBinding
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

		public string[] CommandParams => new string[] { AccountId, Player.Nickname };

		public string ShipName => Player?.ShipName;
		public string AccountId => Player?.AccountId.ToString();

		public double WinRate { get; private set; }
		public double AvgXp { get; private set; }
		public double AvgFrags { get; private set; }
		public double AvgDamage { get; private set; }

		private int colorNameKey;
		private int colorWinRateKey;
		private int colorAvgFragsKey;
		private int colorAvgXpKey;
		private int colorAvgDamageKey;
		private int colorBattlesKey;


		public Visibility Visibility
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

		public Settings Settings { get; set; }
		public DisplayPlayerStats(Settings settings, PlayerShip player) : this()
		{
			Player = player;
			this.Settings = settings;
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
				ColorWinRate = getColor(WinRate, Settings.WinLimits, totalRating, Settings.WinWeight, out totalRating, out colorWinRateKey);
				ColorAvgFrags = getColor(AvgFrags, Settings.FragsLimits, totalRating, Settings.FragsWeight, out totalRating, out colorAvgFragsKey);
				ColorAvgXp = getColor(AvgXp, Settings.XpLimits, totalRating, Settings.XpWeight, out totalRating, out colorAvgXpKey);
				ColorAvgDamage = getColor(AvgDamage, Settings.DmgLimits, totalRating, Settings.DmgWeight, out totalRating, out colorAvgDamageKey);
				ColorBattles = getColor(Player.Battles, Settings.BattleLimits, totalRating, Settings.BattleWeight, out totalRating, out colorBattlesKey);

				colorNameKey = (int)Math.Floor((double)(totalRating / 5));
				ColorName = Settings.Brushes[colorNameKey - 1];
			}

			var color = ColorName.CloneCurrentValue();
			color.Opacity = 0.08;
			color.Freeze();
			Background = color;

			ColorBorder = Player.Relation == 0 ? ColorName : Brushes.Transparent;
		}

		#region Colors
		private static double[] dmgLimits = new double[9] { 75000, 65000, 55000, 45000, 35000, 25000, 15000, 10000, 0 };
		private static double[] xpLimits = new double[9] { 1500, 1200, 1000, 900, 800, 600, 500, 400, 0 };
		private static double[] fragsLimits = new double[9] { 1.5, 1.3, 1.1, 1.0, 0.8, 0.6, 0.4, 0.2, 0 };
		private static double[] winLimits = new double[9] { 90, 80, 70, 60, 50, 40, 30, 20, 0 };
		private static double[] battleLimits = new double[9] { 150, 100, 80, 60, 40, 30, 20, 10, 0 };

		private SolidColorBrush getColor(double value, ObservableCollection<double> limits)
		{
			double dummy = 0;
			int dummyInt = 0;
			return getColor(value, limits, dummy, 1, out dummy, out dummyInt);
		}

		private SolidColorBrush getColor(double value, ObservableCollection<double> limits, double oldTotal, double multiplier, out double total, out int key)
		{
			for (int i = 0; i < limits.Count; i++)
			{
				if (value >= limits[i])
				{
					key = i + 1;
					total = oldTotal + (key * multiplier);
					return Settings.Brushes[i];
				}
			}
			total = oldTotal + (9 * multiplier);
			key = 9;
			return Brushes.Black;
		}
		#endregion

		public MobileDisplayPlayerStats ToMobile()
		{
			return new MobileDisplayPlayerStats()
			{
				Relation = Player.Relation,
				PrivateOrHidden = Player.IsPrivateOrHidden,
				DisplayName = TextName,
				Name = Player.Nickname,
				AccountId = AccountId,
				ShipName = ShipName,
				Battles = Player.Battles,
				Wins = Player.Wins,
				WinRate = WinRate,
				AvgXp = AvgXp,
				AvgFrags = AvgFrags,
				AvgDamage = AvgDamage,

				ColorNameKey = colorNameKey,
				ColorWinRateKey = colorWinRateKey,
				ColorAvgFragsKey = colorAvgFragsKey,
				ColorAvgXpKey = colorAvgXpKey,
				ColorAvgDamageKey = colorAvgDamageKey,
				ColorBattlesKey = colorBattlesKey,
			};
		}
	}
}

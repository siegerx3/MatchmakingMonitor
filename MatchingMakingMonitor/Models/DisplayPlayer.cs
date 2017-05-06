using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MatchingMakingMonitor.Models
{
	public class DisplayPlayer
	{
		public PlayerShip Player { get; set; }

		public SolidColorBrush Background { get; private set; }
		public SolidColorBrush ColorName { get; private set; } = Brushes.Black;
		public SolidColorBrush ColorWinRate { get; private set; } = Brushes.Black;
		public SolidColorBrush ColorAvgXp { get; private set; } = Brushes.Black;
		public SolidColorBrush ColorAvgFrags { get; private set; } = Brushes.Black;
		public SolidColorBrush ColorAvgDamage { get; private set; } = Brushes.Black;

		public string TextBattles => $"Battles: {Player.Battles}";
		public string TextWins => $"Wins: {Player.Wins}";
		public string TextWinRate => $"WinRate: {WinRate}%";
		public string TextAvgXp => $"Avg XP: {AvgXp}";
		public string TextAvgFrags => $"Avg Frags: {AvgFrags}";
		public string TextAvgDamage => $"Avg Damage: {AvgDamage}";

		public string Name => Player?.Nickname;
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

		private SolidColorBrush[] brushes;

		public DisplayPlayer(PlayerShip player, SolidColorBrush[] brushes) : this()
		{
			this.Player = player;
			this.brushes = brushes;
			WinRate = Math.Round(player.Wins / player.Battles * 100, 2);
			AvgFrags = Math.Round(player.Frags / player.Battles, 2);
			AvgXp = Math.Round(player.XpEarned / player.Battles, 0);
			AvgDamage = Math.Round(player.DamageDealt / player.Battles, 0);
			ColorAvgDamage = getAvgDamageColor();
		}

		public DisplayPlayer()
		{
			var color = ColorName.CloneCurrentValue();
			color.Opacity = 0.2;
			color.Freeze();
			Background = color;
		}

		public static DisplayPlayer MockPlayer(bool privateOrHidden = false)
		{
			return new DisplayPlayer()
			{
				Player = new PlayerShip() { Nickname = "Test", AccountId = 12323325, ShipName = "ShipName", IsPrivateOrHidden = privateOrHidden },
				WinRate = 40,
				AvgFrags = 5,
				AvgXp = 1234,
				AvgDamage = 123242,
			};
		}

		private static int[] dmgBoundaries = new int[9] { 75000, 65000, 55000, 45000, 35000, 25000, 15000, 10000, 0 };
		#region NameColor
		private SolidColorBrush getAvgDamageColor()
		{
			for (int i = 0; i < dmgBoundaries.Length; i++)
			{
				if (AvgDamage >= dmgBoundaries[i])
				{
					return brushes[i];
				}
			}
			return Brushes.Black;
		}
		#endregion
	}
}

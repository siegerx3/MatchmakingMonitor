using MatchMakingMonitor.config.Reflection;
using Newtonsoft.Json;

namespace MatchMakingMonitor.config
{
	public class SettingsJson : NotifySettingPropertyChanged
	{
		private Region _region;
		private string _installDirectory;
		private string _token;
		private int _fontSize;
		private string _color1;
		private string _color2;
		private string _color3;
		private string _color4;
		private string _color5;
		private string _color6;
		private string _color7;
		private string _color8;
		private string _color9;
		private double _battleWeight;
		private double _fragsWeight;
		private double _xpWeight;
		private double _dmgWeight;
		private double _winWeight;


		[JsonProperty("installDirectory")]
		public string InstallDirectory
		{
			get => _installDirectory;
			set
			{
				var oldValue = _installDirectory;
				_installDirectory = value;
				FirePropertyChanged(oldValue, value);
			}
		}

		[JsonProperty("region")]
		public Region Region
		{
			get => _region;
			set
			{
				var oldValue = _region;
				_region = value;
				FirePropertyChanged(oldValue, value);
			}
		}

		[JsonProperty("appIds")]
		public AppId[] AppIds { get; set; }

		[JsonProperty("baseUrls")]
		public BaseUrl[] BaseUrls { get; set; }

		[JsonProperty("token")]
		public string Token
		{
			get => _token;
			set
			{
				var oldValue = _token;
				_token = value;
				FirePropertyChanged(oldValue, value);
			}
		}

		[JsonProperty("color1"), UiSetting, ColorSetting]
		public string Color1
		{
			get => _color1;
			set
			{
				var oldValue = _color1;
				_color1 = value;
				FirePropertyChanged(oldValue, value);
			}
		}

		[JsonProperty("color2"), UiSetting, ColorSetting]
		public string Color2
		{
			get => _color2;
			set
			{
				var oldValue = _color2;
				_color2 = value;
				FirePropertyChanged(oldValue, value);
			}
		}

		[JsonProperty("color3"), UiSetting, ColorSetting]
		public string Color3
		{
			get => _color3;
			set
			{
				var oldValue = _color3;
				_color3 = value;
				FirePropertyChanged(oldValue, value);
			}
		}

		[JsonProperty("color4"), UiSetting, ColorSetting]
		public string Color4
		{
			get => _color4;
			set
			{
				var oldValue = _color4;
				_color4 = value;
				FirePropertyChanged(oldValue, value);
			}
		}

		[JsonProperty("color5"), UiSetting, ColorSetting]
		public string Color5
		{
			get => _color5;
			set
			{
				var oldValue = _color5;
				_color5 = value;
				FirePropertyChanged(oldValue, value);
			}
		}

		[JsonProperty("color6"), UiSetting, ColorSetting]
		public string Color6
		{
			get => _color6;
			set
			{
				var oldValue = _color6;
				_color6 = value;
				FirePropertyChanged(oldValue, value);
			}
		}

		[JsonProperty("color7"), UiSetting, ColorSetting]
		public string Color7
		{
			get => _color7;
			set
			{
				var oldValue = _color7;
				_color7 = value;
				FirePropertyChanged(oldValue, value);
			}
		}

		[JsonProperty("color8"), UiSetting, ColorSetting]
		public string Color8
		{
			get => _color8;
			set
			{
				var oldValue = _color8;
				_color8 = value;
				FirePropertyChanged(oldValue, value);
			}
		}

		[JsonProperty("color9"), UiSetting, ColorSetting]
		public string Color9
		{
			get => _color9;
			set
			{
				var oldValue = _color9;
				_color9 = value;
				FirePropertyChanged(oldValue, value);
			}
		}

		[JsonProperty("fontSize"), UiSetting]
		public int FontSize
		{
			get => _fontSize;
			set
			{
				var oldValue = _fontSize;
				_fontSize = value;
				FirePropertyChanged(oldValue, value);
			}
		}

		[JsonProperty("battleLimits"), NestedSetting]
		public LimitValue[] BattleLimits { get; set; }

		[JsonProperty("winRateLimits"), NestedSetting]
		public LimitValue[] WinRateLimits { get; set; }

		[JsonProperty("fragsLimits"), NestedSetting]
		public LimitValue[] FragsLimits { get; set; }

		[JsonProperty("xpLimits"), NestedSetting]
		public LimitsTier[] XpLimits { get; set; }

		[JsonProperty("dmgLimits"), NestedSetting]
		public LimitsType DmgLimits { get; set; }

		[JsonProperty("battleWeight"), UiSetting]
		public double BattleWeight
		{
			get => _battleWeight;
			set
			{
				var oldValue = _battleWeight;
				_battleWeight = value;
				FirePropertyChanged(oldValue, value);
			}
		}

		[JsonProperty("fragsWeight"), UiSetting]
		public double FragsWeight
		{
			get => _fragsWeight;
			set
			{
				var oldValue = _fragsWeight;
				_fragsWeight = value;
				FirePropertyChanged(oldValue, value);
			}
		}

		[JsonProperty("xpWeight"), UiSetting]
		public double XpWeight
		{
			get => _xpWeight;
			set
			{
				var oldValue = _xpWeight;
				_xpWeight = value;
				FirePropertyChanged(oldValue, value);
			}
		}

		[JsonProperty("dmgWeight"), UiSetting]
		public double DmgWeight
		{
			get => _dmgWeight;
			set
			{
				var oldValue = _dmgWeight;
				_dmgWeight = value;
				FirePropertyChanged(oldValue, value);
			}
		}

		[JsonProperty("winWeight"), UiSetting]
		public double WinWeight
		{
			get => _winWeight;
			set
			{
				var oldValue = _winWeight;
				_winWeight = value;
				FirePropertyChanged(oldValue, value);
			}
		}
	}
}

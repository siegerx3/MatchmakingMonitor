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
			set { _installDirectory = value; FirePropertyChanged(); }
		}

		[JsonProperty("region")]
		public Region Region
		{
			get => _region;
			set { _region = value; FirePropertyChanged(); }
		}

		[JsonProperty("appIds")]
		public AppId[] AppIds { get; set; }

		[JsonProperty("baseUrls")]
		public BaseUrl[] BaseUrls { get; set; }

		[JsonProperty("token")]
		public string Token
		{
			get => _token;
			set { _token = value; FirePropertyChanged(); }
		}

		[JsonProperty("color1"), UiSetting, ColorSetting]
		public string Color1
		{
			get => _color1;
			set { _color1 = value; FirePropertyChanged(); }
		}

		[JsonProperty("color2"), UiSetting, ColorSetting]
		public string Color2
		{
			get => _color2;
			set { _color2 = value; FirePropertyChanged(); }
		}

		[JsonProperty("color3"), UiSetting, ColorSetting]
		public string Color3
		{
			get => _color3;
			set { _color3 = value; FirePropertyChanged(); }
		}

		[JsonProperty("color4"), UiSetting, ColorSetting]
		public string Color4
		{
			get => _color4;
			set { _color4 = value; FirePropertyChanged(); }
		}

		[JsonProperty("color5"), UiSetting, ColorSetting]
		public string Color5
		{
			get => _color5;
			set { _color5 = value; FirePropertyChanged(); }
		}

		[JsonProperty("color6"), UiSetting, ColorSetting]
		public string Color6
		{
			get => _color6;
			set { _color6 = value; FirePropertyChanged(); }
		}

		[JsonProperty("color7"), UiSetting, ColorSetting]
		public string Color7
		{
			get => _color7;
			set { _color7 = value; FirePropertyChanged(); }
		}

		[JsonProperty("color8"), UiSetting, ColorSetting]
		public string Color8
		{
			get => _color8;
			set { _color8 = value; FirePropertyChanged(); }
		}

		[JsonProperty("color9"), UiSetting, ColorSetting]
		public string Color9
		{
			get => _color9;
			set { _color9 = value; FirePropertyChanged(); }
		}

		[JsonProperty("fontSize"), UiSetting]
		public int FontSize
		{
			get => _fontSize;
			set { _fontSize = value; FirePropertyChanged(); }
		}

		[JsonProperty("battleLimits"), NestedSetting]
		public LimitsTier[] BattleLimits { get; set; }

		[JsonProperty("winLimits"), NestedSetting]
		public LimitsTier[] WinLimits { get; set; }

		[JsonProperty("fragsLimits"), NestedSetting]
		public LimitsTier[] FragsLimits { get; set; }

		[JsonProperty("xpLimits"), NestedSetting]
		public LimitsTier[] XpLimits { get; set; }

		[JsonProperty("dmgLimits"), NestedSetting]
		public LimitsType DmgLimits { get; set; }

		[JsonProperty("battleWeight"), UiSetting]
		public double BattleWeight
		{
			get => _battleWeight;
			set { _battleWeight = value; FirePropertyChanged(); }
		}

		[JsonProperty("fragsWeight"), UiSetting]
		public double FragsWeight
		{
			get => _fragsWeight;
			set { _fragsWeight = value; FirePropertyChanged(); }
		}

		[JsonProperty("xpWeight"), UiSetting]
		public double XpWeight
		{
			get => _xpWeight;
			set { _xpWeight = value; FirePropertyChanged(); }
		}

		[JsonProperty("dmgWeight"), UiSetting]
		public double DmgWeight
		{
			get => _dmgWeight;
			set { _dmgWeight = value; FirePropertyChanged(); }
		}

		[JsonProperty("winWeight"), UiSetting]
		public double WinWeight
		{
			get => _winWeight;
			set { _winWeight = value; FirePropertyChanged(); }
		}
	}
}

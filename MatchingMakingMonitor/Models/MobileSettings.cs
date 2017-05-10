using MatchMakingMonitor.Services;
// ReSharper disable InconsistentNaming

namespace MatchMakingMonitor.Models
{
	public class MobileSettings
	{
		public string color1 { get; set; }
		public string color2 { get; set; }
		public string color3 { get; set; }
		public string color4 { get; set; }
		public string color5 { get; set; }
		public string color6 { get; set; }
		public string color7 { get; set; }
		public string color8 { get; set; }
		public string color9 { get; set; }

		public static MobileSettings FromSettings(SettingsWrapper settingsWrapper)
		{
			return new MobileSettings()
			{
				color1 = settingsWrapper.CurrentSettings.Color1,
				color2 = settingsWrapper.CurrentSettings.Color2,
				color3 = settingsWrapper.CurrentSettings.Color3,
				color4 = settingsWrapper.CurrentSettings.Color4,
				color5 = settingsWrapper.CurrentSettings.Color5,
				color6 = settingsWrapper.CurrentSettings.Color6,
				color7 = settingsWrapper.CurrentSettings.Color7,
				color8 = settingsWrapper.CurrentSettings.Color8,
				color9 = settingsWrapper.CurrentSettings.Color9
			};
		}

		public void ToSettings(SettingsWrapper settingsWrapper)
		{
			settingsWrapper.CurrentSettings.Color1 = settingsWrapper.CurrentSettings.Color1;
			settingsWrapper.CurrentSettings.Color2 = settingsWrapper.CurrentSettings.Color2;
			settingsWrapper.CurrentSettings.Color3 = settingsWrapper.CurrentSettings.Color3;
			settingsWrapper.CurrentSettings.Color4 = settingsWrapper.CurrentSettings.Color4;
			settingsWrapper.CurrentSettings.Color5 = settingsWrapper.CurrentSettings.Color5;
			settingsWrapper.CurrentSettings.Color6 = settingsWrapper.CurrentSettings.Color6;
			settingsWrapper.CurrentSettings.Color7 = settingsWrapper.CurrentSettings.Color7;
			settingsWrapper.CurrentSettings.Color8 = settingsWrapper.CurrentSettings.Color8;
			settingsWrapper.CurrentSettings.Color9 = settingsWrapper.CurrentSettings.Color9;
		}
	}
}

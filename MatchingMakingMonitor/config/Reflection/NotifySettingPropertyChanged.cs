using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MatchMakingMonitor.config.Reflection
{
	public class NotifySettingPropertyChanged : NestedSetting, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		internal void FirePropertyChanged([CallerMemberName] string propertyName = null)
		{
			SettingChangedSubject?.OnNext(propertyName);
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace MatchMakingMonitor.config.Reflection
{
	public class NotifySettingPropertyChanged : NestedSetting, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		internal void FirePropertyChanged(object oldValue, object newValue, [CallerMemberName] string propertyName = null)
		{
			SettingChangedSubject?.OnNext(new ChangedSetting(oldValue, newValue, propertyName));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

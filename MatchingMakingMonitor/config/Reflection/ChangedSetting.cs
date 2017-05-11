using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MatchMakingMonitor.config.Reflection
{
	public class ChangedSetting
	{
		public string Key { get; }
		public object OldValue { get; }
		public object NewValue { get; }
		public bool Initial { get; set; }
		public bool HasChanged => !Initial && !OldValue.Equals(NewValue);

		public ChangedSetting(object oldvalue, object newValue, [CallerMemberName] string key = null)
		{
			Key = key;
			OldValue = oldvalue;
			NewValue = newValue;
		}
	}
}

using System.Linq;
using System.Reactive.Subjects;
using System.Reflection;

namespace MatchMakingMonitor.config.Reflection
{
	public class NestedSetting
	{
		protected BehaviorSubject<ChangedSetting> SettingChangedSubject;
		protected void AttachListener(System.Type type, object instance, BehaviorSubject<ChangedSetting> settingChangedSubject)
		{
			SettingChangedSubject = settingChangedSubject;
			AttachListenerStatic(type, instance, settingChangedSubject);
		}

		public static void AttachListenerStatic(System.Type type, object instance, BehaviorSubject<ChangedSetting> settingChangedSubject)
		{
			foreach (var prop in type.GetProperties().Where(p => p.GetCustomAttribute(typeof(NestedSettingAttribute), false) != null))
			{
				var value = prop.GetValue(instance);
				if (value == null) continue;
				var enumerable = value as object[];
				if (!value.GetType().IsArray)
				{
					(value as NestedSetting)?.AttachListener(value.GetType(), value, settingChangedSubject);
				}
				else
				{
					if (enumerable == null) continue;
					foreach (var entry in enumerable)
					{
						(entry as NestedSetting)?.AttachListener(value.GetType().GetElementType(), entry, settingChangedSubject);
					}
				}
			}
		}
	}

}

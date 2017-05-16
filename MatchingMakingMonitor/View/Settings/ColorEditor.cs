using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MatchMakingMonitor.config;
using MatchMakingMonitor.config.Reflection;

namespace MatchMakingMonitor.View.Settings
{
	public class ColorEditor
	{
		private Color _color1;
		private Color _color2;
		private Color _color3;
		private Color _color4;
		private Color _color5;
		private Color _color6;
		private Color _color7;
		private Color _color8;
		private Color _color9;

		private readonly BehaviorSubject<ChangedSetting> _changedSubject;
		private readonly ColorValue[] _values;
		private Action _valuesChanged;
		private readonly bool _initial;
		public ColorEditor(BehaviorSubject<ChangedSetting> changedSubject, ColorValue[] values)
		{
			_changedSubject = changedSubject;
			_values = values;
			_initial = true;
			LoadValues();
			_initial = false;
		}

		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		public void LoadValues()
		{
			_color1 = (Color)ColorConverter.ConvertFromString(_values.ElementAt(0).Value);
			_color2 = (Color)ColorConverter.ConvertFromString(_values.ElementAt(1).Value);
			_color3 = (Color)ColorConverter.ConvertFromString(_values.ElementAt(2).Value);
			_color4 = (Color)ColorConverter.ConvertFromString(_values.ElementAt(3).Value);
			_color5 = (Color)ColorConverter.ConvertFromString(_values.ElementAt(4).Value);
			_color6 = (Color)ColorConverter.ConvertFromString(_values.ElementAt(5).Value);
			_color7 = (Color)ColorConverter.ConvertFromString(_values.ElementAt(6).Value);
			_color8 = (Color)ColorConverter.ConvertFromString(_values.ElementAt(7).Value);
			_color9 = (Color)ColorConverter.ConvertFromString(_values.ElementAt(8).Value);
			_valuesChanged?.Invoke();
			if (!_initial)
			{
				_changedSubject.OnNext(new ChangedSetting(null, null, "UISetting"));
			}
		}
	}
}

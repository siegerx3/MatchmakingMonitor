using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using MatchMakingMonitor.config;
using MatchMakingMonitor.config.Reflection;

namespace MatchMakingMonitor.View.Settings
{
	public class DoubleLimitsEditor : ILimitsEditor
	{
		private double _value1;
		private double _value2;
		private double _value3;
		private double _value4;
		private double _value5;
		private double _value6;
		private double _value7;
		private double _value8;
		private double _value9;


		private readonly BehaviorSubject<ChangedSetting> _changedSubject;
		private readonly LimitValue[] _values;
		private Action _valuesChanged;
		private readonly bool _initial;
		public DoubleLimitsEditor(BehaviorSubject<ChangedSetting> changedSubject, LimitValue[] values)
		{
			_changedSubject = changedSubject;
			_values = values;
			_initial = true;
			LoadValues();
			_initial = false;
		}


		public void LoadValues()
		{
			_value1 = _values.ElementAt(0).Value;
			_value2 = _values.ElementAt(1).Value;
			_value3 = _values.ElementAt(2).Value;
			_value4 = _values.ElementAt(3).Value;
			_value5 = _values.ElementAt(4).Value;
			_value6 = _values.ElementAt(5).Value;
			_value7 = _values.ElementAt(6).Value;
			_value8 = _values.ElementAt(7).Value;
			_value9 = _values.ElementAt(8).Value;
			_valuesChanged?.Invoke();
			if (!_initial)
			{
				_changedSubject.OnNext(new ChangedSetting(null, null, "UISetting"));
			}
		}

		public void RegisterValuesChanged(Action action)
		{
			_valuesChanged = action;
		}

		public ILimitsEditor Init(LimitValue[] values)
		{

			return this;
		}

		private bool UpdateValue(string value, ref double field)
		{
			var oldValue = field;
			double newValue;
			if (!double.TryParse(value, out newValue)) return false;
			field = newValue;
			_changedSubject.OnNext(new ChangedSetting(oldValue, newValue, "Value"));
			return true;
		}

		public double Value1 => _value1;
		public double Value2 => _value2;
		public double Value3 => _value3;
		public double Value4 => _value4;
		public double Value5 => _value5;
		public double Value6 => _value6;
		public double Value7 => _value7;
		public double Value8 => _value8;
		public double Value9 => _value9;

		public bool UpdateValue1(string value)
		{
			return UpdateValue(value, ref _value1);
		}

		public bool UpdateValue2(string value)
		{
			return UpdateValue(value, ref _value2);
		}

		public bool UpdateValue3(string value)
		{
			return UpdateValue(value, ref _value3);
		}

		public bool UpdateValue4(string value)
		{
			return UpdateValue(value, ref _value4);
		}

		public bool UpdateValue5(string value)
		{
			return UpdateValue(value, ref _value5);
		}

		public bool UpdateValue6(string value)
		{
			return UpdateValue(value, ref _value6);
		}

		public bool UpdateValue7(string value)
		{
			return UpdateValue(value, ref _value7);
		}

		public bool UpdateValue8(string value)
		{
			return UpdateValue(value, ref _value8);
		}

		public bool UpdateValue9(string value)
		{
			return UpdateValue(value, ref _value9);
		}
	}
}

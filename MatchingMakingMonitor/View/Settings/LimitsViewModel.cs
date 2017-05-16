using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using MatchMakingMonitor.View.Util;

namespace MatchMakingMonitor.View.Settings
{
	public class LimitsViewModel : ViewModelBase
	{
		private readonly ILimitsEditor _limitsEditor;
		private bool suppressUpdate = false;
		public LimitsViewModel(string name, ILimitsEditor limitsEditor)
		{
			Name = name;
			_limitsEditor = limitsEditor;
			_limitsEditor.RegisterValuesChanged(UpdateValuesFromEditor);
			UpdateValuesFromEditor();
		}

		private void UpdateValuesFromEditor()
		{
			suppressUpdate = true;
			Value1 = _limitsEditor.Value1.ToString(CultureInfo.InvariantCulture).Replace('.', ',');
			Value2 = _limitsEditor.Value2.ToString(CultureInfo.InvariantCulture).Replace('.', ',');
			Value3 = _limitsEditor.Value3.ToString(CultureInfo.InvariantCulture).Replace('.', ',');
			Value4 = _limitsEditor.Value4.ToString(CultureInfo.InvariantCulture).Replace('.', ',');
			Value5 = _limitsEditor.Value5.ToString(CultureInfo.InvariantCulture).Replace('.', ',');
			Value6 = _limitsEditor.Value6.ToString(CultureInfo.InvariantCulture).Replace('.', ',');
			Value7 = _limitsEditor.Value7.ToString(CultureInfo.InvariantCulture).Replace('.', ',');
			Value8 = _limitsEditor.Value8.ToString(CultureInfo.InvariantCulture).Replace('.', ',');
			Value9 = _limitsEditor.Value9.ToString(CultureInfo.InvariantCulture).Replace('.', ',');
			suppressUpdate = false;
		}

		public void LoadValues()
		{
			_limitsEditor.LoadValues();
		}

		public string Name { get; set; }

		private string _value1;

		public string Value1
		{
			get => _value1;
			set
			{
				_value1 = value;
				if (!suppressUpdate)
					_limitsEditor.UpdateValue1(_value1);
				FirePropertyChanged();
			}
		}

		private string _value2;

		public string Value2
		{
			get => _value2;
			set
			{
				_value2 = value;
				if (!suppressUpdate)
					_limitsEditor.UpdateValue1(_value2);
				FirePropertyChanged();
			}
		}

		private string _value3;

		public string Value3
		{
			get => _value3;
			set
			{
				_value3 = value;
				if (!suppressUpdate)
					_limitsEditor.UpdateValue1(_value3);
				FirePropertyChanged();
			}
		}

		private string _value4;

		public string Value4
		{
			get => _value4;
			set
			{
				_value4 = value;
				if (!suppressUpdate)
					_limitsEditor.UpdateValue1(_value4);
				FirePropertyChanged();
			}
		}

		private string _value5;

		public string Value5
		{
			get => _value5;
			set
			{
				_value5 = value;
				if (!suppressUpdate)
					_limitsEditor.UpdateValue1(_value5);
				FirePropertyChanged();
			}
		}

		private string _value6;

		public string Value6
		{
			get => _value6;
			set
			{
				_value6 = value;
				if (!suppressUpdate)
					_limitsEditor.UpdateValue1(_value6);
				FirePropertyChanged();
			}
		}

		private string _value7;

		public string Value7
		{
			get => _value7;
			set
			{
				_value7 = value;
				if (!suppressUpdate)
					_limitsEditor.UpdateValue1(_value7);
				FirePropertyChanged();
			}
		}

		private string _value8;

		public string Value8
		{
			get => _value8;
			set
			{
				_value8 = value;
				if (!suppressUpdate)
					_limitsEditor.UpdateValue1(_value8);
				FirePropertyChanged();
			}
		}

		private string _value9;

		public string Value9
		{
			get => _value9;
			set
			{
				_value9 = value;
				if (!suppressUpdate)
					_limitsEditor.UpdateValue1(_value9);
				FirePropertyChanged();
			}
		}
	}
}

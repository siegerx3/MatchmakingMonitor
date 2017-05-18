using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MatchMakingMonitor.View.Util;

namespace MatchMakingMonitor.View.Settings
{
	public class ColorsViewModel : ViewModelBase
	{
		private readonly ColorsEditor _colorsEditor;
		private bool suppressUpdate;
		public ColorsViewModel(string name, ColorsEditor colorsEditor)
		{
			Name = name;
			_colorsEditor = colorsEditor;
			_colorsEditor.RegisterValuesChanged(UpdateValuesFromEditor);
			UpdateValuesFromEditor();
		}

		public string Name { get; set; }

		private void UpdateValuesFromEditor()
		{
			suppressUpdate = true;
			Color1 = _colorsEditor.Color1;
			Color2 = _colorsEditor.Color2;
			Color3 = _colorsEditor.Color3;
			Color4 = _colorsEditor.Color4;
			Color5 = _colorsEditor.Color5;
			Color6 = _colorsEditor.Color6;
			Color7 = _colorsEditor.Color7;
			Color8 = _colorsEditor.Color8;
			Color9 = _colorsEditor.Color9;
			suppressUpdate = false;
		}

		public void LoadValues()
		{
			_colorsEditor.LoadValues();
		}

		private Color _color1;

		public Color Color1
		{
			get => _color1;
			set
			{
				_color1 = value;
				if (!suppressUpdate)
					_colorsEditor.UpdateColor1(_color1);
				FirePropertyChanged();
			}
		}

		private Color _color2;

		public Color Color2
		{
			get => _color2;
			set
			{
				_color2 = value;
				if (!suppressUpdate)
					_colorsEditor.UpdateColor2(_color2);
				FirePropertyChanged();
			}
		}

		private Color _color3;

		public Color Color3
		{
			get => _color3;
			set
			{
				_color3 = value;
				if (!suppressUpdate)
					_colorsEditor.UpdateColor3(_color3);
				FirePropertyChanged();
			}
		}

		private Color _color4;

		public Color Color4
		{
			get => _color4;
			set
			{
				_color4 = value;
				if (!suppressUpdate)
					_colorsEditor.UpdateColor4(_color4);
				FirePropertyChanged();
			}
		}

		private Color _color5;

		public Color Color5
		{
			get => _color5;
			set
			{
				_color5 = value;
				if (!suppressUpdate)
					_colorsEditor.UpdateColor5(_color5);
				FirePropertyChanged();
			}
		}

		private Color _color6;

		public Color Color6
		{
			get => _color6;
			set
			{
				_color6 = value;
				if (!suppressUpdate)
					_colorsEditor.UpdateColor6(_color6);
				FirePropertyChanged();
			}
		}

		private Color _color7;

		public Color Color7
		{
			get => _color7;
			set
			{
				_color7 = value;
				if (!suppressUpdate)
					_colorsEditor.UpdateColor7(_color7);
				FirePropertyChanged();
			}
		}

		private Color _color8;

		public Color Color8
		{
			get => _color8;
			set
			{
				_color8 = value;
				if (!suppressUpdate)
					_colorsEditor.UpdateColor8(_color8);
				FirePropertyChanged();
			}
		}

		private Color _color9;

		public Color Color9
		{
			get => _color9;
			set
			{
				_color9 = value;
				if (!suppressUpdate)
					_colorsEditor.UpdateColor9(_color9);
				FirePropertyChanged();
			}
		}
	}
}

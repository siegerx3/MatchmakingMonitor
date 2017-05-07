using MatchingMakingMonitor.Services;
using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace MatchingMakingMonitor
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class SettingsWindow : Window
	{
		public SettingsWindow(Settings settings)
		{
			InitializeComponent();
		}

		private void IntValidationTextBox(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}

		private void DoubleValidationTextBox(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9\\,]+");
			e.Handled = regex.IsMatch(e.Text);
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
		}
	}
} //end namespace

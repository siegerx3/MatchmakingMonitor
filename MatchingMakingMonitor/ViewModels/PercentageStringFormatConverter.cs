using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace MatchMakingMonitor.ViewModels
{
	class PercentageConverter : IValueConverter
	{
		private static Regex percentRegex = new Regex("^100$|^\\d{0,2}(,\\d{1,2})?$");

		public object Convert(object value, Type targetType, object parameter,
											System.Globalization.CultureInfo culture)
		{
			if (string.IsNullOrEmpty(value.ToString())) return 0;

			if (value.GetType() == typeof(double)) return value.ToString();

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter,
															System.Globalization.CultureInfo culture)
		{
			if (string.IsNullOrEmpty(value.ToString())) return 0;

			var trimmedValue = value.ToString().TrimEnd(new char[] { '%' });
			if (!percentRegex.IsMatch(trimmedValue))
			{
				return new ValidationResult(false, "String format error");
			}

			if (targetType == typeof(double))
			{
				double result;
				if (double.TryParse(trimmedValue, out result))
					return result;
				else
					return value;

			}
			return value;
		}
	}
}

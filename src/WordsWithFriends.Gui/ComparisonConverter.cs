using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WordsWithFriends.Gui
{
	public class ComparisonConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if(object.ReferenceEquals(value, parameter))
			{
				return true;
			}
			if(value == null)
			{
				return false;
			}
			return value.Equals(parameter);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value?.Equals(true) == true ? parameter : new object();
		}
	}
}

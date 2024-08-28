using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ListViewExtensions.Views.Converters
{
	[ValueConversion(typeof(SortingDirection), typeof(Visibility))]
	internal class SortingIconVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(Equals(value, parameter))
				return Visibility.Visible;
			else
				return Visibility.Hidden;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(Equals(value, Visibility.Visible))
				return parameter;
			else
				return SortingCondition.None;
		}
	}
}

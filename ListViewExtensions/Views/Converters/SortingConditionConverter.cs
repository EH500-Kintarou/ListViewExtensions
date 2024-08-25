using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ListViewExtensions.Views.Converters
{
	[ValueConversion(typeof(SortingCondition), typeof(SortingDirection))]
	public class SortingConditionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var propertyName = parameter as string;

			if(value is SortingCondition sc && sc.PropertyName == propertyName)
				return sc.Direction;
			else
				return SortingDirection.None;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var propertyName = parameter as string;

			if(value is SortingDirection dir) {
				if(propertyName == null)
					return new SortingCondition(dir);
				else
					return new SortingCondition(propertyName, dir);
			} else
				return SortingCondition.None;
		}
	}
}

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
			var condition = value as SortingCondition ?? SortingCondition.None;
			var PropertyName = parameter as string ?? throw new ArgumentException(nameof(parameter));
			
			if(condition.PropertyName == PropertyName)
				return condition.Direction;
			else
				return SortingDirection.None;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(value is not SortingDirection direction) direction = SortingDirection.None;
			var PropertyName = parameter as string ?? throw new ArgumentException(nameof(parameter));

			if(direction == SortingDirection.None)
				return SortingCondition.None;
			else
				return new SortingCondition(PropertyName, direction);
		}
	}
}

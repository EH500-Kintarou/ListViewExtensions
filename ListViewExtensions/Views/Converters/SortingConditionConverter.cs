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
			SortingCondition condition = value as SortingCondition;
			string PropertyName = parameter as string;

			if(condition == null)
				condition = SortingCondition.None;
			if(PropertyName == null)
				throw new ArgumentException(nameof(parameter));

			if(condition.PropertyName == PropertyName)
				return condition.Direction;
			else
				return SortingDirection.None;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			SortingDirection direction;
			if(value is SortingDirection)
				direction = (SortingDirection)value;
			else
				direction = SortingDirection.None;
			
			string PropertyName = parameter as string;

			if(PropertyName == null)
				throw new ArgumentException(nameof(parameter));

			if(direction == SortingDirection.None)
				return SortingCondition.None;
			else
				return new SortingCondition(PropertyName, direction);
		}
	}
}

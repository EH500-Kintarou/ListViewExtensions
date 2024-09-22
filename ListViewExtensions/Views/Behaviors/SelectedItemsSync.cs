using Microsoft.Xaml.Behaviors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ListViewExtensions.Views.Behaviors
{
	public class SelectedItemsSync
	{
		private static SelectedItemsBinder GetSelectedValueBinder(DependencyObject obj)
		{
			return (SelectedItemsBinder)obj.GetValue(SelectedValueBinderProperty);
		}
		private static void SetSelectedValueBinder(DependencyObject obj, SelectedItemsBinder items)
		{
			obj.SetValue(SelectedValueBinderProperty, items);
		}
		private static readonly DependencyProperty SelectedValueBinderProperty = DependencyProperty.RegisterAttached("SelectedValueBinder", typeof(SelectedItemsBinder), typeof(SelectedItemsSync));


		public static void SetSource(DependencyObject obj, IList value)
		{
			obj.SetValue(SouceProperty, value);
		}
		public static IList GetSource(DependencyObject obj)
		{
			return (IList)obj.GetValue(SouceProperty);
		}
		public static readonly DependencyProperty SouceProperty = DependencyProperty.RegisterAttached("Source", typeof(IList), typeof(SelectedItemsSync),
			new FrameworkPropertyMetadata(null, OnSelectedValuesChanged));

		private static void OnSelectedValuesChanged(DependencyObject o, DependencyPropertyChangedEventArgs value)
		{
			if(o is ListView lv) {
				var oldBinder = GetSelectedValueBinder(o);
				if(oldBinder != null)
					oldBinder.UnBind();

				var binder = new SelectedItemsBinder(lv, (IList)value.NewValue);
				binder.Bind();
				SetSelectedValueBinder(o, binder);
			}
		}
	}
}

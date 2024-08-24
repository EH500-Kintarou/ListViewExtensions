using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ListViewExtensions.Views.Actions
{
	public class ListViewSelectedItemsAction : TriggerAction<ListView>
	{
		public System.Collections.IList Source
		{
			get { return (System.Collections.IList)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}
		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(System.Collections.IList), typeof(ListViewSelectedItemsAction), new FrameworkPropertyMetadata() { DefaultValue = null, BindsTwoWayByDefault = true });

		protected override void Invoke(object parameter)
		{
			if(this.AssociatedObject.SelectedItems != Source)
				Source = this.AssociatedObject.SelectedItems;
		}
	}
}

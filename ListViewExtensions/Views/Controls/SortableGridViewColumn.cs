using ListViewExtensions.ViewModels;
using ListViewExtensions.Views.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace ListViewExtensions.Views.Controls
{
	[ContentProperty(nameof(Header))]
	[StyleTypedProperty(Property = nameof(HeaderContainerStyle), StyleTargetType = typeof(SortableGridViewColumnHeader))]
	[Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)] // cannot be read & localized as string
	public class SortableGridViewColumn : GridViewColumn
	{
		public SortableGridViewColumn()
		{
		}

		#region Header Property

		public new object Header
		{
			get { return (object)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
		public static readonly new DependencyProperty HeaderProperty =
			DependencyProperty.Register(nameof(Header), typeof(object), typeof(SortableGridViewColumn), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnHeaderChanged)));

		private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var me = (SortableGridViewColumn)d;
			var baseme = (GridViewColumn)d;

			var oldHeader = e.OldValue as SortableGridViewColumnHeader;
			var newHeader = e.NewValue as SortableGridViewColumnHeader;
			var src = me.SortableSource;
			var member = me.DisplayMemberBinding as Binding;

			if(src != null && oldHeader != null)
				DetachSortableSource(src, oldHeader);

			if(newHeader == null)
				newHeader = new SortableGridViewColumnHeader() { Content = e.NewValue };
			baseme.Header = newHeader;

			if(src != null && member != null)
				AttachSortableSource(src, newHeader, member.Path.Path);
		}

		#endregion

		#region SortableSource Property

		public ISortableCollectionViewModel? SortableSource
		{
			get { return (ISortableCollectionViewModel?)GetValue(SortableSourceProperty); }
			set { SetValue(SortableSourceProperty, value); }
		}
		public static readonly DependencyProperty SortableSourceProperty =
			DependencyProperty.Register(nameof(SortableSource), typeof(ISortableCollectionViewModel), typeof(SortableGridViewColumn), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSortableSourceChanged)));

		private static void OnSortableSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var me = (SortableGridViewColumn)d;
			var baseme = (GridViewColumn)d;

			var oldsrc = e.OldValue as ISortableCollectionViewModel;
			var newsrc = e.NewValue as ISortableCollectionViewModel;
			var header = baseme.Header as SortableGridViewColumnHeader;
			var member = me.DisplayMemberBinding as Binding;

			if(oldsrc != null && header != null)
				DetachSortableSource(oldsrc, header);			

			if(newsrc != null && header != null && member != null)
				AttachSortableSource(newsrc, header, member.Path.Path);
		}

		#endregion

		#region DisplayMemberBinding Property

		protected override void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			switch(e.PropertyName) {
				case nameof(DisplayMemberBinding):
					OnDisplayMemberBinding(DisplayMemberBinding);
					break;
			}

			base.OnPropertyChanged(e);
		}

		private void OnDisplayMemberBinding(BindingBase newValue)
		{
			var src = SortableSource;
			var header = base.Header as SortableGridViewColumnHeader;
			var member = newValue as Binding;

			if(src != null && header != null)
				DetachSortableSource(src, header);

			if(src != null && header != null && member != null)
				AttachSortableSource(src, header, member.Path.Path);
		}

		#endregion


		static SortingConditionConverter SortingConditionConv { get; } = new SortingConditionConverter();

		private static void AttachSortableSource(ISortableCollectionViewModel source, SortableGridViewColumnHeader header, string propertyName)
		{
			header.SetBinding(SortableGridViewColumnHeader.SortingDirectionProperty, new Binding(nameof(source.SortingCondition)) {
				Mode = BindingMode.OneWay,
				Converter = SortingConditionConv,
				ConverterParameter = propertyName,
				Source = source,
			});

			header.SetBinding(SortableGridViewColumnHeader.CommandProperty, new Binding(nameof(source.SortByPropertyCommand)) { Source = source, });
			header.CommandParameter = propertyName;
		}

		private static void DetachSortableSource(ISortableCollectionViewModel source, SortableGridViewColumnHeader header)
		{
			BindingOperations.ClearBinding(header, SortableGridViewColumnHeader.SortingDirectionProperty);
			BindingOperations.ClearBinding(header, SortableGridViewColumnHeader.CommandProperty);
			header.CommandParameter = default;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ListViewExtensions.Views.Controls
{
	[TemplatePart(Name = "PART_HeaderGripper", Type = typeof(Thumb))]
	[TemplatePart(Name = "PART_FloatingHeaderCanvas", Type = typeof(Canvas))]
	public class SortableGridViewColumnHeader : GridViewColumnHeader
	{
		static SortableGridViewColumnHeader()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SortableGridViewColumnHeader), new FrameworkPropertyMetadata(typeof(SortableGridViewColumnHeader)));
		}

		public SortingDirection SortingDirection
		{
			get { return (SortingDirection)GetValue(SortingDirectionProperty); }
			set { SetValue(SortingDirectionProperty, value); }
		}
		public static readonly DependencyProperty SortingDirectionProperty =
			DependencyProperty.Register(nameof(SortingDirection), typeof(SortingDirection), typeof(SortableGridViewColumnHeader), new PropertyMetadata(default(SortingDirection)));

		public Dock SortingArrowLocation
		{
			get { return (Dock)GetValue(SortingArrowLocationProperty); }
			set { SetValue(SortingArrowLocationProperty, value); }
		}
		public static readonly DependencyProperty SortingArrowLocationProperty =
			DependencyProperty.Register(nameof(SortingArrowLocation), typeof(Dock), typeof(SortableGridViewColumnHeader), new PropertyMetadata(default(Dock)));

		public Thickness SortingArrowMargin
		{
			get { return (Thickness)GetValue(SortingArrowMarginProperty); }
			set { SetValue(SortingArrowMarginProperty, value); }
		}
		public static readonly DependencyProperty SortingArrowMarginProperty =
			DependencyProperty.Register(nameof(SortingArrowMargin), typeof(Thickness), typeof(SortableGridViewColumnHeader), new PropertyMetadata(default(Thickness)));
	}
}

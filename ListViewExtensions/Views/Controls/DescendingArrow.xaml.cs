using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ListViewExtensions.Views.Controls
{
	public class DescendingArrow : Control
	{
		static DescendingArrow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DescendingArrow), new FrameworkPropertyMetadata(typeof(DescendingArrow)));
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ListViewExtensions.Views.Controls
{
	public class AscendingArrow : Control
	{
		static AscendingArrow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(AscendingArrow), new FrameworkPropertyMetadata(typeof(AscendingArrow)));
		}
	}
}

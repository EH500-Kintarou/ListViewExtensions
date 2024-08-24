using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ListViewExtensions.Views.Controls
{
    public class SortedHeader : ContentControl
    {
		Polygon? AscendingArrow;
		Polygon? DescendingArrow;

		static SortedHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SortedHeader), new FrameworkPropertyMetadata(typeof(SortedHeader)));
        }

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			AscendingArrow = (Polygon)GetTemplateChild("PART_AscendingArrow");
			DescendingArrow = (Polygon)GetTemplateChild("PART_DescendingArrow");

			UpdateArrowVisibility();
		}

		void UpdateArrowVisibility()
		{
			if(AscendingArrow != null && DescendingArrow != null) {
				switch(SortingDirection) {
					case SortingDirection.None:
						AscendingArrow.Visibility = Visibility.Hidden;
						DescendingArrow.Visibility = Visibility.Hidden;
						break;
					case SortingDirection.Ascending:
						AscendingArrow.Visibility = Visibility.Visible;
						DescendingArrow.Visibility = Visibility.Hidden;
						break;
					case SortingDirection.Descending:
						AscendingArrow.Visibility = Visibility.Hidden;
						DescendingArrow.Visibility = Visibility.Visible;
						break;
				}
			}
		}

		/// <summary>
		/// ソートの方向
		/// </summary>
		public SortingDirection SortingDirection
		{
			get { return (SortingDirection)GetValue(SortingDirectionProperty); }
			set { SetValue(SortingDirectionProperty, value); }
		}
		public static readonly DependencyProperty SortingDirectionProperty = DependencyProperty.Register(nameof(SortingDirection), typeof(SortingDirection), typeof(SortedHeader), new UIPropertyMetadata(SortingDirection.None, (d, e)=> {
			var me = (SortedHeader)d;

			me.UpdateArrowVisibility();
		}));
	}
}

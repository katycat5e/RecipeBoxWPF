using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace RecipeBox3
{
    public class ExtendedMenuItem : MenuItem
    {
        public Brush SubMenuBrush
        {
            get { return (Brush)GetValue(SubMenuBrushProperty); }
            set { SetValue(SubMenuBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SubMenuBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubMenuBrushProperty =
            DependencyProperty.Register("SubMenuBrush", typeof(Brush), typeof(ExtendedMenuItem), new PropertyMetadata(SystemColors.ControlBrush));
    }
}

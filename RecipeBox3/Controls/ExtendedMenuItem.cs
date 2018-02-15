using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace RecipeBox3.Controls
{
    /// <summary>Extension of the MenuItem class to allow flat styling</summary>
    public class ExtendedMenuItem : MenuItem
    {
        /// <summary>Brush for the background of the submenu in this item</summary>
        public Brush SubMenuBrush
        {
            get { return (Brush)GetValue(SubMenuBrushProperty); }
            set { SetValue(SubMenuBrushProperty, value); }
        }

        /// <summary>Brush for the background of the submenu in this item</summary>
        public static readonly DependencyProperty SubMenuBrushProperty =
            DependencyProperty.Register("SubMenuBrush", typeof(Brush), typeof(ExtendedMenuItem), new PropertyMetadata(SystemColors.ControlBrush));
    }
}

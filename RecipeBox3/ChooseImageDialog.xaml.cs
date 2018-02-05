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
using System.Windows.Shapes;

namespace RecipeBox3
{
    /// <summary>
    /// Interaction logic for ChooseImageDialog.xaml
    /// </summary>
    public partial class ChooseImageDialog : Window
    {
        public ChooseImageDialog()
        {
            InitializeComponent();
        }
    }

    public class ImageEditorViewModel : DependencyObject
    {
        public System.Drawing.Bitmap ImageData
        {
            get { return (System.Drawing.Bitmap)GetValue(ImageDataProperty); }
            set { SetValue(ImageDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageDataProperty =
            DependencyProperty.Register("ImageData", typeof(System.Drawing.Bitmap), typeof(ImageEditorViewModel), new PropertyMetadata(null));


    }
}

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

namespace RecipeBox3.Controls
{
    /// <summary>
    /// Interaction logic for NumericEntryBox.xaml
    /// </summary>
    public partial class NumericEntryBox : UserControl
    {
        /// <summary>
        /// Create a new NumericEntryBox
        /// </summary>
        public NumericEntryBox()
        {
            InitializeComponent();
        }

        private void InputBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        /// <summary>Minimum value accepted for this input</summary>
        public double MinimumValue
        {
            get { return (double)GetValue(MinimumValueProperty); }
            set { SetValue(MinimumValueProperty, value); }
        }

        /// <summary>Minimum value accepted for this input</summary>
        public static readonly DependencyProperty MinimumValueProperty =
            DependencyProperty.Register("MinimumValue", typeof(double), typeof(NumericEntryBox), new PropertyMetadata(0.0));


        /// <summary>Maximum value accepted for this input</summary>
        public double MaximumValue
        {
            get { return (double)GetValue(MaximumValueProperty); }
            set { SetValue(MaximumValueProperty, value); }
        }

        /// <summary>Maximum value accepted for this input</summary>
        public static readonly DependencyProperty MaximumValueProperty =
            DependencyProperty.Register("MaximumValue", typeof(double), typeof(NumericEntryBox), new PropertyMetadata(100.0));


    }
}

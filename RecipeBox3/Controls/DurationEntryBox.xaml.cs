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
    /// Interaction logic for DurationEntryBox.xaml
    /// </summary>
    public partial class DurationEntryBox : UserControl
    {
        /// <summary>
        /// Create a new NumericEntryBox
        /// </summary>
        public DurationEntryBox()
        {
            InitializeComponent();
        }

        /// <summary>Minimum value accepted for this input</summary>
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>Minimum value accepted for this input</summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(DurationEntryBox), new PropertyMetadata(0.0));


        /// <summary>Maximum value accepted for this input</summary>
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>Maximum value accepted for this input</summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(DurationEntryBox), new PropertyMetadata(100.0));


        /// <summary>Current numeric value of the input</summary>
        public double Value
        {
            get { return 0; }
            set {  }
        }

        private void InputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string curValueString = InputBox.Text;
        }

        static bool TryGetMinutesFromTimeString(string timeString, out int minutes)
        {
            minutes = 0;
            if (timeString == null) return false;

            var partList = new List<TimeStringPart>(1);
            var sb = new StringBuilder();
            char[] inChars = timeString.Trim().ToArray();

            if (inChars.Length > 0)
            {
                bool lastCharNumeric = Char.IsDigit(inChars[0]);

                for (int i = 0; i < inChars.Length; i++)
                {
                    sb.Append(inChars[i]);
                    if (Char.IsDigit(inChars[i]) != lastCharNumeric)
                    {
                        partList.Add(
                            new TimeStringPart()
                            {
                                IsNumeric = lastCharNumeric,
                                Text = sb.ToString()
                            });
                    }
                }
            }

            return false;
        }

        private struct TimeStringPart
        {
            public bool IsNumeric;
            public string Text;
        }
    }
}

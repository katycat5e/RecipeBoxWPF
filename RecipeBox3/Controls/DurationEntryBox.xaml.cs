using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

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


        /// <summary>Whether the current input of this control is a valid duration string</summary>
        public bool ValidInput
        {
            get { return (bool)GetValue(ValidInputProperty); }
            set { SetValue(ValidInputProperty, value); }
        }

        /// <summary>Whether the current input of this control is a valid duration string</summary>
        public static readonly DependencyProperty ValidInputProperty =
            DependencyProperty.Register("ValidInput", typeof(bool), typeof(DurationEntryBox), new PropertyMetadata(true));


        /// <summary>Current numeric value of the input</summary>
        public int MinuteValue
        {
            get { return (int)GetValue(MinuteValueProperty); }
            set { SetValue(MinuteValueProperty, value); }
        }

        /// <summary>Current numeric value of the input</summary>
        public static readonly DependencyProperty MinuteValueProperty =
            DependencyProperty.Register("MinuteValue", typeof(int), typeof(DurationEntryBox), new PropertyMetadata(0));


        /// <summary>Outline color to use when input is a valid duration</summary>
        public Color ValidColor
        {
            get { return (Color)GetValue(ValidColorProperty); }
            set { SetValue(ValidColorProperty, value); }
        }

        /// <summary>Outline color to use when input is a valid duration</summary>
        public static readonly DependencyProperty ValidColorProperty =
            DependencyProperty.Register("ValidColor", typeof(Color), typeof(DurationEntryBox), new PropertyMetadata(Colors.Transparent));


        /// <summary>Outline color to use when input is an invalid duration</summary>
        public Color InvalidColor
        {
            get { return (Color)GetValue(InvalidColorProperty); }
            set { SetValue(InvalidColorProperty, value); }
        }

        /// <summary>Outline color to use when input is an invalid duration</summary>
        public static readonly DependencyProperty InvalidColorProperty =
            DependencyProperty.Register("InvalidColor", typeof(Color), typeof(DurationEntryBox), new PropertyMetadata(Colors.DarkRed));


        /// <inheritdoc/>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property.Name == "MinuteValue" && e.NewValue is int newVal)
            {
                InputBox.Text = TimeStringConverter.ConvertMinutesToString(newVal, true);
            }
        }

        private void InputBox_LostFocus(object sender, RoutedEventArgs e)
        {
            bool validInput = TimeStringConverter.TryGetMinutes(InputBox.Text, out int minutes);
            if (validInput) MinuteValue = minutes;

            ValidInput = validInput;
        }
    }

    /// <summary>Turns a boolean into a valid/invalid indication color</summary>
    public class ValidValueBrushConverter : DependencyObject, IValueConverter
    {
        /// <summary>Color given when input value is true</summary>
        public Color TrueColor
        {
            get { return (Color)GetValue(TrueColorProperty); }
            set { SetValue(TrueColorProperty, value); }
        }

        /// <summary>Color given when input value is true</summary>
        public static readonly DependencyProperty TrueColorProperty =
            DependencyProperty.Register("TrueColor", typeof(Color), typeof(ValidValueBrushConverter), new PropertyMetadata(Colors.Transparent));


        /// <summary>Color given when input value is false</summary>
        public Color FalseColor
        {
            get { return (Color)GetValue(FalseColorProperty); }
            set { SetValue(FalseColorProperty, value); }
        }

        /// <summary>Color given when input value is false</summary>
        public static readonly DependencyProperty FalseColorProperty =
            DependencyProperty.Register("FalseColor", typeof(Color), typeof(ValidValueBrushConverter), new PropertyMetadata(Colors.Red));



        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue == true) return TrueColor;
            else return FalseColor;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

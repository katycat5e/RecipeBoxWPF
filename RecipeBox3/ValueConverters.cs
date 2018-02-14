using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3
{
    public class ByteImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte[] data)
            {
                return ConvertBytesToBitmapImage(data);
            }
            else return null;
        }

        public static BitmapImage ConvertBytesToBitmapImage(byte[] data)
        {
            var ms = new MemoryStream(data);
            var newImage = new BitmapImage();
            newImage.BeginInit();
            newImage.StreamSource = ms;
            newImage.EndInit();

            return newImage;
        }

        public static byte[] ConvertBitmapImageToBytes(BitmapImage image)
        {
            byte[] buffer = null;
            Stream stream = image.StreamSource;

            if (stream != null && stream.Length > 0)
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    buffer = br.ReadBytes((int)stream.Length);
                }
            }

            return buffer;
        }

        public static System.Drawing.Bitmap ConvertBytesToBitmap(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                var output = System.Drawing.Image.FromStream(ms);
                return new System.Drawing.Bitmap(output);
            }
        }

        public static byte[] ConvertBitmapToBytes(System.Drawing.Bitmap bitmap)
        {
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Converter for turning minute durations into string representations and vice versa
    /// </summary>
    public class TimeStringConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int timeMin)
            {
                return ConvertMinutesToString(timeMin, true);
            }
            else return null;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string timeString)
            {
                TryGetMinutes(timeString, out int minutes);
                return minutes;
            }
            else return null;
        }

        private const string HOUR_PATTERN = @"\b(\d+)\s*(?:hours?|hrs?|h)\b";
        private const string MIN_PATTERN = @"\b(\d+)\s*(?:minutes?|mins?|m)\b";

        /// <summary>Try to convert a time string to a number of minutes</summary>
        /// <param name="timeString">Input string to parse</param>
        /// <param name="minutes">Variable to hold the output</param>
        /// <returns>True if the string contained a valid time amount</returns>
        public static bool TryGetMinutes(string timeString, out int minutes)
        {
            minutes = 0;
            if (timeString == null) return false;

            bool validAmount = false;

            var hourMatch = Regex.Match(timeString, HOUR_PATTERN);
            var minMatch = Regex.Match(timeString, MIN_PATTERN);

            if (hourMatch.Success && hourMatch.Groups.Count >= 2)
            {
                if (Int32.TryParse(hourMatch.Groups[1].Value, out int hourAmount))
                {
                    minutes += hourAmount * 60;
                    validAmount |= true;
                }
            }

            if (minMatch.Success && minMatch.Groups.Count >= 2)
            {
                if (Int32.TryParse(minMatch.Groups[1].Value, out int minAmount))
                {
                    minutes += minAmount;
                    validAmount |= true;
                }
            }

            return validAmount;
        }

        /// <summary>Convert a minutes amount to hour + minute string representation</summary>
        /// <param name="minutes">Duration in minutes to convert</param>
        /// <param name="abbreviated">Whether to abbreviate the time units</param>
        /// <returns></returns>
        public static string ConvertMinutesToString(int minutes, bool abbreviated = false)
        {
            string output = "";

            if (minutes >= 60)
            {
                int hourAmount = minutes / 60;
                minutes %= 60;

                output = String.Format("{0} {1}{2} ",
                    hourAmount,
                    (abbreviated) ? "hr" : "hour",
                    (hourAmount != 1 && !abbreviated) ? "s" : null);
            }

            return output + String.Format("{0:0} {1}{2}",
                minutes,
                (abbreviated) ? "min" : "minute",
                (minutes != 1 && !abbreviated) ? "s" : null);
        }
    }

    public class AmountStringConverter : DependencyObject, IMultiValueConverter
    {
        public UnitManager UnitManager
        {
            get { return (UnitManager)GetValue(UnitManagerProperty); }
            set { SetValue(UnitManagerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnitManager.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitManagerProperty =
            DependencyProperty.Register("UnitManager", typeof(UnitManager), typeof(AmountStringConverter), new PropertyMetadata(null));



        public Unit.System UnitSystem
        {
            get { return (Unit.System)GetValue(UnitSystemProperty); }
            set { SetValue(UnitSystemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnitSystem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitSystemProperty =
            DependencyProperty.Register("UnitSystem", typeof(Unit.System), typeof(AmountStringConverter), new PropertyMetadata(Unit.System.Customary));



        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string outputString = null;
            if (values.Length < 2) return null;

            if (values[0] is int unitID && values[1] is decimal amount)
            {
                return UnitManager.GetString(amount, unitID, UnitSystem);
            }

            return outputString;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            if (value is string inputString)
            {
                bool successful = UnitManager.TryParseUnitString(inputString, out int unitID, out decimal amount);
                if (successful)
                {
                    return new object[] { unitID, amount };
                }
            }

            return null;
        }
    }
}

using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3
{
    /// <summary>Class for converting between byte arrays and Image objects</summary>
    public class ByteImageConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte[] data)
            {
                return ConvertBytesToBitmapImage(data);
            }
            else return null;
        }

        /// <summary>Convert a byte array of image data to a BitmapImage</summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BitmapImage ConvertBytesToBitmapImage(byte[] data)
        {
            var ms = new MemoryStream(data);
            var newImage = new BitmapImage();
            newImage.BeginInit();
            newImage.StreamSource = ms;
            newImage.EndInit();

            return newImage;
        }

        /// <summary>Convert a BitmapImage to a byte array</summary>
        /// <param name="image"></param>
        /// <returns></returns>
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

        /// <summary>Convert a byte array of image data to a Bitmap</summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static System.Drawing.Bitmap ConvertBytesToBitmap(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                var output = System.Drawing.Image.FromStream(ms);
                return new System.Drawing.Bitmap(output);
            }
        }

        /// <summary>Convert a Bitmap to a byte array</summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static byte[] ConvertBitmapToBytes(System.Drawing.Bitmap bitmap)
        {
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        /// <inheritdoc/>
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

    /// <summary>Class for converting between unit/amount data and a string representation</summary>
    public class AmountStringConverter : DependencyObject, IMultiValueConverter
    {
        /// <summary>The UnitManager to handle the conversions</summary>
        public UnitManager UnitManager
        {
            get { return (UnitManager)GetValue(UnitManagerProperty); }
            set { SetValue(UnitManagerProperty, value); }
        }

        /// <summary>Backing for <see cref="UnitManager"/></summary>
        public static readonly DependencyProperty UnitManagerProperty =
            DependencyProperty.Register("UnitManager", typeof(UnitManager), typeof(AmountStringConverter), new PropertyMetadata(null));


        /// <summary>The Unit system to use for output strings</summary>
        public Unit.System UnitSystem
        {
            get { return (Unit.System)GetValue(UnitSystemProperty); }
            set { SetValue(UnitSystemProperty, value); }
        }

        /// <summary>Backing for <see cref="UnitSystem"/></summary>
        public static readonly DependencyProperty UnitSystemProperty =
            DependencyProperty.Register("UnitSystem", typeof(Unit.System), typeof(AmountStringConverter), new PropertyMetadata(Unit.System.Customary));


        /// <summary>Convert a unitID and amount to a string representation</summary>
        /// <param name="values">Array containing int unitID at index 0 and decimal amount at index 1</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>String representation or null if conversion failed</returns>
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

        /// <summary>Convert a string into a unit and amount</summary>
        /// <param name="value">String containing an amount and unit</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>
        /// Array containing the unit ID at index 0 and amount at index 1,
        /// or null if conversion failed
        /// </returns>
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

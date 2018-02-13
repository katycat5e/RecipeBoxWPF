using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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

    public class TimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int timeMin)
            {
                if (timeMin > 60)
                {
                    int timeHr = timeMin / 60;
                    timeMin = timeMin % 60;
                    return String.Format("{0} hrs {1} min", timeHr, timeMin);
                }
                else
                {
                    return String.Format("{0} min", timeMin);
                }
            }
            else return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
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

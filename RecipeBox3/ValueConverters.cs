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

namespace RecipeBox3
{
    public class ByteImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte[] data)
            {
                return ConvertBytesToBitmap(data);
            }
            else return null;
        }

        public BitmapImage ConvertBytesToBitmap(byte[] data)
        {
            BitmapImage output = new BitmapImage();
            using (var ms = new MemoryStream(data))
            {
                output.StreamSource = ms;
            }

            return output;
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
}

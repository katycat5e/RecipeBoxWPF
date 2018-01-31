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
    class ByteImageConverter : IValueConverter
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
}

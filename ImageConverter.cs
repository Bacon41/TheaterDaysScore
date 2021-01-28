using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheaterDaysScore {
    class ImageConverter : IValueConverter {
        public static ImageConverter Instance = new ImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null)
                return null;
            
            if (value is string && targetType == typeof(IImage)) {
                string cardLoc = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MirishitaScore", "cards", value + ".png");
                if (File.Exists(cardLoc)) {
                    return new Bitmap(cardLoc);
                }
                return null;
            }

            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}

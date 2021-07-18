using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Globalization;
using System.IO;
using TheaterDaysScore.Services;

namespace TheaterDaysScore {
    class ImageConverter : IValueConverter {
        public static ImageConverter Instance = new ImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null)
                return null;
            
            if (value is string && targetType == typeof(IImage)) {
                string cardLoc = Database.DB.CardImagePath(value.ToString());
                if (File.Exists(cardLoc)) {
                    // This is really bad and should be way safer and less infinite loop-y
                    Bitmap b = null;
                    do {
                        try {
                            b = new Bitmap(cardLoc);
                        } catch (IOException) {
                            // For now, just assume it's waiting to be written and retry
                        } catch (ArgumentException) {
                            return null;
                        }
                    } while (b == null);

                    return b;
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

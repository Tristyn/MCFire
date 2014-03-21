using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MCFire.Modules.Files.Converters
{
    public class NullableBoolToPathConverter : IValueConverter
    {
        public Path TruePath { get; set; }
        public Path FalsePath { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = value as bool?;

            if (state == true) return TruePath;
            if (state == false) return FalsePath;
            return new Path {Data = Geometry.Empty};
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

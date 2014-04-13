using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using MCFire.Modules.WorldExplorer.Models;

namespace MCFire.Modules.Infrastructure.Converters
{
    [ValueConversion(typeof(object), typeof(string))]
    public class InstallationToImageUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return @"pack://application:,,,/MCFire;component/Resources/UnknownBlock.png";
            var type = value.GetType();

            if (type.IsAssignableFrom(typeof(GameInstallation)))
                return @"pack://application:,,,/MCFire;component/Resources/Grass.png";
            if (type.IsAssignableFrom(typeof(ServerInstallation)))
                return @"pack://application:,,,/MCFire;component/Resources/Egg.png";
            return @"pack://application:,,,/MCFire;component/Resources/UnknownBlock.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

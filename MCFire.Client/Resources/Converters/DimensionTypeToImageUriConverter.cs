using System;
using System.Windows.Data;
using MCFire.Client.Primitives;

namespace MCFire.Client.Resources.Converters
{
    [ValueConversion(typeof(DimensionType), typeof(string))]
    public class DimensionTypeToImageUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return @"pack://application:,,,/MCFire.Client;component/Resources/UnknownBlock.png";
            switch ((DimensionType)value)
            {
                case DimensionType.Overworld:
                    return @"pack://application:,,,/MCFire.Client;component/Resources/Grass.png";
                case DimensionType.Nether:
                    return @"pack://application:,,,/MCFire.Client;component/Resources/NetherQuartzOre.png";
                case DimensionType.End:
                    return @"pack://application:,,,/MCFire.Client;component/Resources/DragonEgg.png";
                default:
                    return @"pack://application:,,,/MCFire.Client;component/Resources/UnknownBlock.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

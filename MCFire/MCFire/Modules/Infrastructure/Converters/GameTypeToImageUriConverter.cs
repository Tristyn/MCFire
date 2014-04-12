using System;
using System.Windows.Data;
using Substrate;

namespace MCFire.Modules.Infrastructure.Converters
{
    [ValueConversion(typeof(GameType), typeof(string))]
    public class GameTypeToImageUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((GameType)value)
            {
                case GameType.SURVIVAL:
                    return @"/MCFire;component/Resources/CraftingTable.png";
                case GameType.CREATIVE:
                    return @"/MCFire;component/Resources/CommandBlock.png";
                default:
                    return @"/MCFire;component/Resources/UnknownBlock.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

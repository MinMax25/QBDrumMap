using System.Globalization;
using System.Windows.Data;
using QBDrumMap.Class.Extentions;

namespace QBDrumMap.Class.ValueConverters
{
    public class GetPluginNameConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (UtilMap.MapData == null) return string.Empty;
            if (value is not int i) return string.Empty;
            return UtilMap.MapData.Plugins.FirstOrDefault(x => x.Kits.Any(k => k.ID == i))?.Name ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

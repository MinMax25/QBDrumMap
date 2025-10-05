using System.Globalization;
using System.Windows.Data;
using QBDrumMap.Class.Extentions;

namespace QBDrumMap.Class.ValueConverters
{
    public class GetPartNameConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not int i) return string.Empty;
            return UtilMap.MapData.Parts.FirstOrDefault(x => x.Articulations.Any(a => a.ID == i))?.Name ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

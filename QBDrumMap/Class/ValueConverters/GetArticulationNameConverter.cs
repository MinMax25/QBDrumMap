using System.Globalization;
using System.Windows.Data;
using QBDrumMap.Class.Extentions;

namespace QBDrumMap.Class.ValueConverters
{
    public class GetArticulationNameConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (UtilMap.MapData == null) return string.Empty;
            if (value is not int i) return string.Empty;
            return UtilMap.MapData.Parts.SelectMany(x => x.Articulations).FirstOrDefault(x => x.ID == i)?.Name ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

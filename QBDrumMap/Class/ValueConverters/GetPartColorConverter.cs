using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using QBDrumMap.Class.Extentions;
using QBDrumMap.Class.StudioOne;

namespace QBDrumMap.Class.ValueConverters
{
    public class GetPartColorConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not int i) return string.Empty;
            var col = UtilMap.MapData.Parts.FirstOrDefault(x => x.Articulations.Any(a => a.ID == i))?.Color ?? null;
            return new SolidColorBrush(StudioOneColorConverter.GetStudioOneColor(col));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
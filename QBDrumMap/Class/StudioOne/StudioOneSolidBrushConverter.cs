using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QBDrumMap.Class.StudioOne
{
    public class StudioOneSolidBrushConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush(StudioOneColorConverter.GetStudioOneColor(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

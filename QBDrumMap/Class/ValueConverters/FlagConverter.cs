#nullable enable

using System.Globalization;
using System.Windows.Data;

namespace QBDrumMap.Class.ValueConverters;

public sealed class FlagConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is string cultureName
            ? FlagImageProvider.Resolve(cultureName)
            : null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

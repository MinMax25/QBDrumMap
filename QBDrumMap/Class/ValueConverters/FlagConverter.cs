#nullable enable

using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace QBDrumMap.Class.ValueConverters;

public sealed class FlagConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string cultureName || FlagImageProvider.Resolve(cultureName) is not string path)
        {
            return null;
        }

        var image = new BitmapImage();
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.UriSource = new Uri(path, UriKind.Absolute);
        image.EndInit();
        image.Freeze();
        return image;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

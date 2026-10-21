using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QBDrumMap.Class.StudioOne;

public class StudioOneColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return GetStudioOneColor(value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string result = "00000000";

        if (value is Color color)
        {
            result =
                System.Convert.ToHexString([color.A]) +
                System.Convert.ToHexString([color.B]) +
                System.Convert.ToHexString([color.G]) +
                System.Convert.ToHexString([color.R]);
        }

        return result;
    }

    public static Color GetStudioOneColor(object value)
    {
        string strColor = "00000000";
        string rawValue = $"{value}";

        if (ulong.TryParse(rawValue, NumberStyles.HexNumber, null, out _))
        {
            strColor = rawValue.PadLeft(8, '0');
        }

        string strA = strColor.Substring(0, 2);
        string strB = strColor.Substring(2, 2);
        string strG = strColor.Substring(4, 2);
        string strR = strColor.Substring(6, 2);

        Color color = new()
        {
            A = byte.Parse(strA, NumberStyles.HexNumber),
            B = byte.Parse(strB, NumberStyles.HexNumber),
            G = byte.Parse(strG, NumberStyles.HexNumber),
            R = byte.Parse(strR, NumberStyles.HexNumber)
        };

        return color;
    }
}
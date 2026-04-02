using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QBDrumMap.Class.StudioOne
{
    public class StudioOneColorConverter : IValueConverter
    {
        #region Methods

        #region IValueConverter Implementation

        // Studio One形式の16進数文字列をWPFのColorオブジェクトに変換
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetStudioOneColor(value);
        }

        // WPFのColorオブジェクトをStudio One形式の16進数文字列(ABGR)に変換
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "00000000";

            if (value is Color color)
            {
                // Studio Oneは ABGR の順序で16進数文字列を保持する
                result =
                    System.Convert.ToHexString([color.A]) +
                    System.Convert.ToHexString([color.B]) +
                    System.Convert.ToHexString([color.G]) +
                    System.Convert.ToHexString([color.R]);
            }

            return result;
        }

        #endregion

        #region General

        // 文字列または数値からStudio One仕様のColor構造体を生成
        public static Color GetStudioOneColor(object value)
        {
            string strColor = "00000000";
            string rawValue = $"{value}";

            if (ulong.TryParse(rawValue, NumberStyles.HexNumber, null, out _))
            {
                strColor = rawValue.PadLeft(8, '0');
            }

            // Studio Oneの色の並びは A-B-G-R
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

        #endregion

        #endregion
    }
}
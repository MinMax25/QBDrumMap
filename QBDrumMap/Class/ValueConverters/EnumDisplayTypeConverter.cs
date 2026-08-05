#nullable enable

using System.ComponentModel;
using System.Globalization;

namespace libQB.ValueConverters;

// libQB 1.xで提供されていたenum表示用コンバーターとの互換層。
// QBDrumMapのenumは表示属性を持たないため、列挙子名をそのまま表示する。
public sealed class EnumDisplayTypeConverter : EnumConverter
{
    public EnumDisplayTypeConverter(Type type)
        : base(type)
    {
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        return destinationType == typeof(string) && value != null
            ? value.ToString()
            : base.ConvertTo(context, culture, value, destinationType);
    }
}

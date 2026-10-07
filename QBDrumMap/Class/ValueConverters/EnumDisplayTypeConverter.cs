#nullable enable

using System.ComponentModel;
using System.Globalization;

namespace libQB.ValueConverters;

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

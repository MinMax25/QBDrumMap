using System.ComponentModel;
using System.Text.Json.Serialization;
using libQB.ValueConverters;

namespace QBDrumMap.Class.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter<MIDIFormat>))]
    [TypeConverter(typeof(EnumDisplayTypeConverter))]
    public enum MIDIFormat
    {
        Format0 = 0,
        Format1,
    }
}

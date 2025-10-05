using System.ComponentModel;
using System.Text.Json.Serialization;
using libQB.ValueConverters;

namespace QBDrumMap.Class.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter<MIDIChannel>))]
    [TypeConverter(typeof(EnumDisplayTypeConverter))]
    public enum MIDIChannel
    {
        Channel1 = 1,
        Channel2,
        Channel3,
        Channel4,
        Channel5,
        Channel6,
        Channel7,
        Channel8,
        Channel9,
        Channel10,
        Channel11,
        Channel12,
        Channel13,
        Channel14,
        Channel15,
        Channel16
    }
}
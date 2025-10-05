using System.ComponentModel;
using System.Text.Json.Serialization;
using libQB.ValueConverters;

namespace QBDrumMap.Class.Cubase
{
    [JsonConverter(typeof(JsonStringEnumConverter<CubaseVoiceType>))]
    [TypeConverter(typeof(EnumDisplayTypeConverter))]
    public enum CubaseVoiceType
    {
        Up1 = 0,
        Down1,
        Up2,
        Down2,
    }
}

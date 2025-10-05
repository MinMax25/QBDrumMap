using System.ComponentModel;
using System.Text.Json.Serialization;
using libQB.ValueConverters;

namespace QBDrumMap.Class.MapModels
{
    [JsonConverter(typeof(JsonStringEnumConverter<PluginType>))]
    [TypeConverter(typeof(EnumDisplayTypeConverter))]
    public enum PluginType
    {
        None,
        GM,
        GM2,
        GS,
        XG
    }
}

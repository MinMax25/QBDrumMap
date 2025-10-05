using System.Text.Json.Serialization;

namespace QBDrumMap.Class.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter<Pages>))]
    public enum Pages
    {
        PluginPage,
        PartsPage,
        ArticulationsPage,
        ExportPage,
        ConvertMidiPage
    }
}

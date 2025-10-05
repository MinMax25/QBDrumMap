using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using libQB.ValueConverters;

namespace QBDrumMap.Class.MapModels
{
    [JsonConverter(typeof(JsonStringEnumConverter<Separator>))]
    [TypeConverter(typeof(EnumDisplayTypeConverter))]
    public enum Separator
    {
        [Display(Name = "")]
        None = 0,

        [Display(Name = "Separate")]
        Separate,

        [Display(Name = "Not Loaded")]
        Unload,

        [Display(Name = "Duplicate")]
        Duplicate,
    }
}

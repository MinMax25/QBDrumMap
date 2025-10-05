using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using libQB.ValueConverters;

namespace QBDrumMap.Class.StudioOne
{
    [JsonConverter(typeof(JsonStringEnumConverter<StudioOneNoteHead>))]
    [TypeConverter(typeof(EnumDisplayTypeConverter))]
    public enum StudioOneNoteHead
    {
        [Display(Name = "")]
        None = 0,

        [Display(Name = "X")]
        nhdX,

        [Display(Name = "Triangle")]
        nhTr,

        [Display(Name = "Slash")]
        nhSl,

        [Display(Name = "Circle in X")]
        nhCX,

        [Display(Name = "Diamond")]
        nhDi
    }
}

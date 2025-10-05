using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using libQB.ValueConverters;

namespace QBDrumMap.Class.StudioOne
{
    [JsonConverter(typeof(JsonStringEnumConverter<StudioOneTechnique>))]
    [TypeConverter(typeof(EnumDisplayTypeConverter))]
    public enum StudioOneTechnique
    {
        [Display(Name = "")]
        None = 0,

        [Display(Name = "Open")]
        circ,

        [Display(Name = "Half Open")]
        ccir,

        [Display(Name = "Close")]
        plus,

        [Display(Name = "Roll 1")]
        trm1,

        [Display(Name = "Roll 2")]
        trm2,

        [Display(Name = "Roll 3")]
        trm3,

        [Display(Name = "Cymbal Choke")]
        stac
    }
}
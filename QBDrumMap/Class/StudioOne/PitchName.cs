using System.Xml.Serialization;

namespace QBDrumMap.Class.StudioOne
{
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class PitchName
    {
        [XmlAttribute("pitch")]
        public byte Pitch { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; } = string.Empty;

        [XmlAttribute("color")]
        public string Color { get; set; } = string.Empty;

        [XmlAttribute("scorePitch")]
        public string ScorePitch { get; set; } = string.Empty;

        [XmlAttribute("notehead")]
        public string Notehead { get; set; } = string.Empty;

        [XmlAttribute("technique")]
        public string Technique { get; set; } = string.Empty;

        [XmlAttribute("flags")]
        public string Flags { get; set; } = string.Empty;

        public override string ToString()
        {
            var fields = GetType().GetProperties().Select(x => new { name = x.Name, value = x.GetValue(this) });
            return string.Join(", ", fields.Select(x => $"{x.name} = {x.value}").ToArray());
        }
    }
}

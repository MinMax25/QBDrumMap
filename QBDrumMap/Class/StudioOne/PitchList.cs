using System.IO;
using System.Text;
using System.Xml.Serialization;
using QBDrumMap.Class.Extentions;

namespace QBDrumMap.Class.StudioOne
{
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot("Music.PitchNameList", Namespace = "", IsNullable = false)]
    public partial class PitchList
    {
        [XmlElement("Music.PitchName")]
        public List<PitchName> Items { get; set; } = [];

        [XmlAttribute("title")]
        public string Title { get; set; } = string.Empty;

        public void Save(string path)
        {
            StringBuilder sb = new();

            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine($"<Music.PitchNameList title=\"{Title}\">");

            foreach (var item in Items)
            {
                sb.Append("\t<Music.PitchName");
                sb.Append($" pitch=\"{item.Pitch}\"");
                sb.Append($" name=\"{EscXML(item.Name)}\"");
                if (!string.IsNullOrWhiteSpace(item.Color)) sb.Append($" color=\"{item.Color}\"");
                if (!string.IsNullOrWhiteSpace(item.ScorePitch)) sb.Append($" scorePitch=\"{item.ScorePitch}\"");
                if (!string.IsNullOrWhiteSpace(item.Notehead)) sb.Append($" notehead=\"{item.Notehead}\"");
                if (!string.IsNullOrWhiteSpace(item.Technique)) sb.Append($" technique=\"{item.Technique}\"");
                sb.AppendLine("/>");
            }

            sb.AppendLine("</Music.PitchNameList>");

            File.WriteAllText(path.ToSafeFilePath(), sb.ToString(), Encoding.UTF8);
        }

        public static string EscXML(string str)
        {
            return
                $"{str}".
                    Replace("&", "&amp;").
                    Replace("\"", "&quot;").
                    Replace("'", "&apos;").
                    Replace("<", "&lt;").
                    Replace(">", "&gt;");
        }
    }
}

using System.IO;
using System.Xml.Serialization;

namespace QBDrumMap.Class.StudioOne
{
    public static class StudioOne
    {
        public static PitchList Load(string filepath)
        {
            var xml = new XmlSerializer(typeof(PitchList));
            using StreamReader sr = new(filepath);

            if (xml.Deserialize(sr) is PitchList pitchlist)
            {
                pitchlist.Title = string.IsNullOrWhiteSpace(pitchlist.Title) ? Path.GetFileNameWithoutExtension(filepath) : pitchlist.Title;
                return pitchlist;
            }

            return default;
        }
    }
}

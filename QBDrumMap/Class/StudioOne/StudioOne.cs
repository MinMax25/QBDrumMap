using System.IO;
using System.Xml.Serialization;

namespace QBDrumMap.Class.StudioOne
{
    public static class StudioOne
    {
        #region Methods

        #region General

        // 指定したパスのXMLファイルを読み込み、PitchListオブジェクトを生成
        public static PitchList Load(string filepath)
        {
            var xml = new XmlSerializer(typeof(PitchList));

            using (StreamReader sr = new(filepath))
            {
                if (xml.Deserialize(sr) is PitchList pitchlist)
                {
                    // タイトルが空の場合はファイル名をタイトルとして使用
                    pitchlist.Title = string.IsNullOrWhiteSpace(pitchlist.Title)
                        ? Path.GetFileNameWithoutExtension(filepath)
                        : pitchlist.Title;

                    return pitchlist;
                }
            }

            return default;
        }

        #endregion

        #endregion
    }
}
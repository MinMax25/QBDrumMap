using System.Xml.Serialization;

namespace QBDrumMap.Class.StudioOne
{
    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class PitchName
    {
        #region Properties

        // MIDIノート番号 (0-127)
        [XmlAttribute("pitch")]
        public byte Pitch { get; set; }

        // 表示名
        [XmlAttribute("name")]
        public string Name { get; set; } = string.Empty;

        // 色指定
        [XmlAttribute("color")]
        public string Color { get; set; } = string.Empty;

        // スコア表示時のピッチ
        [XmlAttribute("scorePitch")]
        public string ScorePitch { get; set; } = string.Empty;

        // ノートヘッドの種類
        [XmlAttribute("notehead")]
        public string Notehead { get; set; } = string.Empty;

        // 奏法設定
        [XmlAttribute("technique")]
        public string Technique { get; set; } = string.Empty;

        // 各種フラグ
        [XmlAttribute("flags")]
        public string Flags { get; set; } = string.Empty;

        #endregion

        #region Methods

        #region General

        public override string ToString()
        {
            var fields = GetType().GetProperties().Select(x =>
            {
                return new { name = x.Name, value = x.GetValue(this) };
            });

            return string.Join(", ", fields.Select(x =>
            {
                return $"{x.name} = {x.value}";
            }).ToArray());
        }

        #endregion

        #endregion
    }
}
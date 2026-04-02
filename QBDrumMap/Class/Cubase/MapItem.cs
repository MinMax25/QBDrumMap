namespace QBDrumMap.Class.Cubase
{
    public class MapItem : ListElementItem
    {
        #region Properties

        public int INote { get; set; } = 0;

        public int ONote { get; set; } = 0;

        public int Channel { get; set; } = -1;

        public float Length { get; set; } = 200;

        public int Mute { get; set; } = 0;

        public int DisplayNote { get; set; } = 0;

        public int HeadSymbol { get; set; } = 0;

        public int Voice { get; set; } = 0;

        public int PortIndex { get; set; } = 0;

        public string Name { get; set; } = string.Empty;

        public int QuantizeIndex { get; set; } = 0;

        public int NoteheadSet { get; set; } = 0;

        public string InstrumentEntityID { get; set; } = string.Empty;

        public string TechniqueEntityID { get; set; } = string.Empty;

        #endregion

        #region ctor

        public MapItem()
        {
        }

        public MapItem(string name, int inote)
        {
            Name = name;
            INote = inote;
            ONote = inote;
            DisplayNote = inote;
        }

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
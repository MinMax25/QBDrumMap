namespace QBDrumMap.Class.Cubase
{
    public class QuantizeItem
        : ListElementItem
    {
        public int Grid { get; set; } = 4;
        public int Type { get; set; } = 0;
        public float Swing { get; set; } = 0;
        public int Legato { get; set; } = 50;
    }
}

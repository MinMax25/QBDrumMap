using libMidi.Messages;

namespace QBDrumMap.Class.MapModels
{
    public record ArticulationMapItem
    {
        public int ID { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsSub { get; set; }

        public int PartID { get; set; }

        public int Order { get; set; }

        public int Complement { get; set; }

        public List<int> Pitches { get; } = [];

        public int Pitch => Pitches.Any() ? Pitches[0] : -1;

        public string Note => Pitch.NoteName();

        public string PitchesText => string.Join(", ", Pitches);
    }
}

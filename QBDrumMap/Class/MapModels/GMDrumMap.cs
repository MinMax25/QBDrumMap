namespace QBDrumMap.Class.MapModels
{
    // GM/GS/XG標準ドラムマップのノート番号定義
    public static class GMDrumMap
    {
        #region Properties

        // ドラムキット本体ではなく、パーカッション用途として標準割り当てされているノート番号
        public static IReadOnlyDictionary<int, string> PercussionNotes { get; } = new Dictionary<int, string>
        {
            [54] = "Tambourine",
            [56] = "Cowbell",
            [58] = "Vibraslap",
            [60] = "Hi Bongo",
            [61] = "Low Bongo",
            [62] = "Mute Hi Conga",
            [63] = "Open Hi Conga",
            [64] = "Low Conga",
            [65] = "High Timbale",
            [66] = "Low Timbale",
            [67] = "High Agogo",
            [68] = "Low Agogo",
            [69] = "Cabasa",
            [70] = "Maracas",
            [71] = "Short Whistle",
            [72] = "Long Whistle",
            [73] = "Short Guiro",
            [74] = "Long Guiro",
            [75] = "Claves",
            [76] = "Hi Wood Block",
            [77] = "Low Wood Block",
            [78] = "Mute Cuica",
            [79] = "Open Cuica",
            [80] = "Mute Triangle",
            [81] = "Open Triangle",
        };

        // ドラムキット本体として標準割り当てされているノート番号（代表ピッチ選定の優先判定に使用）
        public static IReadOnlyDictionary<int, string> StandardDrumNotes { get; } = new Dictionary<int, string>
        {
            [35] = "Acoustic Bass Drum",
            [36] = "Bass Drum 1",
            [37] = "Side Stick",
            [38] = "Acoustic Snare",
            [39] = "Hand Clap",
            [40] = "Electric Snare",
            [41] = "Low Floor Tom",
            [42] = "Closed Hi-Hat",
            [43] = "High Floor Tom",
            [44] = "Pedal Hi-Hat",
            [45] = "Low Tom",
            [46] = "Open Hi-Hat",
            [47] = "Low-Mid Tom",
            [48] = "Hi-Mid Tom",
            [49] = "Crash Cymbal 1",
            [50] = "High Tom",
            [51] = "Ride Cymbal 1",
            [52] = "Chinese Cymbal",
            [53] = "Ride Bell",
            [55] = "Splash Cymbal",
            [57] = "Crash Cymbal 2",
            [59] = "Ride Cymbal 2",
        };

        #endregion

        #region Methods

        // 指定したノート番号がGM標準のパーカッション用途かどうか
        public static bool IsPercussionNote(int pitch)
        {
            return PercussionNotes.ContainsKey(pitch);
        }

        // 指定したノート番号がGM標準のドラムキット用途かどうか
        public static bool IsStandardDrumNote(int pitch)
        {
            return StandardDrumNotes.ContainsKey(pitch);
        }

        #endregion
    }
}

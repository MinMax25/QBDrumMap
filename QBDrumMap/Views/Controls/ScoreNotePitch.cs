namespace QBDrumMap.Views.Controls
{
    public class ScoreNotePitch
    (
        int pitch = 0,
        string note = "",
        bool line1IsVisible = false,
        bool line2IsVisible = false,
        bool line3IsVisible = false,
        bool line4IsVisible = false,
        bool line5IsVisible = false,
        bool line6IsVisible = false
    )
        : IElementVisibility
    {
        #region Properties

        public int Pitch { get; set; } = pitch;

        public string Note { get; set; } = note;

        public bool Line1IsVisible { get; set; } = line1IsVisible;

        public bool Line2IsVisible { get; set; } = line2IsVisible;

        public bool Line3IsVisible { get; set; } = line3IsVisible;

        public bool Line4IsVisible { get; set; } = line4IsVisible;

        public bool Line5IsVisible { get; set; } = line5IsVisible;

        public bool Line6IsVisible { get; set; } = line6IsVisible;

        public bool[] Values =>
        [
            Line1IsVisible,
            Line2IsVisible,
            Line3IsVisible,
            Line4IsVisible,
            Line5IsVisible,
            Line6IsVisible
        ];

        #endregion
    }
}

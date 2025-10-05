using QBDrumMap.Class.StudioOne;

namespace QBDrumMap.Views.Controls
{
    public class NoteHeadVisbilityValue
    (
        StudioOneNoteHead noteHead,
        bool nhNo = false,
        bool nhdX = false,
        bool nhSl = false,
        bool nhCX = false,
        bool nhTr = false,
        bool nhDi = false
    )
        : IElementVisibility
    {
        #region Properties

        public StudioOneNoteHead NoteHead { get; set; } = noteHead;

        public bool NhNo { get; set; } = nhNo;

        public bool NhdX { get; set; } = nhdX;

        public bool NhSl { get; set; } = nhSl;

        public bool NhCX { get; set; } = nhCX;

        public bool NhTr { get; set; } = nhTr;

        public bool NhDi { get; set; } = nhDi;

        public bool[] Values =>
        [
            NhNo,
            NhdX,
            NhSl,
            NhCX,
            NhTr,
            NhDi
        ];

        #endregion
    }
}
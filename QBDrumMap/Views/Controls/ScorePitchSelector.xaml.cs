using System.Windows;
using System.Windows.Controls;
using QBDrumMap.Class.StudioOne;

namespace QBDrumMap.Views.Controls
{
    public partial class ScorePitchSelector
        : UserControl
    {
        private const int NOTE_LEFT_OFFSET_PX = 100;

        private const int NOTE_TOP_OFFSET_PX = 40;
        private const int NOTE_TOP_FREEZE_INDEX = 15;
        private const int NOTE_INTERVAL_PX = 5;

        private readonly UIElement[] _noteElements;
        private readonly UIElement[] _staffElements;
        private readonly UIElement[] _noteHeadElements;
        private readonly UIElement[] _techniqueElements;

        #region DependencyProperties

        public static readonly DependencyProperty ScorePitchProperty =
            DependencyProperty.Register(
                nameof(ScorePitch),
                typeof(int),
                typeof(ScorePitchSelector),
                new PropertyMetadata(0, OnScoreValueChanged)
            );

        public static readonly DependencyProperty NoteHeadProperty =
            DependencyProperty.Register(
                nameof(NoteHead),
                typeof(StudioOneNoteHead),
                typeof(ScorePitchSelector),
                new PropertyMetadata(StudioOneNoteHead.None, OnScoreValueChanged)
            );

        public static readonly DependencyProperty TechniqueProperty =
            DependencyProperty.Register(
                nameof(Technique),
                typeof(StudioOneTechnique),
                typeof(ScorePitchSelector),
                new PropertyMetadata(StudioOneTechnique.None, OnScoreValueChanged)
            );

        public int ScorePitch
        {
            get => (int)GetValue(ScorePitchProperty);
            set => SetValue(ScorePitchProperty, value);
        }

        public StudioOneNoteHead NoteHead
        {
            get => (StudioOneNoteHead)GetValue(NoteHeadProperty);
            set => SetValue(NoteHeadProperty, value);
        }

        public StudioOneTechnique Technique
        {
            get => (StudioOneTechnique)GetValue(TechniqueProperty);
            set => SetValue(TechniqueProperty, value);
        }

        #endregion

        #region ctor

        public ScorePitchSelector()
        {
            InitializeComponent();

            _noteElements = [Note0, Note1];
            _staffElements = [StaffTop1, StaffTop2, StaffTop3, StaffTop4, StaffTop5, StaffTop6];
            _noteHeadElements = [nhNo, nhdX, nhSl, nhCX, nhTr, nhDi];
            _techniqueElements = [circ, ccir, plus, trm1, trm2, trm3, stac];

            HideAllNotesAndStaffs();

            Canvas.SetLeft(Note0, NOTE_LEFT_OFFSET_PX);
            Canvas.SetLeft(Note1, NOTE_LEFT_OFFSET_PX);
        }

        #endregion

        #region Methods

        private static void OnScoreValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScorePitchSelector selector)
            {
                selector.UpdateView();
            }
        }

        private void UpdateView()
        {
            var noteInfo = StudioOneScore.ScoreNotePitches.FirstOrDefault(x => x.Pitch == ScorePitch);

            if (noteInfo == null)
            {
                HideAllNotesAndStaffs();
                return;
            }

            int idx = StudioOneScore.ScoreNotePitches.FindIndex(x => x.Pitch == ScorePitch);
            int top = Math.Min(idx, NOTE_TOP_FREEZE_INDEX);

            SetCanvasTop(Note0, NOTE_TOP_OFFSET_PX + idx * NOTE_INTERVAL_PX);
            SetCanvasTop(Note1, NOTE_TOP_OFFSET_PX + top * NOTE_INTERVAL_PX);

            UpdateStaffLines(noteInfo);
            UpdateNotesVisibility(idx != 0);
            UpdateNoteHead();
            UpdateTechnique();
        }

        private void HideAllNotesAndStaffs()
        {
            foreach (var element in _noteElements.Concat(_staffElements))
            {
                element.SetVisibility(false);
            }
        }

        private void UpdateStaffLines(ScoreNotePitch pitch)
        {
            _staffElements.SetVisibility(pitch);
        }

        private void UpdateNotesVisibility(bool isVisible)
        {
            foreach (var note in _noteElements)
            {
                note.SetVisibility(isVisible);
            }
        }

        private void UpdateNoteHead()
        {
            var head = StudioOneScore.NoteHeadVisibillities.FirstOrDefault(x => x.NoteHead == NoteHead);
            _noteHeadElements.SetVisibility(head);
        }

        private void UpdateTechnique()
        {
            var technique = StudioOneScore.TechniqueVisibillities.FirstOrDefault(x => x.Technique == Technique);
            _techniqueElements.SetVisibility(technique);
        }

        private static void SetCanvasTop(UIElement element, double top) => Canvas.SetTop(element, top);

        #endregion
    }
}

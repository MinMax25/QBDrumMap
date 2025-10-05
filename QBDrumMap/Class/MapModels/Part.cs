using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using QBDrumMap.Class.Cubase;
using QBDrumMap.Class.CustomValidations;
using QBDrumMap.Class.StudioOne;

namespace QBDrumMap.Class.MapModels
{
    public partial class Part
        : ModelBase
        , IHasID
    {
        [ObservableProperty]
        private int iD;

        [ObservableProperty]
        [ValidatePart]
        [NotifyDataErrorInfo]
        private string name = string.Empty;

        [ObservableProperty]
        private int scorePitch;

        #region Studio One

        [ObservableProperty]
        private string color;

        [ObservableProperty]
        private StudioOneNoteHead noteHead;

        [ObservableProperty]
        private StudioOneTechnique technique;

        #endregion

        #region Cubase

        [ObservableProperty]
        private string instrumentEntityID = string.Empty;

        [ObservableProperty]
        private int noteHeadSet;

        [ObservableProperty]
        private CubaseVoiceType voice;

        [ObservableProperty]
        private string techniqueEntityID = string.Empty;

        #endregion

        [ObservableProperty]
        private ObservableCollection<Articulation> articulations = [];

        partial void OnIDChanged(int oldValue, int newValue)
        {
            UndoManager?.RegisterPropertyChange(() => ID, oldValue, newValue);
        }

        partial void OnNameChanged(string oldValue, string newValue)
        {
            UndoManager?.RegisterPropertyChange(() => Name, oldValue, newValue);
        }

        partial void OnScorePitchChanged(int oldValue, int newValue)
        {
            UndoManager?.RegisterPropertyChange(() => ScorePitch, oldValue, newValue);
        }

        partial void OnColorChanged(string oldValue, string newValue)
        {
            UndoManager?.RegisterPropertyChange(() => Color, oldValue, newValue);
        }

        partial void OnNoteHeadChanged(StudioOneNoteHead oldValue, StudioOneNoteHead newValue)
        {
            UndoManager?.RegisterPropertyChange(() => NoteHead, oldValue, newValue);
        }

        partial void OnTechniqueChanged(StudioOneTechnique oldValue, StudioOneTechnique newValue)
        {
            UndoManager?.RegisterPropertyChange(() => Technique, oldValue, newValue);
        }

        partial void OnInstrumentEntityIDChanged(string oldValue, string newValue)
        {
            UndoManager?.RegisterPropertyChange(() => InstrumentEntityID, oldValue, newValue);
        }

        partial void OnVoiceChanged(CubaseVoiceType oldValue, CubaseVoiceType newValue)
        {
            UndoManager?.RegisterPropertyChange(() => Voice, oldValue, newValue);
        }

        partial void OnNoteHeadSetChanged(int oldValue, int newValue)
        {
            UndoManager?.RegisterPropertyChange(() => NoteHeadSet, oldValue, newValue);
        }

        partial void OnTechniqueEntityIDChanged(string oldValue, string newValue)
        {
            UndoManager?.RegisterPropertyChange(() => TechniqueEntityID, oldValue, newValue);
        }

        public override object Clone()
        {
            return new Part
            {
                ID = ID,
                Name = Name,
                ScorePitch = ScorePitch,
                Color = Color,
                NoteHead = NoteHead,
                Technique = Technique,
                InstrumentEntityID = InstrumentEntityID,
                NoteHeadSet = NoteHeadSet,
                Voice = Voice,
                TechniqueEntityID = TechniqueEntityID,
                Articulations = new ObservableCollection<Articulation>(Articulations.Select(articulation => (Articulation)articulation.Clone())),
                DisplayOrder = DisplayOrder
            };
        }
    }
}

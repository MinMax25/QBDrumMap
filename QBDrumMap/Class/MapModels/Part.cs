using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using QBDrumMap.Class.Cubase;
using QBDrumMap.Class.CustomValidations;
using QBDrumMap.Class.StudioOne;

namespace QBDrumMap.Class.MapModels
{
    public partial class Part : ModelBase, IHasID
    {
        #region Fields

        // パートの一意なID
        [ObservableProperty]
        private int iD;

        // パート名
        [ObservableProperty]
        [ValidatePart]
        [NotifyDataErrorInfo]
        private string name = string.Empty;

        // スコア表示時のピッチ
        [ObservableProperty]
        private int scorePitch;

        // Studio One: カラー設定
        [ObservableProperty]
        private string color;

        // Studio One: ノートヘッドの種類
        [ObservableProperty]
        private StudioOneNoteHead noteHead;

        // Studio One: 奏法設定
        [ObservableProperty]
        private StudioOneTechnique technique;

        // Cubase: インストゥルメントエンティティID
        [ObservableProperty]
        private string instrumentEntityID = string.Empty;

        // Cubase: ノートヘッドセット番号
        [ObservableProperty]
        private int noteHeadSet;

        // Cubase: ボイスタイプ
        [ObservableProperty]
        private CubaseVoiceType voice;

        // Cubase: テクニックエンティティID
        [ObservableProperty]
        private string techniqueEntityID = string.Empty;

        // パートに属するアーティキュレーションのコレクション
        [ObservableProperty]
        private ObservableCollection<Articulation> articulations = new();

        #endregion

        #region Methods

        #region Property Change Handler

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

        #endregion

        #region General

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
                Articulations = new ObservableCollection<Articulation>(Articulations.Select(articulation =>
                {
                    return (Articulation)articulation.Clone();
                })),
                DisplayOrder = DisplayOrder
            };
        }

        #endregion

        #endregion
    }
}
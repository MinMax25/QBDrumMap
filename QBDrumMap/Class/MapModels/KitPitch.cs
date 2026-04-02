using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using libQB.UndoRedo;
using QBDrumMap.Class.CustomValidations;

namespace QBDrumMap.Class.MapModels
{
    public partial class KitPitch : ModelBase
    {
        #region Fields

        // MIDIノート番号（ピッチ）
        [ObservableProperty]
        private int pitch;

        // ピッチに対する表示名
        [ObservableProperty]
        [ValidateKitPitch]
        [NotifyDataErrorInfo]
        private string name;

        // 割り当てられたアーティキュレーションのID
        [ObservableProperty]
        private int articulationID;

        // セパレーターの種類
        [ObservableProperty]
        private Separator separator;

        #endregion

        #region Properties

        // ノート名（例: C3）
        [JsonIgnore]
        public string Note
        {
            get
            {
                return libMidi.Messages.Pitch.NoteName(this.Pitch);
            }
        }

        // 名前が空かどうか
        [JsonIgnore]
        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrWhiteSpace(Name);
            }
        }

        #endregion

        #region Methods

        #region Property Change Handler

        partial void OnPitchChanged(int oldValue, int newValue)
        {
            UndoManager?.RegisterPropertyChange(() => Pitch, oldValue, newValue);
        }

        partial void OnNameChanged(string oldValue, string newValue)
        {
            if (UndoManager is not null && !UndoManager.IsExecutingUndoRedo)
            {
                var compound = new CompoundUndoableAction();
                compound.Add(new PropertyChangeAction<string>(() => Name, oldValue, newValue));

                if (string.IsNullOrWhiteSpace(newValue))
                {
                    compound.Add(new PropertyChangeAction<int>(() => ArticulationID, ArticulationID, 0));
                    compound.Add(new PropertyChangeAction<Separator>(() => Separator, Separator, Separator.None));

                    // 値の直接更新
                    ArticulationID = 0;
                    Separator = Separator.None;
                }

                UndoManager.PushAction(compound);
            }

            OnPropertyChanged(nameof(IsEmpty));
        }

        partial void OnArticulationIDChanging(int oldValue, int newValue)
        {
            if (IsEmpty)
            {
                return;
            }
            UndoManager?.RegisterPropertyChange(() => ArticulationID, oldValue, newValue);
        }

        partial void OnSeparatorChanging(Separator oldValue, Separator newValue)
        {
            if (IsEmpty)
            {
                return;
            }
            UndoManager?.RegisterPropertyChange(() => Separator, oldValue, newValue);
        }

        #endregion

        #region General

        public override object Clone()
        {
            return new KitPitch
            {
                Pitch = Pitch,
                Name = Name,
                ArticulationID = ArticulationID,
                Separator = Separator,
                DisplayOrder = DisplayOrder
            };
        }

        #endregion

        #endregion
    }
}
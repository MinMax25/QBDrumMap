using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using libMidi.Messages;
using libQB.UndoRedo;
using QBDrumMap.Class.CustomValidations;

namespace QBDrumMap.Class.MapModels
{
    public partial class KitPitch
        : ModelBase
    {
        [ObservableProperty]
        private int pitch;

        [ObservableProperty]
        [ValidateKitPitch]
        [NotifyDataErrorInfo]
        private string name;

        [ObservableProperty]
        private int articulationID;

        [ObservableProperty]
        private Separator separator;

        [JsonIgnore]
        public string Note => Pitch.NoteName();

        [JsonIgnore]
        public bool IsEmpty => string.IsNullOrWhiteSpace(Name);

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
                    ArticulationID = 0;
                    Separator = Separator.None;
                }

                UndoManager.PushAction(compound);
            }

            OnPropertyChanged(nameof(IsEmpty));
        }

        partial void OnArticulationIDChanging(int oldValue, int newValue)
        {
            if (IsEmpty) return;
            UndoManager?.RegisterPropertyChange(() => ArticulationID, oldValue, newValue);
        }

        partial void OnSeparatorChanging(Separator oldValue, Separator newValue)
        {
            if (IsEmpty) return;
            UndoManager?.RegisterPropertyChange(() => Separator, oldValue, newValue);
        }

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
    }
}

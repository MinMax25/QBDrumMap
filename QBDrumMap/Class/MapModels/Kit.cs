using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using QBDrumMap.Class.CustomValidations;

namespace QBDrumMap.Class.MapModels
{
    public partial class Kit
        : ModelBase
        , IHasID
    {
        [ObservableProperty]
        private int iD;

        [ObservableProperty]
        [ValidateKit]
        [NotifyDataErrorInfo]
        private string name = string.Empty;

        [ObservableProperty]
        private int bankSelectMSB;

        [ObservableProperty]
        private int bankSelectLSB;

        [ObservableProperty]
        private int programNumber;

        [ObservableProperty]
        private ObservableCollection<KitPitch> pitches = [];

        partial void OnIDChanged(int oldValue, int newValue)
        {
            UndoManager?.RegisterPropertyChange(() => ID, oldValue, newValue);
        }

        partial void OnNameChanged(string oldValue, string newValue)
        {
            UndoManager?.RegisterPropertyChange(() => Name, oldValue, newValue);
        }

        partial void OnBankSelectMSBChanged(int oldValue, int newValue)
        {
            UndoManager?.RegisterPropertyChange(() => BankSelectMSB, oldValue, newValue);
        }

        partial void OnBankSelectLSBChanged(int oldValue, int newValue)
        {
            UndoManager?.RegisterPropertyChange(() => BankSelectLSB, oldValue, newValue);
        }

        partial void OnProgramNumberChanged(int oldValue, int newValue)
        {
            UndoManager?.RegisterPropertyChange(() => ProgramNumber, oldValue, newValue);
        }

        public override object Clone()
        {
            return new Kit
            {
                ID = ID,
                Name = Name,
                Pitches = new ObservableCollection<KitPitch>(Pitches.Select(pitch => (KitPitch)pitch.Clone())),
                DisplayOrder = DisplayOrder
            };
        }
    }
}

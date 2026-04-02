using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using QBDrumMap.Class.CustomValidations;

namespace QBDrumMap.Class.MapModels
{
    public partial class Kit : ModelBase, IHasID
    {
        #region Fields

        // キットの一意なID
        [ObservableProperty]
        private int iD;

        // キット名
        [ObservableProperty]
        [ValidateKit]
        [NotifyDataErrorInfo]
        private string name = string.Empty;

        // バンクセレクトMSB (CC#0)
        [ObservableProperty]
        private int bankSelectMSB;

        // バンクセレクトLSB (CC#32)
        [ObservableProperty]
        private int bankSelectLSB;

        // プログラムチェンジ番号
        [ObservableProperty]
        private int programNumber;

        // キットに含まれるピッチ割り当てのコレクション
        [ObservableProperty]
        private ObservableCollection<KitPitch> pitches = new();

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

        #endregion

        #region General

        public override object Clone()
        {
            return new Kit
            {
                ID = ID,
                Name = Name,
                BankSelectMSB = BankSelectMSB,
                BankSelectLSB = BankSelectLSB,
                ProgramNumber = ProgramNumber,
                Pitches = new ObservableCollection<KitPitch>(Pitches.Select(pitch =>
                {
                    return (KitPitch)pitch.Clone();
                })),
                DisplayOrder = DisplayOrder
            };
        }

        #endregion

        #endregion
    }
}
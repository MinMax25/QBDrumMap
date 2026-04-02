using CommunityToolkit.Mvvm.ComponentModel;
using QBDrumMap.Class.CustomValidations;

namespace QBDrumMap.Class.MapModels
{
    public partial class Articulation : ModelBase, IHasID
    {
        #region Fields

        // アーティキュレーションの一意なID
        [ObservableProperty]
        private int iD;

        // アーティキュレーション名
        [ObservableProperty]
        [ValidateArticulation]
        [NotifyDataErrorInfo]
        private string name = string.Empty;

        // 補足情報または補完データ
        [ObservableProperty]
        private int complement;

        // ドラムマップ表示時の並び順
        [ObservableProperty]
        private int drumMapOrder;

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

        partial void OnComplementChanged(int oldValue, int newValue)
        {
            UndoManager?.RegisterPropertyChange(() => Complement, oldValue, newValue);
        }

        partial void OnDrumMapOrderChanged(int oldValue, int newValue)
        {
            UndoManager?.RegisterPropertyChange(() => DrumMapOrder, oldValue, newValue);
        }

        #endregion

        #region General

        public override object Clone()
        {
            return new Articulation
            {
                ID = ID,
                Name = Name,
                Complement = Complement,
                DrumMapOrder = DrumMapOrder,
                DisplayOrder = DisplayOrder
            };
        }

        #endregion

        #endregion
    }
}
using CommunityToolkit.Mvvm.ComponentModel;
using QBDrumMap.Class.CustomValidations;

namespace QBDrumMap.Class.MapModels
{
    public partial class Articulation
        : ModelBase
        , IHasID
    {
        [ObservableProperty]
        private int iD;

        [ObservableProperty]
        [ValidateArticulation]
        [NotifyDataErrorInfo]
        private string name = string.Empty;

        [ObservableProperty]
        private int complement;

        [ObservableProperty]
        private int drumMapOrder;

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
    }
}

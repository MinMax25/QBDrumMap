using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using libQB.UndoRedo;
using QBDrumMap.Class.MapModels;

namespace QBDrumMap.Class
{
    public abstract partial class ModelBase
        : ObservableValidator
        , IHasDisplayOrder
        , ICloneable
    {
        [JsonIgnore]
        public int DisplayOrder
        {
            get => _DisplayOrder;
            set => SetProperty(ref _DisplayOrder, value);
        }
        private int _DisplayOrder;

        protected IUndoManager UndoManager;

        public void RegisterUndoManager(IUndoManager undoManager)
        {
            UndoManager = undoManager;
        }

        public void UnregisterUndoManager()
        {
            UndoManager = null;
        }

        public abstract object Clone();
    }
}

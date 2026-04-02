using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using libQB.UndoRedo;
using QBDrumMap.Class.MapModels;

namespace QBDrumMap.Class
{
    public abstract partial class ModelBase : ObservableValidator, IHasDisplayOrder, ICloneable
    {
        #region Fields

        // 並び順を保持するバッキングフィールド
        private int _displayOrder;

        // Undo/Redo操作を管理するマネージャー
        protected IUndoManager UndoManager;

        #endregion

        #region Properties

        // 表示順序（シリアライズ対象外）
        [JsonIgnore]
        public int DisplayOrder
        {
            get
            {
                return _displayOrder;
            }
            set
            {
                SetProperty(ref _displayOrder, value);
            }
        }

        #endregion

        #region Methods

        #region General

        // Undoマネージャーを登録
        public void RegisterUndoManager(IUndoManager undoManager)
        {
            UndoManager = undoManager;
        }

        // Undoマネージャーの登録を解除
        public void UnregisterUndoManager()
        {
            UndoManager = null;
        }

        // オブジェクトのディープコピーを作成（派生クラスで実装）
        public abstract object Clone();

        #endregion

        #endregion
    }
}
using libQB.DialogServices;
using libQB.UndoRedo;
using libQB.WindowServices;
using QBDrumMap.Contracts.Services;

namespace QBDrumMap.Class.Services
{
    public interface IDIContainer
    {
        #region Properties

        #region Services

        // ナビゲーションサービス
        INavigationService Navigation { get; }

        // ダイアログサービス
        IDialogService Dialog { get; }

        // ウィンドウサービス
        IWindowService WindowService { get; }

        // Undo/Redoマネージャー
        IUndoManager UndoManager { get; }

        // 設定管理サービス
        ISettingService SettingService { get; }

        #endregion

        #endregion

        #region Methods

        // コンテナの初期化処理
        void Initialize();

        // データの保存等の終了前処理
        void Flush();

        #endregion
    }
}
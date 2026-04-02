using CommunityToolkit.Mvvm.ComponentModel;
using libQB.DialogServices;
using libQB.UndoRedo;
using libQB.WindowServices;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;
using QBDrumMap.Contracts.Services;
using QBDrumMap.Contracts.ViewModels;

namespace QBDrumMap.Class
{
    public abstract partial class ViewModelBase : ObservableObject, INavigationAware, IDisposable
    {
        #region Fields

        // 重複して破棄されないためのフラグ
        private bool disposedValue;

        // コマンドの有効状態
        [ObservableProperty]
        private bool isCommandEnabled = true;

        // 編集モード中かどうか
        [ObservableProperty]
        private bool isEditing;

        // 依存サービスを保持するコンテナ
        protected internal IDIContainer DIContainer;

        // アプリケーション全体で共有されるマップデータ
        protected internal MapData MapData;

        #endregion

        #region Properties

        #region Services

        // 設定管理サービスへのショートカット
        protected internal ISettingService SettingService
        {
            get
            {
                return DIContainer?.SettingService;
            }
        }

        // ナビゲーションサービスへのショートカット
        protected internal INavigationService Navigation
        {
            get
            {
                return DIContainer?.Navigation;
            }
        }

        // ダイアログサービスへのショートカット
        protected internal IDialogService Dialog
        {
            get
            {
                return DIContainer?.Dialog;
            }
        }

        // ウィンドウ操作サービスへのショートカット
        protected internal IWindowService WindowService
        {
            get
            {
                return DIContainer?.WindowService;
            }
        }

        // Undo/Redoマネージャーへのショートカット
        protected internal IUndoManager UndoManager
        {
            get
            {
                return DIContainer?.UndoManager;
            }
        }

        #endregion

        #endregion

        #region ctor

        public ViewModelBase()
        {
        }

        public ViewModelBase(IDIContainer dIContainer)
        {
            DIContainer = dIContainer;
            MapData = App.GetService<MapData>();
        }

        #endregion

        #region Methods

        #region Property Change Handler

        partial void OnIsEditingChanged(bool oldValue, bool newValue)
        {
            MapData?.SetEditState(this, newValue);
        }

        #endregion

        #region General

        // 画面遷移で離れる際の処理
        public virtual void OnNavigatedFrom()
        {
        }

        // 画面遷移で到達した際の処理
        public virtual void OnNavigatedTo(object parameter)
        {
        }

        #endregion

        #region Dispose

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion
    }
}
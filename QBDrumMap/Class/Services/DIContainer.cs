using libQB.Attributes;
using libQB.DialogServices;
using libQB.UndoRedo;
using libQB.WindowServices;
using QBDrumMap.Contracts.Services;

namespace QBDrumMap.Class.Services
{
    [DISingleton<IDIContainer>]
    public class DIContainer
        : IDIContainer
        , IDisposable
    {
        #region Properties

        #region Services

        public INavigationService Navigation { get; }

        public IWindowService WindowService { get; }

        public IDialogService Dialog { get; }

        public IUndoManager UndoManager { get; }

        public ISettingService SettingService { get; }

        #endregion

        #endregion

        #region Fields

        private bool disposedValue;

        #endregion

        #region ctor

        public DIContainer(
            INavigationService navigationService,
            IWindowService windowService,
            IDialogService dialogService,
            IUndoManager undoManager,
            ISettingService settingService)
        {
            Navigation = navigationService;
            WindowService = windowService;
            Dialog = dialogService;
            UndoManager = undoManager;
            SettingService = settingService;

            Initialize();
        }

        #endregion

        #region Methods

        public void Initialize()
        {
            SettingService?.Load();
        }

        public void Flush()
        {
            SettingService?.Save();
        }

        #region Dispose

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // TODO: 大きなフィールドを null に設定します
                disposedValue = true;
            }
        }

        // // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
        // ~ServiceContainer()
        // {
        //     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion
    }
}
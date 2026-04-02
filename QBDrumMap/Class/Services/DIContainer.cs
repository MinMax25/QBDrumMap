using libQB.Attributes;
using libQB.DialogServices;
using libQB.UndoRedo;
using libQB.WindowServices;
using QBDrumMap.Contracts.Services;

namespace QBDrumMap.Class.Services
{
    [DISingleton<IDIContainer>]
    public class DIContainer : IDIContainer, IDisposable
    {
        #region Fields

        // 重複して破棄されないためのフラグ
        private bool disposedValue;

        #endregion

        #region Properties

        #region Services

        public INavigationService Navigation { get; }

        public IWindowService WindowService { get; }

        public IDialogService Dialog { get; }

        public IUndoManager UndoManager { get; }

        public ISettingService SettingService { get; }

        #endregion

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
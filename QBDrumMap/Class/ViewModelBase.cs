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
    public abstract partial class ViewModelBase
        : ObservableObject
        , INavigationAware
        , IDisposable
    {
        #region Properties

        [ObservableProperty]
        private bool isCommandEnabled = true;

        [ObservableProperty]
        private bool isEditing;

        #region Services

        protected internal IDIContainer DIContainer;

        protected internal ISettingService SettingService => DIContainer?.SettingService;

        protected internal INavigationService Navigation => DIContainer?.Navigation;

        protected internal IDialogService Dialog => DIContainer?.Dialog;

        protected internal IWindowService WindowService => DIContainer?.WindowService;

        protected internal IUndoManager UndoManager => DIContainer?.UndoManager;

        protected internal MapData MapData;

        #endregion

        #endregion

        #region Fields

        private bool disposedValue;

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

        partial void OnIsEditingChanged(bool oldValue, bool newValue)
        {
            MapData.SetEditState(this, newValue);
        }

        public virtual void OnNavigatedFrom()
        {
        }

        public virtual void OnNavigatedTo(object parameter)
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

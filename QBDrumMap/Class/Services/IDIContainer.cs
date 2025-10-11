using libQB.DialogServices;
using libQB.UndoRedo;
using libQB.WindowServices;
using QBDrumMap.Contracts.Services;

namespace QBDrumMap.Class.Services
{
    public interface IDIContainer
    {
        #region Services

        INavigationService Navigation { get; }

        IDialogService Dialog { get; }

        IWindowService WindowService { get; }

        IUndoManager UndoManager { get; }

        ISettingService SettingService { get; }

        #endregion

        #region Methods

        public void Initialize();

        public void Flush();

        #endregion
    }
}

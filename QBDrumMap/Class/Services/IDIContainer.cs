using libQB.DialogServices;
using libQB.UndoRedo;
using libQB.WindowServices;
using QBDrumMap.Contracts.Services;

namespace QBDrumMap.Class.Services;

public interface IDIContainer
{
    INavigationService Navigation { get; }

    IDialogService Dialog { get; }

    IWindowService WindowService { get; }

    IUndoManager UndoManager { get; }

    ISettingService SettingService { get; }

    void Initialize();

    void Flush();
}
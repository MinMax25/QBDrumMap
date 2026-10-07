using libQB.Attributes;
using libQB.DialogServices;
using libQB.UndoRedo;
using libQB.WindowServices;
using QBDrumMap.Contracts.Services;

namespace QBDrumMap.Class.Services;

[DISingleton<IDIContainer>]
public class DIContainer : IDIContainer, IDisposable
{
    private bool disposedValue;

    public INavigationService Navigation { get; }

    public IWindowService WindowService { get; }

    public IDialogService Dialog { get; }

    public IUndoManager UndoManager { get; }

    public ISettingService SettingService { get; }

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

    public void Initialize()
    {
        SettingService?.Load();
    }

    public void Flush()
    {
        SettingService?.Save();
    }

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
}
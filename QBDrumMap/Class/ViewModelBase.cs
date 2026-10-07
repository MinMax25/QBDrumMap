using CommunityToolkit.Mvvm.ComponentModel;
using libQB.DialogServices;
using libQB.UndoRedo;
using libQB.WindowServices;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;
using QBDrumMap.Contracts.Services;
using QBDrumMap.Contracts.ViewModels;

namespace QBDrumMap.Class;

public abstract partial class ViewModelBase : ObservableObject, INavigationAware, IDisposable
{
    private bool disposedValue;

    [ObservableProperty]
    private bool isCommandEnabled = true;

    [ObservableProperty]
    private bool isEditing;

    protected internal IDIContainer DIContainer;

    protected internal MapData MapData;

    protected internal ISettingService SettingService
    {
        get
        {
            return DIContainer?.SettingService;
        }
    }

    protected internal INavigationService Navigation
    {
        get
        {
            return DIContainer?.Navigation;
        }
    }

    protected internal IDialogService Dialog
    {
        get
        {
            return DIContainer?.Dialog;
        }
    }

    protected internal IWindowService WindowService
    {
        get
        {
            return DIContainer?.WindowService;
        }
    }

    protected internal IUndoManager UndoManager
    {
        get
        {
            return DIContainer?.UndoManager;
        }
    }

    public ViewModelBase()
    {
    }

    public ViewModelBase(IDIContainer dIContainer)
    {
        DIContainer = dIContainer;
        MapData = App.GetService<MapData>();
    }

    partial void OnIsEditingChanged(bool oldValue, bool newValue)
    {
        MapData?.SetEditState(this, newValue);
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
using CommunityToolkit.Mvvm.ComponentModel;
using QBDrumMap.Class.Services;

namespace QBDrumMap.Class;

public abstract partial class ViewModelBaseHasContents : ViewModelBase
{
    [ObservableProperty]
    private bool isContentEnabled;

    [ObservableProperty]
    private ViewModelBase contentViewModel;

    public ViewModelBaseHasContents()
    {
    }

    public ViewModelBaseHasContents(IDIContainer diContainer)
        : base(diContainer)
    {
    }
        }
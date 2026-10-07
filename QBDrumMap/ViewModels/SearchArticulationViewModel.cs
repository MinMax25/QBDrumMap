using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using libQB.Attributes;
using libQB.WindowServices;
using QBDrumMap.Class;
using QBDrumMap.Class.Extensions;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;
using QBDrumMap.Views;

namespace QBDrumMap.ViewModels;

[DIWindow<SearchArticulation>]
public partial class SearchArticulationViewModel : ViewModelBase, IResultProvider<Articulation>
{
    public ICollectionView Articulations { get; init; }

    [ObservableProperty]
    private string _filterArticulationName;

    [ObservableProperty]
    private Articulation _selectedArticulation;

    public Articulation GetResult()
    {
        return SelectedArticulation;
    }

    public SearchArticulationViewModel(IDIContainer diContainer)
        : base(diContainer)
    {
        var artics = MapData.Parts
            .GroupBy(x =>
            {
                return x.DisplayOrder;
            })
            .SelectMany(x =>
            {
                return x.SelectMany(y =>
                {
                    return y.Articulations.OrderBy(z =>
                    {
                        return z.DisplayOrder;
                    });
                });
            })
            .ToList();

        Articulations = CollectionViewSource.GetDefaultView(artics);
        Articulations.Filter = FilterMethod;

        FilterArticulationName = SettingService.SearchArticulationFilter;
    }

    partial void OnFilterArticulationNameChanged(string value)
    {
        SettingService.SearchArticulationFilter = value;
        Articulations.Refresh();
    }

    [RelayCommand]
    private void OnArticulationSelected(object sender)
    {
        if (sender is Window window)
        {
            WindowService.CloseWindow(window);
        }
    }

    private bool FilterMethod(object obj)
    {
        if (obj is not Articulation articulation)
        {
            return false;
        }

        if (string.IsNullOrEmpty(FilterArticulationName))
        {
            return true;
        }

        return articulation.Name.Like(FilterArticulationName);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
        }
    }
        }
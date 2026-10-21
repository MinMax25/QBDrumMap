using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using libQB.Attributes;
using QBDrumMap.Class;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;
using QBDrumMap.Views;

namespace QBDrumMap.ViewModels;

[DIPage<ArticulationsPage>]
public partial class ArticulationsViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<Articulation> _articulations = new();

    [ObservableProperty]
    private ObservableCollection<Articulation> _selectedArticulations = new();

    public ArticulationsViewModel(IDIContainer diContainer)
        : base(diContainer)
    {
        OnMapDataLoaded(this, new RoutedEventArgs());
        MapData.Loaded += OnMapDataLoaded;
        MapData.Saved += OnMapDataSaved;
    }

    [RelayCommand]
    private void OnMoveTop() => Articulations.MoveTop(SelectedArticulations, x => x.DrumMapOrder);

    [RelayCommand]
    private void OnMoveUp() => Articulations.MoveUp(SelectedArticulations, x => x.DrumMapOrder);

    [RelayCommand]
    private void OnMoveDown() => Articulations.MoveDown(SelectedArticulations, x => x.DrumMapOrder);

    [RelayCommand]
    private void OnMoveBottom() => Articulations.MoveBottom(SelectedArticulations, x => x.DrumMapOrder);

    private async void OnMapDataLoaded(object sender, RoutedEventArgs e)
    {
        await InitializeDataAsync();
    }

    private async void OnMapDataSaved(object sender, DrumMapIOEventArgs e)
    {
        await InitializeDataAsync();
    }

    private async Task InitializeDataAsync()
    {
        SelectedArticulations.Clear();

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            Articulations.Clear();

            var source = MapData.Parts
                .SelectMany(x =>
                {
                    return x.Articulations;
                })
                .OrderBy(x =>
                {
                    return x.DrumMapOrder;
                });

            foreach (var item in source)
            {
                Articulations.Add(item);
            }
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            MapData.Loaded -= OnMapDataLoaded;
            MapData.Saved -= OnMapDataSaved;
        }
    }
}
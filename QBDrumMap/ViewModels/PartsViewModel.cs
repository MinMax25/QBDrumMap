using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using libQB.Attributes;
using QBDrumMap.Class;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;
using QBDrumMap.ViewModels.Controls;
using QBDrumMap.Views;

namespace QBDrumMap.ViewModels;

[DIPage<PartsPage>]
public partial class PartsViewModel : ViewModelBaseHasContents
{
    [ObservableProperty]
    private ICollectionView _partsView;

    [ObservableProperty]
    private ObservableCollection<Part> _selectedParts = new();

    [ObservableProperty]
    private Part _selectedPart;

    public PartsViewModel(IDIContainer diContainer)
        : base(diContainer)
    {
        Task.Run(InitializeDataAsync);
        ContentViewModel = App.GetService<PartPanelFactory>()?.Create(null);

        MapData.Loaded += OnMapDataLoaded;
        MapData.Saved += OnMapDataSaved;
        MapData.EditStateChanged += OnPropertyChanged;
        PropertyChanged += OnPropertyChanged;
    }

    [RelayCommand]
    private void OnAdd()
    {
        Part drumPart = new()
        {
            ID = MapData.Parts.GetNewID()
        };
        drumPart.Name = $"Part{drumPart.ID}";

        MapData.Parts.AddItem(drumPart, x => x.DisplayOrder);

        SelectedPart = drumPart;
    }

    [RelayCommand]
    private void OnMoveTop() => MapData.Parts.MoveTop(SelectedParts, x => x.DisplayOrder);

    [RelayCommand]
    private void OnMoveUp() => MapData.Parts.MoveUp(SelectedParts, x => x.DisplayOrder);

    [RelayCommand]
    private void OnMoveDown() => MapData.Parts.MoveDown(SelectedParts, x => x.DisplayOrder);

    [RelayCommand]
    private void OnMoveBottom() => MapData.Parts.MoveBottom(SelectedParts, x => x.DisplayOrder);

    [RelayCommand]
    private async Task OnDeleteSelectedParts()
    {
        if (SelectedParts == null || SelectedParts.Count == 0)
        {
            return;
        }

        string names = string.Join(", ", SelectedParts.Select(x => x.Name));
        string message = string.Format(
            App.Text("Message_Command_Delete"),
            App.NameText("Part", Name.Part),
            names);

        if (await Dialog.ShowConfirmAsync(message, App.Text("Title_Command_Delete")) == false)
        {
            return;
        }

        foreach (var p in SelectedParts.ToArray())
        {
            p.Articulations.Clear();
            MapData.Parts.Remove(p);
        }

        SelectedParts.Clear();
        SelectedPart = null;

        PartsView.Refresh();
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(MapData.EditState):
                IsCommandEnabled = !MapData.EditState;
                break;
            default:
                break;
        }
    }

    private async void OnMapDataLoaded(object sender, RoutedEventArgs e)
    {
        await InitializeDataAsync();
    }

    private async void OnMapDataSaved(object sender, DrumMapIOEventArgs e)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            PartsView.Refresh();
        });
    }

    partial void OnSelectedPartChanged(Part oldValue, Part newValue)
    {
        oldValue?.UnregisterUndoManager();
        newValue?.RegisterUndoManager(UndoManager);

        IsCommandEnabled = !MapData.EditState;
        IsContentEnabled = SelectedPart != null;

        if (SelectedPart != (ContentViewModel as PartPanelViewModel)?.Part)
        {
            ContentViewModel?.Dispose();
        }

        ContentViewModel = App.GetService<PartPanelFactory>()?.Create(SelectedPart);
    }

    private async Task InitializeDataAsync()
    {
        SelectedParts.Clear();

        var parts = MapData.Parts.OrderBy(x => x.DisplayOrder).ToList();

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            MapData.Parts.Clear();
            foreach (var part in parts)
            {
                MapData.Parts.Add(part);
            }

            PartsView = CollectionViewSource.GetDefaultView(MapData.Parts);
            MapData.SetEditState(this, false);
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            MapData.Loaded -= OnMapDataLoaded;
            MapData.Saved -= OnMapDataSaved;
            MapData.EditStateChanged -= OnPropertyChanged;
            PropertyChanged -= OnPropertyChanged;

            ContentViewModel?.Dispose();
            SelectedParts.Clear();
        }
    }
        }

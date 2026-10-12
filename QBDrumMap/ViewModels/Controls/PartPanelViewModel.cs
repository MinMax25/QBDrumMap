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
using QBDrumMap.Views;
using QBDrumMap.Views.Controls;

namespace QBDrumMap.ViewModels.Controls;

[DIUserControl<PartPanel>]
public partial class PartPanelViewModel : ViewModelBase
{
    [ObservableProperty]
    private Part _part;

    [ObservableProperty]
    private ICollectionView _articulationsView;

    [ObservableProperty]
    private ObservableCollection<Articulation> _selectedArticulations = new();

    [ObservableProperty]
    private Articulation _selectedArticulation;

    public ISettingService Setting
    {
        get
        {
            return SettingService;
        }
    }

    public PartPanelViewModel(IDIContainer diContainer, Part part)
        : base(diContainer)
    {
        Part = part;

        if (Part != null)
        {
            Part.RegisterUndoManager(UndoManager);

            ObservableCollection<Articulation> articulations = new([.. Part.Articulations.OrderBy(x =>
            {
                return x.DisplayOrder;
            })]);

            // 表示順に並べ直すだけで、編集ではない
            MapData.RunWithoutDirtyChange(() =>
            {
                Part.Articulations.Clear();
                foreach (var articulation in articulations)
                {
                    Part.Articulations.Add(articulation);
                }
            });
        }

        ArticulationsView = CollectionViewSource.GetDefaultView(Part?.Articulations);

        MapData.EditStateChanged += OnPropertyChanged;
    }

    [RelayCommand]
    private async Task OnClearComplement(object value)
    {
        if (SelectedArticulation == null)
        {
            return;
        }

        string message = string.Format(App.Text("Message_Command_Clear"), App.NameText("Complement", Name.Complement));
        if (await Dialog.ShowConfirmAsync(message, App.Text("Title_Command_Clear")) == false)
        {
            return;
        }

        SelectedArticulation.Complement = 0;
    }

    [RelayCommand]
    private void OnSearchComplement(object value)
    {
        if (value is not Articulation articulation)
        {
            return;
        }

        if (SelectedArticulation == null)
        {
            return;
        }

        WindowService.ShowWindowWithCallback<SearchArticulation, SearchArticulationViewModel, Articulation>(
            parameter: articulation,
            owner: Application.Current.Windows.OfType<Window>().FirstOrDefault(w =>
            {
                return w.IsActive;
            }),
            resultCallback: result =>
            {
                if (result != null)
                {
                    SelectedArticulation.Complement = result.ID;
                }
            });
    }

    [RelayCommand]
    private void OnAdd()
    {
        if (Part == null)
        {
            return;
        }

        Articulation articulation = new();

        int count = Part.Articulations.Count + 1;

        var allArticulations = MapData.Parts.SelectMany(x =>
        {
            return x.Articulations;
        });

        int order = allArticulations.Any() ? allArticulations.Max(x =>
        {
            return x.DisplayOrder;
        }) + 1 : 1;

        articulation.ID = MapData.NewArticulationID();
        articulation.Name = $"{Part.Name} {count}";
        articulation.DisplayOrder = int.MaxValue;
        articulation.DrumMapOrder = order;

        Part.Articulations.AddItem(articulation, x => x.DisplayOrder);

        SelectedArticulation = articulation;
    }

    [RelayCommand]
    private void OnMoveTop() => Part?.Articulations.MoveTop(SelectedArticulations, x => x.DisplayOrder);

    [RelayCommand]
    private void OnMoveUp() => Part?.Articulations.MoveUp(SelectedArticulations, x => x.DisplayOrder);

    [RelayCommand]
    private void OnMoveDown() => Part?.Articulations.MoveDown(SelectedArticulations, x => x.DisplayOrder);

    [RelayCommand]
    private void OnMoveBottom() => Part?.Articulations.MoveBottom(SelectedArticulations, x => x.DisplayOrder);

    [RelayCommand]
    private async Task OnDeleteSelectedArticulations()
    {
        if (SelectedArticulations == null || SelectedArticulations.Count == 0)
        {
            return;
        }

        string names = string.Join(", ", SelectedArticulations.Select(x =>
        {
            return x.Name;
        }).ToArray());

        string message = string.Format(App.Text("Message_Command_Delete"), App.NameText("Articulation", Name.Articulation), names);
        if (await Dialog.ShowConfirmAsync(message, App.Text("Title_Command_Delete")) == false)
        {
            return;
        }

        foreach (var item in SelectedArticulations.ToArray())
        {
            var targetPitches = MapData.Plugins
                .SelectMany(x =>
                {
                    return x.Kits;
                })
                .SelectMany(x =>
                {
                    return x.Pitches;
                })
                .Where(x =>
                {
                    return x.ArticulationID == item.ID;
                });

            foreach (var kitPitch in targetPitches.ToArray())
            {
                kitPitch.ArticulationID = 0;
            }

            Part?.Articulations.Remove(item);
        }

        SelectedArticulations.Clear();
        SelectedArticulation = null;

        ArticulationsView?.Refresh();
    }

    partial void OnSelectedArticulationChanged(Articulation oldValue, Articulation newValue)
    {
        oldValue?.UnregisterUndoManager();
        newValue?.RegisterUndoManager(UndoManager);
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

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            MapData.EditStateChanged -= OnPropertyChanged;
        }
    }
}

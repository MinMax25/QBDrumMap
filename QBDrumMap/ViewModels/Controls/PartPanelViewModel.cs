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

namespace QBDrumMap.ViewModels.Controls
{
    [DIUserControl<PartPanel>]
    public partial class PartPanelViewModel
        : ViewModelBase
    {
        #region Properties

        public ISettingService Setting => SettingService;

        [ObservableProperty]
        private Part part;

        [ObservableProperty]
        private ICollectionView articulationsView;

        [ObservableProperty]
        private ObservableCollection<Articulation> selectedArticulations = [];

        [ObservableProperty]
        private Articulation selectedArticulation;

        #endregion

        #region ctor

        public PartPanelViewModel(IDIContainer diContainer, Part part)
            : base(diContainer)
        {
            Part = part;
            if (Part != null)
            {
                Part.RegisterUndoManager(UndoManager);
                ObservableCollection<Articulation> articulations = [.. Part.Articulations.OrderBy(x => x.DisplayOrder)];
                Part.Articulations.Clear();
                articulations.ToList().ForEach(Part.Articulations.Add);
            }

            ArticulationsView = CollectionViewSource.GetDefaultView(Part?.Articulations);

            MapData.EditStateChanged += OnPropertyChanged;
        }

        #endregion

        #region Methods

        #region Commands

        [RelayCommand]
        private async Task OnClearComplement(object value)
        {
            if (SelectedArticulation == null) return;

            string message = string.Format(libQB.Properties.Resources.Message_Command_Clear, Properties.Name.Complement);
            if (await Dialog.ShowConfirmAsync(message, libQB.Properties.Dialog.Title_Command_Clear) == false) return;

            SelectedArticulation.Complement = 0;
        }

        [RelayCommand]
        private void OnSearchComplement(object value)
        {
            if (value is not Articulation articulation) return;
            if (SelectedArticulation == null) return;

            WindowService.ShowWindowWithCallback<SearchArticulation, SearchArticulationViewModel, Articulation>(
                parameter: articulation,
                owner: Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive),
                resultCallback: articulation =>
                {
                    if (articulation != null)
                    {
                        SelectedArticulation.Complement = articulation.ID;
                    }
                });
        }

        [RelayCommand]
        private void OnAdd()
        {
            if (Part == null) return;

            Articulation articulation = new();

            int count = Part.Articulations.Count + 1;
            int order = MapData.Parts.SelectMany(x => x.Articulations).Any() ? MapData.Parts.SelectMany(x => x.Articulations).Max(x => x.DisplayOrder) + 1 : 1;

            articulation.ID = MapData.Parts.SelectMany(x => x.Articulations).GetNewID();
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
            if (SelectedArticulations == null) return;
            if (SelectedArticulations.Count == 0) return;

            string names = string.Join(", ", SelectedArticulations.Select(x => x.Name).ToArray());
            string message = string.Format(libQB.Properties.Resources.Message_Command_Delete, Properties.Name.Articulation, names);
            if (await Dialog.ShowConfirmAsync(message, libQB.Properties.Dialog.Title_Command_Delete) == false) return;

            foreach (var item in SelectedArticulations.ToArray())
            {
                foreach (var kitPitch in MapData.Plugins.SelectMany(x => x.Kits).SelectMany(x => x.Pitches).Where(x => x.ArticulationID == item.ID).ToArray())
                {
                    kitPitch.ArticulationID = 0;
                }
                Part?.Articulations.Remove(item);
            }

            SelectedArticulations.Clear();
            SelectedArticulation = null;

            ArticulationsView.Refresh();
        }

        #endregion

        #region PropertyChanged Callbacks

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

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                MapData.EditStateChanged -= OnPropertyChanged;
            }
        }

        #endregion

        #endregion
    }
}
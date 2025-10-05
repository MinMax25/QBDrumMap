using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using libQB.Attributes;
using QBDrumMap.Class;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;
using QBDrumMap.Views;

namespace QBDrumMap.ViewModels
{
    [DIPage<ArticulationsPage>]
    public partial class ArticulationsViewModel
        : ViewModelBase
    {
        #region Properties

        [ObservableProperty]
        private ObservableCollection<Articulation> articulations = [];

        [ObservableProperty]
        private ObservableCollection<Articulation> selectedArticulations = [];

        #endregion

        #region ctor

        public ArticulationsViewModel(IDIContainer diContainer)
            : base(diContainer)
        {
            OnMapDataLoaded(this, new RoutedEventArgs());
            MapData.Loaded += OnMapDataLoaded;
            MapData.Saved += OnMapDataSaved;
        }

        #endregion

        #region Methods

        #region Commands

        [RelayCommand]
        private void OnMoveTop() => Articulations.MoveTop(SelectedArticulations, x => x.DrumMapOrder);

        [RelayCommand]
        private void OnMoveUp() => Articulations.MoveUp(SelectedArticulations, x => x.DrumMapOrder);

        [RelayCommand]
        private void OnMoveDown() => Articulations.MoveDown(SelectedArticulations, x => x.DrumMapOrder);

        [RelayCommand]
        private void OnMoveBottom() => Articulations.MoveBottom(SelectedArticulations, x => x.DrumMapOrder);

        #endregion

        #region PropertyChanged Callbacks

        #endregion

        #region Event Handling

        private async void OnMapDataLoaded(object sender, RoutedEventArgs e)
        {
            await InitializeDataAsync();
        }

        private async void OnMapDataSaved(object sender, DrumMapIOEventArgs e)
        {
            await InitializeDataAsync();
        }

        #endregion

        #region General

        private async Task InitializeDataAsync()
        {
            SelectedArticulations.Clear();

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Articulations.Clear();
                foreach (var item in MapData.Parts.SelectMany(x => x.Articulations).OrderBy(x => x.DrumMapOrder))
                {
                    Articulations.Add(item);
                }
            });
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                MapData.Loaded -= OnMapDataLoaded;
            }
        }

        #endregion

        #endregion
    }
}

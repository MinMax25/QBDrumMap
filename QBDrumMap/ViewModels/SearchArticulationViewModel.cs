using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using libQB.Attributes;
using libQB.WindowServices;
using QBDrumMap.Class;
using QBDrumMap.Class.Extentions;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;
using QBDrumMap.Views;

namespace QBDrumMap.ViewModels
{
    [DIWindow<SearchArticulation>]
    public partial class SearchArticulationViewModel
        : ViewModelBase
        , IResultProvider<Articulation>
    {
        #region Properties

        public ICollectionView Articulations { get; init; }

        [ObservableProperty]
        private string filterArticulationName;

        [ObservableProperty]
        private Articulation selectedArticulation;

        public Articulation GetResult() => SelectedArticulation;

        #endregion

        #region ctor

        public SearchArticulationViewModel(IDIContainer diContainer)
          : base(diContainer)
        {
            var artics = MapData.Parts.GroupBy(x => x.DisplayOrder).SelectMany(x => x.SelectMany(y => y.Articulations.OrderBy(y => y.DisplayOrder))).ToList();
            Articulations = CollectionViewSource.GetDefaultView(artics);
            Articulations.Filter = FilterMethod;

            FilterArticulationName = SettingService.SearchArticulationFilter;
        }

        #endregion

        #region Methods

        #region PropertyChanged Callbacks

        partial void OnFilterArticulationNameChanged(string value)
        {
            SettingService.SearchArticulationFilter = value;
            Articulations.Refresh();
        }

        #endregion

        #region Commands

        [RelayCommand]
        private void OnArticulationSelected(object sender)
        {
            if (sender is Window window)
            {
                WindowService.CloseWindow(window);
            }
        }

        #endregion

        #region General

        private bool FilterMethod(object obj)
        {
            if (obj is not Articulation articulation) return false;

            if (string.IsNullOrEmpty(FilterArticulationName)) return true;

            return articulation.Name.Like(FilterArticulationName);
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
            }
        }

        #endregion

        #endregion
    }
}

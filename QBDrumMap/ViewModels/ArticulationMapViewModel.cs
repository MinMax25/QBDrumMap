using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using libQB.Attributes;
using libQB.WindowServices;
using QBDrumMap.Class;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;
using QBDrumMap.Views;

namespace QBDrumMap.ViewModels
{
    [DIWindow<ArticulationMapView>]
    public partial class ArticulationMapViewModel
        : ViewModelBase
        , IParameterReceiver
    {
        #region Properties

        [ObservableProperty]
        private ICollectionView articulationMapView;

        [ObservableProperty]
        private Plugin plugin;

        [ObservableProperty]
        private Kit kit;

        [ObservableProperty]
        private ArticulationMapItem selectedRow;

        #endregion

        #region ctor

        public ArticulationMapViewModel(IDIContainer diContainer)
            : base(diContainer)
        {
        }

        #endregion

        #region Methods

        #region General

        public void ReceiveParameter(object parameter)
        {
            if (parameter is not Kit kit) return;

            Kit = kit;

            Plugin = MapData.Plugins.FirstOrDefault(p => p.Kits.Any(k => k.ID == Kit.ID));

            var articMap = ArticulationMap.GetArticulationMap(MapData, Kit.Name);
            ArticulationMapView = CollectionViewSource.GetDefaultView(articMap.Items);
        }

        #endregion

        #endregion
    }
}

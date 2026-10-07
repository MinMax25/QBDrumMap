using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using libQB.Attributes;
using libQB.WindowServices;
using QBDrumMap.Class;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;
using QBDrumMap.Views;

namespace QBDrumMap.ViewModels;

[DIWindow<ArticulationMapView>]
public partial class ArticulationMapViewModel : ViewModelBase, IParameterReceiver
{
    [ObservableProperty]
    private ICollectionView _articulationMapView;

    [ObservableProperty]
    private Plugin _plugin;

    [ObservableProperty]
    private Kit _kit;

    [ObservableProperty]
    private ArticulationMapItem _selectedRow;

    public ArticulationMapViewModel(IDIContainer diContainer)
        : base(diContainer)
    {
    }

    public void ReceiveParameter(object parameter)
    {
        if (parameter is not Kit kit)
        {
            return;
        }

        Kit = kit;

        Plugin = MapData.Plugins.FirstOrDefault(p =>
        {
            return p.Kits.Any(k =>
            {
                return k.ID == Kit.ID;
            });
        });

        var articMap = ArticulationMap.GetArticulationMap(MapData, Kit.Name);
        ArticulationMapView = CollectionViewSource.GetDefaultView(articMap.Items);
    }
}
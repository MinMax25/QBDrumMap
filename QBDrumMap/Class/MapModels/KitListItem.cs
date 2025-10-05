using CommunityToolkit.Mvvm.ComponentModel;

namespace QBDrumMap.Class.MapModels
{
    public partial class KitListItem
        : ViewModelBase
    {
        [ObservableProperty]
        private int pluginID;

        [ObservableProperty]
        private string pluginName;

        [ObservableProperty]
        private int pluginDisplayOrder;

        [ObservableProperty]
        private int kitID;

        [ObservableProperty]
        private string kitName;

        [ObservableProperty]
        private int kitDisplayOrder;
    }
}

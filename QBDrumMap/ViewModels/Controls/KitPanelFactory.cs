using libQB.Attributes;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;

namespace QBDrumMap.ViewModels.Controls
{
    [DISingleton]
    public class KitPanelFactory
    {
        private IDIContainer _container;

        public KitPanelFactory(IDIContainer diContainer)
        {
            _container = diContainer;
        }

        public KitPanelViewModel Create(Plugin plugin)
        {
            return new KitPanelViewModel(_container, plugin);
        }
    }
}

using libQB.Attributes;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;

namespace QBDrumMap.ViewModels.Controls
{
    [DISingleton]
    public class KitPitchesPanelFactory
    {
        private IDIContainer _container;

        public KitPitchesPanelFactory(IDIContainer diContainer)
        {
            _container = diContainer;
        }

        public KitPitchesPanelViewModel Create(Kit kit, bool sameKit)
        {
            return new KitPitchesPanelViewModel(_container, kit, sameKit);
        }
    }
}

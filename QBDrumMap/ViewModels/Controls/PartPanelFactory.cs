using libQB.Attributes;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;

namespace QBDrumMap.ViewModels.Controls
{
    [DISingleton]
    public class PartPanelFactory
    {
        private IDIContainer _container;

        public PartPanelFactory(IDIContainer diContainer)
        {
            _container = diContainer;
        }

        public PartPanelViewModel Create(Part part)
        {
            return new PartPanelViewModel(_container, part);
        }
    }
}

using libQB.Attributes;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;

namespace QBDrumMap.ViewModels.Controls
{
    [DISingleton]
    public class PartPanelFactory
    {
        #region Fields

        // 依存サービスを解決するためのコンテナ
        private readonly IDIContainer _container;

        #endregion

        #region ctor

        public PartPanelFactory(IDIContainer diContainer)
        {
            _container = diContainer;
        }

        #endregion

        #region Methods

        #region General

        // 指定された Part を持つ PartPanelViewModel のインスタンスを生成
        public PartPanelViewModel Create(Part part)
        {
            return new PartPanelViewModel(_container, part);
        }

        #endregion

        #endregion
    }
}
using libQB.Attributes;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;

namespace QBDrumMap.ViewModels.Controls
{
    [DISingleton]
    public class KitPanelFactory
    {
        #region Fields

        // 依存サービスを解決するためのコンテナ
        private readonly IDIContainer _container;

        #endregion

        #region ctor

        public KitPanelFactory(IDIContainer diContainer)
        {
            _container = diContainer;
        }

        #endregion

        #region Methods

        #region General

        // 指定された Plugin を持つ KitPanelViewModel のインスタンスを生成
        public KitPanelViewModel Create(Plugin plugin)
        {
            return new KitPanelViewModel(_container, plugin);
        }

        #endregion

        #endregion
    }
}
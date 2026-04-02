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
    public partial class ArticulationMapViewModel : ViewModelBase, IParameterReceiver
    {
        #region Fields

        // アーティキュレーションマップのビュー（表示・ソート用）
        [ObservableProperty]
        private ICollectionView _articulationMapView;

        // 対象となるプラグインモデル
        [ObservableProperty]
        private Plugin _plugin;

        // 現在表示対象となっているキットモデル
        [ObservableProperty]
        private Kit _kit;

        // グリッド等で選択されている行アイテム
        [ObservableProperty]
        private ArticulationMapItem _selectedRow;

        #endregion

        #region ctor

        public ArticulationMapViewModel(IDIContainer diContainer)
            : base(diContainer)
        {
        }

        #endregion

        #region Methods

        #region General

        // 外部（WindowService等）からパラメータを受け取り、画面を初期化する
        public void ReceiveParameter(object parameter)
        {
            if (parameter is not Kit kit)
            {
                return;
            }

            Kit = kit;

            // 当該キットを保持しているプラグインを検索
            Plugin = MapData.Plugins.FirstOrDefault(p =>
            {
                return p.Kits.Any(k =>
                {
                    return k.ID == Kit.ID;
                });
            });

            // アーティキュレーションマップ情報の生成とビューの取得
            var articMap = ArticulationMap.GetArticulationMap(MapData, Kit.Name);
            ArticulationMapView = CollectionViewSource.GetDefaultView(articMap.Items);
        }

        #endregion

        #endregion
    }
}
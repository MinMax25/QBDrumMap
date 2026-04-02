using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using libQB.Attributes;
using libQB.WindowServices;
using QBDrumMap.Class;
using QBDrumMap.Class.Extensions;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;
using QBDrumMap.Views;

namespace QBDrumMap.ViewModels
{
    [DIWindow<SearchArticulation>]
    public partial class SearchArticulationViewModel : ViewModelBase, IResultProvider<Articulation>
    {
        #region Fields

        // フィルタリング・表示用のアーティキュレーション一覧ビュー
        public ICollectionView Articulations { get; init; }

        // アーティキュレーション名で絞り込むためのフィルター文字列
        [ObservableProperty]
        private string _filterArticulationName;

        // リストで選択されているアーティキュレーション
        [ObservableProperty]
        private Articulation _selectedArticulation;

        #endregion

        #region Properties

        // IResultProviderの実装: 選択された結果を返す
        public Articulation GetResult()
        {
            return SelectedArticulation;
        }

        #endregion

        #region ctor

        public SearchArticulationViewModel(IDIContainer diContainer)
            : base(diContainer)
        {
            var artics = MapData.Parts
                .GroupBy(x =>
                {
                    return x.DisplayOrder;
                })
                .SelectMany(x =>
                {
                    return x.SelectMany(y =>
                    {
                        return y.Articulations.OrderBy(z =>
                        {
                            return z.DisplayOrder;
                        });
                    });
                })
                .ToList();

            Articulations = CollectionViewSource.GetDefaultView(artics);
            Articulations.Filter = FilterMethod;

            FilterArticulationName = SettingService.SearchArticulationFilter;
        }

        #endregion

        #region Methods

        #region PropertyChanged Callbacks

        // フィルター文字列変更時に設定を保存し、ビューを更新する
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

        // コレクションビューのフィルタリングロジック
        private bool FilterMethod(object obj)
        {
            if (obj is not Articulation articulation)
            {
                return false;
            }

            if (string.IsNullOrEmpty(FilterArticulationName))
            {
                return true;
            }

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
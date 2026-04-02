using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using libQB.Attributes;
using QBDrumMap.Class;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;
using QBDrumMap.Views;

namespace QBDrumMap.ViewModels
{
    [DIPage<ArticulationsPage>]
    public partial class ArticulationsViewModel : ViewModelBase
    {
        #region Fields

        // 画面に表示する全アーティキュレーションのリスト
        [ObservableProperty]
        private ObservableCollection<Articulation> _articulations = new();

        // グリッド等で選択されているアーティキュレーションのリスト
        [ObservableProperty]
        private ObservableCollection<Articulation> _selectedArticulations = new();

        #endregion

        #region ctor

        public ArticulationsViewModel(IDIContainer diContainer)
            : base(diContainer)
        {
            OnMapDataLoaded(this, new RoutedEventArgs());
            MapData.Loaded += OnMapDataLoaded;
            MapData.Saved += OnMapDataSaved;
        }

        #endregion

        #region Methods

        #region Commands

        [RelayCommand]
        private void OnMoveTop() => Articulations.MoveTop(SelectedArticulations, x => x.DrumMapOrder);

        [RelayCommand]
        private void OnMoveUp() => Articulations.MoveUp(SelectedArticulations, x => x.DrumMapOrder);

        [RelayCommand]
        private void OnMoveDown() => Articulations.MoveDown(SelectedArticulations, x => x.DrumMapOrder);

        [RelayCommand]
        private void OnMoveBottom() => Articulations.MoveBottom(SelectedArticulations, x => x.DrumMapOrder);

        #endregion

        #region EventHandler

        // マップデータ読み込み完了時のイベントハンドラ
        private async void OnMapDataLoaded(object sender, RoutedEventArgs e)
        {
            await InitializeDataAsync();
        }

        // マップデータ保存完了時のイベントハンドラ
        private async void OnMapDataSaved(object sender, DrumMapIOEventArgs e)
        {
            await InitializeDataAsync();
        }

        #endregion

        #region General

        // 表示用データの非同期初期化処理
        private async Task InitializeDataAsync()
        {
            SelectedArticulations.Clear();

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Articulations.Clear();

                // 全パートからアーティキュレーションをフラットに取得し、ドラムマップ順でソート
                var source = MapData.Parts
                    .SelectMany(x =>
                    {
                        return x.Articulations;
                    })
                    .OrderBy(x =>
                    {
                        return x.DrumMapOrder;
                    });

                foreach (var item in source)
                {
                    Articulations.Add(item);
                }
            });
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                MapData.Loaded -= OnMapDataLoaded;
                MapData.Saved -= OnMapDataSaved;
            }
        }

        #endregion

        #endregion
    }
}
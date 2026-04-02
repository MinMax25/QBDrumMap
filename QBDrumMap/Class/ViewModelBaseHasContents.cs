using CommunityToolkit.Mvvm.ComponentModel;
using QBDrumMap.Class.Services;

namespace QBDrumMap.Class
{
    public abstract partial class ViewModelBaseHasContents : ViewModelBase
    {
        #region Fields

        // コンテンツ（子ビューモデル）が操作可能かどうか
        [ObservableProperty]
        private bool isContentEnabled;

        // 現在保持している子ビューモデル
        [ObservableProperty]
        private ViewModelBase contentViewModel;

        #endregion

        #region Properties

        // ObservablePropertyにより自動生成されるため、追加のプロパティ定義は不要

        #endregion

        #region ctor

        public ViewModelBaseHasContents()
        {
        }

        public ViewModelBaseHasContents(IDIContainer diContainer)
            : base(diContainer)
        {
        }

        #endregion
    }
}
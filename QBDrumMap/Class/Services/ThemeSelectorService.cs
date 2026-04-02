using System.Windows;
using ControlzEx.Theming;
using libQB.Attributes;
using libQB.Enums;

namespace QBDrumMap.Services
{
    [DISingleton<IThemeSelectorService>]
    public class ThemeSelectorService : IThemeSelectorService
    {
        #region ctor

        public ThemeSelectorService()
        {
        }

        #endregion

        #region Methods

        // アプリケーションのテーマ（Dark/Light）とアクセントカラーを変更
        public void SetTheme(BaseTheme theme, string colorname)
        {
            if (Application.Current == null)
            {
                return;
            }

            ThemeManager.Current.ChangeTheme(Application.Current, $"{theme}.{colorname}");
        }

        #endregion
    }
}
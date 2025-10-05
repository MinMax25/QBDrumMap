using System.Windows;
using ControlzEx.Theming;
using libQB.Attributes;
using libQB.Enums;

namespace QBDrumMap.Services
{
    [DISingleton<IThemeSelectorService>]
    public class ThemeSelectorService
        : IThemeSelectorService
    {
        public ThemeSelectorService()
        {
        }

        public void SetTheme(BaseTheme theme, string colorname)
        {
            ThemeManager.Current.ChangeTheme(Application.Current, $"{theme.ToString()}.{colorname}");
        }
    }
}

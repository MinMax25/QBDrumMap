using System.Windows;
using ControlzEx.Theming;
using libQB.Attributes;
using libQB.Enums;

namespace QBDrumMap.Services;

[DISingleton<IThemeSelectorService>]
public class ThemeSelectorService : IThemeSelectorService
{
    public ThemeSelectorService()
    {
    }

    public void SetTheme(BaseTheme theme, string colorname)
    {
        if (Application.Current == null)
        {
            return;
        }

        ThemeManager.Current.ChangeTheme(Application.Current, $"{theme}.{colorname}");
    }
}
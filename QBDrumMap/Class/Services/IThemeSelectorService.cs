using libQB.Enums;

namespace QBDrumMap.Services
{
    public interface IThemeSelectorService
    {
        void SetTheme(BaseTheme theme, string colorname);
    }
}

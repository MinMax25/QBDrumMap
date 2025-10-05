using System.Windows.Controls;

namespace QBDrumMap.Contracts.Services
{
    public interface IPageService
    {
        Type GetPageType(string key);

        Page GetPage(string key);
    }
}
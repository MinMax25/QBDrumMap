using System.Windows.Controls;
using QBDrumMap.Contracts.Services;

namespace QBDrumMap.Services
{
    public class PageService : IPageService
    {
        private readonly Dictionary<string, Type> _pages = [];
        private readonly IServiceProvider _serviceProvider;

        public PageService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Configure(App._pages);
        }

        public Type GetPageType(string key)
        {
            Type pageType;
            lock (_pages)
            {
                if (!_pages.TryGetValue(key, out pageType))
                {
                    throw new ArgumentException($"Page not found: {key}. Did you forget to call PageService.Configure?");
                }
            }

            return pageType;
        }

        public Page GetPage(string key)
        {
            var pageType = GetPageType(key);
            return _serviceProvider.GetService(pageType) as Page;
        }

        private void Configure(Dictionary<Type, Type> items)
        {
            foreach (var item in items)
            {
                lock (_pages)
                {
                    _pages.Add(item.Key.FullName, item.Value);
                }
            }
        }
    }
}

using System.Windows.Controls;
using QBDrumMap.ViewModels;

namespace QBDrumMap.Views
{
    public partial class SettingPage
        : Page
    {
        public SettingPage(SettingViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

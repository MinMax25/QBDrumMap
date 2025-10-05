using System.Windows.Controls;
using QBDrumMap.ViewModels;

namespace QBDrumMap.Views
{
    public partial class ArticulationsPage
        : Page
    {
        public ArticulationsPage(ArticulationsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

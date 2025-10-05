using System.Windows.Controls;
using System.Windows.Data;
using QBDrumMap.Class.MapModels;
using QBDrumMap.ViewModels;

namespace QBDrumMap.Views
{
    public partial class ExportPage
        : Page
    {
        public ExportPage(ExportViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void KitListItems_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = e.Item is KitListItem;
        }
    }
}

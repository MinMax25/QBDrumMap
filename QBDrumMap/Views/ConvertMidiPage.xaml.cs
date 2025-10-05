using System.Windows.Controls;
using System.Windows.Data;
using QBDrumMap.Class.MapModels;
using QBDrumMap.ViewModels;

namespace QBDrumMap.Views
{
    public partial class ConvertMidiPage
        : Page
    {
        public ConvertMidiPage(ConvertMidiViewModel viewModel)
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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QBDrumMap.ViewModels;

namespace QBDrumMap.Views
{
    public partial class PartsPage
        : Page
    {
        public PartsPage(PartsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                FocusManager.SetFocusedElement(this, PartsDataGrid);
                Keyboard.Focus(PartsDataGrid);
            }), System.Windows.Threading.DispatcherPriority.Input);
        }
    }
}

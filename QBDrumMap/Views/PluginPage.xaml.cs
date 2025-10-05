using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QBDrumMap.ViewModels;

namespace QBDrumMap.Views
{
    public partial class PluginPage
        : Page
    {
        public PluginPage(PluginViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                FocusManager.SetFocusedElement(this, PluginDataGrid);
                Keyboard.Focus(PluginDataGrid);
            }), System.Windows.Threading.DispatcherPriority.Input);
        }
    }
}

using System.Diagnostics;
using System.Windows.Navigation;
using MahApps.Metro.Controls;

namespace QBDrumMap.Views
{
    public partial class About
        : MetroWindow
    {
        public About()
        {
            InitializeComponent();
        }

        private void RequestNavigateEventHandler(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}

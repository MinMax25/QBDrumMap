using System.Windows.Input;
using MahApps.Metro.Controls;

namespace QBDrumMap.Views
{
    public partial class ArticulationMapView
        : MetroWindow
    {
        public ArticulationMapView()
        {
            InitializeComponent();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}

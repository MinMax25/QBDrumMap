using System.IO;
using System.Windows;
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

        private void Grid_PreviewDragOver(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files?.Length == 1)
                {
                    string path = files[0];
                    if (Path.GetExtension(path).Equals(".mid", StringComparison.OrdinalIgnoreCase))
                    {
                        e.Effects = DragDropEffects.Copy;
                        e.Handled = true;
                        return;
                    }
                }
            }

            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }
    }
}

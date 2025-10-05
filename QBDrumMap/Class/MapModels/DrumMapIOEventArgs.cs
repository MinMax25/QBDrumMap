using System.Windows;

namespace QBDrumMap.Class.MapModels
{
    public class DrumMapIOEventArgs
        : RoutedEventArgs
    {
        public DrumMapIOEventArgs(bool isSuccessed, Exception ex = null)
        {
            IsSuccessed = isSuccessed;
        }

        public bool IsSuccessed { get; set; }

        public Exception Exception { get; set; }
    }
}

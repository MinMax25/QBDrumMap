using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using libQB.Attributes;
using QBDrumMap.Class;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;
using QBDrumMap.Views.Controls;

namespace QBDrumMap.ViewModels.Controls
{
    [DIUserControl<KitPanel>]
    public partial class KitPanelViewModel
        : ViewModelBaseHasContents
    {
        #region Properties

        public ISettingService Setting => SettingService;

        public IMidiService MIDI => App.GetService<IMidiService>();

        [ObservableProperty]
        private Plugin plugin;

        [ObservableProperty]
        public ICollectionView kitsView;

        [ObservableProperty]
        private ObservableCollection<Kit> selectedKits = [];

        [ObservableProperty]
        private Kit selectedKit;

        [ObservableProperty]
        private bool isExpandedKitList;

        #endregion

        #region Fields

        private bool SameKit;

        #endregion

        #region ctor

        public KitPanelViewModel(IDIContainer diContainer, Plugin plugin)
           : base(diContainer)
        {
            Plugin = plugin;
            if (Plugin != null)
            {
                Plugin.PropertyChanged += Plugin_PropertyChanged;
                ObservableCollection<Kit> kits = [.. Plugin.Kits.OrderBy(x => x.DisplayOrder)];
                Plugin.Kits.Clear();
                kits.ToList().ForEach(Plugin.Kits.Add);
            }
            MIDI.MidiOutDevice = Plugin?.MidiOutDevice ?? string.Empty;

            KitsView = CollectionViewSource.GetDefaultView(Plugin?.Kits);
            ContentViewModel = App.GetService<KitPitchesPanelFactory>()?.Create(null, false);

            IsExpandedKitList = Plugin != null;

            MapData.EditStateChanged += OnPropertyChanged;
            PropertyChanged += OnPropertyChanged;
        }

        #endregion

        #region Methods

        #region Commands

        [RelayCommand]
        private async Task OnDeleteSelectedKits()
        {
            if (SelectedKits == null) return;
            if (SelectedKits.Count == 0) return;

            string names = string.Join("', '", SelectedKits.Select(x => x.Name).ToArray());
            string message = string.Format(libQB.Properties.Resources.Message_Command_Delete, Properties.Name.Kit, names);
            if (await Dialog.ShowConfirmAsync(message, libQB.Properties.Dialog.Title_Command_Delete) == false) return;

            foreach (var k in SelectedKits.ToArray())
            {
                k.Pitches.Clear();
                Plugin?.Kits.Remove(k);
            }

            SelectedKits.Clear();
            SelectedKit = null;

            KitsView.Refresh();
        }

        [RelayCommand]
        private void OnAdd()
        {
            if (Plugin == null) return;

            Kit kit = new();

            int count = (Plugin?.Kits.Count ?? 1) + 1;

            kit.ID = MapData.Plugins.SelectMany(x => x.Kits).GetNewID();
            kit.Name = $"{Plugin.Name} Kit{count}";

            foreach (var p in Enumerable.Range(0, 128))
            {
                kit.Pitches.Add(new KitPitch() { Pitch = p, DisplayOrder = p + 1 });
            }

            Plugin?.Kits.AddItem(kit, x => x.DisplayOrder);

            SelectedKit = kit;
        }

        [RelayCommand]
        private void OnMoveTop() => Plugin?.Kits.MoveTop(SelectedKits, x => x.DisplayOrder);

        [RelayCommand]
        private void OnMoveUp() => Plugin?.Kits.MoveUp(SelectedKits, x => x.DisplayOrder);

        [RelayCommand]
        private void OnMoveDown() => Plugin?.Kits.MoveDown(SelectedKits, x => x.DisplayOrder);

        [RelayCommand]
        private void OnMoveBottom() => Plugin?.Kits.MoveBottom(SelectedKits, x => x.DisplayOrder);

        #endregion

        #region PropertyChanged Callbacks

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(MapData.EditState):
                    IsCommandEnabled = !MapData.EditState;
                    break;
                default:
                    break;
            }
        }

        private void Plugin_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Plugin.MidiOutDevice))
            {
                MIDI.MidiOutDevice = Plugin?.MidiOutDevice ?? string.Empty;
            }
        }

        partial void OnSelectedKitChanged(Kit oldValue, Kit newValue)
        {
            oldValue?.UnregisterUndoManager();
            newValue?.RegisterUndoManager(UndoManager);

            IsCommandEnabled = !MapData.EditState;
            IsContentEnabled = SelectedKit != null;
            if (SelectedKit != (ContentViewModel as KitPitchesPanelViewModel)?.Kit)
            {
                ContentViewModel?.Dispose();
            }
            ContentViewModel = App.GetService<KitPitchesPanelFactory>()?.Create(SelectedKit, SameKit);
            SameKit = true;

            if (newValue == null) return;
            MIDI.SendProgramChange(Plugin, newValue);
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (Plugin != null)
                {
                    Plugin.PropertyChanged -= OnPropertyChanged;
                }
                if (ContentViewModel != null) ContentViewModel.Dispose();
                MapData.EditStateChanged -= OnPropertyChanged;
                PropertyChanged -= OnPropertyChanged;
            }
        }

        #endregion

        #endregion
    }
}
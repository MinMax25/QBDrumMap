using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using libQB.Attributes;
using QBDrumMap.Class;
using QBDrumMap.Class.Cubase;
using QBDrumMap.Class.Extentions;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;
using QBDrumMap.Class.StudioOne;
using QBDrumMap.Views;
using QBDrumMap.Views.Controls;

namespace QBDrumMap.ViewModels.Controls
{
    [DIUserControl<KitPitchesPanel>]
    public partial class KitPitchesPanelViewModel
        : ViewModelBase
    {
        #region Properties

        public ISettingService Setting => SettingService;

        public IMidiService MIDI => App.GetService<IMidiService>();

        public IEnumerable<Kit> BaseOnComboSource => MapData.Plugins
            .OrderBy(x => x.DisplayOrder)
            .SelectMany(x => x.Kits.OrderBy(k => k.DisplayOrder))
            .ToArray();

        [ObservableProperty]
        private bool isSorted = false;

        [ObservableProperty]
        private Kit kit;

        [ObservableProperty]
        private ICollectionView pitchesView;

        [ObservableProperty]
        private string filterName = string.Empty;

        [ObservableProperty]
        private string filterArticulation = string.Empty;

        [ObservableProperty]
        private ObservableCollection<KitPitch> selectedPitches = new();

        [ObservableProperty]
        private KitPitch selectedPitch;

        [ObservableProperty]
        private Kit articulationSource;

        [ObservableProperty]
        private bool isCommandEnabled;

        #endregion

        #region ctor

        public KitPitchesPanelViewModel(IDIContainer diContainer, Kit kit)
            : base(diContainer)
        {
            Kit = kit;

            if (Kit != null)
            {
                var pitches = new ObservableCollection<KitPitch>(Kit.Pitches.OrderBy(x => x.DisplayOrder));
                Kit.Pitches.Clear();
                foreach (var p in pitches) Kit.Pitches.Add(p);
            }

            PitchesView = CollectionViewSource.GetDefaultView(Kit?.Pitches);
            if (PitchesView != null)
                PitchesView.Filter = FilterPitches;

            SetIsCommandEnabled();

            MapData.EditStateChanged += OnPropertyChanged;
            PropertyChanged += OnPropertyChanged;

            MIDI.MidiThruEnabled = (Kit != null);
        }

        #endregion

        #region Methods

        #region Commands

        [RelayCommand]
        private void OnSendNoteOn(object value)
        {
            if (value is not KitPitch pitch) return;
            MIDI.SendNoteOn(pitch.Pitch);
        }

        [RelayCommand]
        private void OnSendNoteOff(object value)
        {
            if (value is not KitPitch pitch) return;
            MIDI.SendNoteOff(pitch.Pitch);
        }

        [RelayCommand]
        private async Task OnClearSelectedPitches()
        {
            if (SelectedPitches.Count == 0) return;

            string message = string.Format(libQB.Properties.Resources.Message_Command_Clear, Properties.Name.KitPitch);
            if (await Dialog.ShowConfirmAsync(message, libQB.Properties.Dialog.Title_Command_Clear) == false) return;

            foreach (var item in SelectedPitches)
            {
                item.Name = null;
                item.ArticulationID = 0;
                item.Separator = Separator.None;
            }
        }

        [RelayCommand]
        private async Task OnClearArticulation(object value)
        {
            if (value is not KitPitch pitch) return;

            string message = string.Format(libQB.Properties.Resources.Message_Command_Clear, Properties.Name.Articulation);
            if (await Dialog.ShowConfirmAsync(message, libQB.Properties.Dialog.Title_Command_Clear) == false) return;

            pitch.ArticulationID = 0;
        }

        [RelayCommand]
        private async Task OnImport()
        {
            string filter = "Drum Map|*.pitchlist;*.drm;*.tsv";
            string path = await Dialog.ShowOpenFileDialog(Properties.Resources.ImportDrumMap, filter, Setting.LastImportFilePath);
            if (path == null) return;

            Setting.LastImportFilePath = path;
            await TryIOFunction(
                "Import",
                () =>
                {
                    switch (Path.GetExtension(path))
                    {
                        case ".pitchlist": ImportPitchlist(path); break;
                        case ".drm": ImportDrm(path); break;
                        case ".tsv": ImportTSVText(path); break;
                        default: throw new ArgumentException("File Extention is Invalid!");
                    }
                },
                Path.GetFileName(path)
            );
        }

        [RelayCommand]
        private void OnMoveTop() => Kit.Pitches.MoveTop(SelectedPitches, x => x.DisplayOrder);

        [RelayCommand]
        private void OnMoveUp() => Kit.Pitches.MoveUp(SelectedPitches, x => x.DisplayOrder);

        [RelayCommand]
        private void OnMoveDown() => Kit.Pitches.MoveDown(SelectedPitches, x => x.DisplayOrder);

        [RelayCommand]
        private void OnMoveBottom() => Kit.Pitches.MoveBottom(SelectedPitches, x => x.DisplayOrder);

        [RelayCommand]
        private void OnSearchArticulation(object value)
        {
            if (value is not KitPitch pitch) return;

            WindowService.ShowWindowWithCallback<SearchArticulation, SearchArticulationViewModel, Articulation>(
                parameter: pitch,
                owner: Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive),
                resultCallback: async articulation =>
                {
                    if (articulation == null) return;
                    var hasMany = Kit.Pitches.Count(x => x.Name == pitch.Name) > 1;
                    string message = Properties.Resources.ArticulationAutomaticFill;

                    if (hasMany && (await Dialog.ShowConfirmAsync(Properties.Name.Articulation, message)).GetValueOrDefault())
                    {
                        foreach (var kitPitch in Kit.Pitches.Where(x => x.Name == pitch.Name))
                        {
                            kitPitch.ArticulationID = articulation.ID;
                        }
                    }
                    else
                    {
                        pitch.ArticulationID = articulation.ID;
                    }
                });
        }

        [RelayCommand]
        private void OnShowArticulationMap()
        {
            if (Kit == null) return;

            WindowService.ShowWindowWithCallback<ArticulationMapView, ArticulationMapViewModel, object>(
                parameter: Kit,
                owner: Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive));
        }

        [RelayCommand]
        private async Task OnResetOrdrByPitch()
        {
            if (Kit == null) return;
            if (await Dialog.ShowConfirmAsync(Properties.Resources.ResetOrderByPitch, Properties.Resources.ResetOrderByPitchConfirm) == false) return;

            foreach (var kitPitch in Kit.Pitches.ToArray())
            {
                kitPitch.DisplayOrder = kitPitch.Pitch + 1;
            }

            Kit.Pitches.SortRefresh(x => x.DisplayOrder);
        }

        [RelayCommand]
        private async Task OnCompressForward()
        {
            if (Kit == null) return;
            if (await Dialog.ShowConfirmAsync(Properties.Resources.CompressForward, Properties.Resources.CompressForwardConfirm) == false) return;

            int count = 1;
            var pitches = Kit.Pitches.OrderBy(x => x.DisplayOrder).ToArray();

            foreach (var kitPitch in pitches.Where(x => !string.IsNullOrWhiteSpace(x.Name)))
            {
                kitPitch.DisplayOrder = count++;
            }

            foreach (var kitPitch in pitches.Where(x => string.IsNullOrWhiteSpace(x.Name)))
            {
                kitPitch.DisplayOrder = count++;
            }

            Kit.Pitches.SortRefresh(x => x.DisplayOrder);
        }

        [RelayCommand]
        private async Task OnExportStudioOne()
        {
            if (Kit == null) return;
            string title = "Export Studio One";

            var path = await Dialog.ShowSelectFolderDialog(title, Setting.ExportStudioOneDefaultPath);
            if (path == null) return;
            Setting.ExportStudioOneDefaultPath = path;

            var pitchList = MapData.GetStudioOnePitchList(Kit.Name);
            string filePath = Path.Combine(path, $"{pitchList.Title}.pitchlist");

            await TryIOFunction(title, () => pitchList.Save(filePath), filePath);
        }

        [RelayCommand]
        private async Task OnExportCubase()
        {
            if (Kit == null) return;
            string title = "Export Cubase";

            var path = await Dialog.ShowSelectFolderDialog(title, Setting.ExportCubaseDefaultPath);
            if (path == null) return;
            Setting.ExportCubaseDefaultPath = path;

            var cubase = MapData.GetCubaseDrumMap(Kit.Name);
            string filePath = Path.Combine(path, $"{Kit.Name}.drm");

            await TryIOFunction(title, () => cubase.Save(filePath), filePath);
        }

        [RelayCommand]
        private async Task OnExportText()
        {
            if (Kit == null) return;
            string title = "Export .tsv";

            var path = await Dialog.ShowSelectFolderDialog(title, Setting.ExportTextDefaultPath);
            if (path == null) return;
            Setting.ExportTextDefaultPath = path;

            var sb = MapData.GetText(Kit.Name);
            string filePath = Path.Combine(path, $"{Kit.Name}.tsv");

            await TryIOFunction(title, () => File.WriteAllText(filePath.ToSafeFilePath(), sb.ToString()), filePath);
        }

        [RelayCommand]
        private async Task OnCopyArticulation()
        {
            if (Kit == null) return;
            if (ArticulationSource == null)
            {
                await Dialog.ShowErrorAsync(Properties.Resources.ContextKitPitchesCopyArticulation, Properties.Resources.ContextKitPitchesArticulationSourceIsEmpty);
                return;
            }

            if (await Dialog.ShowConfirmAsync(Properties.Resources.ContextKitPitchesCopyArticulation, Properties.Resources.PasteConfirm) == false) return;

            foreach (var pitch in Enumerable.Range(0, 128))
            {
                if (!string.IsNullOrWhiteSpace(Kit.Pitches[pitch].Name))
                {
                    Kit.Pitches[pitch].ArticulationID = ArticulationSource.Pitches[pitch].ArticulationID;
                    Kit.Pitches[pitch].Separator = ArticulationSource.Pitches[pitch].Separator;
                }
                else
                {
                    Kit.Pitches[pitch].ArticulationID = 0;
                    Kit.Pitches[pitch].Separator = Separator.None;
                }
            }
        }

        #endregion

        #region PropertyChanged Callbacks

        partial void OnSelectedPitchChanged(KitPitch oldValue, KitPitch newValue)
        {
            oldValue?.UnregisterUndoManager();
            newValue?.RegisterUndoManager(UndoManager);

            if (MIDI.MidiInFixedPitch > 0)
            {
                MIDI.SetSoundCheckPitch(newValue?.Pitch ?? -1);
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(FilterName):
                case nameof(FilterArticulation):
                    SetIsCommandEnabled();
                    PitchesView?.Refresh();
                    break;
                case nameof(IsSorted):
                case nameof(MapData.EditState):
                    SetIsCommandEnabled();
                    break;
            }
        }

        #endregion

        #region General

        private void SetIsCommandEnabled()
        {
            IsCommandEnabled =
                !IsSorted && !MapData.EditState
                && string.IsNullOrWhiteSpace(FilterName)
                && string.IsNullOrWhiteSpace(FilterArticulation);
        }

        private bool FilterPitches(object obj)
        {
            if (obj is not KitPitch kitPitch) return true;

            bool result = true;

            if (!string.IsNullOrEmpty(FilterName))
            {
                result &= kitPitch.Name.Like(FilterName);
            }

            if (!string.IsNullOrEmpty(FilterArticulation))
            {
                var articulation = MapData.Articulations.FirstOrDefault(a => a.ID == kitPitch.ArticulationID);
                result &= articulation != null && articulation.Name.Like(FilterArticulation);
            }

            return result;
        }

        private void ImportPitchlist(string path)
        {
            if (Kit == null) return;

            if (StudioOne.Load(path) is not PitchList pitchlist) throw new Exception();

            Kit.Name = Microsoft.VisualBasic.Strings.Left(pitchlist.Title, ExModel.NAME_MAX_LENGTH);

            foreach (var item in Kit.Pitches.ToArray())
            {
                if (pitchlist.Items.FirstOrDefault(x => x.Pitch == item.Pitch) is PitchName pitchName)
                {
                    item.Name = Microsoft.VisualBasic.Strings.Left(pitchName.Name, ExModel.NAME_MAX_LENGTH);
                    item.ArticulationID = 0;
                    item.Separator = Separator.None;
                    item.DisplayOrder = pitchlist.Items.IndexOf(pitchName) + 1;
                }
                else
                {
                    item.Name = string.Empty;
                    item.ArticulationID = 0;
                    item.Separator = Separator.None;
                    item.DisplayOrder = 1000 + item.Pitch;
                }
            }

            int i = 1;
            foreach (var kitPitch in Kit.Pitches.OrderBy(x => x.DisplayOrder).ToArray())
            {
                kitPitch.DisplayOrder = i++;
            }

            Kit.Pitches.SortRefresh(x => x.DisplayOrder);
        }

        private void ImportDrm(string path)
        {
            if (Kit == null) return;

            if (CubaseDrumMap.Load(path) is not CubaseDrumMap drm) throw new Exception();

            Kit.Name = Microsoft.VisualBasic.Strings.Left(drm.Name, ExModel.NAME_MAX_LENGTH);

            foreach (var item in Kit.Pitches.ToArray())
            {
                if (drm.Map.Items.FirstOrDefault(x => x.INote == item.Pitch) is MapItem mi)
                {
                    item.Name = Microsoft.VisualBasic.Strings.Left(mi.Name, ExModel.NAME_MAX_LENGTH);
                    item.ArticulationID = 0;
                    item.Separator = Separator.None;
                    item.DisplayOrder = drm.Order.IndexOf(item.Pitch) + 1;
                }
                else
                {
                    item.Name = string.Empty;
                    item.ArticulationID = 0;
                    item.Separator = Separator.None;
                    item.DisplayOrder = 1000 + item.Pitch;
                }
            }

            int i = 1;
            foreach (var kitPitch in Kit.Pitches.OrderBy(x => x.DisplayOrder).ToArray())
            {
                kitPitch.DisplayOrder = i++;
            }

            Kit.Pitches.SortRefresh(x => x.DisplayOrder);
        }

        private void ImportTSVText(string path)
        {
            if (Kit == null) return;

            Kit.Name = Microsoft.VisualBasic.Strings.Left(Path.GetFileNameWithoutExtension(path), ExModel.NAME_MAX_LENGTH);
            var text = File.ReadAllText(path);
            var lines = text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length == 0) throw new Exception();
            var header = lines[0].Split('\t');
            if (!header.SequenceEqual(new[] { "Pitch", "Note", "Name", "Articulation", "Separator" })) throw new Exception();

            foreach (var kitPitch in Kit.Pitches.ToArray())
            {
                kitPitch.DisplayOrder = 999;
            }

            int order = 1;
            foreach (var line in lines.Skip(1))
            {
                var items = line.Split('\t');
                if (items.Length != 5) throw new Exception();
                if (!int.TryParse(items[0], out int pitch)) throw new Exception();

                var name = Microsoft.VisualBasic.Strings.Left(items[2], ExModel.NAME_MAX_LENGTH);
                var articulationName = items[3];
                var separatorName = items[4];

                if (string.IsNullOrWhiteSpace(name))
                {
                    articulationName = string.Empty;
                    separatorName = string.Empty;
                }

                int articulationID = 0;
                if (MapData.Parts.SelectMany(a => a.Articulations)
                    .FirstOrDefault(a => a.Name == articulationName) is Articulation articulation)
                {
                    articulationID = articulation.ID;
                }

                Separator separator = Separator.None;
                if (Enum.TryParse<Separator>(separatorName, out var sep))
                {
                    separator = sep;
                }

                if (Kit.Pitches.FirstOrDefault(x => x.Pitch == pitch) is not KitPitch kitPitch) throw new Exception();

                kitPitch.Name = name;
                kitPitch.ArticulationID = articulationID;
                kitPitch.Separator = separator;
                kitPitch.DisplayOrder = order++;
            }

            order = 1;
            foreach (var kitPitch in Kit.Pitches.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Pitch).ToArray())
            {
                kitPitch.DisplayOrder = order++;
            }

            Kit.Pitches.SortRefresh(x => x.DisplayOrder);
        }

        protected async Task TryIOFunction(string title, Action action, string filePath)
        {
            try
            {
                action.Invoke();
                await Dialog.ShowInformationAsync(Path.GetFileName(filePath.ToSafeFilePath()) ?? libQB.Properties.Resources.Message_ProcessComplete, title);
            }
            catch (Exception ex)
            {
                await Dialog.ShowErrorAsync(ex.Message, title);
            }
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                MapData.EditStateChanged -= OnPropertyChanged;
                PropertyChanged -= OnPropertyChanged;
            }
        }

        #endregion

        #endregion
    }
}

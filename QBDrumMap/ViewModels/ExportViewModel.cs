using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using libQB.Attributes;
using QBDrumMap.Class;
using QBDrumMap.Class.Extentions;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;
using QBDrumMap.Views;

namespace QBDrumMap.ViewModels
{
    [DIPage<ExportPage>]
    public partial class ExportViewModel
        : ViewModelBase
    {
        #region Properties

        public ISettingService Setting => SettingService;

        public IEnumerable<Kit> BaseOnComboSource => MapData.Plugins.OrderBy(x => x.DisplayOrder).SelectMany(x => x.Kits.OrderBy(k => k.DisplayOrder)).ToArray();

        private ObservableCollection<KitListItem> Kits = new();

        [ObservableProperty]
        private ICollectionView kitListView;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ExportStudioOneCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportCubaseCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportCubaseBaseOnCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportNoteMapperCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportTextCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportQBDrummerCommand))]
        private ObservableCollection<KitListItem> selectedKits = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ExportStudioOneCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportCubaseCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportCubaseBaseOnCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportNoteMapperCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportTextCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportQBDrummerCommand))]
        private Kit baseOn;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ExportStudioOneCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportCubaseCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportCubaseBaseOnCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportNoteMapperCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportTextCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportQBDrummerCommand))]
        private bool canExport;

        private bool CanExportBaseOn => BaseOn != null && CanExport;

        #endregion

        #region ctor

        public ExportViewModel(IDIContainer diContainer)
            : base(diContainer)
        {
            Task.Run(InitializeDataAsync);

            MapData.Loaded += OnMapDataLoaded;
            MapData.Saved += OnMapDataSaved;
        }

        #endregion

        #region Methods

        #region Commands

        [RelayCommand]
        private void OnSelectedKitsSelectionChanged(int count)
        {
            CanExport = count > 0;
        }

        [RelayCommand(CanExecute = nameof(CanExport))]
        private async Task OnExportStudioOne()
        {
            string title = "Studio One .Pitchlist";

            var path = await SelectFolderAsync(title, () => Setting.ExportStudioOneDefaultPath, p => Setting.ExportStudioOneDefaultPath = p);
            if (path == null) return;

            await TryIOFunction(title, () =>
            {
                foreach (var kit in SelectedKits)
                {
                    var outPath = Path.Combine(path, kit.PluginName);
                    if (!File.Exists(outPath))
                    {
                        Directory.CreateDirectory(outPath);
                    }
                    var pitchList = MapData.GetStudioOnePitchList(kit.KitName);
                    pitchList.Save(Path.Combine(outPath, $"{pitchList.Title}.pitchlist"));
                }
            });
        }

        [RelayCommand(CanExecute = nameof(CanExport))]
        private async Task OnExportCubase()
        {
            string title = "Cubase .drm";

            var path = await SelectFolderAsync(title, () => Setting.ExportCubaseDefaultPath, p => Setting.ExportCubaseDefaultPath = p);
            if (path == null) return;

            await TryIOFunction(title, () =>
            {
                foreach (var kit in SelectedKits)
                {
                    var outPath = Path.Combine(path, kit.PluginName);
                    if (!File.Exists(outPath))
                    {
                        Directory.CreateDirectory(outPath);
                    }
                    var cubase = MapData.GetCubaseDrumMap(kit.KitName);
                    cubase.Save(Path.Combine(outPath, $"{kit.KitName}.drm"));
                }
            });
        }

        [RelayCommand(CanExecute = nameof(CanExport))]
        private async Task OnExportText()
        {
            string title = "Text .tsv";

            var path = await SelectFolderAsync(title, () => Setting.ExportTextDefaultPath, p => Setting.ExportTextDefaultPath = p);
            if (path == null) return;

            await TryIOFunction(title, () =>
            {
                foreach (var kit in SelectedKits)
                {
                    var outPath = Path.Combine(path, kit.PluginName);
                    if (!File.Exists(outPath))
                    {
                        Directory.CreateDirectory(outPath);
                    }
                    var sb = MapData.GetText(kit.KitName);
                    File.WriteAllText(Path.Combine(outPath, $"{kit.KitName}.tsv"), sb.ToString());
                }
            });
        }

        [RelayCommand(CanExecute = nameof(CanExportBaseOn))]
        private async Task OnExportCubaseBaseOn()
        {
            string title = $"Cubase .drm Base On '{BaseOn?.Name}'";

            var path = await SelectFolderAsync(title, () => Setting.ExportCubaseBaseOnDefaultPath, p => Setting.ExportCubaseBaseOnDefaultPath = p);
            if (path == null) return;

            await TryIOFunction(title, () =>
            {
                foreach (var kit in SelectedKits)
                {
                    var outPath = Path.Combine(path, kit.PluginName);
                    if (!File.Exists(outPath))
                    {
                        Directory.CreateDirectory(outPath);
                    }
                    var cubase = MapData.GetCubaseDrumMap(kit.KitName, BaseOn.Name);
                    cubase.Save(Path.Combine(outPath, $"{kit.KitName}.drm"));
                }
            });
        }

        [RelayCommand(CanExecute = nameof(CanExportBaseOn))]
        private async Task OnExportNoteMapper()
        {
            string title = $"NoteMapper Template Base On '{BaseOn?.Name}'";

            var path = await SelectFolderAsync(title, () => Setting.ExportNoteMapperDefaultPath, p => Setting.ExportNoteMapperDefaultPath = p);
            if (path == null) return;

            await TryIOFunction(title, () =>
            {
                var sb = new StringBuilder();

                sb.AppendLine(">Forward Conversion");

                foreach (var kit in GetOrderedSelectedKits())
                {
                    if (ArticulationMap.GetArticulationMap(MapData, kit.KitName) is not ArticulationMap articMap)
                        throw new Exception();

                    sb.Append($"{BaseOn.Name} To {kit.KitName};0,127;");

                    var pairs = BaseOn.Pitches
                        .Where(x => x.ArticulationID > 0)
                        .OrderBy(x => x.Pitch)
                        .Select(a => new
                        {
                            From = a.Pitch,
                            To = articMap.Items.FirstOrDefault(x => x.ID == a.ArticulationID)?.Pitches.FirstOrDefault()
                        })
                        .Where(x => x.To.HasValue)
                        .Select(x => $"{x.From}>{x.To.Value}");

                    sb.AppendLine(string.Join(",", pairs));
                }

                sb.AppendLine(">Reverse Conversion");

                if (ArticulationMap.GetArticulationMap(MapData, BaseOn.Name) is not ArticulationMap baseOnMap)
                    throw new Exception();

                foreach (var kit in GetOrderedSelectedKits())
                {
                    sb.Append($"{kit.KitName} To {BaseOn.Name};0,127;");

                    var pitches = MapData.Plugins.SelectMany(x => x.Kits).FirstOrDefault(x => x.ID == kit.KitID)?.Pitches;
                    if (pitches == null) throw new Exception();

                    var pairs = pitches
                        .Where(x => x.ArticulationID > 0)
                        .OrderBy(x => x.Pitch)
                        .Select(a => new
                        {
                            From = a.Pitch,
                            To = baseOnMap.Items.FirstOrDefault(x => x.ID == a.ArticulationID)?.Pitches.FirstOrDefault()
                        })
                        .Where(x => x.To.HasValue)
                        .Select(x => $"{x.From}>{x.To.Value}");

                    sb.AppendLine(string.Join(",", pairs));
                }

                File.WriteAllText(Path.Combine(path, "Templates.txt"), sb.ToString(), Encoding.UTF8);
            });
        }

        [RelayCommand(CanExecute = nameof(CanExport))]
        private async Task OnExportQBDrummer()
        {
            string title = "QB Drummer Preset .csv";

            var path = await SelectFolderAsync(title, () => Setting.ExportQBDrummerDefaultPath, p => Setting.ExportQBDrummerDefaultPath = p);
            if (path == null) return;

            await TryIOFunction(title, () =>
            {
                foreach (var kit in SelectedKits)
                {
                    if (ArticulationMap.GetArticulationMap(MapData, kit.KitName) is not ArticulationMap articMap)
                        throw new Exception();

                    var outPath = Path.Combine(path, kit.PluginName);
                    if (!File.Exists(outPath))
                    {
                        Directory.CreateDirectory(outPath);
                    }

                    var sb = new StringBuilder();
                    sb.AppendLine("ID,Sub,Name,Pitchs");

                    foreach (var item in articMap.Items.OrderBy(m => m.ID))
                    {
                        sb.AppendLine($"{item.ID},{(item.IsSub ? 1 : 0)},{item.Name},{string.Join("|", item.Pitches)}");
                    }

                    File.WriteAllText(Path.Combine(outPath, $"{kit.KitName}.csv"), sb.ToString());
                }
            });
        }

        #endregion

        #region Event Handling

        private async void OnMapDataLoaded(object sender, RoutedEventArgs e)
        {
            await InitializeDataAsync();
        }

        private async void OnMapDataSaved(object sender, DrumMapIOEventArgs e)
        {
            await InitializeDataAsync();
        }

        #endregion

        #region General

        private async Task InitializeDataAsync()
        {
            SelectedKits.Clear();

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                KitListView = null;

                Kits.Clear();
                foreach (var plugin in MapData.Plugins.OrderBy(x => x.DisplayOrder))
                {
                    foreach (var kit in plugin.Kits.OrderBy(x => x.DisplayOrder))
                    {
                        Kits.Add(new KitListItem
                        {
                            PluginID = plugin.ID,
                            PluginName = plugin.Name,
                            PluginDisplayOrder = plugin.DisplayOrder,
                            KitID = kit.ID,
                            KitName = kit.Name,
                            KitDisplayOrder = kit.DisplayOrder,
                        });
                    }
                }

                KitListView = CollectionViewSource.GetDefaultView(Kits);
                KitListView.GroupDescriptions.Clear();
                KitListView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(KitListItem.PluginName)));
                KitListView.Refresh();
            });
        }

        private IEnumerable<KitListItem> GetOrderedSelectedKits()
        {
            return SelectedKits.OrderBy(x => x.PluginDisplayOrder).ThenBy(x => x.KitDisplayOrder);
        }

        private async Task<string> SelectFolderAsync(string title, Func<string> getDefaultPath, Action<string> setDefaultPath)
        {
            var path = await Dialog.ShowSelectFolderDialog(title, getDefaultPath());
            if (path != null)
            {
                setDefaultPath(path);
            }
            return path;
        }

        protected async Task TryIOFunction(string title, Action action, string successMessage = null)
        {
            try
            {
                action.Invoke();
                await Dialog.ShowInformationAsync(successMessage ?? libQB.Properties.Resources.Message_ProcessComplete);
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
                MapData.Loaded -= OnMapDataLoaded;
                MapData.Saved -= OnMapDataSaved;
            }
        }

        #endregion

        #endregion
    }
}

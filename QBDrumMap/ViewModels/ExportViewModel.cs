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
using QBDrumMap.Class.Extensions;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;
using QBDrumMap.Views;

namespace QBDrumMap.ViewModels
{
    [DIPage<ExportPage>]
    public partial class ExportViewModel : ViewModelBase
    {
        #region Fields

        // キット一覧の生データ
        private ObservableCollection<KitListItem> _kits = new();

        // キットリストのビュー（グルーピング用）
        [ObservableProperty]
        private ICollectionView _kitListView;

        // 選択されているキットのリスト
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ExportStudioOneCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportCubaseCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportCubaseBaseOnCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportNoteMapperCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportTextCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportQBDrummerCommand))]
        private ObservableCollection<KitListItem> _selectedKits = new();

        // 変換・エクスポートの基準となるキット
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ExportStudioOneCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportCubaseCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportCubaseBaseOnCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportNoteMapperCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportTextCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportQBDrummerCommand))]
        private Kit _baseOn;

        // エクスポートが実行可能な状態かどうか
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ExportStudioOneCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportCubaseCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportCubaseBaseOnCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportNoteMapperCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportTextCommand))]
        [NotifyCanExecuteChangedFor(nameof(ExportQBDrummerCommand))]
        private bool _canExport;

        // グループの展開状態
        [ObservableProperty]
        private bool _isGroupExpanded;

        #endregion

        #region Properties

        // 設定サービスへの参照
        public ISettingService Setting
        {
            get
            {
                return SettingService;
            }
        }

        // コンボボックスに表示する基準キットのソース
        public IEnumerable<Kit> BaseOnComboSource
        {
            get
            {
                return MapData.Plugins
                    .OrderBy(x =>
                    {
                        return x.DisplayOrder;
                    })
                    .SelectMany(x =>
                    {
                        return x.Kits.OrderBy(k =>
                        {
                            return k.DisplayOrder;
                        });
                    })
                    .ToArray();
            }
        }

        // 基準キットが指定されている場合のみ実行可能な判定
        private bool CanExportBaseOn
        {
            get
            {
                return BaseOn != null && CanExport;
            }
        }

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

            var path = await SelectFolderAsync(
                title,
                () => Setting.ExportStudioOneDefaultPath,
                p => Setting.ExportStudioOneDefaultPath = p);

            if (path == null)
            {
                return;
            }

            await TryIOFunction(title, () =>
            {
                foreach (var kit in SelectedKits)
                {
                    var outPath = Path.Combine(path, kit.PluginName);
                    if (!Directory.Exists(outPath))
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

            var path = await SelectFolderAsync(
                title,
                () => Setting.ExportCubaseDefaultPath,
                p => Setting.ExportCubaseDefaultPath = p);

            if (path == null)
            {
                return;
            }

            await TryIOFunction(title, () =>
            {
                foreach (var kit in SelectedKits)
                {
                    var outPath = Path.Combine(path, kit.PluginName);
                    if (!Directory.Exists(outPath))
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

            var path = await SelectFolderAsync(
                title,
                () => Setting.ExportTextDefaultPath,
                p => Setting.ExportTextDefaultPath = p);

            if (path == null)
            {
                return;
            }

            await TryIOFunction(title, () =>
            {
                foreach (var kit in SelectedKits)
                {
                    var outPath = Path.Combine(path, kit.PluginName);
                    if (!Directory.Exists(outPath))
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

            var path = await SelectFolderAsync(
                title,
                () => Setting.ExportCubaseBaseOnDefaultPath,
                p => Setting.ExportCubaseBaseOnDefaultPath = p);

            if (path == null)
            {
                return;
            }

            await TryIOFunction(title, () =>
            {
                foreach (var kit in SelectedKits)
                {
                    var outPath = Path.Combine(path, kit.PluginName);
                    if (!Directory.Exists(outPath))
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

            var path = await SelectFolderAsync(
                title,
                () => Setting.ExportNoteMapperDefaultPath,
                p => Setting.ExportNoteMapperDefaultPath = p);

            if (path == null)
            {
                return;
            }

            await TryIOFunction(title, () =>
            {
                var sb = new StringBuilder();
                sb.AppendLine(">Forward Conversion");

                foreach (var kit in GetOrderedSelectedKits())
                {
                    var articMap = ArticulationMap.GetArticulationMap(MapData, kit.KitName);
                    if (articMap == null)
                    {
                        throw new Exception("Articulation map not found.");
                    }

                    sb.Append($"{BaseOn.Name} To {kit.KitName};0,127;");

                    var pairs = BaseOn.Pitches
                        .Where(x =>
                        {
                            return x.ArticulationID > 0;
                        })
                        .OrderBy(x =>
                        {
                            return x.Pitch;
                        })
                        .Select(a =>
                        {
                            var target = articMap.Items.FirstOrDefault(x =>
                            {
                                return x.ID == a.ArticulationID;
                            });

                            return new
                            {
                                From = a.Pitch,
                                To = target?.Pitches.FirstOrDefault()
                            };
                        })
                        .Where(x =>
                        {
                            return x.To.HasValue;
                        })
                        .Select(x =>
                        {
                            return $"{x.From}>{x.To.Value}";
                        });

                    sb.AppendLine(string.Join(",", pairs));
                }

                sb.AppendLine(">Reverse Conversion");

                var baseOnMap = ArticulationMap.GetArticulationMap(MapData, BaseOn.Name);
                if (baseOnMap == null)
                {
                    throw new Exception("Base articulation map not found.");
                }

                foreach (var kit in GetOrderedSelectedKits())
                {
                    sb.Append($"{kit.KitName} To {BaseOn.Name};0,127;");

                    var targetKit = MapData.Plugins
                        .SelectMany(x =>
                        {
                            return x.Kits;
                        })
                        .FirstOrDefault(x =>
                        {
                            return x.ID == kit.KitID;
                        });

                    if (targetKit == null)
                    {
                        throw new Exception("Kit not found.");
                    }

                    var pairs = targetKit.Pitches
                        .Where(x =>
                        {
                            return x.ArticulationID > 0;
                        })
                        .OrderBy(x =>
                        {
                            return x.Pitch;
                        })
                        .Select(a =>
                        {
                            var target = baseOnMap.Items.FirstOrDefault(x =>
                            {
                                return x.ID == a.ArticulationID;
                            });

                            return new
                            {
                                From = a.Pitch,
                                To = target?.Pitches.FirstOrDefault()
                            };
                        })
                        .Where(x =>
                        {
                            return x.To.HasValue;
                        })
                        .Select(x =>
                        {
                            return $"{x.From}>{x.To.Value}";
                        });

                    sb.AppendLine(string.Join(",", pairs));
                }

                File.WriteAllText(Path.Combine(path, "Templates.txt"), sb.ToString(), Encoding.UTF8);
            });
        }

        [RelayCommand(CanExecute = nameof(CanExport))]
        private async Task OnExportQBDrummer()
        {
            string title = "QB Drummer Preset .csv";

            var path = await SelectFolderAsync(
                title,
                () => Setting.ExportQBDrummerDefaultPath,
                p => Setting.ExportQBDrummerDefaultPath = p);

            if (path == null)
            {
                return;
            }

            await TryIOFunction(title, () =>
            {
                foreach (var kit in SelectedKits)
                {
                    var articMap = ArticulationMap.GetArticulationMap(MapData, kit.KitName);
                    if (articMap == null)
                    {
                        throw new Exception("Articulation map not found.");
                    }

                    var outPath = Path.Combine(path, kit.PluginName);
                    if (!Directory.Exists(outPath))
                    {
                        Directory.CreateDirectory(outPath);
                    }

                    var sb = new StringBuilder();
                    sb.AppendLine("ID,Sub,Name,Pitchs");

                    var orderedItems = articMap.Items.OrderBy(m =>
                    {
                        return m.ID;
                    });

                    foreach (var item in orderedItems)
                    {
                        int subValue = item.IsSub ? 1 : 0;
                        string pitches = string.Join("|", item.Pitches);
                        sb.AppendLine($"{item.ID},{subValue},{item.Name},{pitches}");
                    }

                    File.WriteAllText(Path.Combine(outPath, $"{kit.KitName}.csv"), sb.ToString());
                }
            });
        }

        [RelayCommand]
        private void OnExpandAll()
        {
            IsGroupExpanded = true;
        }

        [RelayCommand]
        private void OnShrinkAll()
        {
            IsGroupExpanded = false;
        }

        #endregion

        #region EventHandler

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
                _kits.Clear();

                foreach (var plugin in MapData.Plugins.OrderBy(x => x.DisplayOrder))
                {
                    foreach (var kit in plugin.Kits.OrderBy(x => x.DisplayOrder))
                    {
                        _kits.Add(new KitListItem
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

                KitListView = CollectionViewSource.GetDefaultView(_kits);
                KitListView.GroupDescriptions.Clear();
                KitListView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(KitListItem.PluginName)));
                KitListView.Refresh();
            });

            OnPropertyChanged(nameof(BaseOnComboSource));
        }

        private IEnumerable<KitListItem> GetOrderedSelectedKits()
        {
            return SelectedKits
                .OrderBy(x =>
                {
                    return x.PluginDisplayOrder;
                })
                .ThenBy(x =>
                {
                    return x.KitDisplayOrder;
                });
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
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
using QBDrumMap.Class.Extensions;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;
using QBDrumMap.Class.StudioOne;
using QBDrumMap.Views;
using QBDrumMap.Views.Controls;

namespace QBDrumMap.ViewModels.Controls
{
    [DIUserControl<KitPitchesPanel>]
    public partial class KitPitchesPanelViewModel : ViewModelBase
    {
        #region Fields

        // 前回と同じキットを選択しているかどうかのフラグ
        private bool _sameKit;

        // 表示順でソートされているかどうか
        [ObservableProperty]
        private bool _isSorted = false;

        // 現在編集対象となっているキットモデル
        [ObservableProperty]
        private Kit _kit;

        // ピッチリストのビュー（フィルタリング・表示用）
        [ObservableProperty]
        private ICollectionView _pitchesView;

        // 名前のフィルタリング文字列
        [ObservableProperty]
        private string _filterName = string.Empty;

        // アーティキュレーションのフィルタリング文字列
        [ObservableProperty]
        private string _filterArticulation = string.Empty;

        // 選択されているピッチのコレクション（複数選択用）
        [ObservableProperty]
        private ObservableCollection<KitPitch> _selectedPitches = new();

        // 単一選択されているピッチ
        [ObservableProperty]
        private KitPitch _selectedPitch;

        // アーティキュレーション情報のコピー元となるキット
        [ObservableProperty]
        private Kit _articulationSource;

        // 各種コマンドが実行可能かどうか
        [ObservableProperty]
        private bool _isCommandEnabled;

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

        // MIDIサービスへの参照
        public IMidiService MIDI
        {
            get
            {
                return App.GetService<IMidiService>();
            }
        }

        // アーティキュレーションコピーのソース選択用リスト
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

        #endregion

        #region ctor

        public KitPitchesPanelViewModel(IDIContainer diContainer, Kit kit, bool sameKit)
            : base(diContainer)
        {
            Kit = kit;
            _sameKit = sameKit;

            // 前回と異なるキットの場合は拡張プログラムチェンジ設定をリセット
            Setting.IsExtendedProgramChange &= _sameKit;

            if (Kit != null)
            {
                var pitches = new ObservableCollection<KitPitch>(Kit.Pitches.OrderBy(x =>
                {
                    return x.DisplayOrder;
                }));

                Kit.Pitches.Clear();
                foreach (var p in pitches)
                {
                    Kit.Pitches.Add(p);
                }
            }

            PitchesView = CollectionViewSource.GetDefaultView(Kit?.Pitches);
            if (PitchesView != null)
            {
                PitchesView.Filter = FilterPitches;
            }

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
            if (value is not KitPitch pitch)
            {
                return;
            }
            MIDI.SendNoteOn(pitch.Pitch);
        }

        [RelayCommand]
        private void OnSendNoteOff(object value)
        {
            if (value is not KitPitch pitch)
            {
                return;
            }
            MIDI.SendNoteOff(pitch.Pitch);
        }

        [RelayCommand]
        private async Task OnClearSelectedPitches()
        {
            if (SelectedPitches.Count == 0)
            {
                return;
            }

            string message = string.Format(libQB.Properties.Resources.Message_Command_Clear, Properties.Name.KitPitch);
            if (await Dialog.ShowConfirmAsync(message, libQB.Properties.Dialog.Title_Command_Clear) == false)
            {
                return;
            }

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
            if (value is not KitPitch pitch)
            {
                return;
            }

            string message = string.Format(libQB.Properties.Resources.Message_Command_Clear, Properties.Name.Articulation);
            if (await Dialog.ShowConfirmAsync(message, libQB.Properties.Dialog.Title_Command_Clear) == false)
            {
                return;
            }

            pitch.ArticulationID = 0;
        }

        [RelayCommand]
        private async Task OnImport()
        {
            string filter = "Drum Map|*.pitchlist;*.drm;*.tsv";
            string path = await Dialog.ShowOpenFileDialog(Properties.Resources.ImportDrumMap, filter, Setting.LastImportFilePath);

            if (path == null)
            {
                return;
            }

            Setting.LastImportFilePath = path;
            await TryIOFunction(
                "Import",
                () =>
                {
                    string extension = Path.GetExtension(path);
                    switch (extension)
                    {
                        case ".pitchlist":
                            ImportPitchlist(path);
                            break;
                        case ".drm":
                            ImportDrm(path);
                            break;
                        case ".tsv":
                            ImportTSVText(path);
                            break;
                        default:
                            throw new ArgumentException("File Extention is Invalid!");
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
            if (value is not KitPitch pitch)
            {
                return;
            }

            WindowService.ShowWindowWithCallback<SearchArticulation, SearchArticulationViewModel, Articulation>(
                owner: Application.Current.Windows.OfType<Window>().FirstOrDefault(w =>
                {
                    return w.IsActive;
                }),
                resultCallback: async articulation =>
                {
                    if (articulation == null)
                    {
                        return;
                    }

                    var hasMany = Kit.Pitches.Count(x =>
                    {
                        return x.Name == pitch.Name;
                    }) > 1;

                    string message = Properties.Resources.ArticulationAutomaticFill;

                    if (hasMany && (await Dialog.ShowConfirmAsync(Properties.Name.Articulation, message)).GetValueOrDefault())
                    {
                        foreach (var kitPitch in Kit.Pitches.Where(x =>
                        {
                            return x.Name == pitch.Name;
                        }))
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
            if (Kit == null)
            {
                return;
            }

            WindowService.ShowWindowWithCallback<ArticulationMapView, ArticulationMapViewModel, object>(
                parameter: Kit,
                owner: Application.Current.Windows.OfType<Window>().FirstOrDefault(w =>
                {
                    return w.IsActive;
                }));
        }

        [RelayCommand]
        private async Task OnResetOrdrByPitch()
        {
            if (Kit == null)
            {
                return;
            }

            if (await Dialog.ShowConfirmAsync(Properties.Resources.ResetOrderByPitch, Properties.Resources.ResetOrderByPitchConfirm) == false)
            {
                return;
            }

            foreach (var kitPitch in Kit.Pitches.ToArray())
            {
                kitPitch.DisplayOrder = kitPitch.Pitch + 1;
            }

            Kit.Pitches.SortRefresh(x => x.DisplayOrder);
        }

        [RelayCommand]
        private async Task OnCompressForward()
        {
            if (Kit == null)
            {
                return;
            }

            if (await Dialog.ShowConfirmAsync(Properties.Resources.CompressForward, Properties.Resources.CompressForwardConfirm) == false)
            {
                return;
            }

            int count = 1;
            var pitches = Kit.Pitches.OrderBy(x =>
            {
                return x.DisplayOrder;
            }).ToArray();

            foreach (var kitPitch in pitches.Where(x =>
            {
                return !string.IsNullOrWhiteSpace(x.Name);
            }))
            {
                kitPitch.DisplayOrder = count++;
            }

            foreach (var kitPitch in pitches.Where(x =>
            {
                return string.IsNullOrWhiteSpace(x.Name);
            }))
            {
                kitPitch.DisplayOrder = count++;
            }

            Kit.Pitches.SortRefresh(x => x.DisplayOrder);
        }

        [RelayCommand]
        private async Task OnExportStudioOne()
        {
            if (Kit == null)
            {
                return;
            }

            string title = "Export Studio One";
            var path = await Dialog.ShowSelectFolderDialog(title, Setting.ExportStudioOneDefaultPath);

            if (path == null)
            {
                return;
            }

            Setting.ExportStudioOneDefaultPath = path;
            var pitchList = MapData.GetStudioOnePitchList(Kit.Name);
            string filePath = Path.Combine(path, $"{pitchList.Title}.pitchlist");

            await TryIOFunction(title, () =>
            {
                pitchList.Save(filePath);
            }, filePath);
        }

        [RelayCommand]
        private async Task OnExportCubase()
        {
            if (Kit == null)
            {
                return;
            }

            string title = "Export Cubase";
            var path = await Dialog.ShowSelectFolderDialog(title, Setting.ExportCubaseDefaultPath);

            if (path == null)
            {
                return;
            }

            Setting.ExportCubaseDefaultPath = path;
            var cubase = MapData.GetCubaseDrumMap(Kit.Name);
            string filePath = Path.Combine(path, $"{Kit.Name}.drm");

            await TryIOFunction(title, () =>
            {
                cubase.Save(filePath);
            }, filePath);
        }

        [RelayCommand]
        private async Task OnExportText()
        {
            if (Kit == null)
            {
                return;
            }

            string title = "Export .tsv";
            var path = await Dialog.ShowSelectFolderDialog(title, Setting.ExportTextDefaultPath);

            if (path == null)
            {
                return;
            }

            Setting.ExportTextDefaultPath = path;
            var sb = MapData.GetText(Kit.Name);
            string filePath = Path.Combine(path, $"{Kit.Name}.tsv");

            await TryIOFunction(title, () =>
            {
                File.WriteAllText(filePath.ToSafeFilePath(), sb.ToString());
            }, filePath);
        }

        [RelayCommand]
        private async Task OnCopyArticulation()
        {
            if (Kit == null)
            {
                return;
            }

            if (ArticulationSource == null)
            {
                await Dialog.ShowErrorAsync(Properties.Resources.ContextKitPitchesCopyArticulation, Properties.Resources.ContextKitPitchesArticulationSourceIsEmpty);
                return;
            }

            if (await Dialog.ShowConfirmAsync(Properties.Resources.ContextKitPitchesCopyArticulation, Properties.Resources.PasteConfirm) == false)
            {
                return;
            }

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

        #region Property Change Handler

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

        // コマンドの有効/無効状態を現在のフィルタや編集状態から判定
        private void SetIsCommandEnabled()
        {
            IsCommandEnabled =
                !IsSorted && !MapData.EditState
                && string.IsNullOrWhiteSpace(FilterName)
                && string.IsNullOrWhiteSpace(FilterArticulation);
        }

        // ピッチリストのフィルタリング処理
        private bool FilterPitches(object obj)
        {
            if (obj is not KitPitch kitPitch)
            {
                return true;
            }

            bool result = true;

            if (!string.IsNullOrEmpty(FilterName))
            {
                result &= kitPitch.Name.Like(FilterName);
            }

            if (!string.IsNullOrEmpty(FilterArticulation))
            {
                var articulation = MapData.Articulations.FirstOrDefault(a =>
                {
                    return a.ID == kitPitch.ArticulationID;
                });
                result &= (articulation != null && articulation.Name.Like(FilterArticulation));
            }

            return result;
        }

        // Studio One 形式のインポート
        private void ImportPitchlist(string path)
        {
            if (Kit == null)
            {
                return;
            }

            if (StudioOne.Load(path) is not PitchList pitchlist)
            {
                throw new Exception();
            }

            Kit.Name = Microsoft.VisualBasic.Strings.Left(pitchlist.Title, ExModel.NAME_MAX_LENGTH);

            foreach (var item in Kit.Pitches.ToArray())
            {
                if (pitchlist.Items.FirstOrDefault(x =>
                {
                    return x.Pitch == item.Pitch;
                }) is PitchName pitchName)
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
            foreach (var kitPitch in Kit.Pitches.OrderBy(x =>
            {
                return x.DisplayOrder;
            }).ToArray())
            {
                kitPitch.DisplayOrder = i++;
            }

            Kit.Pitches.SortRefresh(x => x.DisplayOrder);
        }

        // Cubase 形式のインポート
        private void ImportDrm(string path)
        {
            if (Kit == null)
            {
                return;
            }

            if (CubaseDrumMap.Load(path) is not CubaseDrumMap drm)
            {
                throw new Exception();
            }

            Kit.Name = Microsoft.VisualBasic.Strings.Left(drm.Name, ExModel.NAME_MAX_LENGTH);

            foreach (var item in Kit.Pitches.ToArray())
            {
                if (drm.Map.Items.FirstOrDefault(x =>
                {
                    return x.INote == item.Pitch;
                }) is MapItem mi)
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
            foreach (var kitPitch in Kit.Pitches.OrderBy(x =>
            {
                return x.DisplayOrder;
            }).ToArray())
            {
                kitPitch.DisplayOrder = i++;
            }

            Kit.Pitches.SortRefresh(x => x.DisplayOrder);
        }

        // TSV形式のインポート
        private void ImportTSVText(string path)
        {
            if (Kit == null)
            {
                return;
            }

            Kit.Name = Microsoft.VisualBasic.Strings.Left(Path.GetFileNameWithoutExtension(path), ExModel.NAME_MAX_LENGTH);
            var text = File.ReadAllText(path);
            var lines = text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length == 0)
            {
                throw new Exception();
            }

            var header = lines[0].Split('\t');
            if (!header.SequenceEqual(new[] { "Pitch", "Note", "Name", "Articulation", "Separator" }))
            {
                throw new Exception();
            }

            foreach (var kitPitch in Kit.Pitches.ToArray())
            {
                kitPitch.DisplayOrder = 999;
            }

            int order = 1;
            foreach (var line in lines.Skip(1))
            {
                var items = line.Split('\t');
                if (items.Length != 5)
                {
                    throw new Exception();
                }

                if (!int.TryParse(items[0], out int pitch))
                {
                    throw new Exception();
                }

                var name = Microsoft.VisualBasic.Strings.Left(items[2], ExModel.NAME_MAX_LENGTH);
                var articulationName = items[3];
                var separatorName = items[4];

                if (string.IsNullOrWhiteSpace(name))
                {
                    articulationName = string.Empty;
                    separatorName = string.Empty;
                }

                int articulationID = 0;
                if (MapData.Parts.SelectMany(a =>
                {
                    return a.Articulations;
                }).FirstOrDefault(a =>
                {
                    return a.Name == articulationName;
                }) is Articulation articulation)
                {
                    articulationID = articulation.ID;
                }

                Separator separator = Separator.None;
                if (Enum.TryParse<Separator>(separatorName, out var sep))
                {
                    separator = sep;
                }

                if (Kit.Pitches.FirstOrDefault(x =>
                {
                    return x.Pitch == pitch;
                }) is not KitPitch targetPitch)
                {
                    throw new Exception();
                }

                targetPitch.Name = name;
                targetPitch.ArticulationID = articulationID;
                targetPitch.Separator = separator;
                targetPitch.DisplayOrder = order++;
            }

            order = 1;
            foreach (var kitPitch in Kit.Pitches.OrderBy(x =>
            {
                return x.DisplayOrder;
            }).ThenBy(x =>
            {
                return x.Pitch;
            }).ToArray())
            {
                kitPitch.DisplayOrder = order++;
            }

            Kit.Pitches.SortRefresh(x => x.DisplayOrder);
        }

        // I/O 関連の処理をラップして実行するユーティリティ
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
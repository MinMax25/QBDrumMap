using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using libMidi.Messages;
using libMidi.SMF;
using libQB.Attributes;
using QBDrumMap.Class;
using QBDrumMap.Class.Enums;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;
using QBDrumMap.Views;

namespace QBDrumMap.ViewModels
{
    [DIPage<ConvertMidiPage>]
    public partial class ConvertMidiViewModel
        : ViewModelBase
    {
        #region Properties

        public ISettingService Setting => SettingService;

        public IEnumerable<Kit> BaseOnComboSource => [.. MapData.Plugins.OrderBy(x => x.DisplayOrder).SelectMany(x => x.Kits.OrderBy(k => k.DisplayOrder))];

        private ObservableCollection<KitListItem> Kits = new();

        [ObservableProperty]
        private ICollectionView kitListView;

        [ObservableProperty]
        private ObservableCollection<KitListItem> selectedKits = [];

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ConvertCommand))]
        private Kit baseOn;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ConvertCommand))]
        private string lastConvertMIDIFilePath;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ConvertCommand))]
        private int selectedKitsCount;

        private bool CanConvert => !string.IsNullOrWhiteSpace(LastConvertMIDIFilePath) && BaseOn != null && SelectedKitsCount > 0;

        #endregion

        #region ctor

        public ConvertMidiViewModel(IDIContainer diContainer)
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
            SelectedKitsCount = count;
        }

        [RelayCommand]
        private async Task OnOpenFile()
        {
            var path = await Dialog.ShowOpenFileDialog(libQB.Properties.Dialog.Common_OpenFile, "MIDI File|*.mid", GetValidFilePath(LastConvertMIDIFilePath));
            if (path == null) return;
            LastConvertMIDIFilePath = path;
        }

        [RelayCommand(CanExecute = nameof(CanConvert))]
        private async Task OnConvert()
        {
            string title = $"Convert Base On Kit = '{BaseOn?.Name}'";

            if (!File.Exists(LastConvertMIDIFilePath))
            {
                await Dialog.ShowAlertAsync(title, libQB.Properties.Dialog.Alert_FileNotFound);
                LastConvertMIDIFilePath = string.Empty;
                return;
            }

            try
            {
                MidiData baseMIDI = SMFLoader.Load(LastConvertMIDIFilePath);

                ArticulationMap baseArticMap = ArticulationMap.GetArticulationMap(MapData, BaseOn.Name);

                var systemEvents = baseMIDI.GetAllEvents().Where(x => x.Channel == 0).ToList();
                var channelEvents = baseMIDI.GetAllEvents().Where(x => x.Channel > 0).ToList();

                if (Setting.ConvertMIDIFormat == MIDIFormat.Format0)
                {
                    ConvertToFormat0(baseMIDI, baseArticMap, channelEvents);
                }
                else
                {
                    ConvertToFormat1(baseMIDI, baseArticMap, systemEvents, channelEvents);
                }

                await Dialog.ShowInformationAsync(libQB.Properties.Resources.Message_ProcessComplete, title);
            }
            catch
            {
                await Dialog.ShowErrorAsync(Properties.Resources.MIDIError);
            }

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

            OnPropertyChanged(nameof(BaseOnComboSource));
        }

        private static string GetValidFilePath(string path)
        {
            if (File.Exists(path))
            {
                return path;
            }
            else
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            }
        }

        private void ConvertToFormat0(MidiData baseMIDI, ArticulationMap baseArticMap, List<MidiEvent> channelEvents)
        {
            string convertedFilePath = Path.Combine(Path.GetDirectoryName(baseMIDI.FilePath), $"{Path.GetFileNameWithoutExtension(baseMIDI.FilePath)} Converted");
            if (Directory.Exists(convertedFilePath))
            {
                Directory.Delete(convertedFilePath, true);
            }
            Directory.CreateDirectory(convertedFilePath);

            // Out Of Articulation
            MidiData dstMIDI = new(baseMIDI.Division);

            Track outTrack = new(dstMIDI);

            outTrack.EventAdd(new MidiEvent(outTrack) { AbsoluteTick = 0, Message = new SequenceTrackName($"#{baseArticMap.Name}") });

            ConvertChannelEvents(channelEvents, baseArticMap, baseArticMap, outTrack, true);

            if (outTrack.Events.Where(x => x.Message is NoteOn).Any())
            {
                dstMIDI.AddTrack(outTrack);
                dstMIDI.Organize();
                dstMIDI.FilePath = Path.Combine(convertedFilePath, $"#{baseArticMap.Name}.mid");
                SMFConverter.SaveMidiData(dstMIDI, filter: false);
            }

            //
            foreach (var drumKit in SelectedKits.OrderBy(x => x.PluginDisplayOrder).ThenBy(x => x.KitDisplayOrder))
            {
                dstMIDI = new MidiData(baseMIDI.Division);
                ArticulationMap dstArticMap = ArticulationMap.GetArticulationMap(MapData, drumKit.KitName);

                Track dstTrack = new(dstMIDI);

                dstTrack.EventAdd(new MidiEvent(dstTrack) { AbsoluteTick = 0, Message = new SequenceTrackName(drumKit.KitName) });

                ConvertChannelEvents(channelEvents, baseArticMap, dstArticMap, dstTrack);

                dstMIDI.AddTrack(dstTrack);
                dstMIDI.Organize();

                dstMIDI.FilePath = Path.Combine(convertedFilePath, $"{drumKit.KitName}.mid");
                SMFConverter.SaveMidiData(dstMIDI, filter: false);
            }
        }

        private void ConvertToFormat1(MidiData baseMIDI, ArticulationMap baseArticMap, List<MidiEvent> systemEvents, List<MidiEvent> channelEvents)
        {
            MidiData dstMIDI = new(baseMIDI.Division);

            Track sysTrack = new(dstMIDI);
            foreach (var midiEvent in systemEvents)
            {
                if (midiEvent.Message is SequenceTrackName sequenceTrackName)
                {
                    sysTrack.EventAdd(midiEvent with { Message = new SequenceTrackName($"{sequenceTrackName.Text} Converted") });
                }
                else
                {
                    sysTrack.EventAdd(midiEvent);
                }
            }
            dstMIDI.AddTrack(sysTrack);

            // Out Of Articulation
            Track outTrack = new(dstMIDI);

            outTrack.EventAdd(new MidiEvent(outTrack) { AbsoluteTick = 0, Message = new SequenceTrackName($"#{baseArticMap.Name}") });

            ConvertChannelEvents(channelEvents, baseArticMap, baseArticMap, outTrack, true);

            if (outTrack.Events.Where(x => x.Message is NoteOn).Any())
            {
                dstMIDI.AddTrack(outTrack);
            }

            //
            foreach (var drumKit in SelectedKits.OrderBy(x => x.PluginDisplayOrder).ThenBy(x => x.KitDisplayOrder))
            {
                ArticulationMap dstArticMap = ArticulationMap.GetArticulationMap(MapData, drumKit.KitName);

                Track dstTrack = new(dstMIDI);

                dstTrack.EventAdd(new MidiEvent(dstTrack) { AbsoluteTick = 0, Message = new SequenceTrackName(drumKit.KitName) });

                ConvertChannelEvents(channelEvents, baseArticMap, dstArticMap, dstTrack);

                dstMIDI.AddTrack(dstTrack);
            }

            dstMIDI.Organize();
            dstMIDI.FilePath = Path.Combine(Path.GetDirectoryName(baseMIDI.FilePath) ?? "", $"{Path.GetFileNameWithoutExtension(baseMIDI.FilePath)} Converted.mid");
            SMFConverter.SaveMidiData(dstMIDI, filter: false);
        }

        private static void ConvertChannelEvents(List<MidiEvent> channelEvents, ArticulationMap baseArticMap, ArticulationMap dstArticMap, Track dstTrack, bool isOut = false)
        {
            foreach (var midiEvent in channelEvents)
            {
                if (midiEvent.Message is ChannelNoteMessage note)
                {
                    int dstPitch = GetPitch(baseArticMap, dstArticMap, note.Pitch);
                    if (!isOut && dstPitch >= 0)
                    {
                        dstTrack.EventAdd(midiEvent with { Message = note with { Pitch = (byte)dstPitch } });
                    }
                    if (isOut && dstPitch < 0)
                    {
                        dstTrack.EventAdd(midiEvent with { Message = note with { Pitch = note.Pitch } });
                    }
                }
                else
                {
                    dstTrack.EventAdd(midiEvent);
                }
            }
        }

        protected static int GetPitch(ArticulationMap baseArticMap, ArticulationMap dstArticMap, byte basePitch)
        {
            if (baseArticMap?.Items.FirstOrDefault(i => !i.IsSub && i.Pitches.Contains(basePitch)) is ArticulationMapItem baseArtic)
            {
                if (baseArticMap != null && dstArticMap?.Items.FirstOrDefault(i => i.ID == baseArtic.ID) is ArticulationMapItem dstArtic)
                {
                    return dstArtic.Pitch;
                }
            }
            return -1;
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

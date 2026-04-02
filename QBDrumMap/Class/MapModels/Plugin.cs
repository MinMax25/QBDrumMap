using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using QBDrumMap.Class.CustomValidations;
using QBDrumMap.Class.Enums;

namespace QBDrumMap.Class.MapModels
{
    public partial class Plugin : ModelBase, IHasID
    {
        #region Fields

        // プラグインの一意なID
        [ObservableProperty]
        private int iD;

        // プラグイン名
        [ObservableProperty]
        [ValidatePlugin]
        [NotifyDataErrorInfo]
        private string name = string.Empty;

        // プラグインの種類（VST, Hardware等）
        [ObservableProperty]
        private PluginType pluginType = PluginType.None;

        // MIDI出力デバイス名
        [ObservableProperty]
        private string midiOutDevice = string.Empty;

        // 音声確認用MIDIチャンネル
        [ObservableProperty]
        private MIDIChannel soundCheckChannel = MIDIChannel.Channel10;

        // プラグインに含まれるキットのコレクション
        [ObservableProperty]
        private ObservableCollection<Kit> kits = new();

        #endregion

        #region Methods

        #region Property Change Handler

        partial void OnIDChanged(int oldValue, int newValue)
        {
            UndoManager?.RegisterPropertyChange(() => ID, oldValue, newValue);
        }

        partial void OnNameChanged(string oldValue, string newValue)
        {
            UndoManager?.RegisterPropertyChange(() => Name, oldValue, newValue);
        }

        partial void OnPluginTypeChanged(PluginType oldValue, PluginType newValue)
        {
            UndoManager?.RegisterPropertyChange(() => PluginType, oldValue, newValue);
        }

        partial void OnMidiOutDeviceChanged(string oldValue, string newValue)
        {
            UndoManager?.RegisterPropertyChange(() => MidiOutDevice, oldValue, newValue);
        }

        partial void OnSoundCheckChannelChanged(MIDIChannel oldValue, MIDIChannel newValue)
        {
            UndoManager?.RegisterPropertyChange(() => SoundCheckChannel, oldValue, newValue);
        }

        #endregion

        #region General

        public override object Clone()
        {
            return new Plugin
            {
                ID = ID,
                Name = Name,
                PluginType = PluginType,
                MidiOutDevice = MidiOutDevice,
                SoundCheckChannel = SoundCheckChannel,
                Kits = new ObservableCollection<Kit>(Kits.Select(kit =>
                {
                    return (Kit)kit.Clone();
                })),
                DisplayOrder = DisplayOrder
            };
        }

        #endregion

        #endregion
    }
}
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using QBDrumMap.Class.CustomValidations;
using QBDrumMap.Class.Enums;

namespace QBDrumMap.Class.MapModels
{
    public partial class Plugin
        : ModelBase
        , IHasID
    {
        [ObservableProperty]
        private int iD;

        [ObservableProperty]
        [ValidatePlugin]
        [NotifyDataErrorInfo]
        private string name = string.Empty;

        [ObservableProperty]
        private PluginType pluginType = PluginType.None;

        [ObservableProperty]
        private string midiOutDevice = string.Empty;

        [ObservableProperty]
        private MIDIChannel soundCheckChannel = MIDIChannel.Channel10;

        [ObservableProperty]
        private ObservableCollection<Kit> kits = [];

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

        public override object Clone()
        {
            return new Plugin
            {
                ID = ID,
                Name = Name,
                Kits = new ObservableCollection<Kit>(Kits.Select(kit => (Kit)kit.Clone())),
                DisplayOrder = DisplayOrder
            };
        }
    }
}

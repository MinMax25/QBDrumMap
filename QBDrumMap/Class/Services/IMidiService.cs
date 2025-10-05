using System.Collections.ObjectModel;
using NAudio.Midi;
using QBDrumMap.Class.MapModels;

namespace QBDrumMap.Class.Services
{
    public interface IMidiService
        : IDisposable
    {
        ObservableCollection<string> MidiInDevices { get; }
        ObservableCollection<string> MidiOutDevices { get; }

        string MidiInDevice { get; set; }
        string MidiOutDevice { get; set; }

        string MidiInDevState { get; }
        string MidiOutDevState { get; }

        bool MidiThruEnabled { get; set; }
        bool FixedPitchEnabled { get; set; }

        int MidiInFixedPitch { get; set; }

        int MidiOutChannel { get; set; }
        int SoundCheckVelocity { get; set; }

        void SetSoundCheckPitch(int pitch);

        void SendNoteOn(int pitch, bool isFromMidiDevice = false);
        void SendNoteOff(int pitch, bool isFromMidiDevice = false);
        void SendProgramChange(Plugin plugin, Kit kit);

        event EventHandler<MidiInMessageEventArgs> MidiMessageReceived;
    }
}

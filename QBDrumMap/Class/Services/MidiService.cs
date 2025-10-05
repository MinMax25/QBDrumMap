using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using libMidi.Messages.enums;
using libQB.Attributes;
using NAudio.Midi;
using QBDrumMap.Class.Extentions;
using QBDrumMap.Class.MapModels;
using MidiMessage = NAudio.Midi.MidiMessage;

namespace QBDrumMap.Class.Services
{
    [DISingleton<IMidiService>]
    public partial class MidiService
        : ObservableObject
        , IMidiService
    {
        public event EventHandler<MidiInMessageEventArgs> MidiMessageReceived;

        #region Properties

        [ObservableProperty]
        private ObservableCollection<string> midiInDevices = new();

        [ObservableProperty]
        private ObservableCollection<string> midiOutDevices = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MidiInDevState))]
        private string midiInDevice;

        public string MidiInDevState { get; private set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MidiOutDevState))]
        private string midiOutDevice;

        public string MidiOutDevState { get; private set; }

        [ObservableProperty]
        private int midiOutChannel;

        [ObservableProperty]
        private int soundCheckVelocity = 100;

        public bool MidiThruEnabled
        {
            get => _MidiThruEnabled;
            set
            {
                SetProperty(ref _MidiThruEnabled, value);
                KeyVelocity = 0;
                SoundCheckPitch = -1;
            }
        }
        private bool _MidiThruEnabled = false;

        [ObservableProperty]
        private bool fixedPitchEnabled = false;

        public int MidiInFixedPitch { get; set; } = -1;

        #endregion

        #region Fields

        private MidiIn midiInDev;

        private MidiOut midiOutDev;

        private int SoundCheckPitch;

        private int KeyVelocity;

        #endregion

        #region ctor

        public MidiService()
        {
            RescanDevices();

            MidiInDevState = ConnectState(null);
            MidiOutDevState = ConnectState(null);

            MidiMessageReceived += MidiInDev_MessageReceived;
        }

        #endregion

        #region Methods

        #region PropertyChanged Callbacks

        partial void OnMidiInDeviceChanged(string value)
        {
            FlushMidiIn();

            if (!string.IsNullOrWhiteSpace(value))
            {
                for (int i = 0; i < MidiIn.NumberOfDevices; i++)
                {
                    if (MidiIn.DeviceInfo(i).ProductName == value)
                    {
                        try
                        {
                            midiInDev = new MidiIn(i);
                            midiInDev.MessageReceived += (s, e) => MidiMessageReceived?.Invoke(this, e);
                            midiInDev.Start();
                        }
                        catch { }
                        break;
                    }
                }
            }

            MidiInDevState = ConnectState(midiInDev);
        }

        partial void OnMidiOutDeviceChanged(string value)
        {
            FlushMidiOut();

            if (!string.IsNullOrWhiteSpace(value))
            {
                for (int i = 0; i < MidiOut.NumberOfDevices; i++)
                {
                    if (MidiOut.DeviceInfo(i).ProductName == value)
                    {
                        try { midiOutDev = new MidiOut(i); }
                        catch { }
                        break;
                    }
                }
            }

            MidiOutDevState = ConnectState(midiOutDev);
        }

        #endregion

        #region Event Handling

        private void MidiInDev_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            if (MidiThruEnabled == false) return;
            if (midiOutDev == null) return;
            if (e.MidiEvent is not NoteEvent noteEvent) return;

            KeyVelocity = noteEvent.Velocity;

            if (e.MidiEvent is NoteOnEvent noteOn && noteOn.Velocity > 0)
            {
                SendNoteEvent(noteEvent.NoteNumber, (p, _) => SendNoteOn(p, true));
            }
            else if (noteEvent.CommandCode == MidiCommandCode.NoteOff)
            {
                SendNoteEvent(noteEvent.NoteNumber, (p, _) => SendNoteOff(p, true));
            }
        }

        #endregion

        #region General

        private IReadOnlyList<string> GetMidiInDevices()
        {
            var list = new List<string> { string.Empty };
            foreach (var i in Enumerable.Range(0, MidiIn.NumberOfDevices))
            {
                list.Add(MidiIn.DeviceInfo(i).ProductName);
            }
            return list;
        }

        private IReadOnlyList<string> GetMidiOutDevices()
        {
            var list = new List<string> { string.Empty };
            foreach (var i in Enumerable.Range(0, MidiOut.NumberOfDevices))
            {
                list.Add(MidiOut.DeviceInfo(i).ProductName);
            }
            return list;
        }

        private void RescanDevices()
        {
            MidiInDevices.Clear();
            GetMidiInDevices().ToList().ForEach(MidiInDevices.Add);

            MidiOutDevices.Clear();
            GetMidiOutDevices().ToList().ForEach(MidiOutDevices.Add);
        }

        private string ConnectState(object value) => value == null ? "Not Connected" : "Connected";

        private void FlushMidiIn()
        {
            if (midiInDev != null)
            {
                midiInDev.Stop();
                midiInDev.Dispose();
                midiInDev = null;
            }
        }

        private void FlushMidiOut()
        {
            if (midiOutDev != null)
            {
                midiOutDev.Dispose();
                midiOutDev = null;
            }
        }

        private void SendNoteEvent(int pitch, Action<int, bool> action)
        {
            if (!FixedPitchEnabled)
            {
                action.Invoke(pitch, true);
                return;
            }

            if (MidiInFixedPitch != pitch) return;
            if (SoundCheckPitch < 0) return;
            action.Invoke(SoundCheckPitch, true);
        }

        public void SetSoundCheckPitch(int pitch)
        {
            SoundCheckPitch = pitch;
        }

        public void SendNoteOn(int pitch, bool isFromMidiDevice = false)
        {
            if (MidiThruEnabled == false) return;

            if (midiOutDev == null || pitch is < 0 or > 127) return;

            if (isFromMidiDevice == false)
            {
                SoundCheckPitch = pitch;
            }

            var velocity = isFromMidiDevice ? KeyVelocity : SoundCheckVelocity;
            midiOutDev.Send(MidiMessage.StartNote(pitch, velocity, MidiOutChannel).RawData);
        }

        public void SendNoteOff(int pitch, bool isFromMidiDevice = false)
        {
            if (MidiThruEnabled == false) return;

            if (midiOutDev == null || pitch is < 0 or > 127) return;

            var velocity = isFromMidiDevice ? KeyVelocity : SoundCheckVelocity;
            midiOutDev.Send(MidiMessage.StopNote(pitch, velocity, MidiOutChannel).RawData);
        }

        public void SendProgramChange(Plugin plugin, Kit kit)
        {
            MidiOutDevice = plugin.MidiOutDevice;
            MidiOutChannel = (int)plugin.SoundCheckChannel;

            if (midiOutDev == null || kit == null) return;

            midiOutDev.SendBuffer(SysExHelper.GetExclusiveData(SysExType.GMSystemOff));

            switch (plugin.PluginType)
            {
                case PluginType.GM:
                    midiOutDev.SendBuffer(SysExHelper.GetExclusiveData(SysExType.GMSystemOn));
                    break;
                case PluginType.GM2:
                    midiOutDev.SendBuffer(SysExHelper.GetExclusiveData(SysExType.GM2SytemOn));
                    break;
                case PluginType.GS:
                    midiOutDev.SendBuffer(SysExHelper.GetExclusiveData(SysExType.GSReset));
                    break;
                case PluginType.XG:
                    midiOutDev.SendBuffer(SysExHelper.GetExclusiveData(SysExType.XGSystemOn));
                    break;
            }

            int ch = (int)plugin.SoundCheckChannel;

            midiOutDev.Send(MidiMessage.ChangeControl((int)CtrlType.BankMSB, kit.BankSelectMSB, ch).RawData);
            midiOutDev.Send(MidiMessage.ChangeControl((int)CtrlType.BankLSB, kit.BankSelectLSB, ch).RawData);
            midiOutDev.Send(MidiMessage.ChangePatch(kit.ProgramNumber, ch).RawData);
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            MidiMessageReceived -= MidiInDev_MessageReceived;
            FlushMidiIn();
            FlushMidiOut();
        }

        #endregion

        #endregion
    }
}

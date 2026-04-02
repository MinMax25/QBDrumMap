using System.Collections.ObjectModel;
using NAudio.Midi;
using QBDrumMap.Class.MapModels;

namespace QBDrumMap.Class.Services
{
    public interface IMidiService : IDisposable
    {
        #region Properties

        // 利用可能なMIDI入力デバイスのリスト
        ObservableCollection<string> MidiInDevices { get; }

        // 利用可能なMIDI出力デバイスのリスト
        ObservableCollection<string> MidiOutDevices { get; }

        // 現在選択されているMIDI入力デバイス
        string MidiInDevice { get; set; }

        // 現在選択されているMIDI出力デバイス
        string MidiOutDevice { get; set; }

        // MIDI入力デバイスのステータス文字列
        string MidiInDevState { get; }

        // MIDI出力デバイスのステータス文字列
        string MidiOutDevState { get; }

        // MIDIスルー（入力から出力への転送）の有効化
        bool MidiThruEnabled { get; set; }

        // 固定ピッチ送信の有効化
        bool FixedPitchEnabled { get; set; }

        // MIDI入力時の固定ピッチ番号
        int MidiInFixedPitch { get; set; }

        // MIDI出力チャンネル
        int MidiOutChannel { get; set; }

        // 音声確認時のベロシティ
        int SoundCheckVelocity { get; set; }

        #endregion

        #region Methods

        // 音声確認用のピッチを設定
        void SetSoundCheckPitch(int pitch);

        // ノートオンメッセージを送信
        void SendNoteOn(int pitch, bool isFromMidiDevice = false);

        // ノートオフメッセージを送信
        void SendNoteOff(int pitch, bool isFromMidiDevice = false);

        // プログラムチェンジ（音色切り替え）を送信
        void SendProgramChange(Plugin plugin, Kit kit);

        #endregion

        #region Events

        // MIDIメッセージ受信イベント
        event EventHandler<MidiInMessageEventArgs> MidiMessageReceived;

        #endregion
    }
}
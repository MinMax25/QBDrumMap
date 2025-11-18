using System.ComponentModel;
using libQB.Enums;
using QBDrumMap.Class.Enums;

namespace QBDrumMap.Class.Services
{
    public interface ISettingService
        : INotifyPropertyChanged
        , IDisposable
    {
        #region Properties

        BaseTheme BaseTheme { get; set; }

        string ThemeColor { get; set; }

        Languages Language { get; set; }

        Pages StartUpPage { get; set; }

        bool UseArticulationPitchName { get; set; }

        bool UseArticulation { get; set; }

        bool IsOpenTheLastFileOpened { get; set; }

        bool IsPartScoreViewExtended { get; set; }

        string LastOpenedFilePath { get; set; }

        string LastImportFilePath { get; set; }

        string ExportStudioOneDefaultPath { get; set; }

        string ExportCubaseDefaultPath { get; set; }

        string ExportTextDefaultPath { get; set; }

        string ExportCubaseBaseOnDefaultPath { get; set; }

        string ExportNoteMapperDefaultPath { get; set; }

        string ExportQBDrummerDefaultPath { get; set; }

        MIDIFormat ConvertMIDIFormat { get; set; }

        string CubaseInstallPath { get; set; }

        string MidiInDevice { get; set; }

        int MidiInFixedPitch { get; set; }

        string SearchArticulationFilter { get; set; }

        bool IsExtendedProgramChange { get; set; }

        #endregion

        #region Methods

        public void Load();

        public void Save();

        #endregion
    }
}

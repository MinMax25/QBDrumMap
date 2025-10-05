using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using libQB.Enums;
using QBDrumMap.Class.Services;
using InstrumentName = QBDrumMap.Cubase.Score.InstrumentName;
using Instruments = QBDrumMap.Cubase.Score.Instruments;
using NoteHeadSet = QBDrumMap.Cubase.Score.NoteHeadSet;
using Technique = QBDrumMap.Cubase.Score.Technique;

namespace QBDrumMap.Class.Cubase
{
    public static class CubaseScore
    {
        public static ObservableCollection<KeyValuePair<string, string>> InstrumentsComboSource { get; } = new();

        public static ObservableCollection<KeyValuePair<int, string>> NoteHeadSetComboSource { get; } = new();

        public static ObservableCollection<KeyValuePair<string, string>> TechniqueComboSource { get; } = new();

        public static void Initialize()
        {
            if (App.GetService<ISettingService>() is not ISettingService setting) return;

            var cubaseRootPath = setting.CubaseInstallPath ?? string.Empty;
            var language = setting.Language;

            InstrumentsComboSource.Clear();
            NoteHeadSetComboSource.Clear();
            TechniqueComboSource.Clear();

            if (string.IsNullOrWhiteSpace(cubaseRootPath)) return;

            string instpath = Path.Combine(cubaseRootPath, @"Components\ScoringEngine\instruments.xml");
            string nhpath = Path.Combine(cubaseRootPath, @"Components\ScoringEngine\scoreLibrary.xml");
            string namepath = Path.Combine(cubaseRootPath, @"Components\ScoringEngine\l10n\instrumentnames_ja.xml");
            string teqpath = Path.Combine(cubaseRootPath, @"Components\ScoringEngine\playingTechniqueDefinitions.xml");

            if (!File.Exists(instpath)) return;
            if (!File.Exists(nhpath)) return;
            if (!File.Exists(namepath)) return;
            if (!File.Exists(teqpath)) return;

            var libinst = Deserialize<Instruments.kScoreLibrary>(instpath);
            var libnh = Deserialize<NoteHeadSet.kScoreLibrary>(nhpath);
            var libname = Deserialize<InstrumentName.kScoreLibrary>(namepath);
            var libteq = Deserialize<Technique.kScoreLibrary>(teqpath);

            // Instruments
            foreach (var item in libinst.instruments.entities.InstrumentEntityDefinition)
            {
                if (string.IsNullOrWhiteSpace(item.percussionInstrumentDataID)) continue;
                if (language == Languages.Japanese)
                {
                    var jp = libname.instrumentNames.entities.InstrumentNameEntityDefinition.FirstOrDefault(x => x.entityID == item.nameID);
                    InstrumentsComboSource.Add(new KeyValuePair<string, string>(item.entityID, $"{jp?.name}"));
                }
                else
                {
                    InstrumentsComboSource.Add(new KeyValuePair<string, string>(item.entityID, $"{item.name} ({item.entityID})"));
                }
            }

            if (!InstrumentsComboSource.Any(x => string.IsNullOrWhiteSpace(x.Key)))
                InstrumentsComboSource.Insert(0, new KeyValuePair<string, string>(string.Empty, string.Empty));

            // Note Head Set
            int nhcount = 1;
            foreach (var item in libnh.noteheadSetDefinitions.entities.NoteheadSetDefinition)
            {
                NoteHeadSetComboSource.Add(new KeyValuePair<int, string>(nhcount, $"({nhcount}) {item.name}"));
                nhcount++;
            }

            if (!NoteHeadSetComboSource.Any(x => x.Key == 0))
                NoteHeadSetComboSource.Insert(0, new KeyValuePair<int, string>(0, string.Empty));

            // Technique
            foreach (var item in libteq.playingTechniques.entities.PlayingTechniqueDefinition)
            {
                if (item.groupType != "kTechniques") continue;
                TechniqueComboSource.Add(new KeyValuePair<string, string>(item.entityID, item.name));
            }

            if (!TechniqueComboSource.Any(x => string.IsNullOrWhiteSpace(x.Key)))
                TechniqueComboSource.Insert(0, new KeyValuePair<string, string>(string.Empty, string.Empty));
        }

        private static T Deserialize<T>(string path)
        {
            if (!File.Exists(path)) return default;

            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stream);
        }
    }
}

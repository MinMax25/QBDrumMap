using System.Collections.ObjectModel;
using System.ComponentModel;
using libQB.Enums;
using QBDrumMap.Class.Enums;
using QBDrumMap.Class.MapModels;

namespace QBDrumMap.Class.Services
{
    public interface ISettingService : INotifyPropertyChanged, IDisposable
    {
        #region Properties

        // ベーステーマ (Dark/Light)
        BaseTheme BaseTheme { get; set; }

        // アクセントカラー
        string ThemeColor { get; set; }

        // 言語設定
        Languages Language { get; set; }

        // 起動時に表示するページ
        Pages StartUpPage { get; set; }

        // アーティキュレーション名にピッチ名を含めるか
        bool UseArticulationPitchName { get; set; }

        // アーティキュレーション機能を使用するか
        bool UseArticulation { get; set; }

        // 終了時に開いていたファイルを次回起動時に開くか
        bool IsOpenTheLastFileOpened { get; set; }

        // パートスコアビューを拡張表示するか
        bool IsPartScoreViewExtended { get; set; }

        // 最後に開いたファイルのパス
        string LastOpenedFilePath { get; set; }

        // 最後にインポートしたファイルのパス
        string LastImportFilePath { get; set; }

        // Studio One エクスポート時のデフォルトパス
        string ExportStudioOneDefaultPath { get; set; }

        // Cubase エクスポート時のデフォルトパス
        string ExportCubaseDefaultPath { get; set; }

        // テキスト出力時のデフォルトパス
        string ExportTextDefaultPath { get; set; }

        // Cubase (BaseOn形式) エクスポート時のデフォルトパス
        string ExportCubaseBaseOnDefaultPath { get; set; }

        // NoteMapper エクスポート時のデフォルトパス
        string ExportNoteMapperDefaultPath { get; set; }

        // QBDrummer エクスポート時のデフォルトパス
        string ExportQBDrummerDefaultPath { get; set; }

        // 変換時のMIDIフォーマット
        MIDIFormat ConvertMIDIFormat { get; set; }

        // Cubaseのインストールパス
        string CubaseInstallPath { get; set; }

        // MIDI入力デバイス名
        string MidiInDevice { get; set; }

        // MIDI入力の固定ピッチ番号
        int MidiInFixedPitch { get; set; }

        // アーティキュレーション検索用フィルター
        string SearchArticulationFilter { get; set; }

        // プログラムチェンジを拡張設定するか
        bool IsExtendedProgramChange { get; set; }

        // パーツ名の表記ゆれ辞書（アーティキュレーション自動設定で使用）
        ObservableCollection<PartNameAlias> PartNameDictionary { get; set; }

        #endregion

        #region Methods

        // 設定の読み込み
        void Load();

        // 設定の保存
        void Save();

        #endregion
    }
}
using libQB.Enums;
using QBDrumMap.Class.Enums;
using Tomlyn.Serialization;

namespace QBDrumMap.Class.Services;

// Setting.tomlへ永続化するデータ。実行時サービスから保存形式を分離する。
public sealed class AppSettings
{
    [TomlConverter(typeof(EnumTomlConverter<BaseTheme>))]
    public BaseTheme BaseTheme { get; set; } = BaseTheme.Dark;

    public string ThemeColor { get; set; } = "Steel";
    public string Language { get; set; } = "en-US";

    [TomlConverter(typeof(EnumTomlConverter<Pages>))]
    public Pages StartUpPage { get; set; } = Pages.PluginPage;

    public bool UseArticulationPitchName { get; set; } = true;
    public bool UseArticulation { get; set; } = true;
    public bool IsOpenTheLastFileOpened { get; set; } = true;
    public bool IsPartScoreViewExtended { get; set; }
    public string LastOpenedFilePath { get; set; } = string.Empty;
    public string LastImportFilePath { get; set; } = string.Empty;
    public string ExportStudioOneDefaultPath { get; set; } = string.Empty;
    public string ExportCubaseDefaultPath { get; set; } = string.Empty;
    public string ExportTextDefaultPath { get; set; } = string.Empty;
    public string ExportCubaseBaseOnDefaultPath { get; set; } = string.Empty;
    public string ExportNoteMapperDefaultPath { get; set; } = string.Empty;
    public string ExportQBDrummerDefaultPath { get; set; } = string.Empty;

    [TomlConverter(typeof(EnumTomlConverter<MIDIFormat>))]
    public MIDIFormat ConvertMIDIFormat { get; set; } = MIDIFormat.Format1;

    public string CubaseInstallPath { get; set; } = string.Empty;
    public string MidiInDevice { get; set; } = string.Empty;
    public int MidiInFixedPitch { get; set; } = -1;
    public List<PartNameAliasSetting> PartNameDictionary { get; set; } = [];
}

public sealed class PartNameAliasSetting
{
    public string CanonicalName { get; set; } = string.Empty;
    public string Aliases { get; set; } = string.Empty;
}

internal sealed class EnumTomlConverter<TEnum> : TomlConverter<TEnum>
    where TEnum : struct, Enum
{
    public override TEnum Read(TomlReader reader) => Enum.Parse<TEnum>(reader.GetString());

    public override void Write(TomlWriter writer, TEnum value) => writer.WriteStringValue(value.ToString());
}

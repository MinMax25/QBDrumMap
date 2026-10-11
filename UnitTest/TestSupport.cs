using System.IO;
using System.Text.Json;
using QBDrumMap.Class.MapModels;

namespace UnitTest;

/// <summary>テスト共通の補助処理。サンプルQBDは出力フォルダへコピーされた DrumMap.qbd を使う。</summary>
internal static class TestSupport
{
    public static string SampleQbd => Path.Combine(AppContext.BaseDirectory, "DrumMap.qbd");

    public static async Task<MapData> LoadSampleAsync()
    {
        var map = new MapData();
        Assert.True(await map.LoadAsync(SampleQbd), "サンプルQBDの読込に失敗した");
        return map;
    }

    /// <summary>モデルの全内容を比較用の文字列にする（Cloneは保存対象の全項目を持つ）。</summary>
    public static string Dump(MapData map)
    {
        return JsonSerializer.Serialize(map.Clone(), new JsonSerializerOptions { WriteIndented = true });
    }
}

/// <summary>テストごとの一時フォルダ。終了時に削除する。</summary>
internal sealed class TempDir : IDisposable
{
    public string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "QBDrumMapTest_" + Guid.NewGuid().ToString("N"));

    public TempDir()
    {
        Directory.CreateDirectory(Path);
    }

    public string File(string name) => System.IO.Path.Combine(Path, name);

    public void Dispose()
    {
        try
        {
            foreach (var f in Directory.GetFiles(Path))
            {
                System.IO.File.SetAttributes(f, FileAttributes.Normal);
            }
            Directory.Delete(Path, true);
        }
        catch
        {
        }
    }
}

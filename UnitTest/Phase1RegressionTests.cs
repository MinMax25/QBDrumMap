using System.IO;
using QBDrumMap.Class.MapModels;

namespace UnitTest;

/// <summary>フェーズ1（B-01、B-02、N-4）の不具合再現テスト。修正前は失敗し、修正後に成功する。</summary>
[Trait("Category", "Regression")]
public class Phase1RegressionTests
{
    private const string BrokenJson = "{ not json";

    private const string NullPluginsJson = "{ \"Plugins\": null, \"Parts\": [] }";

    private static async Task<(MapData Map, string Before)> LoadedWithEditAsync()
    {
        var map = await TestSupport.LoadSampleAsync();
        map.Plugins[0].Kits[0].Pitches[0].ArticulationID = 12345; // 未保存の編集
        return (map, TestSupport.Dump(map));
    }

    [Fact]
    public async Task B01_CorruptJson_KeepsCurrentData()
    {
        using var dir = new TempDir();
        File.WriteAllText(dir.File("bad.qbd"), BrokenJson);
        var (map, before) = await LoadedWithEditAsync();

        Assert.False(await map.LoadAsync(dir.File("bad.qbd")));

        Assert.Equal(before, TestSupport.Dump(map));
    }

    [Fact]
    public async Task B01_NullPlugins_KeepsCurrentData()
    {
        using var dir = new TempDir();
        File.WriteAllText(dir.File("bad.qbd"), NullPluginsJson);
        var (map, before) = await LoadedWithEditAsync();

        Assert.False(await map.LoadAsync(dir.File("bad.qbd")));

        Assert.Equal(before, TestSupport.Dump(map));
    }

    [Fact]
    public async Task B01_MissingFile_KeepsCurrentData()
    {
        using var dir = new TempDir();
        var (map, before) = await LoadedWithEditAsync();

        Assert.False(await map.LoadAsync(dir.File("missing.qbd")));

        Assert.Equal(before, TestSupport.Dump(map));
    }

    [Fact]
    public async Task B01_FailedLoad_KeepsDirtyState()
    {
        using var dir = new TempDir();
        File.WriteAllText(dir.File("bad.qbd"), BrokenJson);
        var (map, _) = await LoadedWithEditAsync();
        Assert.True(map.IsModified());

        await map.LoadAsync(dir.File("bad.qbd"));

        Assert.True(map.IsModified());
    }

    [Fact]
    public async Task B01_FailedLoad_KeepsCleanState()
    {
        using var dir = new TempDir();
        File.WriteAllText(dir.File("bad.qbd"), BrokenJson);
        var map = await TestSupport.LoadSampleAsync();

        await map.LoadAsync(dir.File("bad.qbd"));

        Assert.False(map.IsModified());
    }

    [Fact]
    public async Task B01_FailedLoad_DoesNotRaiseLoaded_AndReportsError()
    {
        using var dir = new TempDir();
        File.WriteAllText(dir.File("bad.qbd"), BrokenJson);
        var map = await TestSupport.LoadSampleAsync();
        int raised = 0;
        map.Loaded += (_, _) => raised++;

        Assert.False(await map.LoadAsync(dir.File("bad.qbd")));

        Assert.Equal(0, raised);
        Assert.NotNull(map.LastError);
    }

    [Fact]
    public async Task B01_ReloadSameFile_HasNoValidationErrors()
    {
        // 現在のデータと同名の項目を読み込んでも、検証エラーが付かないこと
        var map = await TestSupport.LoadSampleAsync();

        Assert.True(await map.LoadAsync(TestSupport.SampleQbd));

        Assert.False(map.HasError());
    }

    [Fact]
    public async Task B02_FailedSave_KeepsDirty()
    {
        using var dir = new TempDir();
        var (map, _) = await LoadedWithEditAsync();

        Assert.False(await map.SaveAsync(Path.Combine(dir.Path, "no_such_dir", "a.qbd")));

        Assert.True(map.IsModified());
    }

    [Fact]
    public async Task B02_FailedSave_DoesNotRaiseSaved_AndReportsError()
    {
        using var dir = new TempDir();
        var (map, _) = await LoadedWithEditAsync();
        int raised = 0;
        map.Saved += (_, _) => raised++;

        Assert.False(await map.SaveAsync(Path.Combine(dir.Path, "no_such_dir", "a.qbd")));

        Assert.Equal(0, raised);
        Assert.NotNull(map.LastError);
    }

    [Fact]
    public async Task N4_DuplicateArticulationId_FailsWithoutChangingMemory()
    {
        using var dir = new TempDir();
        var map = await TestSupport.LoadSampleAsync();
        foreach (var kit in map.Plugins.SelectMany(p => p.Kits))
        {
            kit.ID += 1000; // 保存時に振り直しが発生する状態にする
        }
        var arts = map.Parts.SelectMany(p => p.Articulations).ToList();
        arts[1].ID = arts[0].ID; // FixID が失敗する重複
        var before = TestSupport.Dump(map);

        Assert.False(await map.SaveAsync(dir.File("a.qbd")));

        Assert.Equal(before, TestSupport.Dump(map));
        Assert.False(File.Exists(dir.File("a.qbd")));
    }

    [Fact]
    public async Task N4_FailedSave_KeepsExistingFileAndLeavesNoTempFile()
    {
        using var dir = new TempDir();
        File.WriteAllText(dir.File("a.qbd"), "precious");
        var (map, _) = await LoadedWithEditAsync();

        // 保存先が別プロセスに開かれている状態（置換が失敗する）
        using (new FileStream(dir.File("a.qbd"), FileMode.Open, FileAccess.Read, FileShare.None))
        {
            Assert.False(await map.SaveAsync(dir.File("a.qbd")));
        }

        Assert.Equal("precious", File.ReadAllText(dir.File("a.qbd")));
        Assert.Equal(new[] { dir.File("a.qbd") }, Directory.GetFiles(dir.Path));
        Assert.True(map.IsModified());
    }
}

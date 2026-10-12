using System.IO;
using QBDrumMap.Class.MapModels;

namespace UnitTest;

/// <summary>現行の正常動作を固定する Baseline テスト（修正の前後とも成功すること）。</summary>
[Trait("Category", "Baseline")]
public class QbdRoundTripTests
{
    [Fact]
    public async Task Sample_HasExpectedCounts()
    {
        var map = await TestSupport.LoadSampleAsync();

        Assert.Equal(15, map.Plugins.Count);
        Assert.Equal(39, map.Parts.Count);
        Assert.Equal(139, map.Plugins.Sum(p => p.Kits.Count));
        Assert.All(map.Plugins.SelectMany(p => p.Kits), k => Assert.Equal(128, k.Pitches.Count));
    }

    [Fact]
    public async Task Load_ThenSave_IsTextIdenticalToSample()
    {
        using var dir = new TempDir();
        var map = await TestSupport.LoadSampleAsync();

        Assert.True(await map.SaveAsync(dir.File("a.qbd")));

        Assert.Equal(File.ReadAllText(TestSupport.SampleQbd), File.ReadAllText(dir.File("a.qbd")));
    }

    [Fact]
    public async Task Save_Reload_Save_IsStable()
    {
        using var dir = new TempDir();
        var first = await TestSupport.LoadSampleAsync();
        Assert.True(await first.SaveAsync(dir.File("a.qbd")));

        var second = new MapData();
        Assert.True(await second.LoadAsync(dir.File("a.qbd")));
        Assert.True(await second.SaveAsync(dir.File("b.qbd")));

        Assert.Equal(TestSupport.Dump(first), TestSupport.Dump(second));
        Assert.Equal(File.ReadAllText(dir.File("a.qbd")), File.ReadAllText(dir.File("b.qbd")));
    }

    [Fact]
    public async Task IsModified_FalseAfterLoad_AndAfterReload()
    {
        using var dir = new TempDir();
        var map = await TestSupport.LoadSampleAsync();
        Assert.False(map.IsModified());

        Assert.True(await map.SaveAsync(dir.File("a.qbd")));
        var reloaded = new MapData();
        Assert.True(await reloaded.LoadAsync(dir.File("a.qbd")));

        Assert.False(reloaded.IsModified());
    }

    [Fact]
    public async Task IsModified_TrueAfterEdit_FalseAfterSave()
    {
        using var dir = new TempDir();
        var map = await TestSupport.LoadSampleAsync();

        map.Plugins[0].Kits[0].Pitches[0].ArticulationID = 12345;
        Assert.True(map.IsModified());

        Assert.True(await map.SaveAsync(dir.File("a.qbd")));
        Assert.False(map.IsModified());
    }

    [Fact]
    public async Task Save_NormalizesFileOnly_AndKeepsLiveData()
    {
        using var dir = new TempDir();
        var map = await TestSupport.LoadSampleAsync();
        foreach (var kit in map.Plugins.SelectMany(p => p.Kits))
        {
            kit.ID += 1000;
        }
        var before = TestSupport.Dump(map);

        Assert.True(await map.SaveAsync(dir.File("a.qbd")));

        // 編集中のデータは保存で変わらず、ファイル側だけ正規化（ID振り直し）される
        Assert.Equal(before, TestSupport.Dump(map));
        var fromFile = new MapData();
        Assert.True(await fromFile.LoadAsync(dir.File("a.qbd")));
        Assert.Equal(1, fromFile.Plugins[0].Kits[0].ID);
        Assert.Equal(1001, map.Plugins[0].Kits[0].ID);
        Assert.False(map.IsModified());
    }

    [Fact]
    public async Task Save_OverExistingFile_ReplacesContent_AndLeavesNoTempFile()
    {
        using var dir = new TempDir();
        File.WriteAllText(dir.File("a.qbd"), "old");
        var map = await TestSupport.LoadSampleAsync();

        Assert.True(await map.SaveAsync(dir.File("a.qbd")));

        Assert.Equal(File.ReadAllText(TestSupport.SampleQbd), File.ReadAllText(dir.File("a.qbd")));
        Assert.Equal(new[] { dir.File("a.qbd") }, Directory.GetFiles(dir.Path));
    }

    [Fact]
    public async Task Events_FireOnceOnSuccess()
    {
        using var dir = new TempDir();
        var map = new MapData();
        int loaded = 0, saved = 0;
        map.Loaded += (_, _) => loaded++;
        map.Saved += (_, _) => saved++;

        Assert.True(await map.LoadAsync(TestSupport.SampleQbd));
        Assert.True(await map.SaveAsync(dir.File("a.qbd")));

        Assert.Equal(1, loaded);
        Assert.Equal(1, saved);
    }
}

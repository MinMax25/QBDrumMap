using System.IO;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;

namespace UnitTest;

public class PhaseATests
{
    [Fact]
    public async Task Save_KeepsLiveIdsAndOrder_WhenIdsHaveGaps()
    {
        using var dir = new TempDir();
        var map = await TestSupport.LoadSampleAsync();
        map.Parts[0].ID = 900;
        map.Plugins.Remove(map.Plugins[2]);  
        var before = TestSupport.Dump(map);

        Assert.True(await map.SaveAsync(dir.File("a.qbd")));

        Assert.Equal(before, TestSupport.Dump(map));
    }

    [Fact]
    public async Task Dirty_FalseAfterLoad_AndAfterNew()
    {
        var map = await TestSupport.LoadSampleAsync();
        Assert.False(map.IsModified());

        map.Plugins[0].Name = "edited";
        Assert.True(map.IsModified());

        map.Initialize();
        Assert.False(map.IsModified());
    }

    [Fact]
    public async Task Dirty_TrueAfterPropertyEdit_CollectionEdit_AndNestedEdit()
    {
        var map = await TestSupport.LoadSampleAsync();

        map.Parts[0].Articulations[0].Name = "x";
        Assert.True(map.IsModified());

        map = await TestSupport.LoadSampleAsync();
        map.Plugins[0].Kits.RemoveAt(0);
        Assert.True(map.IsModified());

        map = await TestSupport.LoadSampleAsync();
        map.Parts.Add(new Part { ID = 99, Name = "new" });
        Assert.True(map.IsModified());

        using var dir = new TempDir();
        Assert.True(await map.SaveAsync(dir.File("a.qbd")));
        Assert.False(map.IsModified());
        map.Parts[^1].Name = "renamed";
        Assert.True(map.IsModified());
    }

    [Fact]
    public async Task Dirty_NotRaisedByValidationStateOnly()
    {
        var map = await TestSupport.LoadSampleAsync();

        map.Plugins[0].ResetValidation();

        Assert.False(map.IsModified());
    }

    [Fact]
    public async Task Dirty_TrueAfterUndoStyleRevert_EvenIfSameAsSaved()
    {
        using var dir = new TempDir();
        var map = await TestSupport.LoadSampleAsync();
        var original = map.Plugins[0].Name;
        Assert.True(await map.SaveAsync(dir.File("a.qbd")));

        map.Plugins[0].Name = "tmp";
        map.Plugins[0].Name = original;  

        Assert.True(map.IsModified());
    }

    [Fact]
    public async Task FailedSave_KeepsDirtyAndData()
    {
        using var dir = new TempDir();
        var map = await TestSupport.LoadSampleAsync();
        map.Plugins[0].Name = "edited";
        var before = TestSupport.Dump(map);

        Assert.False(await map.SaveAsync(Path.Combine(dir.Path, "no_such_dir", "a.qbd")));

        Assert.True(map.IsModified());
        Assert.Equal(before, TestSupport.Dump(map));
    }

    [Fact]
    public async Task Save_ThenReload_LogicalDataMatches()
    {
        using var dir = new TempDir();
        var map = await TestSupport.LoadSampleAsync();
        map.Plugins[0].Name = "edited";

        Assert.True(await map.SaveAsync(dir.File("a.qbd")));
        var reloaded = new MapData();
        Assert.True(await reloaded.LoadAsync(dir.File("a.qbd")));

        Assert.Equal(TestSupport.Dump(reloaded), TestSupport.Dump(map));
    }

    [Fact]
    public async Task ReorderForDisplay_DoesNotChangeDirty()
    {
        var map = await TestSupport.LoadSampleAsync();

        map.RunWithoutDirtyChange(() =>
        {
            var plugins = map.Plugins.ToList();
            map.Plugins.Clear();
            plugins.ForEach(map.Plugins.Add);
        });
        Assert.False(map.IsModified());

        map.Plugins[0].Name = "edited";
        map.RunWithoutDirtyChange(() =>
        {
            var plugins = map.Plugins.ToList();
            map.Plugins.Clear();
            plugins.ForEach(map.Plugins.Add);
        });
        Assert.True(map.IsModified());
    }

    [Fact]
    public async Task Guard_NotModified_DoesNotAsk()
    {
        int asked = 0;
        var ok = await UnsavedChangesGuard.ConfirmAsync(() => false, () => { asked++; return Task.FromResult(UnsavedChoice.Cancel); }, () => Task.FromResult(true));
        Assert.True(ok);
        Assert.Equal(0, asked);
    }

    [Fact]
    public async Task Guard_Save_Succeeds_Continues()
    {
        int saved = 0;
        var ok = await UnsavedChangesGuard.ConfirmAsync(() => true, () => Task.FromResult(UnsavedChoice.Save), () => { saved++; return Task.FromResult(true); });
        Assert.True(ok);
        Assert.Equal(1, saved);
    }

    [Fact]
    public async Task Guard_Save_FailsOrCancelled_Aborts()
    {
        var ok = await UnsavedChangesGuard.ConfirmAsync(() => true, () => Task.FromResult(UnsavedChoice.Save), () => Task.FromResult(false));
        Assert.False(ok);
    }

    [Fact]
    public async Task Guard_DontSave_ContinuesWithoutSaving()
    {
        int saved = 0;
        var ok = await UnsavedChangesGuard.ConfirmAsync(() => true, () => Task.FromResult(UnsavedChoice.DontSave), () => { saved++; return Task.FromResult(true); });
        Assert.True(ok);
        Assert.Equal(0, saved);
    }

    [Fact]
    public async Task Guard_Cancel_AbortsWithoutSaving()
    {
        int saved = 0;
        var ok = await UnsavedChangesGuard.ConfirmAsync(() => true, () => Task.FromResult(UnsavedChoice.Cancel), () => { saved++; return Task.FromResult(true); });
        Assert.False(ok);
        Assert.Equal(0, saved);
    }
}

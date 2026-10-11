using System.IO;
using QBDrumMap.Class.MapModels;

namespace UnitTest;

/// <summary>通知（Loaded／Saved）の購読側で例外が出ても、データ処理の成否に影響しないことの再現テスト。</summary>
[Trait("Category", "Regression")]
public class NotificationTests
{
    private static InvalidOperationException Boom() => new("subscriber failure");

    [Fact]
    public async Task Loaded_SubscriberThrows_LoadStillSucceeds_AndOthersAreNotified()
    {
        var map = new MapData();
        int second = 0;
        map.Loaded += (_, _) => throw Boom();
        map.Loaded += (_, _) => second++;

        bool ok = await map.LoadAsync(TestSupport.SampleQbd);

        Assert.True(ok);
        Assert.Equal(15, map.Plugins.Count);
        Assert.False(map.IsModified());
        Assert.Null(map.LastError);
        Assert.NotNull(map.NotifyError);
        Assert.Equal(1, second);
    }

    [Fact]
    public async Task Saved_SubscriberThrows_SaveStillSucceeds_AndOthersAreNotified()
    {
        using var dir = new TempDir();
        var map = await TestSupport.LoadSampleAsync();
        int second = 0;
        map.Saved += (_, _) => throw Boom();
        map.Saved += (_, _) => second++;

        bool ok = await map.SaveAsync(dir.File("a.qbd"));

        Assert.True(ok);
        Assert.Equal(File.ReadAllText(TestSupport.SampleQbd), File.ReadAllText(dir.File("a.qbd")));
        Assert.False(map.IsModified());
        Assert.Null(map.LastError);
        Assert.NotNull(map.NotifyError);
        Assert.Equal(1, second);
    }

    [Fact]
    public async Task Initialize_SubscriberThrows_DoesNotPropagate_AndDataIsCleared()
    {
        var map = await TestSupport.LoadSampleAsync();
        map.Loaded += (_, _) => throw Boom();

        map.Initialize();

        Assert.Empty(map.Plugins);
        Assert.Empty(map.Parts);
        Assert.NotNull(map.NotifyError);
    }

    [Fact]
    public async Task NotifyError_IsClearedByNextSuccessfulOperation()
    {
        var map = new MapData();
        EventHandler<DrumMapIOEventArgs> thrower = (_, _) => throw Boom();
        map.Loaded += thrower;
        await map.LoadAsync(TestSupport.SampleQbd);
        Assert.NotNull(map.NotifyError);

        map.Loaded -= thrower;
        await map.LoadAsync(TestSupport.SampleQbd);

        Assert.Null(map.NotifyError);
    }
}

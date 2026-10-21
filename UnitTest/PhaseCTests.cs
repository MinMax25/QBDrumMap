using QBDrumMap.Class.MapModels;

namespace UnitTest;

public class PhaseCTests
{
    private static readonly string[] Sep = { "\r\n", "\n" };

    private static string[] Lines(string csv) => csv.Split(Sep, StringSplitOptions.RemoveEmptyEntries);

    private static MapData Build()
    {
        var map = new MapData();

        var p1 = new Part { ID = 5, Name = "P1", DisplayOrder = 2 };
        p1.Articulations.Add(new Articulation { ID = 30, Name = "B", DisplayOrder = 2 });
        p1.Articulations.Add(new Articulation { ID = 10, Name = "A", DisplayOrder = 1 });
        var p2 = new Part { ID = 2, Name = "P2", DisplayOrder = 1 };
        p2.Articulations.Add(new Articulation { ID = 7, Name = "C", DisplayOrder = 1 });
        map.Parts.Add(p1);
        map.Parts.Add(p2);

        var plugin = new Plugin { ID = 1, Name = "PL", DisplayOrder = 1 };
        var k1 = new Kit { ID = 1, Name = "K1", DisplayOrder = 1 };
        var k2 = new Kit { ID = 2, Name = "K2", DisplayOrder = 2 };
        plugin.Kits.Add(k1);
        plugin.Kits.Add(k2);
        map.Plugins.Add(plugin);

        k1.Pitches.Add(new KitPitch { Pitch = 36, Name = "n", ArticulationID = 7, DisplayOrder = 1 });
        k1.Pitches.Add(new KitPitch { Pitch = 38, Name = "n", ArticulationID = 10, DisplayOrder = 2 });
        k1.Pitches.Add(new KitPitch { Pitch = 40, Name = "n", ArticulationID = 999, DisplayOrder = 3 });
        k2.Pitches.Add(new KitPitch { Pitch = 50, Name = "n", ArticulationID = 30, DisplayOrder = 1 });
        k2.Pitches.Add(new KitPitch { Pitch = 52, Name = "n", ArticulationID = 10, DisplayOrder = 2 });

        return map;
    }

    [Fact]
    public void Numbers_FollowPartThenArticulationDisplayOrder_From1()
    {
        var n = ArticulationMap.GetQBDrummerNumbers(Build());

        Assert.Equal(1, n[7]);
        Assert.Equal(2, n[10]);
        Assert.Equal(3, n[30]);
    }

    [Fact]
    public void Csv_UsesSequentialNumbers_NotInternalIds()
    {
        var lines = Lines(ArticulationMap.ToQBDrummerCsv(Build(), "K1"));

        Assert.Equal("ID,Sub,Name,Pitchs", lines[0]);
        Assert.Equal("1,0,C,36", lines[1]);
        Assert.Equal("2,0,A,38", lines[2]);
        Assert.StartsWith("3,1,B,", lines[3]);
    }

    [Fact]
    public void SameArticulation_HasSameNumber_AcrossKits()
    {
        var map = Build();
        var l1 = Lines(ArticulationMap.ToQBDrummerCsv(map, "K1"));
        var l2 = Lines(ArticulationMap.ToQBDrummerCsv(map, "K2"));

        Assert.Contains("2,0,A,38", l1);
        Assert.Contains("2,0,A,52", l2);
        Assert.Contains("3,0,B,50", l2);
    }

    [Fact]
    public void OrphanPitch_IsExcluded()
    {
        var csv = ArticulationMap.ToQBDrummerCsv(Build(), "K1");

        Assert.DoesNotContain(Lines(csv).Skip(1), l => l.Split(',').Last().Split('|').Contains("40"));
    }

    [Fact]
    public async Task Csv_SameForSavedAndReloaded_WhenLogicalDataSame()
    {
        using var dir = new TempDir();
        var map = Build();
        Assert.True(await map.SaveAsync(dir.File("a.qbd")));
        var reloaded = new MapData();
        Assert.True(await reloaded.LoadAsync(dir.File("a.qbd")));

        Assert.Equal(ArticulationMap.ToQBDrummerCsv(map, "K1"), ArticulationMap.ToQBDrummerCsv(reloaded, "K1"));
    }

    private static MapData Many(int count)
    {
        var map = new MapData();
        var part = new Part { ID = 1, Name = "P", DisplayOrder = 1 };
        for (int i = 1; i <= count; i++)
        {
            part.Articulations.Add(new Articulation { ID = i, Name = $"a{i}", DisplayOrder = i });
        }
        map.Parts.Add(part);
        return map;
    }

    [Fact]
    public void UpperBound_4095_IsAllowed()
    {
        var n = ArticulationMap.GetQBDrummerNumbers(Many(4095));

        Assert.Equal(4095, n[4095]);
    }

    [Fact]
    public void UpperBound_4096_Fails_WithoutTruncation()
    {
        Assert.Throws<InvalidOperationException>(() => ArticulationMap.GetQBDrummerNumbers(Many(4096)));
    }
}

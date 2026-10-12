using System.IO;
using QBDrumMap.Class.Extensions;
using QBDrumMap.Class.MapModels;

namespace UnitTest;

/// <summary>フェーズB：ID単調採番と参照切れの扱い。</summary>
public class PhaseBTests
{
    [Fact]
    public async Task NewID_IsAboveMax_AfterLoad()
    {
        var map = await TestSupport.LoadSampleAsync();

        Assert.Equal(map.Plugins.Max(x => x.ID) + 1, map.NewPluginID());
        Assert.Equal(map.Plugins.SelectMany(x => x.Kits).Max(x => x.ID) + 1, map.NewKitID());
        Assert.Equal(map.Parts.Max(x => x.ID) + 1, map.NewPartID());
        Assert.Equal(map.Articulations.Max(x => x.ID) + 1, map.NewArticulationID());
    }

    [Fact]
    public async Task NewID_NotReused_AfterDelete()
    {
        var map = await TestSupport.LoadSampleAsync();
        var first = map.NewArticulationID();
        map.Parts[0].Articulations.Add(new Articulation { ID = first, Name = "a" });

        map.Parts[0].Articulations.Remove(map.Parts[0].Articulations.Last());
        var second = map.NewArticulationID();

        Assert.True(second > first);
    }

    [Fact]
    public async Task NewID_NotReused_AfterDeletingMiddle()
    {
        var map = await TestSupport.LoadSampleAsync();
        var gap = map.Plugins[3].ID;
        map.Plugins.RemoveAt(3);

        Assert.NotEqual(gap, map.NewPluginID());
        Assert.True(map.NewPluginID() > map.Plugins.Max(x => x.ID));
    }

    [Fact]
    public async Task NewID_NotReused_AfterDeletingLast_AndNotRewoundBySave()
    {
        using var dir = new TempDir();
        var map = await TestSupport.LoadSampleAsync();
        var id = map.NewPartID();
        map.Parts.Add(new Part { ID = id, Name = "tmp" });
        map.Parts.Remove(map.Parts.Last());

        Assert.True(await map.SaveAsync(dir.File("a.qbd")));

        Assert.True(map.NewPartID() > id);
    }

    [Fact]
    public async Task NewID_Reinitialized_OnLoadAndNew()
    {
        var map = await TestSupport.LoadSampleAsync();
        map.NewKitID();
        map.NewKitID();

        Assert.True(await map.LoadAsync(TestSupport.SampleQbd));
        Assert.Equal(map.Plugins.SelectMany(x => x.Kits).Max(x => x.ID) + 1, map.NewKitID());

        map.Initialize();
        Assert.Equal(1, map.NewKitID());
    }

    [Fact]
    public async Task NewID_IsPerMapData_NotShared()
    {
        var a = await TestSupport.LoadSampleAsync();
        var b = new MapData();
        a.NewPluginID();
        a.NewPluginID();

        Assert.Equal(1, b.NewPluginID());
    }

    [Fact]
    public async Task OrphanArticulationID_IsInvalid_AndNotReconnectedToNewArticulation()
    {
        var map = await TestSupport.LoadSampleAsync();
        var part = map.Parts[0];
        var victim = part.Articulations[0];
        var pitch = map.Plugins[0].Kits[0].Pitches[0];
        pitch.ArticulationID = victim.ID;

        part.Articulations.Remove(victim);
        var newId = map.NewArticulationID();
        part.Articulations.Add(new Articulation { ID = newId, Name = "new" });

        Assert.False(map.IsValidArticulationID(pitch.ArticulationID));
        Assert.NotEqual(newId, pitch.ArticulationID);
    }

    [Fact]
    public async Task OrphanComplement_IsIgnored_InArticulationMap()
    {
        var map = await TestSupport.LoadSampleAsync();
        var kit = map.Plugins[0].Kits[0];
        foreach (var a in map.Articulations)
        {
            a.Complement = 0;
        }
        map.Parts[0].Articulations[0].Complement = 99999; // 実在しない

        var articMap = ArticulationMap.GetArticulationMap(map, kit.Name); // 例外にならない

        Assert.NotEmpty(articMap.Items);
    }

    [Fact]
    public async Task ValidComplement_StillResolved()
    {
        var map = new MapData();
        var part = new Part { ID = 1, Name = "p", DisplayOrder = 1 };
        part.Articulations.Add(new Articulation { ID = 1, Name = "a1", DisplayOrder = 1 });
        part.Articulations.Add(new Articulation { ID = 2, Name = "a2", DisplayOrder = 2, Complement = 1 });
        map.Parts.Add(part);
        var plugin = new Plugin { ID = 1, Name = "pl", DisplayOrder = 1 };
        var kit = new Kit { ID = 1, Name = "k", DisplayOrder = 1 };
        kit.Pitches.Add(new KitPitch { Pitch = 36, Name = "x", ArticulationID = 1, DisplayOrder = 1 });
        plugin.Kits.Add(kit);
        map.Plugins.Add(plugin);

        var articMap = ArticulationMap.GetArticulationMap(map, "k");

        Assert.Equal(new[] { 36 }, articMap.Items.Single(x => x.ID == 2).Pitches);
        await Task.CompletedTask;
    }

    [Fact]
    public async Task AutoAssign_DoesNotPickOrphanReferenceFromOtherKit()
    {
        var map = await TestSupport.LoadSampleAsync();
        var kits = map.Plugins.SelectMany(p => p.Kits).ToArray();
        var source = kits[0];
        var target = kits[1];

        foreach (var p in map.Plugins.SelectMany(x => x.Kits).SelectMany(x => x.Pitches))
        {
            p.ArticulationID = 0;
        }
        var srcPitch = source.Pitches.First(x => !string.IsNullOrWhiteSpace(x.Name));
        srcPitch.ArticulationID = 88888; // 孤立した参照
        var tgtPitch = target.Pitches.First(x => x.Name == srcPitch.Name);

        map.AutoAssignArticulations(target, Array.Empty<PartNameAlias>());

        Assert.NotEqual(88888, tgtPitch.ArticulationID);
    }

    [Fact]
    public async Task AutoAssign_UsesValidReferenceFromOtherKit()
    {
        var map = await TestSupport.LoadSampleAsync();
        var kits = map.Plugins.SelectMany(p => p.Kits).ToArray();
        foreach (var p in kits.SelectMany(x => x.Pitches)) p.ArticulationID = 0;
        var artic = map.Articulations.First();
        var srcPitch = kits[0].Pitches.First(x => !string.IsNullOrWhiteSpace(x.Name));
        srcPitch.ArticulationID = artic.ID;
        var tgtPitch = kits[1].Pitches.First(x => x.Name == srcPitch.Name);

        map.AutoAssignArticulations(kits[1], Array.Empty<PartNameAlias>());

        Assert.Equal(artic.ID, tgtPitch.ArticulationID);
    }
}

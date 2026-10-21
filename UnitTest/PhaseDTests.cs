using System.Collections.ObjectModel;
using QBDrumMap.Class.MapModels;

namespace UnitTest;

public class PhaseDTests
{
    private static ObservableCollection<Part> Make(params int[] orders)
    {
        var list = new ObservableCollection<Part>();
        foreach (var o in orders)
        {
            list.Add(new Part { ID = list.Count + 1, Name = $"P{list.Count + 1}", DisplayOrder = o });
        }
        return list;
    }

    private static ObservableCollection<Part> Pick(ObservableCollection<Part> source, params int[] ids)
        => new(ids.Select(id => source.Single(x => x.ID == id)));

    private static int[] Ids(ObservableCollection<Part> source)
        => source.OrderBy(x => x.DisplayOrder).Select(x => x.ID).ToArray();

    private static int[] Orders(ObservableCollection<Part> source)
        => source.OrderBy(x => x.DisplayOrder).Select(x => x.DisplayOrder).ToArray();

    [Fact]
    public void Up_NonContiguousSelection_KeepsRelativeOrder()
    {
        var s = Make(1, 2, 3, 4, 5, 6);

        s.MoveUp(Pick(s, 2, 5), x => x.DisplayOrder);

        Assert.Equal(new[] { 2, 5, 1, 3, 4, 6 }, Ids(s));
        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6 }, Orders(s));
    }

    [Fact]
    public void Down_NonContiguousSelection_KeepsRelativeOrder()
    {
        var s = Make(1, 2, 3, 4, 5, 6);

        s.MoveDown(Pick(s, 2, 5), x => x.DisplayOrder);

        Assert.Equal(new[] { 1, 3, 4, 6, 2, 5 }, Ids(s));
    }

    private static ObservableCollection<Part> Abcde() => Make(1, 2, 3, 4, 5);

    [Fact]
    public void SpecExample_FourDirections()
    {
        var up = Abcde();
        up.MoveUp(Pick(up, 2, 4), x => x.DisplayOrder);
        Assert.Equal(new[] { 2, 4, 1, 3, 5 }, Ids(up));

        var down = Abcde();
        down.MoveDown(Pick(down, 2, 4), x => x.DisplayOrder);
        Assert.Equal(new[] { 1, 3, 5, 2, 4 }, Ids(down));

        var top = Abcde();
        top.MoveTop(Pick(top, 2, 4), x => x.DisplayOrder);
        Assert.Equal(new[] { 2, 4, 1, 3, 5 }, Ids(top));

        var bottom = Abcde();
        bottom.MoveBottom(Pick(bottom, 2, 4), x => x.DisplayOrder);
        Assert.Equal(new[] { 1, 3, 5, 2, 4 }, Ids(bottom));
    }

    [Fact]
    public void UpAndTop_Differ_InGeneralCase()
    {
        var up = Make(1, 2, 3, 4, 5, 6, 7);
        up.MoveUp(Pick(up, 4, 6), x => x.DisplayOrder);
        Assert.Equal(new[] { 1, 2, 4, 6, 3, 5, 7 }, Ids(up));

        var top = Make(1, 2, 3, 4, 5, 6, 7);
        top.MoveTop(Pick(top, 4, 6), x => x.DisplayOrder);
        Assert.Equal(new[] { 4, 6, 1, 2, 3, 5, 7 }, Ids(top));

        var down = Make(1, 2, 3, 4, 5, 6, 7);
        down.MoveDown(Pick(down, 2, 4), x => x.DisplayOrder);
        Assert.Equal(new[] { 1, 3, 5, 2, 4, 6, 7 }, Ids(down));

        var bottom = Make(1, 2, 3, 4, 5, 6, 7);
        bottom.MoveBottom(Pick(bottom, 2, 4), x => x.DisplayOrder);
        Assert.Equal(new[] { 1, 3, 5, 6, 7, 2, 4 }, Ids(bottom));
    }

    [Fact]
    public void ContiguousSelection_FourDirections()
    {
        var up = Make(1, 2, 3, 4, 5);
        up.MoveUp(Pick(up, 3, 4), x => x.DisplayOrder);
        Assert.Equal(new[] { 1, 3, 4, 2, 5 }, Ids(up));

        var down = Make(1, 2, 3, 4, 5);
        down.MoveDown(Pick(down, 2, 3), x => x.DisplayOrder);
        Assert.Equal(new[] { 1, 4, 2, 3, 5 }, Ids(down));

        var top = Make(1, 2, 3, 4, 5);
        top.MoveTop(Pick(top, 3, 4), x => x.DisplayOrder);
        Assert.Equal(new[] { 3, 4, 1, 2, 5 }, Ids(top));

        var bottom = Make(1, 2, 3, 4, 5);
        bottom.MoveBottom(Pick(bottom, 2, 3), x => x.DisplayOrder);
        Assert.Equal(new[] { 1, 4, 5, 2, 3 }, Ids(bottom));
    }

    [Fact]
    public void NoOp_DoesNotChangeData_ForEmptyOrAtEdge()
    {
        var s = Make(2, 5, 9, 20);
        var before = s.Select(x => x.DisplayOrder).ToArray();

        s.MoveUp(new ObservableCollection<Part>(), x => x.DisplayOrder);
        s.MoveUp(Pick(s, 1), x => x.DisplayOrder);
        s.MoveDown(Pick(s, 4), x => x.DisplayOrder);
        s.MoveTop(Pick(s, 1), x => x.DisplayOrder);
        s.MoveBottom(Pick(s, 4), x => x.DisplayOrder);

        Assert.Equal(before, s.Select(x => x.DisplayOrder).ToArray());
    }

    [Fact]
    public void Duplicates_And_Gaps_WorkWithGroupMove()
    {
        var s = Make(1, 1, 5, 5, 9);

        s.MoveUp(Pick(s, 2, 4), x => x.DisplayOrder);

        Assert.Equal(new[] { 2, 4, 1, 3, 5 }, Ids(s));
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, Orders(s));
    }

    [Fact]
    public void OtherInformation_IsNotChanged_ByMove()
    {
        var s = Make(1, 2, 3, 4);
        var names = s.Select(x => (x.ID, x.Name)).ToArray();

        s.MoveDown(Pick(s, 1, 3), x => x.DisplayOrder);

        Assert.Equal(names, s.Select(x => (x.ID, x.Name)).ToArray());
    }

    [Fact]
    public void UndoRedo_StillWorks_AfterMove()
    {
        var undo = new libQB.UndoRedo.UndoManager();
        var s = Make(1, 2, 3);
        s[0].RegisterUndoManager(undo);
        s[0].Name = "changed";

        s.MoveDown(Pick(s, 2), x => x.DisplayOrder);
        undo.Undo();
        Assert.Equal("P1", s[0].Name);
        undo.Redo();
        Assert.Equal("changed", s[0].Name);
    }

    [Fact]
    public async Task Dirty_TrueAfterMove_ButNotAfterNoOp()
    {
        var map = await TestSupport.LoadSampleAsync();
        var first = new ObservableCollection<Part> { map.Parts.OrderBy(x => x.DisplayOrder).First() };

        map.Parts.MoveUp(first, x => x.DisplayOrder);
        Assert.False(map.IsModified());

        map.Parts.MoveDown(first, x => x.DisplayOrder);
        Assert.True(map.IsModified());
    }

    [Fact]
    public void ReverseSelectionOrder_GivesSameResult()
    {
        var a = Make(1, 2, 3, 4, 5, 6);
        var b = Make(1, 2, 3, 4, 5, 6);

        a.MoveUp(Pick(a, 2, 5), x => x.DisplayOrder);
        b.MoveUp(Pick(b, 5, 2), x => x.DisplayOrder);

        Assert.Equal(Ids(a), Ids(b));
    }

    [Fact]
    public void Up_WhenFirstSelectedIsAlreadyTop_OthersStillMove_FirstStays()
    {
        var s = Make(1, 2, 3, 4);

        s.MoveUp(Pick(s, 1, 3), x => x.DisplayOrder);

        Assert.Equal(new[] { 1, 3, 2, 4 }, Ids(s));
    }

    [Fact]
    public void Up_ContiguousBlockAtTop_NoChange()
    {
        var s = Make(1, 2, 3, 4);

        s.MoveUp(Pick(s, 1, 2), x => x.DisplayOrder);

        Assert.Equal(new[] { 1, 2, 3, 4 }, Ids(s));
    }

    [Fact]
    public void Down_ContiguousBlockAtBottom_NoChange()
    {
        var s = Make(1, 2, 3, 4);

        s.MoveDown(Pick(s, 3, 4), x => x.DisplayOrder);

        Assert.Equal(new[] { 1, 2, 3, 4 }, Ids(s));
    }

    [Fact]
    public void Top_And_Bottom_GatherSelection_KeepRelativeOrder()
    {
        var a = Make(1, 2, 3, 4, 5);
        a.MoveTop(Pick(a, 4, 2), x => x.DisplayOrder);
        Assert.Equal(new[] { 2, 4, 1, 3, 5 }, Ids(a));

        var b = Make(1, 2, 3, 4, 5);
        b.MoveBottom(Pick(b, 4, 2), x => x.DisplayOrder);
        Assert.Equal(new[] { 1, 3, 5, 2, 4 }, Ids(b));
    }

    [Fact]
    public void Gaps_AreNormalizedOnMove()
    {
        var s = Make(2, 5, 9, 20);

        s.MoveDown(Pick(s, 1), x => x.DisplayOrder);

        Assert.Equal(new[] { 2, 1, 3, 4 }, Ids(s));
        Assert.Equal(new[] { 1, 2, 3, 4 }, Orders(s));
    }

    [Fact]
    public void Gaps_SelectedSingleAtEdge_DoesNotChangeValues()
    {
        var s = Make(2, 5, 9);

        s.MoveUp(Pick(s, 1), x => x.DisplayOrder);

        Assert.Equal(new[] { 2, 5, 9 }, Orders(s));
    }

    [Fact]
    public void Duplicates_KeepStableOrder_ThenRenumber()
    {
        var s = Make(1, 1, 1, 2);

        s.MoveDown(Pick(s, 1), x => x.DisplayOrder);

        Assert.Equal(new[] { 2, 1, 3, 4 }, Ids(s));
        Assert.Equal(new[] { 1, 2, 3, 4 }, Orders(s));
    }

    [Fact]
    public void AllSelected_NoChange()
    {
        var s = Make(1, 2, 3);

        s.MoveUp(Pick(s, 1, 2, 3), x => x.DisplayOrder);
        s.MoveDown(Pick(s, 1, 2, 3), x => x.DisplayOrder);
        s.MoveTop(Pick(s, 1, 2, 3), x => x.DisplayOrder);
        s.MoveBottom(Pick(s, 1, 2, 3), x => x.DisplayOrder);

        Assert.Equal(new[] { 1, 2, 3 }, Ids(s));
    }

    [Fact]
    public void SingleSelection_MovesOneStep()
    {
        var s = Make(1, 2, 3);

        s.MoveUp(Pick(s, 3), x => x.DisplayOrder);
        Assert.Equal(new[] { 1, 3, 2 }, Ids(s));

        s.MoveDown(Pick(s, 3), x => x.DisplayOrder);
        Assert.Equal(new[] { 1, 2, 3 }, Ids(s));
    }

    [Fact]
    public void EmptySelection_NoChange()
    {
        var s = Make(1, 2, 3);

        s.MoveTop(new ObservableCollection<Part>(), x => x.DisplayOrder);

        Assert.Equal(new[] { 1, 2, 3 }, Ids(s));
    }

    [Fact]
    public void OtherProperty_DrumMapOrder_IsUsedWhenSpecified()
    {
        var s = new ObservableCollection<Articulation>
        {
            new() { ID = 1, DisplayOrder = 1, DrumMapOrder = 3 },
            new() { ID = 2, DisplayOrder = 2, DrumMapOrder = 1 },
            new() { ID = 3, DisplayOrder = 3, DrumMapOrder = 2 },
        };
        var selected = new ObservableCollection<Articulation> { s[0] };

        s.MoveUp(selected, x => x.DrumMapOrder);

        Assert.Equal(new[] { 2, 1, 3 }, s.OrderBy(x => x.DrumMapOrder).Select(x => x.ID).ToArray());
        Assert.Equal(new[] { 1, 2, 3 }, s.Select(x => x.DisplayOrder).ToArray());
    }
}

using System.IO;
using System.Xml.Linq;
using libQB.UndoRedo;
using QBDrumMap.Class.MapModels;

namespace UnitTest;

/// <summary>残存項目：多言語リソースの整合と、保存がUndo履歴を変えないこと。</summary>
public class LocalizationAndUndoTests
{
    // このWorkで追加した文言のキー
    private static readonly string[] AddedKeys =
    {
        "Confirm_UnsavedChanges", "Button_Save", "Button_DontSave", "Button_Cancel",
        "Busy_Saving", "Busy_Loading", "Error_QBDrummerTooManyArticulations",
        "Error_FileIO", "Title_ExportFormat", "Title_ConvertBaseOnKit",
        "Common_OpenFile", "Common_SaveFile", "ButtonImport",
    };

    private static string ProjectDir()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, "QBDrumMap", "LanguagePacks")))
        {
            dir = dir.Parent;
        }
        Assert.NotNull(dir);
        return Path.Combine(dir!.FullName, "QBDrumMap");
    }

    private static IEnumerable<string> ResxFiles()
    {
        var root = ProjectDir();
        yield return Path.Combine(root, "Properties", "Resources.resx");
        yield return Path.Combine(root, "Properties", "Resources.ja.resx");
        foreach (var f in Directory.GetFiles(Path.Combine(root, "LanguagePacks"), "QBDrumMap.Resources.resx", SearchOption.AllDirectories))
        {
            yield return f;
        }
    }

    [Fact]
    public void Resx_HasNineLanguages()
    {
        Assert.Equal(9, ResxFiles().Count());
    }

    [Fact]
    public void AddedKeys_ExistInAllResx_WithNonEmptyValue_AndNoDuplicates()
    {
        foreach (var file in ResxFiles())
        {
            var data = XDocument.Load(file).Root!.Elements("data").ToList();
            var names = data.Select(x => (string)x.Attribute("name")!).ToList();

            Assert.True(names.Count == names.Distinct().Count(), $"重複キー: {file}");
            foreach (var key in AddedKeys)
            {
                var value = data.FirstOrDefault(x => (string)x.Attribute("name")! == key)?.Element("value")?.Value;
                Assert.False(string.IsNullOrWhiteSpace(value), $"{key} が {file} に無い、または空");
            }
        }
    }

    [Fact]
    public void FormatKeys_KeepPlaceholders_InAllResx()
    {
        foreach (var file in ResxFiles())
        {
            var data = XDocument.Load(file).Root!.Elements("data").ToDictionary(x => (string)x.Attribute("name")!, x => x.Element("value")!.Value);

            Assert.Contains("{0}", data["Error_QBDrummerTooManyArticulations"]);
            Assert.Contains("{0}", data["Title_ExportFormat"]);
            Assert.Contains("{0}", data["Title_ConvertBaseOnKit"]);
        }
    }

    [Fact]
    public void Source_HasNoHardCodedFileIoErrorTexts()
    {
        var root = ProjectDir();
        var sources = Directory.GetFiles(Path.Combine(root, "ViewModels"), "*.cs", SearchOption.AllDirectories);

        foreach (var file in sources)
        {
            var text = File.ReadAllText(file);
            // 既存キー（App.Text）の既定値としての出現だけを許す
            foreach (var literal in new[] { "\"Load\"", "\"Save\"", "\"Import\"" })
            {
                Assert.DoesNotContain($", {literal});", text);
            }
            Assert.DoesNotContain("File IO Error{Environment.NewLine}", text.Replace("App.Text(\"Error_FileIO\", \"File IO Error\")", ""));
        }
    }

    [Fact]
    public async Task Save_DoesNotChangeUndoHistory_AndUndoRedoWorkAfter()
    {
        using var dir = new TempDir();
        var map = await TestSupport.LoadSampleAsync();
        var undo = new UndoManager();
        var plugin = map.Plugins[0];
        var original = plugin.Name;
        plugin.RegisterUndoManager(undo);
        plugin.Name = "edited";
        Assert.True(undo.CanUndo);
        Assert.False(undo.CanRedo);

        Assert.True(await map.SaveAsync(dir.File("a.qbd")));

        // 保存はUndo履歴を変えず、保存後も通常どおりUndo/Redoできる
        Assert.True(undo.CanUndo);
        Assert.False(undo.CanRedo);
        Assert.Equal("edited", plugin.Name);
        undo.Undo();
        Assert.Equal(original, plugin.Name);
        undo.Redo();
        Assert.Equal("edited", plugin.Name);
    }

    [Fact]
    public async Task FailedLoad_KeepsData_Dirty_AndUndoHistory()
    {
        using var dir = new TempDir();
        File.WriteAllText(dir.File("bad.qbd"), "{ broken");
        var map = await TestSupport.LoadSampleAsync();
        var undo = new UndoManager();
        map.Plugins[0].RegisterUndoManager(undo);
        map.Plugins[0].Name = "edited";
        var before = TestSupport.Dump(map);

        Assert.False(await map.LoadAsync(dir.File("bad.qbd")));

        Assert.Equal(before, TestSupport.Dump(map));
        Assert.True(map.IsModified());
        Assert.True(undo.CanUndo);
        Assert.NotNull(map.LastError);
    }
}

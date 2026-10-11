using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using CommunityToolkit.Mvvm.ComponentModel;
using libQB.Attributes;

namespace QBDrumMap.Class.MapModels;

[DISingleton]
public partial class MapData : ObservableObject
{
    public EventHandler<DrumMapIOEventArgs> Loaded;

    public EventHandler<DrumMapIOEventArgs> Saved;

    public EventHandler<PropertyChangedEventArgs> EditStateChanged;

    private MapData replica;

    [JsonIgnore]
    public Exception LastError { get; private set; }

    /// <summary>Loaded/Saved の購読側で出た例外。データ処理の成否とは分離して記録する。</summary>
    [JsonIgnore]
    public Exception NotifyError { get; private set; }

    private List<ViewModelBase> EditingList = new();

    [ObservableProperty]
    private string appName;

    [ObservableProperty]
    private string modelVersion;

    [ObservableProperty]
    private ObservableCollection<Plugin> plugins = new();

    [ObservableProperty]
    private ObservableCollection<Part> parts = new();

    private bool _EditState;

    [JsonIgnore]
    public IEnumerable<Articulation> Articulations
    {
        get
        {
            return Parts.SelectMany(p => p.Articulations);
        }
    }

    [JsonIgnore]
    public bool EditState
    {
        get
        {
            return _EditState;
        }
        private set
        {
            SetProperty(ref _EditState, value);
        }
    }

    public MapData()
    {
        AppName = typeof(App).Assembly.GetName().Name;
        ModelVersion = "1.0.0";
    }

    public void SetEditState(ViewModelBase sender, bool isEditing)
    {
        bool stateChanged = false;

        if (isEditing)
        {
            if (!EditingList.Contains(sender))
            {
                EditingList.Add(sender);
                stateChanged = true;
            }
        }
        else
        {
            if (EditingList.Contains(sender))
            {
                EditingList.Remove(sender);
                stateChanged = true;
            }
        }

        if (stateChanged)
        {
            EditState = EditingList.Count > 0;
            EditStateChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EditState)));
        }
    }

    public void Initialize()
    {
        replica = new MapData();

        Plugins.Clear();
        Parts.Clear();

        NotifyError = null;
        RaiseSafely(Loaded, new DrumMapIOEventArgs(true));
    }

    public async Task<bool> LoadAsync(string path)
    {
        try
        {
            string jsonString = await File.ReadAllTextAsync(path);
            var data = JsonSerializer.Deserialize<MapData>(jsonString);

            if (data == null)
            {
                throw new InvalidDataException("QBD file is empty.");
            }

            // 現在のデータには触れず、新しいデータを最後まで組み立ててから反映する
            var newPlugins = new List<Plugin>();
            var newParts = new List<Part>();

            foreach (var plugin in data.Plugins.Select((x, i) => new { item = x, order = i }))
            {
                var newPlugin = new Plugin
                {
                    ID = plugin.item.ID,
                    Name = plugin.item.Name,
                    PluginType = plugin.item.PluginType,
                    MidiOutDevice = plugin.item.MidiOutDevice,
                    SoundCheckChannel = plugin.item.SoundCheckChannel,
                    DisplayOrder = plugin.order + 1
                };

                foreach (var kit in plugin.item.Kits.Select((x, i) => new { item = x, order = i }))
                {
                    var newKit = new Kit
                    {
                        ID = kit.item.ID,
                        Name = kit.item.Name,
                        BankSelectMSB = kit.item.BankSelectMSB,
                        BankSelectLSB = kit.item.BankSelectLSB,
                        ProgramNumber = kit.item.ProgramNumber,
                        DisplayOrder = kit.order + 1
                    };

                    foreach (var kitPitch in kit.item.Pitches.Select((x, i) => new { item = x, order = i }))
                    {
                        var newKitPitch = new KitPitch
                        {
                            Pitch = kitPitch.item.Pitch,
                            Name = kitPitch.item.Name,
                            ArticulationID = kitPitch.item.ArticulationID,
                            Separator = kitPitch.item.Separator,
                            DisplayOrder = kitPitch.order + 1
                        };

                        newKit.Pitches.Add(newKitPitch);
                    }

                    newPlugin.Kits.Add(newKit);
                }

                newPlugins.Add(newPlugin);
            }

            foreach (var part in data.Parts.Select((x, i) => new { item = x, order = i }))
            {
                var newPart = new Part
                {
                    ID = part.item.ID,
                    Name = part.item.Name,
                    ScorePitch = part.item.ScorePitch,
                    Color = part.item.Color,
                    NoteHead = part.item.NoteHead,
                    Voice = part.item.Voice,
                    Technique = part.item.Technique,
                    NoteHeadSet = part.item.NoteHeadSet,
                    InstrumentEntityID = part.item.InstrumentEntityID,
                    TechniqueEntityID = part.item.TechniqueEntityID,
                    DisplayOrder = part.order + 1
                };

                foreach (var articulation in part.item.Articulations.Select((x, i) => new { item = x, order = i }))
                {
                    var newArticulation = new Articulation
                    {
                        ID = articulation.item.ID,
                        Name = articulation.item.Name,
                        Complement = articulation.item.Complement,
                        DrumMapOrder = articulation.item.DrumMapOrder,
                        DisplayOrder = articulation.order + 1
                    };

                    newPart.Articulations.Add(newArticulation);
                }

                newParts.Add(newPart);
            }

            // 組み立て中に現在のデータとの重複で付いた検証エラーは、読込結果の状態ではないため消す
            foreach (var model in newPlugins.Cast<ModelBase>()
                .Concat(newPlugins.SelectMany(x => x.Kits))
                .Concat(newPlugins.SelectMany(x => x.Kits).SelectMany(x => x.Pitches))
                .Concat(newParts)
                .Concat(newParts.SelectMany(x => x.Articulations)))
            {
                model.ResetValidation();
            }

            // ここから先は失敗しない操作だけを行う
            Plugins.Clear();
            Parts.Clear();

            foreach (var plugin in newPlugins)
            {
                Plugins.Add(plugin);
            }

            foreach (var part in newParts)
            {
                Parts.Add(part);
            }

            replica = data;
            LastError = null;
            NotifyError = null;

            // データは確定済み。通知の失敗は読込の成否に影響させない
            RaiseSafely(Loaded, new DrumMapIOEventArgs(true));

            return true;
        }
        catch (Exception ex)
        {
            // 失敗時は現在のデータ・Dirty状態・画面を変えない（イベントも発火しない）
            LastError = ex;
            return false;
        }
    }

    public async Task<bool> SaveAsync(string path)
    {
        string tempPath = null;
        MapData snapshot;

        try
        {
            // 保存内容は複製で作る。ここまでの失敗では編集中のデータに触れない
            snapshot = Clone();
            FixID(snapshot);

            var options = new JsonSerializerOptions
            {
                IgnoreReadOnlyFields = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };

            var jsonString = JsonSerializer.Serialize(snapshot, options);

            // 同じフォルダの一時ファイルへ書き切ってから、保存先へ確定する
            var fullPath = Path.GetFullPath(path);
            tempPath = Path.Combine(Path.GetDirectoryName(fullPath), $".{Path.GetFileName(fullPath)}.{Guid.NewGuid():N}.tmp");
            await File.WriteAllTextAsync(tempPath, jsonString);

            if (File.Exists(fullPath))
            {
                File.Replace(tempPath, fullPath, null);
            }
            else
            {
                File.Move(tempPath, fullPath);
            }

            tempPath = null;
        }
        catch (Exception ex)
        {
            // 失敗時は保存先・編集中のデータ・Dirty状態を変えない（イベントも発火しない）
            LastError = ex;
            DeleteTempFile(tempPath);
            return false;
        }

        // ファイルは確定済み。以降はファイルの内容を正として、メモリ側を合わせる
        replica = snapshot;
        LastError = null;

        try
        {
            FixID(this);
        }
        catch
        {
            // 振り直しを反映しきれなかった場合は、replicaとの差としてDirtyのままになり、再保存で解消できる
        }

        NotifyError = null;
        RaiseSafely(Saved, new DrumMapIOEventArgs(true));

        return true;
    }

    /// <summary>購読者ごとに例外を捕捉して通知する。例外は NotifyError に記録し、他の購読者への通知は続ける。</summary>
    private void RaiseSafely(EventHandler<DrumMapIOEventArgs> handler, DrumMapIOEventArgs e)
    {
        if (handler == null)
        {
            return;
        }

        foreach (EventHandler<DrumMapIOEventArgs> subscriber in handler.GetInvocationList())
        {
            try
            {
                subscriber(this, e);
            }
            catch (Exception ex)
            {
                NotifyError = ex;
            }
        }
    }

    private static void DeleteTempFile(string tempPath)
    {
        try
        {
            if (tempPath != null && File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
        catch
        {
            // 一時ファイルの片付けに失敗しても、保存の成否には影響させない
        }
    }

    public MapData Clone()
    {
        var clone = new MapData();

        foreach (var plugin in Plugins.OrderBy(x => x.DisplayOrder))
        {
            var newPlugin = new Plugin
            {
                ID = plugin.ID,
                Name = plugin.Name,
                PluginType = plugin.PluginType,
                MidiOutDevice = plugin.MidiOutDevice,
                SoundCheckChannel = plugin.SoundCheckChannel,
                DisplayOrder = plugin.DisplayOrder,
            };

            foreach (var kit in plugin.Kits.OrderBy(x => x.DisplayOrder))
            {
                var newKit = new Kit
                {
                    ID = kit.ID,
                    Name = kit.Name,
                    BankSelectMSB = kit.BankSelectMSB,
                    BankSelectLSB = kit.BankSelectLSB,
                    ProgramNumber = kit.ProgramNumber,
                    DisplayOrder = kit.DisplayOrder,
                };

                foreach (var kitPitch in kit.Pitches.OrderBy(x => x.DisplayOrder))
                {
                    var newKitPitch = new KitPitch
                    {
                        Pitch = kitPitch.Pitch,
                        Name = kitPitch.Name,
                        ArticulationID = kitPitch.ArticulationID,
                        Separator = kitPitch.Separator,
                        DisplayOrder = kitPitch.DisplayOrder,
                    };

                    newKit.Pitches.Add(newKitPitch);
                }

                newPlugin.Kits.Add(newKit);
            }

            clone.Plugins.Add(newPlugin);
        }

        foreach (var part in Parts.OrderBy(x => x.DisplayOrder))
        {
            var newPart = new Part
            {
                ID = part.ID,
                Name = part.Name,
                ScorePitch = part.ScorePitch,
                Color = part.Color,
                NoteHead = part.NoteHead,
                Voice = part.Voice,
                Technique = part.Technique,
                NoteHeadSet = part.NoteHeadSet,
                InstrumentEntityID = part.InstrumentEntityID,
                TechniqueEntityID = part.TechniqueEntityID,
                DisplayOrder = part.DisplayOrder,
            };

            foreach (var articulation in part.Articulations.OrderBy(x => x.DisplayOrder))
            {
                var newArticulation = new Articulation
                {
                    ID = articulation.ID,
                    Name = articulation.Name,
                    Complement = articulation.Complement,
                    DrumMapOrder = articulation.DrumMapOrder,
                    DisplayOrder = articulation.DisplayOrder,
                };

                newPart.Articulations.Add(newArticulation);
            }

            clone.Parts.Add(newPart);
        }

        return clone;
    }

    public bool IsModified()
    {
        if (replica == null)
        {
            if (Plugins.Any() || Parts.Any())
            {
                return true;
            }
            return false;
        }
        return CompareMapData(replica, this);
    }

    public bool HasError()
    {
        return Plugins.Any(x => x.HasErrors) ||
               Plugins.SelectMany(x => x.Kits).Any(x => x.HasErrors) ||
               Plugins.SelectMany(x => x.Kits).SelectMany(x => x.Pitches).Any(x => x.HasErrors) ||
               Parts.Any(x => x.HasErrors) ||
               Parts.SelectMany(x => x.Articulations).Any(x => x.HasErrors);
    }

    private void FixID(MapData map)
    {
        int kitID = 1;
        foreach (var plugin in map.Plugins.OrderBy(x => x.DisplayOrder).Select((x, i) => new { item = x, order = i }).ToArray())
        {
            plugin.item.ID = plugin.order + 1;
            plugin.item.DisplayOrder = plugin.order + 1;
            foreach (var kit in plugin.item.Kits.OrderBy(x => x.DisplayOrder).Select((x, i) => new { item = x, order = i }).ToArray())
            {
                kit.item.ID = kitID++;
                kit.item.DisplayOrder = kit.order + 1;
            }
        }

        Dictionary<int, int> artics = new();
        int articID = 1;
        foreach (var part in map.Parts.OrderBy(x => x.DisplayOrder).Select((x, i) => new { item = x, order = i }).ToArray())
        {
            part.item.ID = part.order + 1;
            part.item.DisplayOrder = part.order + 1;
            foreach (var articulation in part.item.Articulations.OrderBy(x => x.DisplayOrder).Select((x, i) => new { item = x, order = i }).ToArray())
            {
                articulation.item.DisplayOrder = articulation.order + 1;
                artics.Add(articulation.item.ID, articID++);
            }
        }

        Func<int, int> getNewArticulationID = (oldId) =>
        {
            if (artics.TryGetValue(oldId, out var id))
            {
                return id;
            }
            return 0;
        };

        int drumMapOrder = 1;
        foreach (var articulation in map.Parts.SelectMany(x => x.Articulations).OrderBy(a => a.DrumMapOrder).ToArray())
        {
            articulation.DrumMapOrder = drumMapOrder++;
        }

        foreach (var kitPitch in map.Plugins.SelectMany(x => x.Kits).SelectMany(x => x.Pitches).ToArray())
        {
            if (kitPitch.ArticulationID != 0)
            {
                kitPitch.ArticulationID = getNewArticulationID(kitPitch.ArticulationID);
            }
        }

        foreach (var articulation in map.Parts.SelectMany(x => x.Articulations).ToArray())
        {
            if (articulation.ID != 0)
            {
                articulation.ID = getNewArticulationID(articulation.ID);
            }
            if (articulation.Complement != 0)
            {
                articulation.Complement = getNewArticulationID(articulation.Complement);
            }
        }
    }

    private bool CompareMapData(MapData src, MapData dst)
    {
        if (src.Plugins.Count != dst.Plugins.Count)
        {
            return true;
        }
        if (src.Parts.Count != dst.Parts.Count)
        {
            return true;
        }

        for (int i = 0; i < src.Plugins.Count; i++)
        {
            if (ComparePlugin(src.Plugins[i], dst.Plugins.OrderBy(x => x.DisplayOrder).ToArray()[i]))
            {
                return true;
            }
        }

        for (int i = 0; i < src.Parts.Count; i++)
        {
            if (ComparePart(src.Parts[i], dst.Parts.OrderBy(x => x.DisplayOrder).ToArray()[i]))
            {
                return true;
            }
        }

        return false;
    }

    private bool ComparePlugin(Plugin src, Plugin dst)
    {
        if (src.Kits.Count != dst.Kits.Count)
        {
            return true;
        }
        if (src.ID != dst.ID)
        {
            return true;
        }
        if (src.Name != dst.Name)
        {
            return true;
        }
        if (src.PluginType != dst.PluginType)
        {
            return true;
        }
        if (src.MidiOutDevice != dst.MidiOutDevice)
        {
            return true;
        }
        if (src.SoundCheckChannel != dst.SoundCheckChannel)
        {
            return true;
        }

        for (int i = 0; i < src.Kits.Count; i++)
        {
            if (CompareKit(src.Kits[i], dst.Kits.OrderBy(x => x.DisplayOrder).ToArray()[i]))
            {
                return true;
            }
        }
        return false;
    }

    private bool CompareKit(Kit src, Kit dst)
    {
        if (src.ID != dst.ID)
        {
            return true;
        }
        if (src.Name != dst.Name)
        {
            return true;
        }
        if (src.Pitches.Count != dst.Pitches.Count)
        {
            return true;
        }
        if (src.BankSelectMSB != dst.BankSelectMSB)
        {
            return true;
        }
        if (src.BankSelectLSB != dst.BankSelectLSB)
        {
            return true;
        }
        if (src.ProgramNumber != dst.ProgramNumber)
        {
            return true;
        }

        var srcPitches = src.Pitches.OrderBy(x => x.DisplayOrder).ToArray();
        var dstPitches = dst.Pitches.OrderBy(x => x.DisplayOrder).ToArray();

        for (int i = 0; i < srcPitches.Length; i++)
        {
            if (CompareKitPitch(srcPitches[i], dstPitches[i]))
            {
                return true;
            }
        }
        return false;
    }

    private bool CompareKitPitch(KitPitch src, KitPitch dst)
    {
        if (src.Pitch != dst.Pitch)
        {
            return true;
        }
        if (src.Name != dst.Name)
        {
            return true;
        }
        if (src.ArticulationID != dst.ArticulationID)
        {
            return true;
        }
        if (src.Separator != dst.Separator)
        {
            return true;
        }
        return false;
    }

    private bool ComparePart(Part src, Part dst)
    {
        if (src.Name != dst.Name)
        {
            return true;
        }
        if (src.ScorePitch != dst.ScorePitch)
        {
            return true;
        }
        if (src.Color != dst.Color)
        {
            return true;
        }
        if (src.NoteHead != dst.NoteHead)
        {
            return true;
        }
        if (src.Technique != dst.Technique)
        {
            return true;
        }
        if (src.InstrumentEntityID != dst.InstrumentEntityID)
        {
            return true;
        }
        if (src.NoteHeadSet != dst.NoteHeadSet)
        {
            return true;
        }
        if (src.Voice != dst.Voice)
        {
            return true;
        }
        if (src.TechniqueEntityID != dst.TechniqueEntityID)
        {
            return true;
        }
        if (src.Articulations.Count != dst.Articulations.Count)
        {
            return true;
        }

        var srcArtics = src.Articulations.OrderBy(x => x.DisplayOrder).ToArray();
        var dstArtics = dst.Articulations.OrderBy(x => x.DisplayOrder).ToArray();

        for (int i = 0; i < srcArtics.Length; i++)
        {
            if (CompareArticulation(srcArtics[i], dstArtics[i]))
            {
                return true;
            }
        }

        return false;
    }

    private bool CompareArticulation(Articulation src, Articulation dst)
    {
        if (src.ID != dst.ID)
        {
            return true;
        }
        if (src.Name != dst.Name)
        {
            return true;
        }
        if (src.Complement != dst.Complement)
        {
            return true;
        }
        if (src.DrumMapOrder != dst.DrumMapOrder)
        {
            return true;
        }
        return false;
    }
}
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

    private bool isDirty;

    private int lastPluginID;
    private int lastKitID;
    private int lastPartID;
    private int lastArticulationID;

    [JsonIgnore]
    public Exception LastError { get; private set; }

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

        HookCollection(Plugins);
        HookCollection(Parts);
    }

    partial void OnPluginsChanged(ObservableCollection<Plugin> value)
    {
        HookCollection(value);
    }

    partial void OnPartsChanged(ObservableCollection<Part> value)
    {
        HookCollection(value);
    }

    public void RunWithoutDirtyChange(Action action)
    {
        bool dirty = isDirty;
        try
        {
            action();
        }
        finally
        {
            isDirty = dirty;
        }
    }

    private void HookCollection<T>(ObservableCollection<T> collection)
    {
        if (collection == null)
        {
            return;
        }

        collection.CollectionChanged -= OnModelCollectionChanged;
        collection.CollectionChanged += OnModelCollectionChanged;

        foreach (var item in collection)
        {
            HookModel(item, true);
        }
    }

    private void UnhookCollection<T>(ObservableCollection<T> collection)
    {
        if (collection == null)
        {
            return;
        }

        collection.CollectionChanged -= OnModelCollectionChanged;

        foreach (var item in collection)
        {
            HookModel(item, false);
        }
    }

    private void HookModel(object model, bool attach)
    {
        if (model is INotifyPropertyChanged notify)
        {
            notify.PropertyChanged -= OnModelPropertyChanged;
            if (attach)
            {
                notify.PropertyChanged += OnModelPropertyChanged;
            }
        }

        switch (model)
        {
            case Plugin plugin:
                HookChildren(plugin.Kits, attach);
                break;
            case Kit kit:
                HookChildren(kit.Pitches, attach);
                break;
            case Part part:
                HookChildren(part.Articulations, attach);
                break;
        }
    }

    private void HookChildren<T>(ObservableCollection<T> collection, bool attach)
    {
        if (attach)
        {
            HookCollection(collection);
        }
        else
        {
            UnhookCollection(collection);
        }
    }

    private void OnModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (var item in e.OldItems)
            {
                HookModel(item, false);
            }
        }

        if (e.NewItems != null)
        {
            foreach (var item in e.NewItems)
            {
                HookModel(item, true);
            }
        }

        isDirty = true;
    }

    private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ObservableValidator.HasErrors))
        {
            return;
        }

        switch (sender)
        {
            case Plugin plugin when e.PropertyName == nameof(Plugin.Kits):
                HookChildren(plugin.Kits, true);
                break;
            case Kit kit when e.PropertyName == nameof(Kit.Pitches):
                HookChildren(kit.Pitches, true);
                break;
            case Part part when e.PropertyName == nameof(Part.Articulations):
                HookChildren(part.Articulations, true);
                break;
        }

        isDirty = true;
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

    public int NewPluginID() => Issue(ref lastPluginID, Plugins.Select(x => x.ID));

    public int NewKitID() => Issue(ref lastKitID, Plugins.SelectMany(x => x.Kits).Select(x => x.ID));

    public int NewPartID() => Issue(ref lastPartID, Parts.Select(x => x.ID));

    public int NewArticulationID() => Issue(ref lastArticulationID, Articulations.Select(x => x.ID));

    private static int Issue(ref int last, IEnumerable<int> current)
    {
        last = Math.Max(last, current.DefaultIfEmpty(0).Max()) + 1;
        return last;
    }

    private void ResetIssuedIDs()
    {
        lastPluginID = Plugins.Select(x => x.ID).DefaultIfEmpty(0).Max();
        lastKitID = Plugins.SelectMany(x => x.Kits).Select(x => x.ID).DefaultIfEmpty(0).Max();
        lastPartID = Parts.Select(x => x.ID).DefaultIfEmpty(0).Max();
        lastArticulationID = Articulations.Select(x => x.ID).DefaultIfEmpty(0).Max();
    }

    public bool IsValidArticulationID(int id)
    {
        return id != 0 && Articulations.Any(x => x.ID == id);
    }

    public void Initialize()
    {
        Plugins.Clear();
        Parts.Clear();

        ResetIssuedIDs();

        isDirty = false;
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

            foreach (var model in newPlugins.Cast<ModelBase>()
                .Concat(newPlugins.SelectMany(x => x.Kits))
                .Concat(newPlugins.SelectMany(x => x.Kits).SelectMany(x => x.Pitches))
                .Concat(newParts)
                .Concat(newParts.SelectMany(x => x.Articulations)))
            {
                model.ResetValidation();
            }

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

            ResetIssuedIDs();
            isDirty = false;
            LastError = null;
            NotifyError = null;

            RaiseSafely(Loaded, new DrumMapIOEventArgs(true));

            return true;
        }
        catch (Exception ex)
        {
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
            LastError = ex;
            DeleteTempFile(tempPath);
            return false;
        }

        isDirty = false;
        LastError = null;

        NotifyError = null;
        RaiseSafely(Saved, new DrumMapIOEventArgs(true));

        return true;
    }

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
        return isDirty;
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
}

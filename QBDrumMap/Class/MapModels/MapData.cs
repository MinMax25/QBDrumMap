using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using CommunityToolkit.Mvvm.ComponentModel;
using libQB.Attributes;

namespace QBDrumMap.Class.MapModels
{
    [DISingleton]
    public partial class MapData
        : ObservableObject
    {
        public EventHandler<DrumMapIOEventArgs> Loaded;

        public EventHandler<DrumMapIOEventArgs> Saved;

        public EventHandler<PropertyChangedEventArgs> EditStateChanged;

        #region Properties

        private MapData replica;

        private List<ViewModelBase> EdigingList = new();

        [ObservableProperty]
        private string appName;

        [ObservableProperty]
        private string modelVersion;

        [ObservableProperty]
        private ObservableCollection<Plugin> plugins = new();

        [ObservableProperty]
        private ObservableCollection<Part> parts = new();

        [JsonIgnore]
        public IEnumerable<Articulation> Articulations => Parts.SelectMany(p => p.Articulations);

        [JsonIgnore]
        public bool EditState
        {
            get => _EditState;
            private set => SetProperty(ref _EditState, value);
        }
        private bool _EditState;

        #endregion

        #region ctor

        public MapData()
        {
            AppName = typeof(App).Assembly.GetName().Name;
            ModelVersion = "1.0.0";
        }

        #endregion

        #region Methods

        public void SetEditState(ViewModelBase sender, bool isEditing)
        {
            bool stateChanged = false;

            if (isEditing)
            {
                if (!EdigingList.Contains(sender))
                {
                    EdigingList.Add(sender);
                    stateChanged = true;
                }
            }
            else
            {
                if (EdigingList.Contains(sender))
                {
                    EdigingList.Remove(sender);
                    stateChanged = true;
                }
            }

            if (stateChanged)
            {
                EditState = EdigingList.Count > 0;
                EditStateChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EditState)));
            }
        }

        public void Initialize()
        {
            replica = new MapData();

            Plugins.Clear();
            Parts.Clear();

            Loaded?.Invoke(this, new DrumMapIOEventArgs(true));
        }

        public async Task<bool> LoadAsync(string path)
        {
            try
            {
                Plugins.Clear();
                Parts.Clear();

                string jsonStrng = await File.ReadAllTextAsync(path);
                var data = JsonSerializer.Deserialize<MapData>(jsonStrng);

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

                    Plugins.Add(newPlugin);
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

                    Parts.Add(newPart);
                }

                replica = data;

                Loaded?.Invoke(this, new DrumMapIOEventArgs(true));

                return true;
            }
            catch (Exception ex)
            {
                Loaded?.Invoke(this, new DrumMapIOEventArgs(false, ex));
                return false;
            }
        }

        public async Task<bool> SaveAsync(string path)
        {
            try
            {
                FixID(this);
                replica = Clone();

                var options = new JsonSerializerOptions
                {
                    IgnoreReadOnlyFields = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = true
                };
                var jsonString = JsonSerializer.Serialize(replica, options);
                await File.WriteAllTextAsync(path, jsonString);

                Saved?.Invoke(this, new DrumMapIOEventArgs(true));

                return true;
            }
            catch (Exception ex)
            {
                Saved?.Invoke(this, new DrumMapIOEventArgs(false, ex));
                return false;
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

        public void FixID(MapData map)
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

            int drumMapOrder = 1;
            foreach (var articulation in map.Parts.SelectMany(x => x.Articulations).OrderBy(a => a.DrumMapOrder).ToArray())
            {
                articulation.DrumMapOrder = drumMapOrder++;
            }

            foreach (var kitPitch in map.Plugins.SelectMany(x => x.Kits).SelectMany(x => x.Pitches).ToArray())
            {
                if (kitPitch.ArticulationID != 0)
                {
                    kitPitch.ArticulationID = artics[kitPitch.ArticulationID];
                }
            }

            foreach (var articulation in map.Parts.SelectMany(x => x.Articulations).ToArray())
            {
                if (articulation.ID != 0)
                {
                    articulation.ID = artics[articulation.ID];
                }
                if (articulation.Complement != 0)
                {
                    articulation.Complement = artics[articulation.Complement];
                }
            }
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

        private bool CompareMapData(MapData src, MapData dst)
        {
            if (src.Plugins.Count != dst.Plugins.Count) return true;
            if (src.Parts.Count != dst.Parts.Count) return true;

            for (int i = 0; i < src.Plugins.Count; i++)
            {
                if (ComparePlugin(src.Plugins[i], dst.Plugins.OrderBy(x => x.DisplayOrder).ToArray()[i])) return true;
            }

            for (int i = 0; i < src.Parts.Count; i++)
            {
                if (ComparePart(src.Parts[i], dst.Parts.OrderBy(x => x.DisplayOrder).ToArray()[i])) return true;
            }

            return false;
        }

        private bool ComparePlugin(Plugin src, Plugin dst)
        {
            if (src.Kits.Count != dst.Kits.Count) return true;
            if (src.ID != dst.ID) return true;
            if (src.Name != dst.Name) return true;
            if (src.PluginType != dst.PluginType) return true;
            if (src.MidiOutDevice != dst.MidiOutDevice) return true;
            if (src.SoundCheckChannel != dst.SoundCheckChannel) return true;

            for (int i = 0; i < src.Kits.Count; i++)
            {
                if (CompareKit(src.Kits[i], dst.Kits.OrderBy(x => x.DisplayOrder).ToArray()[i])) return true;
            }
            return false;
        }

        private bool CompareKit(Kit src, Kit dst)
        {
            if (src.ID != dst.ID) return true;
            if (src.Name != dst.Name) return true;
            if (src.Pitches.Count != dst.Pitches.Count) return true;
            if (src.BankSelectMSB != dst.BankSelectMSB) return true;
            if (src.BankSelectLSB != dst.BankSelectLSB) return true;
            if (src.ProgramNumber != dst.ProgramNumber) return true;

            for (int i = 0; i < 128; i++)
            {
                if (CompareKitPitch(src.Pitches[i], dst.Pitches.OrderBy(x => x.DisplayOrder).ToArray()[i])) return true;
            }
            return false;
        }

        private bool CompareKitPitch(KitPitch src, KitPitch dst)
        {
            if (src.Pitch != dst.Pitch) return true;
            if (src.Name != dst.Name) return true;
            if (src.ArticulationID != dst.ArticulationID) return true;
            if (src.Separator != dst.Separator) return true;
            return false;
        }

        private bool ComparePart(Part src, Part dst)
        {
            if (src.Name != dst.Name) return true;
            if (src.ScorePitch != dst.ScorePitch) return true;
            if (src.Color != dst.Color) return true;
            if (src.NoteHead != dst.NoteHead) return true;
            if (src.Technique != dst.Technique) return true;
            if (src.InstrumentEntityID != dst.InstrumentEntityID) return true;
            if (src.NoteHeadSet != dst.NoteHeadSet) return true;
            if (src.Voice != dst.Voice) return true;
            if (src.TechniqueEntityID != dst.TechniqueEntityID) return true;
            if (src.Articulations.Count != dst.Articulations.Count) return true;

            for (int i = 0; i < src.Articulations.Count; i++)
            {
                if (CompareArticulation(src.Articulations[i], dst.Articulations.OrderBy(x => x.DisplayOrder).ToArray()[i])) return true;
                if (src.Articulations.OrderBy(x => x.DrumMapOrder).Skip(i).First().ID != dst.Articulations.OrderBy(x => x.DrumMapOrder).Skip(i).First().ID) return true;
            }

            return false;
        }

        private bool CompareArticulation(Articulation src, Articulation dst)
        {
            if (src.ID != dst.ID) return true;
            if (src.Name != dst.Name) return true;
            if (src.Complement != dst.Complement) return true;
            return false;
        }

        public bool HasError()
        {
            return
                Plugins.Any(x => x.HasErrors) ||
                Plugins.SelectMany(x => x.Kits).Any(x => x.HasErrors) ||
                Plugins.SelectMany(x => x.Kits).SelectMany(x => x.Pitches).Any(x => x.HasErrors) ||
                Parts.Any(x => x.HasErrors) ||
                Parts.SelectMany(x => x.Articulations).Any(x => x.HasErrors);
        }

        #endregion
    }
}

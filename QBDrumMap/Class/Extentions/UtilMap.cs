using System.Text;
using libMidi.Messages;
using QBDrumMap.Class.Cubase;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;
using QBDrumMap.Class.StudioOne;

namespace QBDrumMap.Class.Extentions
{
    public static class UtilMap
    {
        private static ISettingService Setting => App.GetService<ISettingService>();

        public static MapData MapData => App.GetService<MapData>();

        public static Dictionary<int, string> NoteNames => Enumerable.Range(-1, 129).ToDictionary(x => x, x => $"{(x < 0 ? string.Empty : x.ToString() + " - " + Pitch.NoteName(x))}");

        public static List<int> ControlValues => Enumerable.Range(0, 128).ToList();

        public static Dictionary<int, string> ProgramNumbers => Enumerable.Range(0, 128).ToDictionary(x => x, x => $"PG {x + 1}");

        public static IEnumerable<KitPitch> GetKitPitechs(this MapData data, string kitName)
        {
            if (data.Plugins.SelectMany(x => x.Kits).FirstOrDefault(x => x.Name == kitName) is not Kit kit)
            {
                throw new ArgumentException();
            }

            if (kit.Pitches.Any(x => x.ArticulationID != 0) && Setting.UseArticulation == true)
            {
                return data.GetKitPitechsHasArticulation(kit);
            }
            else
            {
                return data.GetKitPitechesNoArticulation(kit);
            }
        }

        public static StringBuilder GetText(this MapData data, string kitName)
        {
            StringBuilder sb = new();

            if (data.Plugins.SelectMany(p => p.Kits).FirstOrDefault(k => k.Name == kitName) is Kit kit)
            {
                sb.AppendLine("Pitch\tNote\tName\tArticulation\tSeparator");
                foreach (var kitPitch in kit.Pitches.OrderBy(x => x.DisplayOrder))
                {
                    var articulationName = MapData.Parts.SelectMany(a => a.Articulations).FirstOrDefault(a => a.ID == kitPitch.ArticulationID)?.Name ?? string.Empty;
                    var separatorName = kitPitch.Separator == Separator.None ? string.Empty : kitPitch.Separator.ToString();
                    sb.AppendLine($"{kitPitch.Pitch}\t{kitPitch.Note}\t{kitPitch.Name}\t{articulationName}\t{separatorName}");
                }
            }

            return sb;
        }

        public static PitchList GetStudioOnePitchList(this MapData data, string kitName)
        {
            PitchList result = new()
            {
                Title = kitName
            };

            foreach (var item in data.GetKitPitechs(kitName))
            {
                Articulation artic = data.Parts.SelectMany(x => x.Articulations).FirstOrDefault(a => a.ID == item.ArticulationID);
                Part part = data.Parts.FirstOrDefault(x => x.Articulations.Any(a => a.ID == item.ArticulationID));

                string name = item.Name;
                if (Setting.UseArticulationPitchName && !string.IsNullOrWhiteSpace(artic?.Name))
                {
                    name = $"{artic?.Name}";
                }

                if (item.ArticulationID != 0)
                {
                    name = GetPitchName(item, name);
                }

                string color = part?.Color ?? "00000000";
                if (color == "00000000")
                {
                    color = null;
                }

                PitchName pitchName =
                    Setting.UseArticulation
                        ? new()
                        {
                            Pitch = (byte)item.Pitch,
                            Name = name,
                            ScorePitch = (part?.ScorePitch ?? 0) == 0 ? null : $"{part?.ScorePitch}",
                            Color = color,
                            Notehead = (part?.NoteHead ?? StudioOneNoteHead.None) == StudioOneNoteHead.None ? null : Enum.GetName(part?.NoteHead ?? StudioOneNoteHead.None),
                            Technique = (part?.Technique ?? StudioOneTechnique.None) == StudioOneTechnique.None ? null : Enum.GetName(part?.Technique ?? StudioOneTechnique.None)
                        }
                        : new()
                        {
                            Pitch = (byte)item.Pitch,
                            Name = name,
                        };

                result.Items.Add(pitchName);
            }

            return result;
        }

        public static CubaseDrumMap GetCubaseDrumMap(this MapData data, string drumKitName, string baseOn = null)
        {
            var setting = App.GetService<ISettingService>();

            var isBaseOn = baseOn != null;
            var result = new CubaseDrumMap();
            var items = data.GetKitPitechs(baseOn ?? drumKitName).ToList();
            var articMap = ArticulationMap.GetArticulationMap(data, drumKitName);

            result.Initialize();

            result.Name = drumKitName;

            Dictionary<int, int> odr = [];

            Enumerable.Range(0, 128).ToList().ForEach(pitch =>
            {
                odr.Add(pitch, 999);
                if (items.Where(x => x.Pitch == pitch).FirstOrDefault() is KitPitch item)
                {
                    Articulation artic = data.Parts.SelectMany(x => x.Articulations).FirstOrDefault(a => a.ID == item.ArticulationID);
                    Part part = data.Parts.FirstOrDefault(x => x.Articulations.Any(a => a.ID == item.ArticulationID));

                    string name = item.Name;
                    if (setting.UseArticulationPitchName && !string.IsNullOrWhiteSpace(artic?.Name))
                    {
                        name = $"{artic?.Name}";
                    }

                    int onote = !isBaseOn ? item.Pitch : articMap.Items.FirstOrDefault(m => m.ID == item.ArticulationID)?.Pitches.FirstOrDefault() ?? -1;
                    if (item.Pitch != 0 && onote == -1) return;

                    if (item.ArticulationID != 0)
                    {
                        name = GetPitchName(item, name);
                    }

                    result.Map.Items[pitch].Name = name;
                    result.Map.Items[pitch].ONote = onote;
                    if (Setting.UseArticulation)
                    {
                        result.Map.Items[pitch].DisplayNote = int.Parse($"0{part?.ScorePitch}");
                        result.Map.Items[pitch].NoteheadSet = int.Parse($"0{part?.NoteHeadSet}");
                        result.Map.Items[pitch].Voice = (int)(part?.Voice ?? CubaseVoiceType.Up1);
                        result.Map.Items[pitch].InstrumentEntityID = part?.InstrumentEntityID;
                        result.Map.Items[pitch].TechniqueEntityID = part?.TechniqueEntityID;
                    }
                }
            });

            int i = 1;
            items.ForEach(item =>
            {
                if (!string.IsNullOrWhiteSpace(result.Map.Items[item.Pitch].Name))
                {
                    odr[item.Pitch] = i;
                }
                i++;
            });

            result.Order.Clear();

            result.Order.AddRange(
                odr
                .OrderBy(x => x.Value)
                .ThenBy(x => x.Key)
                .Select(x => x.Key)
                .ToArray()
            );

            return result;
        }

        private static IEnumerable<KitPitch> GetKitPitechsHasArticulation(this MapData data, Kit kit)
        {
            List<KitPitch> result = [];

            var allPitches =
                kit.Pitches.Where(x => x.Pitch >= 0 && !string.IsNullOrWhiteSpace(x.Name))
                .Select(x => new { p = x, a = data.Parts.SelectMany(x => x.Articulations).FirstOrDefault(a => a.ID == x.ArticulationID) ?? new Articulation() })
                .ToArray();

            var hasArticNoSeparate =
                allPitches.Where(x => x.p.ArticulationID != 0 && x.p.Separator == Separator.None)
                .OrderBy(x => x.a.DrumMapOrder)
                .ToArray();

            var noArticNoSeparate =
                allPitches.Where(x => x.p.ArticulationID == 0 && x.p.Separator == Separator.None)
                .OrderBy(x => x.p.DisplayOrder)
                .ThenBy(x => x.p.Pitch)
                .ToArray();

            var hasArticAndSeparate =
                allPitches.Where(x => x.p.ArticulationID != 0 && x.p.Separator != Separator.None)
                .OrderBy(x => x.p.Separator)
                .ThenBy(x => x.a.DrumMapOrder)
                .ToArray();

            var noArticAndSeparate =
                allPitches.Where(x => x.p.ArticulationID == 0 && x.p.Separator != Separator.None)
                .OrderBy(x => x.p.Separator)
                .ThenBy(x => x.p.DisplayOrder)
                .ThenBy(x => x.p.Pitch)
                .ToArray();

            if (noArticAndSeparate.Any())
            {
                hasArticNoSeparate.Union(noArticNoSeparate).ToList().ForEach(x => result.Add(x.p));
            }
            else
            {
                hasArticNoSeparate.ToList().ForEach(x => result.Add(x.p));
            }

            if ((hasArticNoSeparate.Any() || noArticAndSeparate.Any()) && (hasArticAndSeparate.Any() || noArticNoSeparate.Any()))
            {
                var blank = Enumerable.Range(0, 128).Except(allPitches.Select(x => x.p.Pitch).ToArray());
                if (blank.Any())
                {
                    result.Add(new KitPitch() { Pitch = blank.First(), Name = "==========" });
                }
            }

            if (noArticAndSeparate.Any() == false)
            {
                noArticNoSeparate.ToList().ForEach(x => result.Add(x.p));
            }

            hasArticAndSeparate.Union(noArticAndSeparate).ToList().ForEach(x => result.Add(x.p));

            return result;
        }

        private static IEnumerable<KitPitch> GetKitPitechesNoArticulation(this MapData data, Kit kit)
        {
            return
                kit.Pitches
                .Where(p => !string.IsNullOrWhiteSpace(p.Name))
                .OrderBy(p => p.DisplayOrder)
                .ToArray();
        }

        private static string GetPitchName(KitPitch item, string name)
        {
            name = name +
                item.Separator switch
                {
                    Separator.None => string.Empty,
                    Separator.Separate => " (-)",
                    Separator.Unload => " (+)",
                    Separator.Duplicate => " (*)",
                    _ => throw new ArgumentException(),
                };
            return name;
        }
    }
}

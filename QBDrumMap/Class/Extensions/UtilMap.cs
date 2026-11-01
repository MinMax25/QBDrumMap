using System.Text;
using libMidi.Messages;
using QBDrumMap.Class.Cubase;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;
using QBDrumMap.Class.StudioOne;

namespace QBDrumMap.Class.Extensions;

public static class UtilMap
{
    private static ISettingService Setting => App.GetService<ISettingService>();

    public static MapData MapData => App.GetService<MapData>() ?? new MapData();

    public static Dictionary<int, string> NoteNames => Enumerable.Range(-1, 129).ToDictionary(x => x, x =>
    {
        return $"{(x < 0 ? string.Empty : x.ToString() + " - " + Pitch.NoteName(x))}";
    });

    public static List<int> ControlValues => Enumerable.Range(0, 128).ToList();

    public static Dictionary<int, string> ProgramNumbers => Enumerable.Range(0, 128).ToDictionary(x => x, x =>
    {
        return $"PG {x + 1}";
    });

    public static IEnumerable<KitPitch> GetKitPitches(this MapData data, string kitName)
    {
        if (data.Plugins.SelectMany(x => x.Kits).FirstOrDefault(x => x.Name == kitName) is not Kit kit)
        {
            throw new ArgumentException();
        }

        if (kit.Pitches.Any(x => data.IsValidArticulationID(x.ArticulationID)) && Setting?.UseArticulation == true)
        {
            return GetKitPitchesHasArticulation(data, kit);
        }
        else
        {
            return GetKitPitchesNoArticulation(data, kit);
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

        foreach (var item in data.GetKitPitches(kitName))
        {
            Articulation artic = data.Parts.SelectMany(x => x.Articulations).FirstOrDefault(a => a.ID == item.ArticulationID);
            Part part = data.Parts.FirstOrDefault(x => x.Articulations.Any(a => a.ID == item.ArticulationID));

            string name = item.Name;
            if (Setting?.UseArticulationPitchName == true && !string.IsNullOrWhiteSpace(artic?.Name))
            {
                name = $"{artic?.Name}";
            }

            if (data.IsValidArticulationID(item.ArticulationID))
            {
                name = GetPitchName(item, name);
            }

            string color = part?.Color ?? "00000000";
            if (color == "00000000")
            {
                color = null;
            }

            PitchName pitchName = Setting.UseArticulation
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

    public static string GetReaperLuaScript(this MapData data, string kitName)
    {
        var items = data.GetKitPitches(kitName).ToList();

        var distinctPitches = new List<int>();
        var pitchToName = new Dictionary<int, string>();

        foreach (var item in items)
        {
            if (item.Pitch < 0 || item.Pitch > 127)
            {
                continue;
            }

            Articulation artic = data.Parts.SelectMany(x => x.Articulations).FirstOrDefault(a => a.ID == item.ArticulationID);
            string name = item.Name;
            if (Setting?.UseArticulationPitchName == true && !string.IsNullOrWhiteSpace(artic?.Name))
            {
                name = $"{artic?.Name}";
            }

            if (data.IsValidArticulationID(item.ArticulationID))
            {
                name = GetPitchName(item, name);
            }

            if (pitchToName.TryGetValue(item.Pitch, out var existingName))
            {
                if (existingName != name && !string.IsNullOrWhiteSpace(name))
                {
                    pitchToName[item.Pitch] = $"{existingName} / {name}";
                }
            }
            else
            {
                distinctPitches.Add(item.Pitch);
                pitchToName[item.Pitch] = name;
            }
        }

        // QBDrumMap UI order is top-to-bottom.
        // REAPER CUSTOM_NOTE_ORDER lists pitches from bottom to top.
        // Therefore, reverse distinctPitches to obtain the correct REAPER order.
        var customOrder = distinctPitches.AsEnumerable().Reverse().ToList();

        var sb = new StringBuilder();
        sb.AppendLine("-- ==============================================================================");
        sb.AppendLine("-- ReaScript: Apply QBDrumMap Drum Map");
        sb.AppendLine($"-- Kit: {kitName}");
        sb.AppendLine("-- Generated by QBDrumMap");
        sb.AppendLine("-- ==============================================================================");
        sb.AppendLine();
        sb.AppendLine($"local MAP_NAME = \"{EscapeLuaString(kitName)}\"");
        sb.AppendLine();
        sb.AppendLine("local NOTES = {");
        foreach (var pitch in distinctPitches)
        {
            sb.AppendLine($"    {{ pitch = {pitch}, name = \"{EscapeLuaString(pitchToName[pitch])}\" }},");
        }
        sb.AppendLine("}");
        sb.AppendLine();
        sb.AppendLine($"local CUSTOM_ORDER = {{ {string.Join(", ", customOrder)} }}");
        sb.AppendLine();
        sb.AppendLine(@"-- Safely update or insert CUSTOM_NOTE_ORDER in the track state chunk.
-- Note: CUSTOM_NOTE_ORDER is a REAPER internal track state parameter that specifies
-- the vertical display order of note rows in the MIDI editor (from bottom to top).
local function UpdateTrackChunkNoteOrder(track, order_pitches)
    if not order_pitches or #order_pitches == 0 then
        return true, nil
    end

    local ok, chunk = reaper.GetTrackStateChunk(track, """", false)
    if not ok or not chunk or chunk == """" then
        return false, ""Failed to read track state chunk.""
    end

    local order_str = ""CUSTOM_NOTE_ORDER "" .. table.concat(order_pitches, "" "")

    -- Split chunk into lines
    local lines = {}
    for line in (chunk .. ""\n""):gmatch(""([^\n]*)\n"") do
        if #line > 0 and line:byte(-1) == 13 then
            line = line:sub(1, -2)
        end
        table.insert(lines, line)
    end
    if #lines > 0 and lines[#lines] == """" then
        table.remove(lines)
    end

    local depth = 0
    local custom_order_indices = {}
    local first_child_idx = nil
    local track_close_idx = nil

    for idx, line in ipairs(lines) do
        local trimmed = line:match(""^%s*(.-)%s*$"") or """"
        if trimmed:sub(1, 1) == ""<"" then
            depth = depth + 1
            if depth == 2 and not first_child_idx then
                first_child_idx = idx
            end
        elseif trimmed:sub(1, 1) == "">"" then
            if depth == 1 then
                track_close_idx = idx
            end
            depth = depth - 1
        elseif depth == 1 then
            if trimmed:match(""^CUSTOM_NOTE_ORDER[%s$]"") or trimmed == ""CUSTOM_NOTE_ORDER"" then
                table.insert(custom_order_indices, idx)
            end
        end
    end

    if #custom_order_indices > 1 then
        return false, ""Aborted: Multiple CUSTOM_NOTE_ORDER entries found at top level of track chunk.""
    end

    if #custom_order_indices == 1 then
        local target_idx = custom_order_indices[1]
        local indent = lines[target_idx]:match(""^(%s*)"") or ""  ""
        lines[target_idx] = indent .. order_str
    else
        local insert_pos = first_child_idx or track_close_idx
        if not insert_pos then
            return false, ""Aborted: Could not determine valid insertion point in track chunk.""
        end
        table.insert(lines, insert_pos, ""  "" .. order_str)
    end

    local new_chunk = table.concat(lines, ""\n"") .. ""\n""

    local ok_set = reaper.SetTrackStateChunk(track, new_chunk, false)
    if not ok_set then
        return false, ""reaper.SetTrackStateChunk returned false.""
    end

    -- Verify the update was accepted by REAPER
    local ok_verify, verify_chunk = reaper.GetTrackStateChunk(track, """", false)
    if not ok_verify or not verify_chunk:find(order_str, 1, true) then
        reaper.SetTrackStateChunk(track, chunk, false)
        return false, ""Track state chunk verification failed. Rolled back.""
    end

    return true, nil
end

local function Main()
    if #NOTES == 0 then
        reaper.MB(""This drum map contains no valid notes to apply."", ""QBDrumMap - "" .. MAP_NAME, 0)
        return
    end

    -- Validate NOTES array (range 0-127, valid pitch numbers)
    for idx, item in ipairs(NOTES) do
        if type(item.pitch) ~= ""number"" or item.pitch < 0 or item.pitch > 127 then
            reaper.MB(string.format(""Invalid pitch (%s) at index %d."", tostring(item.pitch), idx), ""QBDrumMap Error"", 0)
            return
        end
    end

    local track_count = reaper.CountSelectedTracks(0)
    if track_count == 0 then
        reaper.MB(""No track selected.\nPlease select exactly one track in REAPER before running this script."", ""QBDrumMap - "" .. MAP_NAME, 0)
        return
    end
    if track_count > 1 then
        reaper.MB(string.format(""Multiple tracks selected (%d).\nPlease select only one track."", track_count), ""QBDrumMap - "" .. MAP_NAME, 0)
        return
    end

    local track = reaper.GetSelectedTrack(0, 0)
    if not track then
        reaper.MB(""Could not access the selected track."", ""QBDrumMap - "" .. MAP_NAME, 0)
        return
    end

    local _, track_name = reaper.GetTrackName(track)
    if not track_name or track_name == """" then
        track_name = ""Untitled Track""
    end

    local confirm_msg = string.format(
        ""Apply drum map '%s' to track '%s'?\n\n"" ..
        ""- Mapped notes: %d\n"" ..
        ""- Display order will be applied to the MIDI editor.\n"" ..
        ""- Existing common note names on this track will be replaced.\n\n"" ..
        ""Continue?"",
        MAP_NAME, track_name, #NOTES
    )

    local res = reaper.MB(confirm_msg, ""QBDrumMap - "" .. MAP_NAME, 1)
    if res ~= 1 then
        return
    end

    -- Capture original state for safe rollback in case of unexpected failure
    local original_chunk_ok, original_chunk = reaper.GetTrackStateChunk(track, """", false)
    local old_names = {}
    for p = 0, 127 do
        local n = reaper.GetTrackMIDINoteNameEx(0, track, p, -1)
        if n and n ~= """" then
            old_names[p] = n
        end
    end

    local function RollbackState(err_msg)
        if original_chunk_ok and original_chunk and original_chunk ~= """" then
            reaper.SetTrackStateChunk(track, original_chunk, false)
        end
        for p = 0, 127 do
            local prev = old_names[p] or """"
            reaper.SetTrackMIDINoteNameEx(0, track, p, -1, prev)
        end
        reaper.PreventUIRefresh(-1)
        reaper.UpdateArrange()
        reaper.Undo_EndBlock(""Apply QBDrumMap: "" .. MAP_NAME, -1)
        reaper.MB(""Failed to apply drum map:\n"" .. tostring(err_msg) .. ""\n\nChanges have been rolled back."", ""QBDrumMap Error"", 0)
    end

    reaper.Undo_BeginBlock()
    reaper.PreventUIRefresh(1)

    local success, err = pcall(function()
        -- 1. Apply note display order to track chunk
        local ok_order, err_order = UpdateTrackChunkNoteOrder(track, CUSTOM_ORDER)
        if not ok_order then
            error(err_order or ""Failed to update track chunk."")
        end

        -- 2. Clear old note names for pitches 0-127 (all-channel scope chan = -1)
        for p = 0, 127 do
            reaper.SetTrackMIDINoteNameEx(0, track, p, -1, """")
        end

        -- 3. Set new note names
        for _, item in ipairs(NOTES) do
            local ok_set = reaper.SetTrackMIDINoteNameEx(0, track, item.pitch, -1, item.name)
            if not ok_set then
                error(""SetTrackMIDINoteNameEx failed for pitch "" .. tostring(item.pitch))
            end
        end
    end)

    if not success then
        RollbackState(err)
        return
    end

    reaper.PreventUIRefresh(-1)
    reaper.UpdateArrange()
    reaper.Undo_EndBlock(""Apply QBDrumMap: "" .. MAP_NAME, -1)

    reaper.MB(
        string.format(
            ""Drum map '%s' applied successfully to track '%s'.\n\n"" ..
            ""Note: If the MIDI editor is currently open, please close and reopen it to refresh the note row display."",
            MAP_NAME, track_name
        ),
        ""QBDrumMap"",
        0
    )
end

Main()
");

        return sb.ToString();
    }

    private static string EscapeLuaString(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        foreach (char c in s)
        {
            switch (c)
            {
                case '\\': sb.Append(@"\\"); break;
                case '"': sb.Append(@"\"""); break;
                case '\r': sb.Append(@"\r"); break;
                case '\n': sb.Append(@"\n"); break;
                case '\t': sb.Append(@"\t"); break;
                default:
                    if (c < 32)
                    {
                        sb.Append($"\\{(int)c:D3}");
                    }
                    else
                    {
                        sb.Append(c);
                    }
                    break;
            }
        }
        return sb.ToString();
    }

    public static CubaseDrumMap GetCubaseDrumMap(this MapData data, string drumKitName, string baseOn = null)
    {
        var setting = App.GetService<ISettingService>();
        var isBaseOn = baseOn != null;
        var result = new CubaseDrumMap();
        var items = data.GetKitPitches(baseOn ?? drumKitName).ToList();
        var articMap = ArticulationMap.GetArticulationMap(data, drumKitName);

        result.Initialize();
        result.Name = drumKitName;

        Dictionary<int, int> odr = new();

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
                if (item.Pitch != 0 && onote == -1)
                {
                    return;
                }

                if (data.IsValidArticulationID(item.ArticulationID))
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
            odr.OrderBy(x => x.Value)
               .ThenBy(x => x.Key)
               .Select(x => x.Key)
               .ToArray()
        );

        return result;
    }

    private static IEnumerable<KitPitch> GetKitPitchesHasArticulation(this MapData data, Kit kit)
    {
        List<KitPitch> result = new();

        var validIds = data.Articulations.Select(a => a.ID).ToHashSet();
        bool HasArticulation(KitPitch p) => p.ArticulationID != 0 && validIds.Contains(p.ArticulationID);

        var allPitches = kit.Pitches.Where(x => x.Pitch >= 0 && !string.IsNullOrWhiteSpace(x.Name))
            .Select(x => new
            {
                p = x,
                a = data.Parts.SelectMany(p => p.Articulations).FirstOrDefault(a => a.ID == x.ArticulationID) ?? new Articulation()
            })
            .ToArray();

        var hasArticNoSeparate = allPitches.Where(x => HasArticulation(x.p) && x.p.Separator == Separator.None)
            .OrderBy(x => x.a.DrumMapOrder)
            .ToArray();

        var noArticNoSeparate = allPitches.Where(x => !HasArticulation(x.p) && x.p.Separator == Separator.None)
            .OrderBy(x => x.p.DisplayOrder)
            .ThenBy(x => x.p.Pitch)
            .ToArray();

        var hasArticAndSeparate = allPitches.Where(x => HasArticulation(x.p) && x.p.Separator != Separator.None)
            .OrderBy(x => x.p.Separator)
            .ThenBy(x => x.a.DrumMapOrder)
            .ToArray();

        var noArticAndSeparate = allPitches.Where(x => !HasArticulation(x.p) && x.p.Separator != Separator.None)
            .OrderBy(x => x.p.Separator)
            .ThenBy(x => x.p.DisplayOrder)
            .ThenBy(x => x.p.Pitch)
            .ToArray();

        if (noArticAndSeparate.Any())
        {
            hasArticNoSeparate.Union(noArticNoSeparate).ToList().ForEach(x =>
            {
                result.Add(x.p);
            });
        }
        else
        {
            hasArticNoSeparate.ToList().ForEach(x =>
            {
                result.Add(x.p);
            });
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
            noArticNoSeparate.ToList().ForEach(x =>
            {
                result.Add(x.p);
            });
        }

        hasArticAndSeparate.Union(noArticAndSeparate).ToList().ForEach(x =>
        {
            result.Add(x.p);
        });

        return result;
    }

    private static IEnumerable<KitPitch> GetKitPitchesNoArticulation(this MapData data, Kit kit)
    {
        return kit.Pitches
            .Where(p => !string.IsNullOrWhiteSpace(p.Name))
            .OrderBy(p => p.DisplayOrder)
            .ToArray();
    }

    private static string GetPitchName(KitPitch item, string name)
    {
        name = name + item.Separator switch
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
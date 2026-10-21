namespace QBDrumMap.Class.MapModels;

public record ArticulationMap
{
    public string Name { get; set; } = string.Empty;

    public List<ArticulationMapItem> Items { get; } = new();

    public const int QBDrummerMaxArticulationNumber = 0x0fff;

    public static Dictionary<int, int> GetQBDrummerNumbers(MapData data)
    {
        var numbers = new Dictionary<int, int>();
        int no = 1;

        foreach (var articulation in data.Parts
            .OrderBy(p => p.DisplayOrder)
            .SelectMany(p => p.Articulations.OrderBy(a => a.DisplayOrder)))
        {
            if (no > QBDrummerMaxArticulationNumber)
            {
                throw new InvalidOperationException(string.Format(
                    App.Text("Error_QBDrummerTooManyArticulations", "QB Drummer CSV supports up to {0} articulations."),
                    QBDrummerMaxArticulationNumber));
            }

            if (numbers.TryAdd(articulation.ID, no))
            {
                no++;
            }
        }

        return numbers;
    }

    public static string ToQBDrummerCsv(MapData data, string kitName)
    {
        var articMap = GetArticulationMap(data, kitName);
        var numbers = GetQBDrummerNumbers(data);

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("ID,Sub,Name,Pitchs");

        foreach (var item in articMap.Items
            .Where(m => numbers.ContainsKey(m.ID))
            .OrderBy(m => numbers[m.ID]))
        {
            int subValue = item.IsSub ? 1 : 0;
            string pitches = string.Join("|", item.Pitches);
            sb.AppendLine($"{numbers[item.ID]},{subValue},{item.Name},{pitches}");
        }

        return sb.ToString();
    }

    public static ArticulationMap GetArticulationMap(MapData data, string kitName)
    {
        if (data.Plugins.SelectMany(x => x.Kits).FirstOrDefault(x => x.Name == kitName) is not Kit kit)
        {
            throw new InvalidOperationException($"Kit '{kitName}' not found.");
        }

        ArticulationMap articMap = new()
        {
            Name = kitName
        };

        foreach (var part in data.Parts)
        {
            foreach (var articulation in part.Articulations)
            {
                articMap.Items.Add(new ArticulationMapItem()
                {
                    ID = articulation.ID,
                    Name = articulation.Name,
                    Order = articulation.DisplayOrder,
                    PartID = part.ID,
                    Complement = articulation.Complement,
                });
            }
        }

        foreach (var pitch in kit.Pitches.OrderBy(x => x.Separator))
        {
            if (articMap.Items.FirstOrDefault(x => x.ID == pitch.ArticulationID) is ArticulationMapItem item)
            {
                item.Pitches.Add(pitch.Pitch);
            }
        }

        foreach (var item in articMap.Items)
        {
            item.IsSub = !item.Pitches.Any();
        }

        foreach (var g in articMap.Items.GroupBy(x => x.PartID))
        {
            int pitch = -1;
            foreach (var item in g.OrderBy(x => x.Order).ToArray())
            {
                if (item.Pitches.Any())
                {
                    pitch = item.Pitches.First();
                }
                else if (pitch > -1)
                {
                    item.Pitches.Add(pitch);
                }
            }
        }

        foreach (var g in articMap.Items.GroupBy(x => x.PartID))
        {
            int pitch = -1;
            foreach (var item in g.OrderByDescending(x => x.Order).ToArray())
            {
                if (item.Pitches.Any())
                {
                    pitch = item.Pitches.First();
                }
                else if (pitch > -1)
                {
                    item.Pitches.Add(pitch);
                }
            }
        }

        foreach (var item in articMap.Items.Where(x => !x.Pitches.Any() && x.Complement != 0))
        {
            if (articMap.Items.FirstOrDefault(a => a.ID == item.Complement) is not ArticulationMapItem mapItem)
            {
                continue;
            }

            if (mapItem.Pitches.Any())
            {
                item.Pitches.Add(mapItem.Pitches.First());
            }
        }

        return articMap;
    }
}
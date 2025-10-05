namespace QBDrumMap.Class.MapModels
{
    public record ArticulationMap
    {
        public string Name { get; set; } = string.Empty;

        public List<ArticulationMapItem> Items { get; } = [];

        public static ArticulationMap GetArticulationMap(MapData data, string kitName)
        {
            if (data.Plugins.SelectMany(x => x.Kits).FirstOrDefault(x => x.Name == kitName) is not Kit kit)
            {
                throw new Exception();
            }

            ArticulationMap articMap = new() { Name = kitName };

            foreach (var part in data.Parts)
            {
                foreach (var articulation in part.Articulations)
                {
                    articMap.Items.Add(
                        new ArticulationMapItem()
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

            // パート別、昇順に穴埋め
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

            // パート別、降順に穴埋め
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

            // 代替アーティキュレーションによる穴埋め
            foreach (var item in articMap.Items.Where(x => !x.Pitches.Any() && x.Complement != 0))
            {
                if (articMap.Items.FirstOrDefault(a => a.ID == item.Complement) is not ArticulationMapItem mapItem)
                {
                    throw new Exception();
                }

                if (mapItem.Pitches.Any())
                {
                    item.Pitches.Add(mapItem.Pitches.First());
                }
            }

            return articMap;
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;

namespace QBDrumMap.Class.MapModels;

public partial class PartNameAlias : ObservableObject
{
    [ObservableProperty]
    private string canonicalName = string.Empty;

    [ObservableProperty]
    private string aliases = string.Empty;

    public IEnumerable<string> GetAllNames()
    {
        var names = new List<string>();

        if (!string.IsNullOrWhiteSpace(CanonicalName))
        {
            names.Add(CanonicalName.Trim());
        }

        if (!string.IsNullOrWhiteSpace(Aliases))
        {
            names.AddRange(Aliases.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        }

        return names.Where(x => !string.IsNullOrWhiteSpace(x));
    }
        }

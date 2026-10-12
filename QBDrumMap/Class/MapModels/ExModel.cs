using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Data;

namespace QBDrumMap.Class.MapModels;

public static class ExModel
{
    private static readonly PropertyChangedEventArgs EmptyEventArgs = new PropertyChangedEventArgs(string.Empty);

    public static int NAME_MAX_LENGTH = 64;

    public static void AddItem<T>(this ObservableCollection<T> source, T value, Expression<Func<T, dynamic>> f)
        where T : IHasDisplayOrder
    {
        source.SuppressNotifications();

        f.SetValue(value, int.MaxValue);

        source.Add(value);

        int count = 1;

        foreach (var item in source.OrderBy(f.Compile()).ToArray())
        {
            f.SetValue(item, count++);
        }

        source.SortRefresh(f);

        source.ResumeNotifications();
    }

    public static void MoveTop<T>(this ObservableCollection<T> source, ObservableCollection<T> target, Expression<Func<T, dynamic>> f)
        where T : IHasDisplayOrder
    {
        Reorder(source, target, f, (items, selected) =>
            items.Where(selected.Contains).Concat(items.Where(x => !selected.Contains(x))).ToList());
    }

    public static void MoveUp<T>(this ObservableCollection<T> source, ObservableCollection<T> target, Expression<Func<T, dynamic>> f)
        where T : IHasDisplayOrder
    {
        // 選択行を相対順のまま1つのグループにまとめ、最初の選択行の位置から1段上へ置く
        Reorder(source, target, f, (items, selected) =>
        {
            var others = items.Where(x => !selected.Contains(x)).ToList();
            int index = Math.Max(0, items.TakeWhile(x => !selected.Contains(x)).Count() - 1);
            others.InsertRange(index, items.Where(selected.Contains));
            return others;
        });
    }

    public static void MoveDown<T>(this ObservableCollection<T> source, ObservableCollection<T> target, Expression<Func<T, dynamic>> f)
        where T : IHasDisplayOrder
    {
        // 選択行を相対順のまま1つのグループにまとめ、最後の選択行の位置から1段下へ置く
        Reorder(source, target, f, (items, selected) =>
        {
            var others = items.Where(x => !selected.Contains(x)).ToList();
            int lastSelected = items.FindLastIndex(selected.Contains);
            int index = Math.Min(others.Count, items.Take(lastSelected).Count(x => !selected.Contains(x)) + 1);
            others.InsertRange(index, items.Where(selected.Contains));
            return others;
        });
    }

    public static void MoveBottom<T>(this ObservableCollection<T> source, ObservableCollection<T> target, Expression<Func<T, dynamic>> f)
        where T : IHasDisplayOrder
    {
        Reorder(source, target, f, (items, selected) =>
            items.Where(x => !selected.Contains(x)).Concat(items.Where(selected.Contains)).ToList());
    }

    /// <summary>
    /// 現在の表示順に並べた一覧の上で選択行を動かし、結果を1始まりの連番で採番し直す。
    /// 表示順が同値の行は、コレクション内の並びを維持する。選択コレクションの格納順や欠番には依存しない。
    /// </summary>
    private static void Reorder<T>(
        ObservableCollection<T> source,
        ObservableCollection<T> target,
        Expression<Func<T, dynamic>> f,
        Func<List<T>, HashSet<T>, List<T>> arrange)
        where T : IHasDisplayOrder
    {
        var selected = target.Where(source.Contains).ToHashSet();
        if (selected.Count == 0)
        {
            return;
        }

        var key = f.Compile();
        var current = source
            .Select((item, index) => new { item, index })
            .OrderBy(x => key(x.item))
            .ThenBy(x => x.index)
            .Select(x => x.item)
            .ToList();

        var arranged = arrange(current.ToList(), selected);

        // すでに端などで並びが変わらない場合は何も変更しない
        if (arranged.SequenceEqual(current))
        {
            return;
        }

        int order = 1;
        foreach (var item in arranged)
        {
            if (f.GetValue(item) != order)
            {
                f.SetValue(item, order);
            }

            order++;
        }

        source.SortRefresh(f);
    }

    public static void SortRefresh<T>(this ObservableCollection<T> source, Expression<Func<T, dynamic>> f)
        where T : IHasDisplayOrder
    {
        string propertyName = f?.GetPropertyName() ?? nameof(IHasDisplayOrder.DisplayOrder);

        var sort = CollectionViewSource.GetDefaultView(source);

        sort.SortDescriptions.Clear();
        sort.SortDescriptions.Add(new SortDescription(propertyName, ListSortDirection.Ascending));
        sort.Refresh();
    }

    public static void SuppressNotifications<T>(this ObservableCollection<T> collection)
    {
        collection.CollectionChanged -= OnCollectionChanged;
    }

    public static void ResumeNotifications<T>(this ObservableCollection<T> collection)
    {
        collection.CollectionChanged += OnCollectionChanged;
    }

    private static string GetPropertyName<T>(this Expression<Func<T, dynamic>> f)
    {
        var unary = f.Body as UnaryExpression;
        if (unary?.Operand is not MemberExpression member)
        {
            if (f.Body is not MemberExpression memberBody)
            {
                throw new ArgumentException("Expression must be a member expression");
            }

            member = memberBody;
        }

        return member.Member.Name;
    }

    private static void SetValue<T>(this Expression<Func<T, dynamic>> f, T target, dynamic value)
    {
        string pn = f.GetPropertyName();
        typeof(T).GetProperty(pn)?.SetValue(target, value, null);
    }

    private static dynamic GetValue<T>(this Expression<Func<T, dynamic>> f, T target)
    {
        string pn = f.GetPropertyName();
        return typeof(T).GetProperty(pn)?.GetValue(target);
    }

    private static void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
    }
}
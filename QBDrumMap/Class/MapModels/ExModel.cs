using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Data;

namespace QBDrumMap.Class.MapModels
{
    public static class ExModel
    {
        #region Fields

        // 空のプロパティ変更イベント引数
        private static readonly PropertyChangedEventArgs EmptyEventArgs = new PropertyChangedEventArgs(string.Empty);

        // 名前の最大文字数
        public static int NAME_MAX_LENGTH = 64;

        #endregion

        #region Methods

        #region General

        public static int GetNewID(this IEnumerable<IHasID> target)
        {
            int min = target.Any() ? target.Min(x => x.ID) : 0;
            int max = target.Any() ? target.Max(x => x.ID) : 0;

            List<int> numbers = target.Select(x => x.ID).ToList();

            List<int> missing = Enumerable.Range(1, max - min + 1)
                .Except(numbers)
                .ToList();

            if (numbers.Any() && missing.Any())
            {
                return missing.First();
            }

            return max + 1;
        }

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
            if (!target.Any())
            {
                return;
            }

            source.SuppressNotifications();

            var tops = target.OrderBy(f.Compile()).ToArray();
            var bottoms = source.Where(x => !target.Contains(x)).OrderBy(f.Compile()).ToArray();

            int order = 1;

            foreach (var item in tops)
            {
                f.SetValue(item, order++);
            }

            foreach (var item in bottoms)
            {
                f.SetValue(item, order++);
            }

            source.SortRefresh(f);

            source.ResumeNotifications();
        }

        public static void MoveUp<T>(this ObservableCollection<T> source, ObservableCollection<T> target, Expression<Func<T, dynamic>> f)
            where T : IHasDisplayOrder
        {
            if (!target.Any())
            {
                return;
            }

            source.SuppressNotifications();

            int pos = f.GetValue(target[0]) - 1;

            if (pos <= 0)
            {
                source.ResumeNotifications();
                return;
            }

            var tops = source.Where(x => f.GetValue(x) < pos).OrderBy(f.Compile()).ToArray();
            var middles = target.OrderBy(f.Compile()).ToArray();
            var bottoms = source.Where(x => !target.Contains(x) && f.GetValue(x) >= pos).OrderBy(f.Compile()).ToArray();

            int order = 1;

            foreach (var item in tops)
            {
                f.SetValue(item, order++);
            }

            foreach (var item in middles)
            {
                f.SetValue(item, order++);
            }

            foreach (var item in bottoms)
            {
                f.SetValue(item, order++);
            }

            source.SortRefresh(f);

            source.ResumeNotifications();
        }

        public static void MoveDown<T>(this ObservableCollection<T> source, ObservableCollection<T> target, Expression<Func<T, dynamic>> f)
            where T : IHasDisplayOrder
        {
            if (!target.Any())
            {
                return;
            }

            int pos = f.GetValue(target.Last()) + 1;

            if (pos > source.Max(f.Compile()))
            {
                return;
            }

            source.SuppressNotifications();

            var tops = source.Where(x => !target.Contains(x) && f.GetValue(x) <= pos).OrderBy(f.Compile()).ToArray();
            var middles = target.OrderBy(f.Compile()).ToArray();
            var bottoms = source.Where(x => f.GetValue(x) > pos).OrderBy(f.Compile()).ToArray();

            int order = 1;

            foreach (var item in tops)
            {
                f.SetValue(item, order++);
            }

            foreach (var item in middles)
            {
                f.SetValue(item, order++);
            }

            foreach (var item in bottoms)
            {
                f.SetValue(item, order++);
            }

            source.SortRefresh(f);

            source.ResumeNotifications();
        }

        public static void MoveBottom<T>(this ObservableCollection<T> source, ObservableCollection<T> target, Expression<Func<T, dynamic>> f)
            where T : IHasDisplayOrder
        {
            if (!target.Any())
            {
                return;
            }

            source.SuppressNotifications();

            var tops = target.OrderBy(f.Compile()).ToArray();
            var bottoms = source.Where(x => !target.Contains(x)).OrderBy(f.Compile()).ToArray();

            int order = 1;

            foreach (var item in bottoms)
            {
                f.SetValue(item, order++);
            }

            foreach (var item in tops)
            {
                f.SetValue(item, order++);
            }

            source.SortRefresh(f);

            source.ResumeNotifications();
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

        #endregion

        #region Private

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
            // 通知の抑制用スタブ
        }

        #endregion

        #endregion
    }
}
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;

namespace QBDrumMap.Class.Behaviors
{
    public class DataGridGroupExpandBehavior
        : Behavior<DataGrid>
    {
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(
                nameof(IsExpanded),
                typeof(bool),
                typeof(DataGridGroupExpandBehavior),
                new PropertyMetadata(false, OnIsExpandedChanged));

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (DataGridGroupExpandBehavior)d;
            behavior.UpdateExpanders((bool)e.NewValue);
        }

        private void UpdateExpanders(bool isExpanded)
        {
            if (AssociatedObject == null) return;

            var groupItems = FindVisualChildren<GroupItem>(AssociatedObject);
            foreach (var groupItem in groupItems)
            {
                if (VisualTreeHelper.GetChildrenCount(groupItem) == 0) continue;

                var expander = FindVisualChild<Expander>(groupItem);
                if (expander != null)
                {
                    expander.IsExpanded = isExpanded;
                }
            }
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T t) yield return t;

                foreach (var childOfChild in FindVisualChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }

        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t) return t;

                var result = FindVisualChild<T>(child);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
}

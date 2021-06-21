using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using TT.Diary.Desktop.ViewModels.Extensions;

namespace TT.Diary.Desktop.Converters
{
    public class TreeViewRootConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not TreeViewItem item)
            {
                return false;
            }

            var tree = DependencyObjectExtension.FindAncestor<TreeView>(item, 0);

            if (tree == null)
            {
                return false;
            }

            return tree.ItemContainerGenerator.ContainerFromIndex(0) == item;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
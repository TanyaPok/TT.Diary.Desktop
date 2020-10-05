using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TT.Diary.Desktop.ViewModels.Common.Extensions
{
    public static class DependencyObjectExtension
    {
        public static DependencyObject FindParentAsTreeViewItem(this DependencyObject current, int step = 0)
        {
            return FindAnchestor<TreeViewItem>(current, step);
        }

        public static object GetTreeViewItemData(this DependencyObject obj)
        {
            var item = obj as TreeViewItem;

            if (item == null)
            {
                throw new ArgumentException(string.Format(ErrorMessages.UnexpectedType.GetDescription(), obj));
            }

            return item.Header;
        }

        public static T FindAnchestor<T>(DependencyObject current, int step) where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    if (step > 0)
                    {
                        step--;
                    }
                    else
                    {
                        return (T)current;
                    }
                }

                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);

            return null;
        }
    }
}

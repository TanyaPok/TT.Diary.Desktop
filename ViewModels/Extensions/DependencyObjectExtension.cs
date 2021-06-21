using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TT.Diary.Desktop.ViewModels.Common;

namespace TT.Diary.Desktop.ViewModels.Extensions
{
    public static class DependencyObjectExtension
    {
        public static DependencyObject FindParentAsTreeViewItem(this DependencyObject current, int step = 0)
        {
            return FindAncestor<TreeViewItem>(current, step);
        }

        public static object GetTreeViewItemData(this DependencyObject obj)
        {
            if (obj is not TreeViewItem item)
            {
                throw new ArgumentException(ErrorMessages.UnexpectedType.GetDescription(), nameof(obj));
            }

            return item.Header;
        }

        public static T FindAncestor<T>(DependencyObject current, int step) where T : DependencyObject
        {
            do
            {
                if (current is T dependencyObject)
                {
                    if (step > 0)
                    {
                        step--;
                    }
                    else
                    {
                        return dependencyObject;
                    }
                }

                current = VisualTreeHelper.GetParent(current);
            } while (current != null);

            return null;
        }
    }
}
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using TT.Diary.Desktop.ViewModels.Common.Extensions;
using TT.Diary.Desktop.ViewModels.Common.Interfaces;

namespace TT.Diary.Desktop.ViewModels.Lists
{
    public class CategoryDragAndDropAlgorithm<T> : IDragAndDrop where T : AbstractListItem, new()
    {
        private readonly string DRAG_FORMAT = "dragCategoryFormat";

        public ICommand MouseMoveObjectCommand { get; private set; }

        public ICommand DragEnterOverLeaveObjectCommand { get; private set; }

        public ICommand DropObjectCommand { get; private set; }

        public CategoryDragAndDropAlgorithm()
        {
            MouseMoveObjectCommand = new RelayCommand<MouseEventArgs>(MouseMoveCategory, canExecute: e => true);
            DragEnterOverLeaveObjectCommand = new RelayCommand<DragEventArgs>(DragEnterCategory, canExecute: e => true);
            DropObjectCommand = new RelayCommand<DragEventArgs>(DropCategory, canExecute: e => true);
        }

        private void MouseMoveCategory(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var control = ((DependencyObject)e.OriginalSource).FindParentAsTreeViewItem();
                if (control == null)
                    return;
                
                var data = control.GetTreeViewItemData() as Category<T>;
                if (!data.IsReadOnlyMode)
                    return;

                var parent = control.FindParentAsTreeViewItem(1);
                if (parent == null)
                    return;

                var parentData = parent.GetTreeViewItemData() as Category<T>;
                if (!parentData.IsReadOnlyMode)
                    return;

                DataObject dragData = new DataObject(DRAG_FORMAT, new KeyValuePair<Category<T>, Category<T>>(parentData, data));
                DragDrop.DoDragDrop(control, dragData, DragDropEffects.Move);
            }
        }

        private void DragEnterCategory(DragEventArgs e)
        {
            var control = ((DependencyObject)e.OriginalSource).FindParentAsTreeViewItem();
            if (control == null || !e.Data.GetDataPresent(DRAG_FORMAT))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            var data = control.GetTreeViewItemData();
            var targetData = data as Category<T>;
            var sourceData = (KeyValuePair<Category<T>, Category<T>>)e.Data.GetData(DRAG_FORMAT);
            if (sourceData.Value == targetData || sourceData.Value.ParentId == targetData.Id || sourceData.Value.HasSubCategory(targetData))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private void DropCategory(DragEventArgs e)
        {
            var control = ((DependencyObject)e.OriginalSource).FindParentAsTreeViewItem();
            if (control == null || !e.Data.GetDataPresent(DRAG_FORMAT))
            {
                return;
            }

            var data = control.GetTreeViewItemData();
            var targetData = data as Category<T>;
            var sourceData = (KeyValuePair<Category<T>, Category<T>>)e.Data.GetData(DRAG_FORMAT);

            if (!sourceData.Value.HasErrors)
            {
                sourceData.Value.Edit(targetData, sourceData.Key);
            }
        }
    }
}

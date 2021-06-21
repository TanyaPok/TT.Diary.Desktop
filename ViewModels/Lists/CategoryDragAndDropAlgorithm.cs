using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TT.Diary.Desktop.ViewModels.Commands;
using TT.Diary.Desktop.ViewModels.Extensions;

namespace TT.Diary.Desktop.ViewModels.Lists
{
    public class CategoryDragAndDropAlgorithm<T> : IDragAndDrop where T : AbstractListItem, new()
    {
        private readonly string _dragFormat = "dragCategoryFormat";

        public ICommand MouseMoveObjectCommand { get; }

        public ICommand DragEnterOverLeaveObjectCommand { get; }

        public ICommand DropObjectCommand { get; }

        public CategoryDragAndDropAlgorithm()
        {
            MouseMoveObjectCommand = new RelayCommand<MouseEventArgs>(MouseMoveCategory, canExecute: e => true);
            DragEnterOverLeaveObjectCommand = new RelayCommand<DragEventArgs>(DragEnterCategory, canExecute: e => true);
            DropObjectCommand =
                new RelayCommand<DragEventArgs>(async (e) => { await DropCategory(e); }, canExecute: e => true, true);
        }

        private void MouseMoveCategory(MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            var control = ((DependencyObject) e.OriginalSource).FindParentAsTreeViewItem();

            if (!(control?.GetTreeViewItemData() is Category<T> data) || !data.IsReadOnlyMode)
                return;

            var parent = control.FindParentAsTreeViewItem(1);

            if (!(parent?.GetTreeViewItemData() is Category<T> parentData) || !parentData.IsReadOnlyMode)
                return;

            var dragData = new DataObject(_dragFormat,
                new KeyValuePair<Category<T>, Category<T>>(parentData, data));
            DragDrop.DoDragDrop(control, dragData, DragDropEffects.Move);
        }

        private void DragEnterCategory(DragEventArgs e)
        {
            var control = ((DependencyObject) e.OriginalSource).FindParentAsTreeViewItem();
            if (control == null || e.Data == null || !e.Data.GetDataPresent(_dragFormat))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            var data = control.GetTreeViewItemData();
            var targetData = data as Category<T>;
            var sourceData = (KeyValuePair<Category<T>, Category<T>>) e.Data.GetData(_dragFormat);
            if (sourceData.Value != targetData && targetData != null && sourceData.Value.ParentId != targetData.Id &&
                !sourceData.Value.HasSubCategory(targetData)) return;
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private async Task DropCategory(DragEventArgs e)
        {
            var control = ((DependencyObject) e.OriginalSource).FindParentAsTreeViewItem();
            if (control == null || e.Data == null || !e.Data.GetDataPresent(_dragFormat))
            {
                return;
            }

            var data = control.GetTreeViewItemData();
            var targetData = data as Category<T>;
            var sourceData = (KeyValuePair<Category<T>, Category<T>>) e.Data.GetData(_dragFormat);

            if (!sourceData.Value.HasErrors)
            {
                await sourceData.Value.Move(targetData, sourceData.Key);
            }
        }
    }
}
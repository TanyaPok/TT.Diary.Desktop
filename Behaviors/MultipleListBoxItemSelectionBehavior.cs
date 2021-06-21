using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace TT.Diary.Desktop.Behaviors
{
    public class MultipleListBoxItemSelectionBehavior<T> : Behavior<ListBox> where T : Enum
    {
        private bool _viewHandled;
        private bool _modelHandled;
        private readonly Array _values;

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(
                nameof(SelectedItems),
                typeof(T),
                typeof(MultipleListBoxItemSelectionBehavior<T>),
                new FrameworkPropertyMetadata(
                    default(T),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedItemsChanged
                )
            );

        public T SelectedItems
        {
            get => (T) GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        protected MultipleListBoxItemSelectionBehavior()
        {
            _values = Enum.GetValues(typeof(T));
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += OnListBoxSelectionChanged;
            ((INotifyCollectionChanged) AssociatedObject.Items).CollectionChanged += OnListBoxItemsChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject == null) return;
            AssociatedObject.SelectionChanged -= OnListBoxSelectionChanged;
            ((INotifyCollectionChanged) AssociatedObject.Items).CollectionChanged -= OnListBoxItemsChanged;
        }

        private static void OnSelectedItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var behavior = (MultipleListBoxItemSelectionBehavior<T>) sender;

            if (behavior._modelHandled)
            {
                return;
            }

            if (behavior.AssociatedObject == null)
            {
                return;
            }

            behavior._modelHandled = true;
            behavior.SelectItems();
            behavior._modelHandled = false;
        }

        /// <summary>
        /// Propagate selected items from model to view
        /// </summary>
        private void SelectItems()
        {
            _viewHandled = true;
            AssociatedObject.SelectedItems.Clear();

            if (SelectedItems == null)
            {
                _viewHandled = false;
                return;
            }

            foreach (Enum value in _values)
            {
                if (SelectedItems.HasFlag(value))
                {
                    AssociatedObject.SelectedItems.Add(value);
                }
            }

            _viewHandled = false;
        }

        /// <summary>
        /// Propagate selected items from view to model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnListBoxSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (_viewHandled)
            {
                return;
            }

            if (AssociatedObject.Items.SourceCollection == null)
            {
                return;
            }

            var values = AssociatedObject.SelectedItems.Cast<int>().Aggregate((p, c) => p + c);
            SelectedItems = (T) Enum.Parse(typeof(T), values.ToString());
        }

        /// <summary>
        /// Re-select items when the set of items changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnListBoxItemsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (_viewHandled)
            {
                return;
            }

            if (AssociatedObject.Items.SourceCollection == null)
            {
                return;
            }

            SelectItems();
        }
    }
}
using System;
using System.Windows;
using System.Windows.Data;

namespace TT.Diary.Desktop.Views.Controls
{
    /// <summary>
    /// Allow enabling binding, for example, MenuContext to DataContext of the MenuContext's owner
    /// </summary>
    public class DataContextSpy : Freezable
    {
        /// <summary>
        /// Gets/sets whether the spy will return the CurrentItem of the 
        /// ICollectionView that wraps the data context, assuming it is
        /// a collection of some sort. If the data context is not a 
        /// collection, this property has no effect. 
        /// The default value is true.
        /// </summary>
        private bool IsSynchronizedWithCurrentItem { get; }

        public object DataContext
        {
            get => GetValue(DataContextProperty);
            set => SetValue(DataContextProperty, value);
        }

        // Borrow the DataContext dependency property from FrameworkElement.
        public static readonly DependencyProperty DataContextProperty =
            FrameworkElement.DataContextProperty.AddOwner(
                typeof(DataContextSpy),
                new PropertyMetadata(null, null, OnCoerceDataContext));

        public DataContextSpy()
        {
            // This binding allows the spy to inherit a DataContext.
            BindingOperations.SetBinding(this, DataContextProperty, new Binding());
            IsSynchronizedWithCurrentItem = true;
        }

        static object OnCoerceDataContext(DependencyObject depObj, object value)
        {
            if (depObj is not DataContextSpy spy)
                return value;

            if (!spy.IsSynchronizedWithCurrentItem) return value;
            var view = CollectionViewSource.GetDefaultView(value);
            return view != null ? view.CurrentItem : value;
        }

        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace TT.Diary.Desktop.Views.Controls.TimeManagement
{
    /// <summary>
    /// Interaction logic for ScheduledList.xaml
    /// </summary>
    public partial class ScheduledList : UserControl
    {
        private readonly string UNEXPECTED_TYPE = @"Unexpected type {0}";

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(ScheduledList),
            new FrameworkPropertyMetadata(string.Empty));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
           nameof(Icon),
           typeof(Image),
           typeof(ScheduledList));

        public Image Icon
        {
            get => (Image)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty TrackersAreaWidthProperty = DependencyProperty.Register(
            nameof(TrackersAreaWidth),
            typeof(double),
            typeof(ScheduledList),
            new FrameworkPropertyMetadata(50.0));

        public double TrackersAreaWidth
        {
            get => (double)GetValue(TrackersAreaWidthProperty);
            set => SetValue(TrackersAreaWidthProperty, value);
        }

        public static readonly DependencyProperty TrackerAreaWidthProperty = DependencyProperty.Register(
            nameof(TrackerAreaWidth),
            typeof(double),
            typeof(ScheduledList),
            new FrameworkPropertyMetadata(50.0));

        public double TrackerAreaWidth
        {
            get => (double)GetValue(TrackerAreaWidthProperty);
            set => SetValue(TrackerAreaWidthProperty, value);
        }

        public ObservableCollection<DateTime> Dates { get; } = new ObservableCollection<DateTime>();

        public ScheduledList()
        {
            InitializeComponent();
            DataContextChanged += ScheduledList_DataContextChanged;
        }

        ~ScheduledList()
        {
            DataContextChanged -= ScheduledList_DataContextChanged;
        }

        private void ScheduledList_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var context = e.NewValue as IScheduledListSource;

            if (context == null)
            {
                throw new ArgumentException(UNEXPECTED_TYPE, nameof(context));
            }

            if (context.DateRange == null)
            {
                throw new ArgumentException("Do not set {0}", nameof(context.DateRange));
            }

            if (context.DateRange.InvocationListMethodsCount == 0)
            {
                context.DateRange.PropertyChanged += DateRange_PropertyChanged;
            }

            SetUp(context.DateRange);
        }

        private void DateRange_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var dateRange = sender as DateRange;

            if (dateRange == null)
            {
                throw new ArgumentException(UNEXPECTED_TYPE, nameof(dateRange));
            }

            SetUp(dateRange);
        }

        private void SetUp(DateRange dateRange)
        {
            var start = dateRange.StartDate.Date;
            var finish = dateRange.FinishDate.HasValue ? dateRange.FinishDate.Value.Date : start;

            if (start > finish)
            {
                throw new ArgumentException("StartDate must be less or equal FinishDate");
            }

            Dispatcher.BeginInvoke(
                (Action)(() =>
                {
                    Dates.Clear();

                    do
                    {
                        Dates.Add(start);
                        start = start.AddDays(1);
                    } while (finish >= start);

                    TrackersAreaWidth = Dates.Count * TrackerAreaWidth;
                }),
                DispatcherPriority.DataBind);
        }
    }
}

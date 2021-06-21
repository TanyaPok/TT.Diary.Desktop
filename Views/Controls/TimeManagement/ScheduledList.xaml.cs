using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using TT.Diary.Desktop.ViewModels.TimeManagement;

namespace TT.Diary.Desktop.Views.Controls.TimeManagement
{
    /// <summary>
    /// Interaction logic for ScheduledList.xaml
    /// </summary>
    public partial class ScheduledList : UserControl
    {
        private const string UNEXPECTED_MESSAGE = @"Unexpected type {0}";

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(ScheduledList),
            new FrameworkPropertyMetadata(string.Empty));

        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty WeekdaysVisibilityProperty = DependencyProperty.Register(
            nameof(WeekdaysVisibility),
            typeof(Visibility),
            typeof(ScheduledList),
            new FrameworkPropertyMetadata(Visibility.Visible));

        public Visibility WeekdaysVisibility
        {
            get => (Visibility) GetValue(WeekdaysVisibilityProperty);
            set => SetValue(WeekdaysVisibilityProperty, value);
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            nameof(Icon),
            typeof(Image),
            typeof(ScheduledList));

        public Image Icon
        {
            get => (Image) GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty TrackersAreaWidthProperty = DependencyProperty.Register(
            nameof(TrackersAreaWidth),
            typeof(double),
            typeof(ScheduledList),
            new FrameworkPropertyMetadata(70.0));

        public double TrackersAreaWidth
        {
            get => (double) GetValue(TrackersAreaWidthProperty);
            set => SetValue(TrackersAreaWidthProperty, value);
        }

        public static readonly DependencyProperty TrackerAreaWidthProperty = DependencyProperty.Register(
            nameof(TrackerAreaWidth),
            typeof(double),
            typeof(ScheduledList),
            new FrameworkPropertyMetadata(70.0));

        public double TrackerAreaWidth
        {
            get => (double) GetValue(TrackerAreaWidthProperty);
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

        private static void ScheduledList_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not IScheduledListSource context)
            {
                throw new ArgumentException(UNEXPECTED_MESSAGE, nameof(context));
            }

            if (context.DateRange == null)
            {
                throw new ArgumentException(UNEXPECTED_MESSAGE, nameof(context.DateRange));
            }

            if (sender is not ScheduledList self)
            {
                throw new ArgumentException(UNEXPECTED_MESSAGE, nameof(sender));
            }

            if (context.DateRange.InvocationListMethodsCount == 0)
            {
                context.DateRange.PropertyChanged += ((o, args) => { Configure(self, context); });
            }

            Configure(self, context);
        }

        private static void Configure(ScheduledList self, IScheduledListSource source)
        {
            var start = source.DateRange.StartDate.Date;
            var finish = source.DateRange.FinishDate.Date;

            if (start > finish)
            {
                return;
            }

            self.Dates.Clear();

            do
            {
                self.Dates.Add(start);
                start = start.AddDays(1);
            } while (finish >= start);

            self.TrackersAreaWidth = self.Dates.Count * self.TrackerAreaWidth;
        }
    }
}
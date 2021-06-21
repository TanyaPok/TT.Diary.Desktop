using System.Windows;
using System.Windows.Controls;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Lists;
using TT.Diary.Desktop.ViewModels.TimeManagement;

namespace TT.Diary.Desktop.Views.TimeManagement
{
    /// <summary>
    /// Interaction logic for ScheduledToDo.xaml
    /// </summary>
    public partial class ScheduledToDo : UserControl
    {
        public static readonly DependencyProperty DateRangeProperty = DependencyProperty.Register(
            nameof(DateRange),
            typeof(DateRange),
            typeof(ScheduledToDo),
            new PropertyMetadata()
            {
                PropertyChangedCallback = OnDateRangeChanged
            });

        public DateRange DateRange
        {
            get => (DateRange) GetValue(DateRangeProperty);
            set => SetValue(DateRangeProperty, value);
        }

        public MtObservableCollection<ITracker> Trackers { get; } = new MtObservableCollection<ITracker>();

        public ScheduledToDo()
        {
            InitializeComponent();
        }

        private static void OnDateRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ScheduledToDo) d;
            ScheduledItem.ConfigureTrackers(
                (ToDo<ScheduleSettings>) ((FrameworkElement) d).DataContext,
                control.DateRange,
                control.Trackers);
        }
    }
}
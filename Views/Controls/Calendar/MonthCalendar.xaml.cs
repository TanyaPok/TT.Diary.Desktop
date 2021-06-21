using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using TT.Diary.Desktop.ViewModels.Calendar;

namespace TT.Diary.Desktop.Views.Controls.Calendar
{
    /// <summary>
    /// Interaction logic for MonthCalendar.xaml
    /// </summary>
    public partial class MonthCalendar : UserControl
    {
        public static readonly DependencyProperty HeadingProperty;

        public string Heading
        {
            get => (string) GetValue(HeadingProperty);
            set => SetValue(HeadingProperty, value);
        }

        public static readonly DependencyProperty DaysProperty;

        public ObservableCollection<AbstractMonthCalendarData> Days
        {
            get => (ObservableCollection<AbstractMonthCalendarData>) GetValue(DaysProperty);
            set => SetValue(DaysProperty, value);
        }

        static MonthCalendar()
        {
            HeadingProperty = DependencyProperty.Register("Heading", typeof(string), typeof(MonthCalendar),
                new FrameworkPropertyMetadata(string.Empty));

            var frameworkPropertyMetadata =
                new FrameworkPropertyMetadata(new ObservableCollection<AbstractMonthCalendarData>())
                {
                    CoerceValueCallback = CoerceDays
                };

            DaysProperty = DependencyProperty.Register("Days", typeof(ObservableCollection<AbstractMonthCalendarData>),
                typeof(MonthCalendar), frameworkPropertyMetadata);
        }

        public MonthCalendar()
        {
            InitializeComponent();
        }

        private static object CoerceDays(DependencyObject d, object baseValue)
        {
            var list = (IList<AbstractMonthCalendarData>) baseValue;

            if (list.Count == 0)
            {
                return list;
            }

            var startWeekday = list[0].DayOfWeek;

            list.Insert(0, new Cap() {Head = "Mon"});
            list.Insert(1, new Cap() {Head = "Tue"});
            list.Insert(2, new Cap() {Head = "Wed"});
            list.Insert(3, new Cap() {Head = "Thu"});
            list.Insert(4, new Cap() {Head = "Fri"});
            list.Insert(5, new Cap() {Head = "Sat"});
            list.Insert(6, new Cap() {Head = "Sun"});

            if (startWeekday == DayOfWeek.Sunday)
            {
                for (var i = 7; i < 6 + 7; i++)
                {
                    list.Insert(i, new Cap());
                }

                return list;
            }

            for (var i = 7; i < (int) startWeekday - 1 + 7; i++)
            {
                list.Insert(i, new Cap());
            }

            return list;
        }
    }
}
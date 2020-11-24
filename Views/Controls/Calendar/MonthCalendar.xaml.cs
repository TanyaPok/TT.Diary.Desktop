using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

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
            get => (string)GetValue(HeadingProperty);
            set => SetValue(HeadingProperty, value);
        }

        public static readonly DependencyProperty DaysProperty;

        public ObservableCollection<IMonthCalendarData> Days
        {
            get => (ObservableCollection<IMonthCalendarData>)GetValue(DaysProperty);
            set => SetValue(DaysProperty, value);
        }

        static MonthCalendar()
        {
            HeadingProperty = DependencyProperty.Register("Heading", typeof(string), typeof(MonthCalendar), new FrameworkPropertyMetadata(string.Empty));

            FrameworkPropertyMetadata frameworkPropertyMetadata = new FrameworkPropertyMetadata(new ObservableCollection<IMonthCalendarData>())
            {
                CoerceValueCallback = new CoerceValueCallback(CoerceDays)
            };

            DaysProperty = DependencyProperty.Register("Days", typeof(ObservableCollection<IMonthCalendarData>), typeof(MonthCalendar), frameworkPropertyMetadata);
        }

        public MonthCalendar()
        {
            InitializeComponent();
        }

        private static object CoerceDays(DependencyObject d, object baseValue)
        {
            IList<IMonthCalendarData> list = (IList<IMonthCalendarData>)baseValue;

            if (list.Count == 0)
            {
                return list;
            }

            var startWeekday = ((MonthDay)list[0]).Date.DayOfWeek;

            list.Insert(0, new Cap() { Text = "Mon" });
            list.Insert(1, new Cap() { Text = "Tue" });
            list.Insert(2, new Cap() { Text = "Wed" });
            list.Insert(3, new Cap() { Text = "Thu" });
            list.Insert(4, new Cap() { Text = "Fri" });
            list.Insert(5, new Cap() { Text = "Sat" });
            list.Insert(6, new Cap() { Text = "Sun" });

            if (startWeekday == DayOfWeek.Sunday)
            {
                for (var i = 7; i < 6 + 7; i++)
                {
                    list.Insert(i, new Cap());
                }

                return list;
            }

            for (var i = 7; i < (int)startWeekday - 1 + 7; i++)
            {
                list.Insert(i, new Cap());
            }

            return list;
        }
    }
}

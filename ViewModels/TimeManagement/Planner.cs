using System.Collections.Generic;
using TT.Diary.Desktop.ViewModels.Lists;

namespace TT.Diary.Desktop.ViewModels.TimeManagement
{
    public class Planner
    {
        public IEnumerable<Note> Notes { get; set; }

        public IEnumerable<Habit<ScheduleSettings>> Habits { get; set; }

        public IEnumerable<ToDo<ScheduleSettings>> ToDoList { get; set; }

        public IEnumerable<Appointment<ScheduleSettings>> Appointments { get; set; }

        public IEnumerable<Wish<ScheduleSettings>> WishList { get; set; }
    }
}
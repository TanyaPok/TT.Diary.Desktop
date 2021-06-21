namespace TT.Diary.Desktop.ViewModels.Common
{
    public abstract class ServiceOperationContract
    {
        public const string DateFormat = "yyyy-MM-dd";

        public const string SetUser = "user";

        public const string GetWishList = "wishlist?userid={0}";
        public const string GetUnscheduledWishList = "unscheduledwishlist?userid={0}";
        public const string GetTodoList = "todolist?userid={0}";
        public const string GetUnscheduledTodoList = "unscheduledtodolist?userid={0}";
        public const string GetHabits = "habits?userid={0}";
        public const string GetUnscheduledHabits = "unscheduledhabits?userid={0}";
        public const string GetNotes = "notes?userid={0}";
        public const string GetAppointments = "appointments?userid={0}";
        public const string GetUnscheduledAppointments = "unscheduledappointments?userid={0}";

        public const string RequestFormat = "{0}/{1}";
        public const string Category = "category";
        public const string Wish = "wish";
        public const string Todo = "todo";
        public const string Appointment = "appointment";
        public const string Habit = "habit";
        public const string Note = "note";

        public const string HabitSchedule = "habitschedule";
        public const string HabitTracker = "habittracker";
        public const string HabitTrackers = "habittrackers/{0}";

        public const string TodoSchedule = "todoschedule";
        public const string TodoTracker = "todotracker";
        public const string TodoTrackers = "todotrackers/{0}";

        public const string AppointmentSchedule = "appointmentschedule";
        public const string AppointmentTracker = "appointmenttracker";
        public const string AppointmentTrackers = "appointmenttrackers/{0}";

        public const string WishSchedule = "wishschedule";

        public const string GetDailySchedule = "planner?userid={0}&startdate={1}&finishdate={2}";
        public const string GetYearlySchedule = "annualproductivity?userid={0}&startdate={1}&finishdate={2}";
        public const string GetMonthlySchedule = "scheduledappointments?userid={0}&startdate={1}&finishdate={2}";
    }
}
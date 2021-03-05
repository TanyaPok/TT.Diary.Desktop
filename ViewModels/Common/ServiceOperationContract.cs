namespace TT.Diary.Desktop.ViewModels.Common
{
    public abstract class ServiceOperationContract
    {
        public const string DATE_FORMAT = "yyyy-MM-dd";

        public const string SET_USER = "user";

        public const string GET_WISH_LIST = "wishlist?userid={0}";
        public const string GET_UNSCHEDULED_WISH_LIST = "unscheduledwishlist?userid={0}";
        public const string GET_TODO_LIST = "todolist?userid={0}";
        public const string GET_UNSCHEDULED_TODO_LIST = "unscheduledtodolist?userid={0}";
        public const string GET_HABITS = "habits?userid={0}";
        public const string GET_UNSCHEDULED_HABITS = "unscheduledhabits?userid={0}";
        public const string GET_NOTES = "notes?userid={0}";
        public const string GET_APPOINTMENTS = "appointments?userid={0}";
        public const string GET_UNSCHEDULED_APPOINTMENTS = "unscheduledappointments?userid={0}";

        public const string REQUEST_FORMAT = "{0}/{1}";
        public const string CATEGORY = "category";
        public const string WISH = "wish";
        public const string TODO = "todo";
        public const string APPOINTMENT = "appointment";
        public const string HABIT = "habit";
        public const string NOTE = "note";

        public const string HABIT_SCHEDULE = "habitschedule";
        public const string HABIT_TRACKER = "habittracker";
        public const string HABIT_TRACKERS = "habittrackers/{0}";

        public const string TODO_SCHEDULE = "todoschedule";
        public const string TODO_TRACKER = "todotracker";
        public const string TODO_TRACKERS = "todotrackers/{0}";

        public const string APPOINTMENT_SCHEDULE = "appointmentschedule";
        public const string APPOINTMENT_TRACKER = "appointmenttracker";
        public const string APPOINTMENT_TRACKERS = "appointmenttrackers/{0}";

        public const string WISH_SCHEDULE = "wishschedule";

        public const string GET_DAILY_SCHEDULE = "planner?userid={0}&startdate={1}&finishdate={2}";
        public const string GET_YEARLY_SCHEDULE = "annualproductivity?userid={0}&startdate={1}&finishdate={2}";
        public const string GET_MONTHLY_SCHEDULE = "scheduledappointments?userid={0}&startdate={1}&finishdate={2}";
    }
}

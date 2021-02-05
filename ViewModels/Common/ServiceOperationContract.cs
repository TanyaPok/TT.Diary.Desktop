namespace TT.Diary.Desktop.ViewModels.Common
{
    public abstract class ServiceOperationContract
    {
        public const string DATE_FORMAT = "yyyy-MM-dd";

        public const string SET_USER = "user";

        public const string GET_WISH_LIST = "wishlist?userid={0}";
        public const string GET_TODO_LIST = "todolist?userid={0}";
        public const string GET_HABITS = "habits?userid={0}&onlyunscheduled=false";
        public const string GET_UNSCHEDULED_HABITS = "habits?userid={0}&onlyunscheduled=true";
        public const string GET_NOTES = "notes?userid={0}";

        public const string REQUEST_FORMAT = "{0}/{1}";
        public const string CATEGORY = "category";
        public const string WISH = "wish";
        public const string TODO = "todo";
        public const string HABIT = "habit";
        public const string NOTE = "note";

        public const string HABIT_SCHEDULE = "habitschedule";
        public const string HABIT_TRACKER = "habittracker";
        public const string HABIT_TRACKERS = "habittrackers";

        public const string SCHEDULE_REQUEST_FORMAT = "{0}?userid={1}&startdate={2}&finishdate={3}";
        public const string GET_DAILY_SCHEDULE = "planner";
        public const string GET_YEARLY_SCHEDULE = "annualproductivity";
        public const string GET_MONTHLY_SCHEDULE = "scheduledappointments";
    }
}

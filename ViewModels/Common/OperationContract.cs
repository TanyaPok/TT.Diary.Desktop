namespace TT.Diary.Desktop.ViewModels.Common
{
    public abstract class OperationContract
    {
        public const string REQUEST_FORMAT = "{0}/{1}";
        public const string SCHEDULE_REQUEST_FORMAT = "{0}?userid={1}&startdate={2}&finishdate={3}";

        public const string SET_USER = "user";

        public const string GET_WISH_LIST = "wishlist";
        public const string GET_TODO_LIST = "todolist";
        public const string GET_HABITS = "habits";
        public const string GET_NOTES = "notes";

        public const string CATEGORY = "category";
        public const string WISH = "wish";
        public const string TODO = "todo";
        public const string HABIT = "habit";
        public const string NOTE = "note";

        public const string GET_DAILY_SCHEDULE = "planner";
    }
}

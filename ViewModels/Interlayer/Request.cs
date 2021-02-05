namespace TT.Diary.Desktop.ViewModels.Interlayer
{
    /// <summary>
    /// After sending, will be disposed
    /// </summary>
    public class Request
    {
        public string OperationContract { get; internal set; }

        public object Data { get; internal set; }

        public string AdditionalInfo { get; internal set; }
    }
}

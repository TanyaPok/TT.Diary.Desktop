using System.ComponentModel;

namespace TT.Diary.Desktop.ViewModels.Common
{
    public enum ErrorMessages
    {
        [Description("OAuth authorization error: {0}.")]
        OAuthAuthorizationError,

        [Description("Invalid authorization response: {0}.")]
        InvalidAuthorizationResponse,

        [Description("Received request with invalid state ({0}).")]
        RequestInvalidState,

        [Description("User cannot be set - response with status code ({0}).")]
        SetUserError,

        [Description("Unexpected type {0}.")] UnexpectedType,

        [Description("List {0} cannot be got - response with status code ({1}).")]
        GetList,

        [Description("{0} cannot be removed\n{1}")]
        Remove,

        [Description("{0} cannot be edited\n{1}")]
        Edit,

        [Description("{0} cannot be added\n{1}")]
        Add,

        [Description("Schedule for {0} cannot be got - response with status code ({1}).")]
        GetSchedule,
    }

    public enum ValidationMessages
    {
        [Description("Empty description")] EmptyDescription,

        [Description("Incorrect {0} range")] IncorrectRange,

        [Description("Must be greater than 0")]
        IncorrectGap
    }
}
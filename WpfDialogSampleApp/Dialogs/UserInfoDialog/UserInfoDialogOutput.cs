using WpfDialogSampleApp.Core.Dialogs.Interfaces;

namespace WpfDialogSampleApp.Dialogs.UserInfoDialog
{
    public enum UserInfoDialogResult
    {
        Ok,
        Cancel
    }

    public class UserInfoDialogOutput : IDialogContentOutput
    {
        public UserInfoDialogResult Result { get; }
        public string Name { get; }
        public string Email { get; }

        public UserInfoDialogOutput(UserInfoDialogResult result, string name = "", string email = "")
        {
            Result = result;
            Name = name;
            Email = email;
        }

        public bool IsConfirmed => Result == UserInfoDialogResult.Ok;
    }
}

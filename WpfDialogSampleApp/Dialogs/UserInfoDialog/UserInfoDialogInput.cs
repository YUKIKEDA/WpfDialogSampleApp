using WpfDialogSampleApp.Core.Dialogs.Interfaces;

namespace WpfDialogSampleApp.Dialogs.UserInfoDialog
{
    public class UserInfoDialogInput : IDialogContentInput
    {
        public string InitialName { get; }
        public string InitialEmail { get; }

        public UserInfoDialogInput(string initialName = "", string initialEmail = "")
        {
            InitialName = initialName;
            InitialEmail = initialEmail;
        }
    }
}

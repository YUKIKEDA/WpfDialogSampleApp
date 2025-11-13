using WpfDialogSampleApp.Core.Dialogs.Interfaces;

namespace WpfDialogSampleApp.Dialogs.ConfirmDialog
{
    public class ConfirmDialogInput : IDialogContentInput
    {
        public string Title { get; }
        public string Message { get; }
        public string OkButtonText { get; }
        public string CancelButtonText { get; }

        public ConfirmDialogInput(string title, string message, string okButtonText = "OK", string cancelButtonText = "キャンセル")
        {
            Title = title;
            Message = message;
            OkButtonText = okButtonText;
            CancelButtonText = cancelButtonText;
        }
    }
}

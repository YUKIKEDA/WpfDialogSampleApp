using WpfDialogSampleApp.Core.Dialogs.Interfaces;

namespace WpfDialogSampleApp.Dialogs.ProgressDialog
{
    public class ProgressDialogInput : IDialogContentInput
    {
        public string Title { get; }
        public string Message { get; }
        public bool IsIndeterminate { get; }
        public bool CanCancel { get; }
        public string CancelButtonText { get; }

        public ProgressDialogInput(
            string title, 
            string message, 
            bool isIndeterminate = false,
            bool canCancel = true,
            string cancelButtonText = "キャンセル")
        {
            Title = title;
            Message = message;
            IsIndeterminate = isIndeterminate;
            CanCancel = canCancel;
            CancelButtonText = cancelButtonText;
        }
    }
}

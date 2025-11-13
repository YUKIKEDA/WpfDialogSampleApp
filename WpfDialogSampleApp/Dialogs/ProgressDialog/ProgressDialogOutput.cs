using WpfDialogSampleApp.Core.Dialogs.Interfaces;

namespace WpfDialogSampleApp.Dialogs.ProgressDialog
{
    public enum ProgressDialogResult
    {
        Completed,
        Cancelled
    }

    public class ProgressDialogOutput : IDialogContentOutput
    {
        public ProgressDialogResult Result { get; }

        public ProgressDialogOutput(ProgressDialogResult result)
        {
            Result = result;
        }

        public bool IsCompleted => Result == ProgressDialogResult.Completed;
        public bool IsCancelled => Result == ProgressDialogResult.Cancelled;
    }
}

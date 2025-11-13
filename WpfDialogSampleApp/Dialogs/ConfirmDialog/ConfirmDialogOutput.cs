using WpfDialogSampleApp.Core.Dialogs.Interfaces;

namespace WpfDialogSampleApp.Dialogs.ConfirmDialog
{
    public enum ConfirmDialogResult
    {
        Ok,
        Cancel
    }

    public class ConfirmDialogOutput : IDialogContentOutput
    {
        public ConfirmDialogResult Result { get; }

        public ConfirmDialogOutput(ConfirmDialogResult result)
        {
            Result = result;
        }

        public bool IsConfirmed => Result == ConfirmDialogResult.Ok;
    }
}

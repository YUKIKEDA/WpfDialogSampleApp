using WpfDialogSampleApp.Core.Dialogs.Interfaces;

namespace WpfDialogSampleApp.Dialogs.CheckboxConfirmDialog
{
    public enum CheckboxConfirmDialogResult
    {
        Ok,
        Cancel
    }

    public class CheckboxConfirmDialogOutput : IDialogContentOutput
    {
        public CheckboxConfirmDialogResult Result { get; }
        public bool IsCheckboxChecked { get; }

        public CheckboxConfirmDialogOutput(CheckboxConfirmDialogResult result, bool isCheckboxChecked = false)
        {
            Result = result;
            IsCheckboxChecked = isCheckboxChecked;
        }

        public bool IsConfirmed => Result == CheckboxConfirmDialogResult.Ok;
    }
}

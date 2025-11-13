using WpfDialogSampleApp.Core.Dialogs.Interfaces;

namespace WpfDialogSampleApp.Dialogs.CheckboxConfirmDialog
{
    public class CheckboxConfirmDialogInput : IDialogContentInput
    {
        public string Title { get; }
        public string Message { get; }
        public string CheckboxText { get; }
        public bool IsCheckboxChecked { get; }
        public string OkButtonText { get; }
        public string CancelButtonText { get; }

        public CheckboxConfirmDialogInput(
            string title, 
            string message, 
            string checkboxText, 
            bool isCheckboxChecked = false,
            string okButtonText = "OK", 
            string cancelButtonText = "キャンセル")
        {
            Title = title;
            Message = message;
            CheckboxText = checkboxText;
            IsCheckboxChecked = isCheckboxChecked;
            OkButtonText = okButtonText;
            CancelButtonText = cancelButtonText;
        }
    }
}

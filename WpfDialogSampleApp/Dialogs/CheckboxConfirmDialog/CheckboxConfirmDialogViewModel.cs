using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.ComponentModel;
using System.Reactive.Disposables;
using WpfDialogSampleApp.Core.Dialogs.Interfaces;

namespace WpfDialogSampleApp.Dialogs.CheckboxConfirmDialog
{
    public class CheckboxConfirmDialogViewModel : INotifyPropertyChanged, IDialogContentViewModel<CheckboxConfirmDialogInput, CheckboxConfirmDialogOutput>, IDisposable
    {
        private TaskCompletionSource<CheckboxConfirmDialogOutput>? _taskCompletionSource;
        private readonly CompositeDisposable _disposables = new();

        public ReactiveProperty<string> Title { get; }
        public ReactiveProperty<string> Message { get; }
        public ReactiveProperty<string> CheckboxText { get; }
        public ReactiveProperty<bool> IsCheckboxChecked { get; }
        public ReactiveProperty<string> OkButtonText { get; }
        public ReactiveProperty<string> CancelButtonText { get; }
        public ReactiveCommand OkCommand { get; }
        public ReactiveCommand CancelCommand { get; }

        public CheckboxConfirmDialogViewModel()
        {
            // ReactivePropertyの初期化
            Title = new ReactiveProperty<string>(string.Empty)
                .AddTo(_disposables);

            Message = new ReactiveProperty<string>(string.Empty)
                .AddTo(_disposables);

            CheckboxText = new ReactiveProperty<string>(string.Empty)
                .AddTo(_disposables);

            IsCheckboxChecked = new ReactiveProperty<bool>(false)
                .AddTo(_disposables);

            OkButtonText = new ReactiveProperty<string>("OK")
                .AddTo(_disposables);

            CancelButtonText = new ReactiveProperty<string>("キャンセル")
                .AddTo(_disposables);

            // コマンドの初期化
            OkCommand = new ReactiveCommand()
                .WithSubscribe(ExecuteOk)
                .AddTo(_disposables);

            CancelCommand = new ReactiveCommand()
                .WithSubscribe(ExecuteCancel)
                .AddTo(_disposables);
        }

        public void Initialize(CheckboxConfirmDialogInput parameters, TaskCompletionSource<CheckboxConfirmDialogOutput> taskCompletionSource)
        {
            Title.Value = parameters.Title;
            Message.Value = parameters.Message;
            CheckboxText.Value = parameters.CheckboxText;
            IsCheckboxChecked.Value = parameters.IsCheckboxChecked;
            OkButtonText.Value = parameters.OkButtonText;
            CancelButtonText.Value = parameters.CancelButtonText;
            _taskCompletionSource = taskCompletionSource;
        }

        private void ExecuteOk()
        {
            _taskCompletionSource?.SetResult(new CheckboxConfirmDialogOutput(CheckboxConfirmDialogResult.Ok, IsCheckboxChecked.Value));
        }

        private void ExecuteCancel()
        {
            _taskCompletionSource?.SetResult(new CheckboxConfirmDialogOutput(CheckboxConfirmDialogResult.Cancel, IsCheckboxChecked.Value));
        }

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add { }
            remove { }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}

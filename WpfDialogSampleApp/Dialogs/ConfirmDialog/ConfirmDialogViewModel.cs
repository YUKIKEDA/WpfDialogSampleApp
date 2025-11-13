using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.ComponentModel;
using System.Reactive.Disposables;
using WpfDialogSampleApp.Core.Dialogs.Interfaces;

namespace WpfDialogSampleApp.Dialogs.ConfirmDialog
{
    public class ConfirmDialogViewModel : INotifyPropertyChanged, IDialogContentViewModel<ConfirmDialogInput, ConfirmDialogOutput>, IDisposable
    {
        private TaskCompletionSource<ConfirmDialogOutput>? _taskCompletionSource;
        private readonly CompositeDisposable _disposables = new();

        public ReactiveProperty<string> Title { get; }
        public ReactiveProperty<string> Message { get; }
        public ReactiveProperty<string> OkButtonText { get; }
        public ReactiveProperty<string> CancelButtonText { get; }
        public ReactiveCommand OkCommand { get; }
        public ReactiveCommand CancelCommand { get; }

        public ConfirmDialogViewModel()
        {
            // ReactivePropertyの初期化
            Title = new ReactiveProperty<string>(string.Empty)
                .AddTo(_disposables);

            Message = new ReactiveProperty<string>(string.Empty)
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

        public void Initialize(ConfirmDialogInput parameters, TaskCompletionSource<ConfirmDialogOutput> taskCompletionSource)
        {
            Title.Value = parameters.Title;
            Message.Value = parameters.Message;
            OkButtonText.Value = parameters.OkButtonText;
            CancelButtonText.Value = parameters.CancelButtonText;
            _taskCompletionSource = taskCompletionSource;
        }

        private void ExecuteOk()
        {
            _taskCompletionSource?.SetResult(new ConfirmDialogOutput(ConfirmDialogResult.Ok));
        }

        private void ExecuteCancel()
        {
            _taskCompletionSource?.SetResult(new ConfirmDialogOutput(ConfirmDialogResult.Cancel));
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

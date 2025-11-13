using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.ComponentModel;
using System.Reactive.Disposables;
using WpfDialogSampleApp.Core.Dialogs.Interfaces;

namespace WpfDialogSampleApp.Dialogs.ProgressDialog
{
    public class ProgressDialogViewModel : INotifyPropertyChanged, IDialogContentViewModel<ProgressDialogInput, ProgressDialogOutput>, IDisposable
    {
        private TaskCompletionSource<ProgressDialogOutput>? _taskCompletionSource;
        private readonly CompositeDisposable _disposables = new();
        private CancellationTokenSource? _cancellationTokenSource;

        public ReactiveProperty<string> Title { get; }
        public ReactiveProperty<string> Message { get; }
        public ReactiveProperty<double> ProgressValue { get; }
        public ReactiveProperty<bool> IsIndeterminate { get; }
        public ReactiveProperty<bool> CanCancel { get; }
        public ReactiveProperty<string> CancelButtonText { get; }
        public ReactiveCommand CancelCommand { get; }

        // 進捗更新用のプロパティ
        public ReactiveProperty<string> CurrentOperation { get; }
        public ReactiveProperty<bool> IsCompleted { get; }

        public ProgressDialogViewModel()
        {
            // ReactivePropertyの初期化
            Title = new ReactiveProperty<string>(string.Empty)
                .AddTo(_disposables);

            Message = new ReactiveProperty<string>(string.Empty)
                .AddTo(_disposables);

            ProgressValue = new ReactiveProperty<double>(0.0)
                .AddTo(_disposables);

            IsIndeterminate = new ReactiveProperty<bool>(false)
                .AddTo(_disposables);

            CanCancel = new ReactiveProperty<bool>(true)
                .AddTo(_disposables);

            CancelButtonText = new ReactiveProperty<string>("キャンセル")
                .AddTo(_disposables);

            CurrentOperation = new ReactiveProperty<string>(string.Empty)
                .AddTo(_disposables);

            IsCompleted = new ReactiveProperty<bool>(false)
                .AddTo(_disposables);

            // キャンセルコマンド
            CancelCommand = CanCancel
                .ToReactiveCommand()
                .WithSubscribe(ExecuteCancel)
                .AddTo(_disposables);
        }

        public void Initialize(ProgressDialogInput parameters, TaskCompletionSource<ProgressDialogOutput> taskCompletionSource)
        {
            Title.Value = parameters.Title;
            Message.Value = parameters.Message;
            IsIndeterminate.Value = parameters.IsIndeterminate;
            CanCancel.Value = parameters.CanCancel;
            CancelButtonText.Value = parameters.CancelButtonText;
            _taskCompletionSource = taskCompletionSource;

            _cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// 進捗を更新します
        /// </summary>
        /// <param name="progress">進捗値（0.0-100.0）</param>
        /// <param name="operation">現在の操作内容</param>
        public void UpdateProgress(double progress, string operation = "")
        {
            ProgressValue.Value = Math.Max(0, Math.Min(100, progress));
            if (!string.IsNullOrEmpty(operation))
            {
                CurrentOperation.Value = operation;
            }
        }

        /// <summary>
        /// 進捗ダイアログを完了します
        /// </summary>
        public void Complete()
        {
            IsCompleted.Value = true;
            ProgressValue.Value = 100;
            _taskCompletionSource?.SetResult(new ProgressDialogOutput(ProgressDialogResult.Completed));
        }

        /// <summary>
        /// キャンセルトークンを取得します
        /// </summary>
        public CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? CancellationToken.None;

        private void ExecuteCancel()
        {
            _cancellationTokenSource?.Cancel();
            _taskCompletionSource?.SetResult(new ProgressDialogOutput(ProgressDialogResult.Cancelled));
        }

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add { }
            remove { }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
            _disposables.Dispose();
        }
    }
}

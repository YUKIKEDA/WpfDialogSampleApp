using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using WpfDialogSampleApp.Core.Dialogs.Interfaces;
using WpfDialogSampleApp.Core.Dialogs.Services;
using WpfDialogSampleApp.Dialogs.ConfirmDialog;

namespace WpfDialogSampleApp.Dialogs.UserInfoDialog
{
    public class UserInfoDialogViewModel : INotifyPropertyChanged, IDialogContentViewModel<UserInfoDialogInput, UserInfoDialogOutput>, IDisposable
    {
        private TaskCompletionSource<UserInfoDialogOutput>? _taskCompletionSource;
        private readonly CompositeDisposable _disposables = new();
        private readonly IDialogService? _dialogService;

        public ReactiveProperty<string> Name { get; }
        public ReactiveProperty<string> Email { get; }
        public ReactiveCommand OkCommand { get; }
        public ReactiveCommand CancelCommand { get; }

        public UserInfoDialogViewModel(IDialogService? dialogService = null)
        {
            _dialogService = dialogService;
            // ReactivePropertyの初期化
            Name = new ReactiveProperty<string>(string.Empty)
                .AddTo(_disposables);

            Email = new ReactiveProperty<string>(string.Empty)
                .AddTo(_disposables);

            // OKコマンド - 名前とメールが両方入力されている場合のみ有効
            var canExecuteOk = Name
                .CombineLatest(Email, (name, email) => 
                    !string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(email))
                .ToReactiveProperty()
                .AddTo(_disposables);

            OkCommand = canExecuteOk
                .ToReactiveCommand()
                .WithSubscribe(ExecuteOk)
                .AddTo(_disposables);

            // キャンセルコマンド - 常に実行可能
            CancelCommand = new ReactiveCommand()
                .WithSubscribe(ExecuteCancel)
                .AddTo(_disposables);
        }

        public void Initialize(UserInfoDialogInput parameters, TaskCompletionSource<UserInfoDialogOutput> taskCompletionSource)
        {
            Name.Value = parameters.InitialName;
            Email.Value = parameters.InitialEmail;
            _taskCompletionSource = taskCompletionSource;
        }

        private async void ExecuteOk()
        {
            // ネストしたダイアログの例：確認ダイアログを表示
            if (_dialogService != null)
            {
                var confirmResult = await _dialogService.ShowDialogAsync<ConfirmDialogViewModel, ConfirmDialogInput, ConfirmDialogOutput>(
                    new ConfirmDialogInput(
                        "確認", 
                        $"以下の情報で登録しますか？\n\n名前: {Name.Value}\nメール: {Email.Value}",
                        "登録", 
                        "キャンセル"));

                if (!confirmResult.IsConfirmed)
                {
                    return; // キャンセルされた場合は何もしない
                }
            }

            _taskCompletionSource?.SetResult(new UserInfoDialogOutput(UserInfoDialogResult.Ok, Name.Value, Email.Value));
        }

        private void ExecuteCancel()
        {
            _taskCompletionSource?.SetResult(new UserInfoDialogOutput(UserInfoDialogResult.Cancel));
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

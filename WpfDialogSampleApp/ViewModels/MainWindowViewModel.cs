using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.ComponentModel;
using System.Reactive.Disposables;
using WpfDialogSampleApp.Core.Dialogs.Services;
using WpfDialogSampleApp.Core.Dialogs.ViewModels;
using WpfDialogSampleApp.Dialogs.UserInfoDialog;

namespace WpfDialogSampleApp.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly IDialogService _dialogService;
    private readonly CompositeDisposable _disposables = new();

    public DialogViewModel? DialogViewModel { get; set; }

    // 最後のダイアログ結果を表示するためのプロパティ
    public ReactiveProperty<string> LastResult { get; }
    public ReactiveProperty<string> LastName { get; }
    public ReactiveProperty<string> LastEmail { get; }

    // ダイアログを開くコマンド
    public ReactiveCommand ShowUserInfoDialogCommand { get; }

    public MainWindowViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;

        // ReactivePropertyの初期化
        LastResult = new ReactiveProperty<string>("まだダイアログが実行されていません")
            .AddTo(_disposables);

        LastName = new ReactiveProperty<string>(string.Empty)
            .AddTo(_disposables);

        LastEmail = new ReactiveProperty<string>(string.Empty)
            .AddTo(_disposables);

        // ダイアログ表示コマンド
        ShowUserInfoDialogCommand = new ReactiveCommand()
            .WithSubscribe(async () => await ShowUserInfoDialogAsync())
            .AddTo(_disposables);
    }

    // ダイアログ表示の使用例
    public async Task ShowUserInfoDialogAsync()
    {
        try
        {
            var result = await _dialogService.ShowDialogAsync<UserInfoDialogViewModel, UserInfoDialogInput, UserInfoDialogOutput>(
                new UserInfoDialogInput("初期名前", "initial@email.com"));

            // 結果の処理
            if (result.IsConfirmed)
            {
                // 確認された場合の処理
                LastResult.Value = "OK が押されました";
                LastName.Value = result.Name;
                LastEmail.Value = result.Email;
            }
            else
            {
                // キャンセルされた場合の処理
                LastResult.Value = "キャンセルされました";
                LastName.Value = string.Empty;
                LastEmail.Value = string.Empty;
            }
        }
        catch (Exception ex)
        {
            LastResult.Value = $"エラーが発生しました: {ex.Message}";
        }
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

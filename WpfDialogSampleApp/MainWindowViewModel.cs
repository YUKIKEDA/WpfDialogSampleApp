using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.ComponentModel;
using System.Reactive.Disposables;
using WpfDialogSampleApp.Core.Dialogs.Services;
using WpfDialogSampleApp.Core.Dialogs.ViewModels;
using WpfDialogSampleApp.Dialogs.UserInfoDialog;
using WpfDialogSampleApp.Dialogs.CheckboxConfirmDialog;
using WpfDialogSampleApp.Dialogs.ProgressDialog;

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
    public ReactiveCommand ShowCheckboxConfirmDialogCommand { get; }
    public ReactiveCommand ShowProgressDialogCommand { get; }

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

        ShowCheckboxConfirmDialogCommand = new ReactiveCommand()
            .WithSubscribe(async () => await ShowCheckboxConfirmDialogAsync())
            .AddTo(_disposables);

        ShowProgressDialogCommand = new ReactiveCommand()
            .WithSubscribe(async () => await ShowProgressDialogAsync())
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

    // チェックボックス付き確認ダイアログの表示例
    public async Task ShowCheckboxConfirmDialogAsync()
    {
        try
        {
            var result = await _dialogService.ShowDialogAsync<CheckboxConfirmDialogViewModel, CheckboxConfirmDialogInput, CheckboxConfirmDialogOutput>(
                new CheckboxConfirmDialogInput(
                    "設定の確認",
                    "この操作を実行すると、現在の設定が変更されます。\n続行してもよろしいですか？",
                    "今後この確認を表示しない",
                    false,
                    "実行",
                    "キャンセル"));

            if (result.IsConfirmed)
            {
                LastResult.Value = $"実行されました（今後表示しない: {(result.IsCheckboxChecked ? "はい" : "いいえ")}）";
                LastName.Value = "チェックボックス確認ダイアログ";
                LastEmail.Value = $"チェックボックス状態: {result.IsCheckboxChecked}";
            }
            else
            {
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

    // 進捗ダイアログの表示例
    public async Task ShowProgressDialogAsync()
    {
        try
        {
            // 進捗ダイアログを表示し、ViewModelの参照を取得
            var (progressViewModel, dialogTask) = _dialogService.ShowProgressDialogAsync<ProgressDialogViewModel, ProgressDialogInput, ProgressDialogOutput>(
                new ProgressDialogInput(
                    "ファイル処理中",
                    "ファイルを処理しています。しばらくお待ちください...",
                    false, // 確定的な進捗
                    true,  // キャンセル可能
                    "キャンセル"));

            // バックグラウンドで進捗を更新
            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(500); // ダイアログが表示されるまで少し待機

                    // 進捗シミュレーション
                    for (int i = 0; i <= 100; i += 5)
                    {
                        // キャンセルされた場合は処理を中断
                        if (progressViewModel.CancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        // 進捗を更新
                        progressViewModel.UpdateProgress(i, $"ステップ {i / 5 + 1}/21 を処理中...");
                        
                        // 処理時間をシミュレート
                        await Task.Delay(200, progressViewModel.CancellationToken);
                    }

                    // 処理完了
                    if (!progressViewModel.CancellationToken.IsCancellationRequested)
                    {
                        progressViewModel.UpdateProgress(100, "処理が完了しました");
                        await Task.Delay(500); // 完了状態を少し表示
                        progressViewModel.Complete();
                    }
                }
                catch (OperationCanceledException)
                {
                    // キャンセルされた場合は何もしない
                }
                catch (Exception)
                {
                    // エラーが発生した場合はダイアログを閉じる
                    progressViewModel.Complete();
                }
            });

            var result = await dialogTask;

            if (result.IsCompleted)
            {
                LastResult.Value = "処理が完了しました";
                LastName.Value = "進捗ダイアログ";
                LastEmail.Value = "正常完了";
            }
            else if (result.IsCancelled)
            {
                LastResult.Value = "処理がキャンセルされました";
                LastName.Value = "進捗ダイアログ";
                LastEmail.Value = "キャンセル";
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

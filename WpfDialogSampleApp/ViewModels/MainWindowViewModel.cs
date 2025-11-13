using WpfDialogSampleApp.Core.Dialogs.Services;
using WpfDialogSampleApp.Core.Dialogs.ViewModels;
using WpfDialogSampleApp.Dialogs.UserInfoDialog;

namespace WpfDialogSampleApp.ViewModels;

public class MainWindowViewModel
{
    private readonly IDialogService _dialogService;

    public DialogViewModel? DialogViewModel { get; set; }

    public MainWindowViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;
        
        // ダイアログの登録例
        // _dialogService.RegisterDialog<UserInfoDialogView, UserInfoDialogViewModel>();
    }

    // ダイアログ表示の使用例
    public async Task ShowUserInfoDialogAsync()
    {
        var result = await _dialogService.ShowDialogAsync<UserInfoDialogViewModel, UserInfoDialogInput, UserInfoDialogOutput>(
            new UserInfoDialogInput("初期名前", "initial@email.com"));
        
        // 結果の処理
        if (result.IsConfirmed)
        {
            // 確認された場合の処理
            var name = result.Name;
            var email = result.Email;
            // TODO: 実際の処理を実装
        }
    }

    // パラメータなしでダイアログを表示する例
    // public async Task ShowSimpleDialogAsync()
    // {
    //     var result = await _dialogService.ShowDialogAsync<SimpleDialogViewModel, SimpleDialogOutput>();
    //     
    //     // 結果の処理
    // }
}

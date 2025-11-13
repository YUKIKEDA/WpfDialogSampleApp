# WPF ダイアログサービス

FlatWpfDialogの実装を参考にした、WPFアプリケーション用のダイアログサービスです。

## 特徴

- 型安全なダイアログ表示
- 入力パラメータと出力結果の型指定
- 非同期ダイアログ表示
- ViewとViewModelの分離
- 依存性注入対応

## 基本的な使用方法

### 1. ダイアログサービスの登録

```csharp
// DIコンテナでの登録例
services.AddSingleton<DialogViewModel>();
services.AddSingleton<IDialogService, DialogService>();
```

### 2. ダイアログの作成

#### Input/Outputクラスの定義

```csharp
// 入力パラメータ
public class UserInfoDialogInput : IDialogContentInput
{
    public string InitialName { get; }
    public string InitialEmail { get; }
    
    public UserInfoDialogInput(string initialName = "", string initialEmail = "")
    {
        InitialName = initialName;
        InitialEmail = initialEmail;
    }
}

// 出力結果
public class UserInfoDialogOutput : IDialogContentOutput
{
    public UserInfoDialogResult Result { get; }
    public string Name { get; }
    public string Email { get; }
    
    public UserInfoDialogOutput(UserInfoDialogResult result, string name = "", string email = "")
    {
        Result = result;
        Name = name;
        Email = email;
    }
    
    public bool IsConfirmed => Result == UserInfoDialogResult.Ok;
}
```

#### ViewModelの実装

```csharp
public class UserInfoDialogViewModel : INotifyPropertyChanged, IDialogContentViewModel<UserInfoDialogInput, UserInfoDialogOutput>
{
    private TaskCompletionSource<UserInfoDialogOutput>? _taskCompletionSource;
    
    // プロパティとコマンドの実装...
    
    public void Initialize(UserInfoDialogInput parameters, TaskCompletionSource<UserInfoDialogOutput> taskCompletionSource)
    {
        Name = parameters.InitialName;
        Email = parameters.InitialEmail;
        _taskCompletionSource = taskCompletionSource;
    }
    
    private void ExecuteOk()
    {
        _taskCompletionSource?.SetResult(new UserInfoDialogOutput(UserInfoDialogResult.Ok, Name, Email));
    }
}
```

### 3. ダイアログの表示

```csharp
using WpfDialogSampleApp.Core.Dialogs.Services;
using WpfDialogSampleApp.Dialogs.UserInfoDialog;

public class MainWindowViewModel
{
    private readonly IDialogService _dialogService;
    
    public MainWindowViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;
        
        // ダイアログの登録
        _dialogService.RegisterDialog<UserInfoDialogView, UserInfoDialogViewModel>();
    }
    
    public async Task ShowUserInfoDialogAsync()
    {
        var input = new UserInfoDialogInput("初期名前", "initial@email.com");
        var result = await _dialogService.ShowDialogAsync<UserInfoDialogViewModel, UserInfoDialogInput, UserInfoDialogOutput>(input);
        
        if (result.IsConfirmed)
        {
            // OKが押された場合の処理
            var name = result.Name;
            var email = result.Email;
        }
    }
    
    // パラメータなしでダイアログを表示
    public async Task ShowSimpleDialogAsync()
    {
        var result = await _dialogService.ShowDialogAsync<SimpleDialogViewModel, SimpleDialogOutput>();
    }
}
```

### 4. MainWindowでのDialogViewの配置

```xml
<Window>
    <Grid>
        <!-- メインコンテンツ -->
        <ContentControl Content="{Binding MainContent}" />
        
        <!-- ダイアログオーバーレイ -->
        <dialogs:DialogView DataContext="{Binding DialogViewModel}" />
    </Grid>
</Window>
```

## プロジェクト構造

```
WpfDialogSampleApp.Core/
├── Dialogs/
│   ├── Interfaces/
│   │   ├── IDialogContentInput.cs
│   │   ├── IDialogContentOutput.cs
│   │   └── IDialogContentViewModel.cs
│   ├── Services/
│   │   ├── IDialogService.cs
│   │   └── DialogService.cs
│   ├── ViewModels/
│   │   └── DialogViewModel.cs
│   └── Views/
│       ├── DialogView.xaml
│       └── DialogView.xaml.cs

WpfDialogSampleApp/
├── Dialogs/
│   └── UserInfoDialog/
│       ├── UserInfoDialogInput.cs
│       ├── UserInfoDialogOutput.cs
│       ├── UserInfoDialogViewModel.cs
│       ├── UserInfoDialogView.xaml
│       └── UserInfoDialogView.xaml.cs
└── ViewModels/
    └── MainWindowViewModel.cs
```

## 注意事項

- ダイアログのViewModelは`IDialogContentViewModel<TInput, TOutput>`を実装する必要があります
- `Initialize`メソッドで受け取った`TaskCompletionSource`を使用してダイアログを閉じます
- ViewModelファクトリを使用する場合は`RegisterViewModelFactory`メソッドを使用してください

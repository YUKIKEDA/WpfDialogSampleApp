# WPF ダイアログサービス

FlatWpfDialogの実装を参考にした、WPFアプリケーション用のダイアログサービスです。

## 特徴

- **型安全なダイアログ表示**: ジェネリクスによる入力・出力の型安全性
- **非同期ダイアログ表示**: async/awaitパターンによる非ブロッキング処理
- **複数ダイアログ対応**: ダイアログの上に新しいダイアログを表示可能（スタック管理）
- **ReactiveProperty.WPF対応**: リアクティブプログラミングによる効率的なUI更新
- **リアクティブダイアログ**: ViewModelの参照を返してリアルタイム更新が可能
- **ViewとViewModelの分離**: MVVM パターンによる保守性の高い設計
- **依存性注入対応**: DIコンテナとの統合

## 基本的な使用方法

### 1. 必要なパッケージのインストール

```xml
<PackageReference Include="ReactiveProperty.WPF" Version="9.6.0" />
```

### 2. ダイアログサービスの登録

```csharp
// App.xaml.cs での登録例
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // DialogViewModelとDialogServiceの初期化
        var dialogViewModel = new DialogViewModel();
        var dialogService = new DialogService(dialogViewModel);

        // ダイアログの登録
        dialogService.RegisterDialog<UserInfoDialogView, UserInfoDialogViewModel>();
        dialogService.RegisterDialog<ConfirmDialogView, ConfirmDialogViewModel>();
        dialogService.RegisterDialog<ProgressDialogView, ProgressDialogViewModel>();

        // ViewModelファクトリの登録（DIが必要な場合）
        dialogService.RegisterViewModelFactory<UserInfoDialogViewModel>(() => new UserInfoDialogViewModel(dialogService));

        // MainWindowの初期化
        var mainViewModel = new MainWindowViewModel(dialogService);
        mainViewModel.DialogViewModel = dialogViewModel;

        var mainWindow = new MainWindow { DataContext = mainViewModel };
        mainWindow.Show();
    }
}
```

### 3. ダイアログの作成

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

#### ViewModelの実装（ReactiveProperty.WPF使用）

```csharp
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Disposables;

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
        Name = new ReactiveProperty<string>(string.Empty).AddTo(_disposables);
        Email = new ReactiveProperty<string>(string.Empty).AddTo(_disposables);

        // OKコマンド - 名前とメールが両方入力されている場合のみ有効
        var canExecuteOk = Name
            .CombineLatest(Email, (name, email) => 
                !string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(email))
            .ToReactiveProperty()
            .AddTo(_disposables);

        OkCommand = canExecuteOk
            .ToReactiveCommand()
            .WithSubscribe(async () => await ExecuteOk())
            .AddTo(_disposables);

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

    private async Task ExecuteOk()
    {
        // ネストしたダイアログの例：確認ダイアログを表示
        if (_dialogService != null)
        {
            var confirmResult = await _dialogService.ShowDialogAsync<ConfirmDialogViewModel, ConfirmDialogInput, ConfirmDialogOutput>(
                new ConfirmDialogInput("確認", $"以下の情報で登録しますか？\n\n名前: {Name.Value}\nメール: {Email.Value}"));

            if (!confirmResult.IsConfirmed)
                return; // キャンセルされた場合は何もしない
        }

        _taskCompletionSource?.SetResult(new UserInfoDialogOutput(UserInfoDialogResult.Ok, Name.Value, Email.Value));
    }

    private void ExecuteCancel()
    {
        _taskCompletionSource?.SetResult(new UserInfoDialogOutput(UserInfoDialogResult.Cancel));
    }

    public event PropertyChangedEventHandler? PropertyChanged { add { } remove { } }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}
```

### 4. ダイアログの表示

#### 通常のダイアログ表示

```csharp
public class MainWindowViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly IDialogService _dialogService;
    private readonly CompositeDisposable _disposables = new();

    public DialogViewModel? DialogViewModel { get; set; }
    public ReactiveProperty<string> LastResult { get; }
    public ReactiveCommand ShowUserInfoDialogCommand { get; }

    public MainWindowViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;

        LastResult = new ReactiveProperty<string>("まだダイアログが実行されていません").AddTo(_disposables);

        ShowUserInfoDialogCommand = new ReactiveCommand()
            .WithSubscribe(async () => await ShowUserInfoDialogAsync())
            .AddTo(_disposables);
    }

    public async Task ShowUserInfoDialogAsync()
    {
        try
        {
            var result = await _dialogService.ShowDialogAsync<UserInfoDialogViewModel, UserInfoDialogInput, UserInfoDialogOutput>(
                new UserInfoDialogInput("初期名前", "initial@email.com"));

            if (result.IsConfirmed)
            {
                LastResult.Value = $"OK が押されました - 名前: {result.Name}, メール: {result.Email}";
            }
            else
            {
                LastResult.Value = "キャンセルされました";
            }
        }
        catch (Exception ex)
        {
            LastResult.Value = $"エラーが発生しました: {ex.Message}";
        }
    }

    // パラメータなしでダイアログを表示
    public async Task ShowSimpleDialogAsync()
    {
        var result = await _dialogService.ShowDialogAsync<SimpleDialogViewModel, SimpleDialogOutput>();
    }
}
```

#### リアクティブダイアログ表示（進捗ダイアログなど）

```csharp
public async Task ShowProgressDialogAsync()
{
    try
    {
        // リアクティブダイアログを表示し、ViewModelの参照を取得
        var (progressViewModel, dialogTask) = _dialogService.ShowReactiveDialogAsync<ProgressDialogViewModel, ProgressDialogInput, ProgressDialogOutput>(
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
                for (int i = 0; i <= 100; i += 5)
                {
                    if (progressViewModel.CancellationToken.IsCancellationRequested)
                        return;

                    progressViewModel.UpdateProgress(i, $"ステップ {i / 5 + 1}/21 を処理中...");
                    await Task.Delay(200, progressViewModel.CancellationToken);
                }

                if (!progressViewModel.CancellationToken.IsCancellationRequested)
                {
                    progressViewModel.UpdateProgress(100, "処理が完了しました");
                    await Task.Delay(500);
                    progressViewModel.Complete();
                }
            }
            catch (OperationCanceledException)
            {
                // キャンセル時の処理
            }
        });

        var result = await dialogTask;

        if (result.IsCompleted)
        {
            LastResult.Value = "処理が完了しました";
        }
        else if (result.IsCancelled)
        {
            LastResult.Value = "処理がキャンセルされました";
        }
    }
    catch (Exception ex)
    {
        LastResult.Value = $"エラーが発生しました: {ex.Message}";
    }
}
```

### 5. MainWindowでのDialogViewの配置

```xml
<Window x:Class="WpfDialogSampleApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:dialogs="clr-namespace:WpfDialogSampleApp.Core.Dialogs.Views;assembly=WpfDialogSampleApp.Core"
        Title="WPF Dialog Sample" Height="450" Width="800">
    <Grid>
        <!-- メインコンテンツ -->
        <StackPanel Margin="20">
            <TextBlock Text="WPF ダイアログサービス サンプル" FontSize="18" FontWeight="Bold" Margin="0,0,0,20"/>
            
            <Button Content="ユーザー情報ダイアログを表示" 
                    Command="{Binding ShowUserInfoDialogCommand}" 
                    Margin="0,0,0,10" Padding="10,5"/>
            
            <Button Content="チェックボックス確認ダイアログを表示" 
                    Command="{Binding ShowCheckboxConfirmDialogCommand}" 
                    Margin="0,0,0,10" Padding="10,5"/>
            
            <Button Content="進捗ダイアログを表示" 
                    Command="{Binding ShowProgressDialogCommand}" 
                    Margin="0,0,0,20" Padding="10,5"/>
            
            <TextBlock Text="最後の結果:" FontWeight="Bold" Margin="0,0,0,5"/>
            <TextBlock Text="{Binding LastResult.Value}" TextWrapping="Wrap"/>
        </StackPanel>
        
        <!-- ダイアログオーバーレイ（最前面に配置） -->
        <dialogs:DialogView DataContext="{Binding DialogViewModel}" />
    </Grid>
</Window>
```

### 6. XAMLでのReactivePropertyバインディング

ReactiveProperty.WPFを使用する場合、XAMLでのバインディングは `.Value` プロパティを使用します：

```xml
<UserControl x:Class="WpfDialogSampleApp.Dialogs.UserInfoDialog.UserInfoDialogView">
    <StackPanel Margin="20">
        <TextBlock Text="ユーザー情報を入力してください" FontSize="16" FontWeight="Bold" Margin="0,0,0,15"/>
        
        <TextBlock Text="名前:" Margin="0,0,0,5"/>
        <TextBox Text="{Binding Name.Value, UpdateSourceTrigger=PropertyChanged}" Margin="0,0,0,10"/>
        
        <TextBlock Text="メールアドレス:" Margin="0,0,0,5"/>
        <TextBox Text="{Binding Email.Value, UpdateSourceTrigger=PropertyChanged}" Margin="0,0,0,15"/>
        
        <StackPanel Orientation="Horizontal" HorizontalAlignment="Right">
            <Button Content="OK" Command="{Binding OkCommand}" Margin="0,0,10,0" Padding="20,5"/>
            <Button Content="キャンセル" Command="{Binding CancelCommand}" Padding="20,5"/>
        </StackPanel>
    </StackPanel>
</UserControl>
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
│   ├── UserInfoDialog/
│   │   ├── UserInfoDialogInput.cs
│   │   ├── UserInfoDialogOutput.cs
│   │   ├── UserInfoDialogViewModel.cs
│   │   ├── UserInfoDialogView.xaml
│   │   └── UserInfoDialogView.xaml.cs
│   ├── ConfirmDialog/
│   │   ├── ConfirmDialogInput.cs
│   │   ├── ConfirmDialogOutput.cs
│   │   ├── ConfirmDialogViewModel.cs
│   │   ├── ConfirmDialogView.xaml
│   │   └── ConfirmDialogView.xaml.cs
│   ├── CheckboxConfirmDialog/
│   │   ├── CheckboxConfirmDialogInput.cs
│   │   ├── CheckboxConfirmDialogOutput.cs
│   │   ├── CheckboxConfirmDialogViewModel.cs
│   │   ├── CheckboxConfirmDialogView.xaml
│   │   └── CheckboxConfirmDialogView.xaml.cs
│   └── ProgressDialog/
│       ├── ProgressDialogInput.cs
│       ├── ProgressDialogOutput.cs
│       ├── ProgressDialogViewModel.cs
│       ├── ProgressDialogView.xaml
│       └── ProgressDialogView.xaml.cs
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── MainWindowViewModel.cs
└── App.xaml.cs
```

## API リファレンス

### IDialogService メソッド

#### ShowDialogAsync<TViewModel, TInput, TOutput>(TInput input)
通常のダイアログを表示し、結果を待機します。

#### ShowDialogAsync<TViewModel, TOutput>()
入力パラメータなしでダイアログを表示します。

#### ShowReactiveDialogAsync<TViewModel, TInput, TOutput>(TInput input)
リアクティブダイアログを表示し、ViewModelの参照と結果タスクを返します。リアルタイム更新が必要な場合に使用します。

#### ShowReactiveDialogAsync<TViewModel, TOutput>()
入力パラメータなしでリアクティブダイアログを表示します。

#### RegisterDialog<TView, TViewModel>()
ダイアログのViewとViewModelの組み合わせを登録します。

#### RegisterViewModelFactory<TViewModel>(Func<TViewModel> factory)
ViewModelのファクトリメソッドを登録します。DIが必要な場合に使用します。

## 高度な機能

### 複数ダイアログのスタック管理

このダイアログサービスは複数のダイアログを同時に表示できます：

```csharp
// 最初のダイアログを表示
var result1 = _dialogService.ShowDialogAsync<UserInfoDialogViewModel, UserInfoDialogInput, UserInfoDialogOutput>(input1);

// 最初のダイアログの上に2番目のダイアログを表示
var result2 = _dialogService.ShowDialogAsync<ConfirmDialogViewModel, ConfirmDialogInput, ConfirmDialogOutput>(input2);

// 後から開いたダイアログが前面に表示される
```

### ネストしたダイアログ

ダイアログ内から別のダイアログを呼び出すことができます：

```csharp
private async Task ExecuteOk()
{
    // 確認ダイアログを表示
    var confirmResult = await _dialogService.ShowDialogAsync<ConfirmDialogViewModel, ConfirmDialogInput, ConfirmDialogOutput>(
        new ConfirmDialogInput("確認", "この操作を実行しますか？"));

    if (confirmResult.IsConfirmed)
    {
        // メインダイアログを閉じる
        _taskCompletionSource?.SetResult(new UserInfoDialogOutput(UserInfoDialogResult.Ok, Name.Value, Email.Value));
    }
}
```

## 注意事項

### 必須実装

- ダイアログのViewModelは`IDialogContentViewModel<TInput, TOutput>`を実装する必要があります
- `Initialize`メソッドで受け取った`TaskCompletionSource`を使用してダイアログを閉じます
- ViewModelファクトリを使用する場合は`RegisterViewModelFactory`メソッドを使用してください

### ReactiveProperty.WPF使用時の注意点

- XAMLでのバインディングは `{Binding PropertyName.Value}` を使用します
- `CompositeDisposable`を使用してリソースの適切な解放を行います
- ViewModelは`IDisposable`を実装し、`Dispose`メソッドで`CompositeDisposable.Dispose()`を呼び出します

### ダイアログの閉じ方

ダイアログは以下の方法で閉じることができます：

1. **通常の閉じ方**: `TaskCompletionSource.SetResult()`を呼び出す
2. **キャンセル**: `TaskCompletionSource.SetCanceled()`を呼び出す
3. **例外**: `TaskCompletionSource.SetException()`を呼び出す

### パフォーマンス考慮事項

- 大量のダイアログを短時間で開く場合は、適切なリソース管理を行ってください
- 長時間表示されるダイアログでは、メモリリークを避けるため適切な`Dispose`処理を実装してください

## ライセンス

このプロジェクトはサンプル実装です。自由にご利用ください。

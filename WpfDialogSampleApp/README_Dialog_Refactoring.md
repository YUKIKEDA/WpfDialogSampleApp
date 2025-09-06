# WPF ダイアログ システム - Microsoft.Xaml.Behaviors設計版

このプロジェクトでは、**Microsoft.Xaml.Behaviors**を活用したエンタープライズレベルの再利用可能なダイアログシステムを構築しました。

## 🎯 設計哲学：「XAMLビヘイビアによる宣言的設計」

### なぜMicrosoft.Xaml.Behaviorsなのか？
1. **宣言的設計**: XAMLで直接ビヘイビアを宣言
2. **コードビハインド最小化**: ロジックをビヘイビアに分離
3. **再利用性の向上**: 任意のWindowに簡単にアタッチ可能
4. **Blendサポート**: デザイナーツールでの操作が可能
5. **標準ライブラリ**: Microsoftが提供する公式ライブラリ

## 🚀 主要な改善点

### 1. XAMLビヘイビアベースの設計
- `Behavior<Window>` を継承した `DialogBehavior`
- XAMLから直接アタッチ可能
- コードビハインドの大幅削減

### 2. シンプルなサービス設計
- 軽量でシンプルなサービス管理
- 過度な複雑性を避けた実用的な設計
- テスタビリティとシンプルさのバランス

### 3. 型安全なダイアログファクトリー
- ジェネリックベースの型安全なダイアログ作成
- コンパイル時の型チェック
- ViewModelの自動インスタンス化

### 4. 非同期処理とイベントベース設計
- PropertyChangedイベントの監視から脱却
- `IDialogHandle` による適切なライフサイクル管理
- 非同期ダイアログ処理のサポート

### 5. 重複制御とリソース管理
- 同一ViewModelでの重複ダイアログ表示防止
- 自動的なリソースクリーンアップ
- アプリケーション終了時の全ダイアログ閉じ

## 追加されたファイル

### 1. Behaviors/DialogBehavior.cs
- `Behavior<Window>` を継承したXAMLビヘイビア
- 親ウィンドウの位置追従機能
- 共通のダイアログ設定
- イベントハンドラーの自動管理
- OnAttached/OnDetachingによる適切なライフサイクル管理

### 2. Services/IDialogService.cs
- ダイアログ表示を抽象化するインターフェース
- モーダル/非モーダル表示の統一
- メッセージボックス機能の統一

### 3. Services/DialogService.cs
- IDialogServiceの実装
- ViewModelとViewの動的マッピング
- ダイアログのライフサイクル管理
- IDialogAwareインターフェースのサポート

### 4. ViewModels/DialogViewModelBase.cs
- ダイアログ用ViewModelの基底クラス
- IDialogAwareの実装
- 共通のダイアログ操作メソッド



## 修正されたファイル

### 1. Views/UserInfoDialog.xaml
- 通常の `<Window>` として実装
- XAMLで `<i:Interaction.Behaviors>` を使用してビヘイビアをアタッチ
- 宣言的なダイアログ機能の追加

### 2. Views/UserInfoDialog.xaml.cs
- `Window` クラスから継承
- 最小限のコードビハインド
- XAMLビヘイビアへの参照のみ

### 3. WpfDialogSampleApp.csproj
- `Microsoft.Xaml.Behaviors.Wpf` パッケージの追加

### 4. ViewModels/UserInfoDialogViewModel.cs
- DialogViewModelBaseから継承
- Window操作コードを削除
- IDialogAwareの活用

### 5. ViewModels/MainWindowViewModel.cs
- DialogServiceの使用
- 直接的なWindow操作を削除
- より疎結合な設計

## 改善点

### 1. 再利用性
- 新しいダイアログは`DialogBase`を継承するだけで共通機能を取得
- ViewModelは`DialogViewModelBase`から継承して簡潔に

### 2. テスタビリティ
- ViewModelからWindow依存を完全に分離
- IDialogServiceをモックして単体テスト可能

### 3. 保守性
- ダイアログ共通ロジックの一元化
- コードビハインドの最小化

### 4. 拡張性
- 新しいダイアログタイプの簡単な追加
- カスタムダイアログサービスの実装可能

## 💡 改善された使用方法

### 1. XAMLビヘイビアによるダイアログ作成
```xml
<Window x:Class="MyApp.MyCustomDialog"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:i="http://schemas.microsoft.com/xaml/behaviors"
        xmlns:behaviors="clr-namespace:MyApp.Behaviors">
    
    <i:Interaction.Behaviors>
        <behaviors:DialogBehavior />
    </i:Interaction.Behaviors>
    
    <!-- ダイアログのコンテンツ -->
</Window>
```

```csharp
public partial class MyCustomDialog : Window
{
    public MyCustomDialog()
    {
        InitializeComponent();
        // ビヘイビアは自動的にアタッチされる
    }
}
```

### 2. 型安全なダイアログ表示
```csharp
// 型安全な非モーダルダイアログ
var handle = dialogService.Show<UserInfoDialogViewModel>();
handle.Closed += (s, e) => {
    if (e.Result == true && e.ViewModel is UserInfoDialogViewModel vm) {
        // 結果の処理
    }
};

// 初期設定付きダイアログ
var viewModel = new UserInfoDialogViewModel();
viewModel.UserInfo.Name = "初期値";
var result = dialogService.ShowModal(viewModel);
```

### 3. 非同期ダイアログ処理
```csharp
// 非同期モーダルダイアログ
var result = await dialogService.ShowModalAsync<UserInfoDialogViewModel>();
if (result == true) {
    // 保存成功の処理
}
```

### 4. テストでの使用
```csharp
// テスト用のシンプルなモック実装
var mockDialogService = Mock.Of<IDialogService>();

// ViewModelのテスト
var viewModel = new MainWindowViewModel(mockDialogService);
// テスト実行...
```

### 5. シンプルな設定
```csharp
// App.xaml.cs での設定
var dialogService = new DialogService();
dialogService.RegisterDialog<UserInfoDialogViewModel, UserInfoDialog>();

var mainViewModel = new MainWindowViewModel(dialogService);
```

## 🏗️ アーキテクチャ改善

### 問題だった点
1. **継承の強制**: DialogBaseクラスによる設計の制約
2. **コードビハインドの肥大化**: UIロジックとビジネスロジックの混在
3. **強い結合**: ViewModelがWindowを直接操作
4. **コールバック地獄**: PropertyChangedイベントの複雑な監視
5. **重複制御なし**: 同じダイアログの重複表示
6. **テスト困難**: モックが困難な設計
7. **リソースリーク**: イベントハンドラーの解除忘れ

### 解決策
1. **XAMLビヘイビア**: Microsoft.Xaml.Behaviorsによる宣言的設計
2. **コードビハインド最小化**: ロジックをビヘイビアに分離
3. **IDialogService**: 抽象化による疎結合
4. **IDialogHandle**: イベントベースのライフサイクル管理
5. **重複制御**: 同一ViewModelでの自動制御
6. **テスタビリティ**: シンプルなモック注入
7. **自動クリーンアップ**: Behavior<T>での適切なリソース管理

## ✨ **「XAMLビヘイビアによる宣言的設計」の勝利**

このリファクタリングにより、WPFアプリケーションでのダイアログ管理が：
- **宣言的**: XAMLで直接ビヘイビアを定義
- **再利用可能**: 任意のWindowに簡単にアタッチ
- **保守しやすい**: ロジックとUIの明確な分離
- **テスタブル**: Microsoft標準のビヘイビアフレームワーク活用

このアプローチは、MVVMパターンとの親和性が高く、企業レベルの開発に最適な設計となりました。

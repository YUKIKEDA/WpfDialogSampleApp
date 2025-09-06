# WPF ダイアログ システム - 改善版

このプロジェクトでは、エンタープライズレベルの再利用可能なダイアログシステムを構築し、設計上の問題を解決しました。

## 🚀 主要な改善点

### 1. 依存性注入（DI）の導入
- `ServiceContainer` クラスによる軽量DIコンテナー
- サービスの自動解決とライフサイクル管理
- テスタビリティの大幅向上

### 2. 型安全なダイアログファクトリー
- ジェネリックベースの型安全なダイアログ作成
- コンパイル時の型チェック
- ViewModelの自動インスタンス化

### 3. 非同期処理とイベントベース設計
- PropertyChangedイベントの監視から脱却
- `IDialogHandle` による適切なライフサイクル管理
- 非同期ダイアログ処理のサポート

### 4. 重複制御とリソース管理
- 同一ViewModelでの重複ダイアログ表示防止
- 自動的なリソースクリーンアップ
- アプリケーション終了時の全ダイアログ閉じ

## 追加されたファイル

### 1. Views/DialogBase.cs
- 再利用可能なダイアログの基底クラス
- 親ウィンドウの位置追従機能
- 共通のダイアログ設定
- イベントハンドラーの自動管理

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
- `<Window>` から `<local:DialogBase>` に変更
- ローカル名前空間の追加

### 2. Views/UserInfoDialog.xaml.cs
- 複雑なイベントハンドリングコードを削除
- DialogBaseから継承してクリーンなコードビハインド

### 3. ViewModels/UserInfoDialogViewModel.cs
- DialogViewModelBaseから継承
- Window操作コードを削除
- IDialogAwareの活用

### 4. ViewModels/MainWindowViewModel.cs
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

### 1. 型安全なダイアログ表示
```csharp
// 型安全な非モーダルダイアログ
var handle = dialogService.Show<UserInfoDialogViewModel>();
handle.Closed += (s, e) => {
    if (e.Result == true && e.ViewModel is UserInfoDialogViewModel vm) {
        // 結果の処理
    }
};

// 初期設定付きダイアログ
var result = dialogService.ShowModal<UserInfoDialogViewModel>(vm => {
    vm.UserInfo.Name = "初期値";
});
```

### 2. 非同期ダイアログ処理
```csharp
// 非同期モーダルダイアログ
var result = await dialogService.ShowModalAsync<UserInfoDialogViewModel>();
if (result == true) {
    // 保存成功の処理
}
```

### 3. テストでの使用
```csharp
// テスト用モックサービス
var mockService = new MockDialogService();
mockService.MockModalResult = true;

// ViewModelのテスト
var viewModel = new MainWindowViewModel(mockService);
viewModel.ShowUserInfoDialogCommand.Execute(null);

// 呼び出し確認
Assert.Contains("ShowModal<UserInfoDialogViewModel>", mockService.CallHistory);
```

### 4. DIコンテナーでの登録
```csharp
// App.xaml.cs での設定
container.RegisterSingleton<IDialogService, DialogService>();
container.RegisterTransient<UserInfoDialogViewModel>();

// サービスでダイアログ登録
dialogService.RegisterDialog<UserInfoDialogViewModel, UserInfoDialog>();
```

## 🏗️ アーキテクチャ改善

### 問題だった点
1. **強い結合**: ViewModelがWindowを直接操作
2. **コールバック地獄**: PropertyChangedイベントの複雑な監視
3. **重複制御なし**: 同じダイアログの重複表示
4. **テスト困難**: モックが困難な設計
5. **リソースリーク**: イベントハンドラーの解除忘れ

### 解決策
1. **IDialogService**: 抽象化による疎結合
2. **IDialogHandle**: イベントベースのライフサイクル管理
3. **重複制御**: 同一ViewModelでの自動制御
4. **MockDialogService**: テスト専用実装
5. **自動クリーンアップ**: DialogHandleでの自動リソース解放

このリファクタリングにより、WPFアプリケーションでのダイアログ管理が企業レベルの品質に向上しました。

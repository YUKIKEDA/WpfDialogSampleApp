# WPF ダイアログ リファクタリング概要

このプロジェクトでは、再利用可能なダイアログシステムを構築し、コードビハインドからビジネスロジックを分離しました。

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

## 使用方法

### 新しいダイアログの作成

1. `DialogBase`を継承したXAMLファイル
2. `DialogViewModelBase`を継承したViewModelクラス
3. `DialogService`にマッピングを登録

```csharp
// サービスへの登録
dialogService.RegisterDialog<MyDialogViewModel, MyDialog>();

// 使用
var viewModel = new MyDialogViewModel();
dialogService.Show(viewModel); // 非モーダル
var result = dialogService.ShowModal(viewModel); // モーダル
```

このリファクタリングにより、WPFアプリケーションでのダイアログ管理がより効率的で保守しやすくなりました。

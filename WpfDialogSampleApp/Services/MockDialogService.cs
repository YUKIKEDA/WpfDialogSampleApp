using WpfDialogSampleApp.Views;

namespace WpfDialogSampleApp.Services
{
    /// <summary>
    /// テスト用のモックダイアログサービス
    /// </summary>
    public class MockDialogService : IDialogService
    {
        public bool? MockModalResult { get; set; } = true;
        public MessageBoxResult MockMessageBoxResult { get; set; } = MessageBoxResult.OK;
        public bool MockConfirmationResult { get; set; } = true;

        // 呼び出し履歴の記録
        public List<string> CallHistory { get; } = new();
        public Dictionary<string, object> LastDialogViewModels { get; } = new();

        public void RegisterDialog<TViewModel, TView>()
            where TViewModel : class
            where TView : DialogBase
        {
            CallHistory.Add($"RegisterDialog<{typeof(TViewModel).Name}, {typeof(TView).Name}>");
        }

        public bool? ShowModal<TViewModel>(TViewModel viewModel) where TViewModel : class
        {
            CallHistory.Add($"ShowModal<{typeof(TViewModel).Name}>");
            LastDialogViewModels[typeof(TViewModel).Name] = viewModel;
            return MockModalResult;
        }

        public IDialogHandle Show<TViewModel>(TViewModel viewModel) where TViewModel : class
        {
            CallHistory.Add($"Show<{typeof(TViewModel).Name}>");
            LastDialogViewModels[typeof(TViewModel).Name] = viewModel;
            return new MockDialogHandle(viewModel, MockModalResult);
        }

        public async Task<bool?> ShowModalAsync<TViewModel>(TViewModel viewModel) where TViewModel : class
        {
            CallHistory.Add($"ShowModalAsync<{typeof(TViewModel).Name}>");
            LastDialogViewModels[typeof(TViewModel).Name] = viewModel;
            return await Task.FromResult(MockModalResult);
        }

        public bool? ShowModal<TViewModel>(Action<TViewModel>? configure = null) where TViewModel : class, new()
        {
            var viewModel = new TViewModel();
            configure?.Invoke(viewModel);
            return ShowModal(viewModel);
        }

        public IDialogHandle Show<TViewModel>(Action<TViewModel>? configure = null) where TViewModel : class, new()
        {
            var viewModel = new TViewModel();
            configure?.Invoke(viewModel);
            return Show(viewModel);
        }

        public MessageBoxResult ShowMessageBox(string message, string title = "メッセージ", MessageBoxType messageType = MessageBoxType.Information)
        {
            CallHistory.Add($"ShowMessageBox: {message}");
            return MockMessageBoxResult;
        }

        public bool ShowConfirmation(string message, string title = "確認")
        {
            CallHistory.Add($"ShowConfirmation: {message}");
            return MockConfirmationResult;
        }

        public void CloseAllDialogs()
        {
            CallHistory.Add("CloseAllDialogs");
        }
    }

    /// <summary>
    /// テスト用のモックダイアログハンドル
    /// </summary>
    public class MockDialogHandle : IDialogHandle
    {
        public object ViewModel { get; }
        public bool IsActive { get; private set; } = true;

        public event EventHandler<DialogClosedEventArgs>? Closed;

        private readonly bool? _mockResult;

        public MockDialogHandle(object viewModel, bool? mockResult)
        {
            ViewModel = viewModel;
            _mockResult = mockResult;
        }

        public void Close(bool? result = null)
        {
            if (IsActive)
            {
                IsActive = false;
                Closed?.Invoke(this, new DialogClosedEventArgs(result ?? _mockResult, ViewModel));
            }
        }

        /// <summary>
        /// テスト用：ダイアログが閉じられたことをシミュレート
        /// </summary>
        public void SimulateClose(bool? result = null)
        {
            Close(result);
        }
    }
}

using System.Windows;
using WpfDialogSampleApp.Core.Dialogs.Interfaces;
using WpfDialogSampleApp.Core.Dialogs.ViewModels;

namespace WpfDialogSampleApp.Core.Dialogs.Services
{
    public class DialogService : IDialogService
    {
        private readonly DialogViewModel _dialogViewModel;
        private readonly Dictionary<Type, Type> _dialogMappings = [];
        private readonly Dictionary<Type, Func<object>> _viewModelFactories = [];

        public DialogService(DialogViewModel dialogViewModel)
        {
            _dialogViewModel = dialogViewModel;
        }

        public void RegisterDialog<TView, TViewModel>()
            where TView : FrameworkElement, new()
            where TViewModel : class
        {
            _dialogMappings[typeof(TViewModel)] = typeof(TView);
        }

        public void RegisterViewModelFactory<TViewModel>(Func<TViewModel> factory)
            where TViewModel : class
        {
            _viewModelFactories[typeof(TViewModel)] = () => factory();
        }

        public async Task<TOutput> ShowDialogAsync<TViewModel, TInput, TOutput>(TInput input)
            where TViewModel : class, IDialogContentViewModel<TInput, TOutput>
            where TInput : IDialogContentInput
            where TOutput : IDialogContentOutput
        {
            var viewModel = CreateViewModel<TViewModel>();
            return await _dialogViewModel.ShowAsync(viewModel, input);
        }

        public async Task<TOutput> ShowDialogAsync<TViewModel, TOutput>()
            where TViewModel : class, IDialogContentViewModel<EmptyDialogContentInput, TOutput>
            where TOutput : IDialogContentOutput
        {
            var viewModel = CreateViewModel<TViewModel>();
            return await _dialogViewModel.ShowAsync(viewModel, new EmptyDialogContentInput());
        }

        public (TViewModel viewModel, Task<TOutput> dialogTask) ShowProgressDialogAsync<TViewModel, TInput, TOutput>(TInput input)
            where TViewModel : class, IDialogContentViewModel<TInput, TOutput>
            where TInput : IDialogContentInput
            where TOutput : IDialogContentOutput
        {
            var viewModel = CreateViewModel<TViewModel>();
            var dialogTask = _dialogViewModel.ShowAsync(viewModel, input);
            return (viewModel, dialogTask);
        }

        private TViewModel CreateViewModel<TViewModel>() where TViewModel : class
        {
            if (_viewModelFactories.TryGetValue(typeof(TViewModel), out var factory))
            {
                return (TViewModel)factory();
            }

            // デフォルトコンストラクタでインスタンス作成を試行
            try
            {
                return Activator.CreateInstance<TViewModel>();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"ViewModelの作成に失敗しました: {typeof(TViewModel).Name}. " +
                    $"RegisterViewModelFactory<{typeof(TViewModel).Name}>()でファクトリを登録するか、" +
                    $"デフォルトコンストラクタを提供してください。", ex);
            }
        }
    }
}

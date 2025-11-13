using System.Windows;
using WpfDialogSampleApp.Core.Dialogs.Interfaces;

namespace WpfDialogSampleApp.Core.Dialogs.Services
{
    public interface IDialogService
    {
        /// <summary>
        /// ダイアログのViewとViewModelの組み合わせを登録します
        /// </summary>
        /// <typeparam name="TView">ダイアログのViewの型</typeparam>
        /// <typeparam name="TViewModel">ダイアログのViewModelの型</typeparam>
        void RegisterDialog<TView, TViewModel>()
            where TView : FrameworkElement, new()
            where TViewModel : class;

        /// <summary>
        /// ViewModelのファクトリメソッドを登録します
        /// </summary>
        /// <typeparam name="TViewModel">ViewModelの型</typeparam>
        /// <param name="factory">ViewModelを作成するファクトリメソッド</param>
        void RegisterViewModelFactory<TViewModel>(Func<TViewModel> factory)
            where TViewModel : class;

        /// <summary>
        /// 指定されたViewModelでダイアログを表示します
        /// </summary>
        /// <typeparam name="TViewModel">ダイアログのViewModelの型</typeparam>
        /// <typeparam name="TInput">入力パラメータの型</typeparam>
        /// <typeparam name="TOutput">出力結果の型</typeparam>
        /// <param name="input">ダイアログに渡す入力パラメータ</param>
        /// <returns>ダイアログの実行結果</returns>
        Task<TOutput> ShowDialogAsync<TViewModel, TInput, TOutput>(TInput input)
            where TViewModel : class, IDialogContentViewModel<TInput, TOutput>
            where TInput : IDialogContentInput
            where TOutput : IDialogContentOutput;

        /// <summary>
        /// 指定されたViewModelでダイアログを表示します（入力パラメータなし）
        /// </summary>
        /// <typeparam name="TViewModel">ダイアログのViewModelの型</typeparam>
        /// <typeparam name="TOutput">出力結果の型</typeparam>
        /// <returns>ダイアログの実行結果</returns>
        Task<TOutput> ShowDialogAsync<TViewModel, TOutput>()
            where TViewModel : class, IDialogContentViewModel<EmptyDialogContentInput, TOutput>
            where TOutput : IDialogContentOutput;

        /// <summary>
        /// 進捗ダイアログを表示し、ViewModelの参照を返します
        /// </summary>
        /// <typeparam name="TViewModel">ダイアログのViewModelの型</typeparam>
        /// <typeparam name="TInput">入力パラメータの型</typeparam>
        /// <typeparam name="TOutput">出力結果の型</typeparam>
        /// <param name="input">ダイアログに渡す入力パラメータ</param>
        /// <returns>ViewModelの参照とダイアログの実行結果のタスク</returns>
        (TViewModel viewModel, Task<TOutput> dialogTask) ShowProgressDialogAsync<TViewModel, TInput, TOutput>(TInput input)
            where TViewModel : class, IDialogContentViewModel<TInput, TOutput>
            where TInput : IDialogContentInput
            where TOutput : IDialogContentOutput;
    }

    public class EmptyDialogContentInput : IDialogContentInput
    {
    }
}

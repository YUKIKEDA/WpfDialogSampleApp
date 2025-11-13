using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WpfDialogSampleApp.Core.Dialogs.Interfaces;

namespace WpfDialogSampleApp.Core.Dialogs.ViewModels
{
    public class DialogInfo
    {
        public object ContentViewModel { get; set; } = null!;
        public TaskCompletionSource<object> TaskCompletionSource { get; set; } = null!;
        public int ZIndex { get; set; }
    }

    public class DialogViewModel : INotifyPropertyChanged
    {
        private readonly object _lockObject = new();
        private int _nextZIndex = 1000;

        public ObservableCollection<DialogInfo> DialogStack { get; } = new();

        public bool IsVisible => DialogStack.Count > 0;

        public async Task<TOutput> ShowAsync<TInput, TOutput>(IDialogContentViewModel<TInput, TOutput> dialogContentViewModel, TInput input) 
            where TInput : IDialogContentInput 
            where TOutput : IDialogContentOutput
        {
            var taskCompletionSource = new TaskCompletionSource<TOutput>();
            
            // ダイアログをスタックに追加
            var dialogInfo = new DialogInfo
            {
                ContentViewModel = dialogContentViewModel,
                TaskCompletionSource = new TaskCompletionSource<object>(),
                ZIndex = GetNextZIndex()
            };

            lock (_lockObject)
            {
                DialogStack.Add(dialogInfo);
            }

            OnPropertyChanged(nameof(IsVisible));

            // ダイアログの初期化
            dialogContentViewModel.Initialize(input, taskCompletionSource);

            try
            {
                // ダイアログの結果を待機
                var result = await taskCompletionSource.Task.WaitAsync(CancellationToken.None);

                return result;
            }
            finally
            {
                // ダイアログをスタックから削除
                lock (_lockObject)
                {
                    DialogStack.Remove(dialogInfo);
                }
                OnPropertyChanged(nameof(IsVisible));
            }
        }

        private int GetNextZIndex()
        {
            return Interlocked.Increment(ref _nextZIndex);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
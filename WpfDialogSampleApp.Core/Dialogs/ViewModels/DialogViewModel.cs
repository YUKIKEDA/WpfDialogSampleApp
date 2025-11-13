using System.ComponentModel;
using System.Runtime.CompilerServices;
using WpfDialogSampleApp.Core.Dialogs.Interfaces;

namespace WpfDialogSampleApp.Core.Dialogs.ViewModels
{
    public class DialogViewModel : INotifyPropertyChanged
    {
        private object? _dialogContentViewModel;
        private bool _isVisible = false;

        public object? DialogContentViewModel
        {
            get => _dialogContentViewModel;
            set
            {
                _dialogContentViewModel = value;
                OnPropertyChanged();
            }
        }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        public async Task<TOutput> ShowAsync<TInput, TOutput>(IDialogContentViewModel<TInput, TOutput> dialogContentViewModel, TInput input) 
            where TInput : IDialogContentInput 
            where TOutput : IDialogContentOutput
        {
            IsVisible = true;
            DialogContentViewModel = dialogContentViewModel;

            var taskCompletionSource = new TaskCompletionSource<TOutput>();
            
            dialogContentViewModel.Initialize(input, taskCompletionSource);
            
            var result = await taskCompletionSource.Task.WaitAsync(CancellationToken.None);

            IsVisible = false;

            return result;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
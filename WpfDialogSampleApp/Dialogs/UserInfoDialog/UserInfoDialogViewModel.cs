using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WpfDialogSampleApp.Core.Dialogs.Interfaces;

namespace WpfDialogSampleApp.Dialogs.UserInfoDialog
{
    public class UserInfoDialogViewModel : INotifyPropertyChanged, IDialogContentViewModel<UserInfoDialogInput, UserInfoDialogOutput>
    {
        private TaskCompletionSource<UserInfoDialogOutput>? _taskCompletionSource;
        private string _name = string.Empty;
        private string _email = string.Empty;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
                ((RelayCommand)OkCommand).RaiseCanExecuteChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
                ((RelayCommand)OkCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public UserInfoDialogViewModel()
        {
            OkCommand = new RelayCommand(ExecuteOk, CanExecuteOk);
            CancelCommand = new RelayCommand(ExecuteCancel);
        }

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

        private bool CanExecuteOk()
        {
            return !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Email);
        }

        private void ExecuteCancel()
        {
            _taskCompletionSource?.SetResult(new UserInfoDialogOutput(UserInfoDialogResult.Cancel));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public void Execute(object? parameter)
        {
            _execute();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

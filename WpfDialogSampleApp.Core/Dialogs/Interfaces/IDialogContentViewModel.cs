namespace WpfDialogSampleApp.Core.Dialogs.Interfaces
{
    internal interface IDialogContentViewModel<TInput, TOutput> 
        where TInput : IDialogContentInput 
        where TOutput : IDialogContentOutput
    {
        void Initialize(TInput parameters, TaskCompletionSource<TOutput> taskCompletionSource);
    }
}


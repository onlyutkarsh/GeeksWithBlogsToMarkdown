using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GeeksWithBlogsToMarkdown.Commands.Base
{
    public class AsyncDelegateCommand : AsyncDelegateCommand<object>, IAsyncCommand
    {
        public AsyncDelegateCommand(Func<Task> executeMethod)
            : base(o => executeMethod())
        {
        }

        public AsyncDelegateCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod)
            : base(o => executeMethod(), o => canExecuteMethod())
        {
        }
    }

    public class AsyncDelegateCommand<T> : IAsyncCommand<T>, ICommand
    {
        private readonly Func<T, Task> _executeMethod;
        private readonly DelegateCommand<T> _underlyingCommand;
        private bool _isExecuting;

        public event EventHandler CanExecuteChanged
        {
            add { _underlyingCommand.CanExecuteChanged += value; }
            remove { _underlyingCommand.CanExecuteChanged -= value; }
        }

        public ICommand Command { get { return this; } }

        public AsyncDelegateCommand(Func<T, Task> executeMethod)
                            : this(executeMethod, _ => true)
        {
        }

        public AsyncDelegateCommand(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod)
        {
            _executeMethod = executeMethod;
            _underlyingCommand = new DelegateCommand<T>(x => { }, canExecuteMethod);
        }

        public bool CanExecute(object parameter)
        {
            return !_isExecuting && _underlyingCommand.CanExecute((T)parameter);
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync((T)parameter);
        }

        public async Task ExecuteAsync(T obj)
        {
            try
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();
                await _executeMethod(obj);
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            _underlyingCommand.RaiseCanExecuteChanged();
        }
    }
}
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Projektledningsverktyg.Commands
{
    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<object, Task> _execute;
        private bool _isExecuting;

        public AsyncRelayCommand(Func<object, Task> execute)
        {
            _execute = execute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => !_isExecuting;

        public async void Execute(object parameter)
        {
            if (_isExecuting)
                return;
            _isExecuting = true;
            await _execute(parameter);
            _isExecuting = false;
        }
    }
}

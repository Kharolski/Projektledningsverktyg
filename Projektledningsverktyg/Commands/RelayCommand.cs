using System;
using System.Windows.Input;

namespace Projektledningsverktyg.Commands
{
    // This class implements ICommand interface which WPF uses to handle user interactions
    public class RelayCommand : ICommand
    {
        // Stores the action (method) that will be executed when command is triggered
        private readonly Action _execute;

        // Constructor
        public RelayCommand(Action execute)
        {
            _execute = execute;
        }

        // Required by ICommand interface - handles when command can be executed
        public event EventHandler CanExecuteChanged;

        // Determines if command can be executed - returns true means always executable
        public bool CanExecute(object parameter) => true;

        // This method is called when command is triggered (e.g., button click)
        public void Execute(object parameter)
        {
            _execute();
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
    }
}

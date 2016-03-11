using System;
using System.Windows.Input;

namespace UserInterface.Commands
{
    public class RemoveConditionCommand : ICommand
    {
        private Action<object> _execute;
        private Func<object, bool> _canExecute;

        public RemoveConditionCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            if (canExecute == null)
            {
                // no can execute provided, then always executable
                canExecute = o => true;
            }
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute((object[])parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}
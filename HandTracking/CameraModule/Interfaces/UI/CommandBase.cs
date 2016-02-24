using System;
using System.Windows.Input;

namespace CameraModule.Interfaces.UI
{
    public abstract class CommandBase : ICommand
    {
        /// <summary>
        ///     Method that determines whether the command can execute.
        /// </summary>
        /// <param name="parameter">Data to be passed to the command, otherwise null</param>
        /// <returns>true if command can execute, false otherwise</returns>
        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Defines method to be called when command is invoked.
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }
            OnExecute(parameter);
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute. This affects buttons, menu items, etc. 
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        ///     Executes the command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected abstract void OnExecute(object parameter);
    }
}
using System;
using System.Windows.Input;

namespace TreeBreadcrumbControl.Commands
{
    public abstract class RelayCommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        void ICommand.Execute(object parameter) => Execute(parameter);

        bool ICommand.CanExecute(object parameter) => CanExecute(parameter);

        protected abstract void Execute(object parameter);

        protected abstract bool CanExecute(object parameter);
    }
}

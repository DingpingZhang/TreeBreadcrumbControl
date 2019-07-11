using System;

namespace TreeBreadcrumbControl.Commands
{
    public class RelayCommand<T> : RelayCommandBase
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        protected override bool CanExecute(object parameter) => parameter != null && parameter is T value && CanExecute(value);

        protected override void Execute(object parameter)
        {
            if (parameter is T value)
            {
                Execute(value);
            }
        }

        public bool CanExecute(T parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(T parameter) => _execute?.Invoke(parameter);
    }
}

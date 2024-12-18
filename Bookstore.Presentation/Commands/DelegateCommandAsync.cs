using System.Windows.Input;

namespace Bookstore.Presentation
{
    public class DelegateCommandAsync : ICommand
    {
        private readonly Func<object, Task>? execute;
        private readonly Func<object?, bool>? canExecute;

        public event EventHandler? CanExecuteChanged;
        public DelegateCommandAsync(Func<object, Task> execute, Func<object?, bool>? canExecute = null)
        {
            ArgumentNullException.ThrowIfNull(execute);
            this.execute = execute;
            this.canExecute = canExecute;
        }
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        public bool CanExecute(object? parameter) => canExecute is null ? true : canExecute(parameter);

        public void Execute(object? parameter) => execute!(parameter!);

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SteelHall.MAUI
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        /// <summary>
        /// DelegateCommand Constructor
        /// </summary>
        /// <param name="canExecute">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <param name="execute">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public DelegateCommand(Predicate<object?>? canExecute, Action<object?> execute) => (_canExecute, _execute) = (canExecute, execute);

        /// <summary>
        /// DelegateCommand Constructor
        /// </summary>
        /// <param name="execute">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public DelegateCommand(Action<object?> execute) : this(null, execute)
        {
        }

        /// <summary>
        /// This method can be used to raise the CanExecuteChanged handler.
        /// This will force WPF to re-query the status of this command directly.
        /// </summary>
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object? parameter) => _execute.Invoke(parameter);

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler? CanExecuteChanged;
    }
}

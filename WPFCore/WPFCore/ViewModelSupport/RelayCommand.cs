using System;
using System.Diagnostics;
using System.Windows.Input;

namespace WPFCore.ViewModelSupport
{
    /// <summary>
    /// RelayCommand allows you to inject the command's logic via delegates passed into its constructor. 
    /// This approach allows for terse, concise command implementation in ViewModel classes.
    /// </summary>
    /// <remarks>
    /// Note: This class ignores CommandParameters passed into <see cref="Execute"/> and <see cref="CanExecute"/>.
    /// Taken from and slightly modified:
    /// http://msdn.microsoft.com/en-us/magazine/dd419663.aspx#id0090030
    /// </remarks>
    public sealed class RelayCommand : ICommand
    {
        private readonly Func<bool> canExecute;
        private readonly Action execute;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        public RelayCommand(Action execute):this(execute, null)
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="execute">The action to execute</param>
        /// <param name="canExecute">A function that checks whether the command may execute</param>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentException("execute");

            this.canExecute = canExecute;
            this.execute = execute;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (this.canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (this.canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns><c>True</c> if the command may be executed, <c>False</c> otherwise</returns>
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute();
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter"></param>
        [DebuggerStepThrough]
        public void Execute(object parameter)
        {
            this.execute();
        }
    }

    /// <summary>
    /// RelayCommand&lt;T&gt; allows you to inject the command's logic via delegates passed into its constructor. 
    /// This approach allows for terse, concise command implementation in ViewModel classes.
    /// The type &lt;T&gt; defines the type of the CommandParameter.
    /// </summary>
    /// <remarks>
    /// Taken from and slightly modified:
    /// http://msdn.microsoft.com/en-us/magazine/dd419663.aspx#id0090030
    /// </remarks>
    public sealed class RelayCommand<T> : ICommand
    {
        private readonly Predicate<T> canExecute;
        private readonly Action<T> execute;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="execute">The action to execute</param>
        /// <param name="canExecute">A function that checks whether the command may execute</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
                throw new ArgumentException("execute");

            this.canExecute = canExecute;
            this.execute = execute;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (this.canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (this.canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// Will always return <c>true</c> if <paramref name="parameter"/> is <c>null</c>.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns><c>True</c> if the command may be executed, <c>False</c> otherwise</returns>
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            if(parameter is T)
                return this.canExecute == null || this.canExecute((T) parameter);

            return true;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter"></param>
        [DebuggerStepThrough]
        public void Execute(object parameter)
        {
            this.execute((T) parameter);
        }
    }
}

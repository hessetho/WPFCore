using System;

namespace WPFCore.XAML.Behaviors.Internal
{
    /// <summary>
    /// Defines the interface for a strategy of execution for the CommandBehaviorBinding
    /// </summary>
    internal interface IExecutionStrategy
    {
        /// <summary>
        /// Gets or sets the Behavior that we execute this strategy
        /// </summary>
        CommandBehaviorBinding Behavior { get; set; }

        /// <summary>
        /// Executes according to the strategy type
        /// </summary>
        /// <param name="parameter">The parameter to be used in the execution</param>
        void Execute(object parameter);
    }

    /// <summary>
    /// Executes a command 
    /// </summary>
    internal class CommandExecutionStrategy : IExecutionStrategy
    {
        #region IExecutionStrategy Members
        /// <summary>
        /// Gets or sets the Behavior that we execute this strategy
        /// </summary>
        public CommandBehaviorBinding Behavior { get; set; }

        /// <summary>
        /// Executes the Command that is stored in the CommandProperty of the CommandExecution
        /// </summary>
        /// <param name="parameter">The parameter for the command</param>
        public void Execute(object parameter)
        {
            if (this.Behavior == null)
                throw new InvalidOperationException("Behavior property cannot be null when executing a strategy");

            if (this.Behavior.Command.CanExecute(this.Behavior.CommandParameter))
                this.Behavior.Command.Execute(this.Behavior.CommandParameter);
        }

        #endregion
    }

    /// <summary>
    /// executes a delegate
    /// </summary>
    internal class ActionExecutionStrategy : IExecutionStrategy
    {

        #region IExecutionStrategy Members

        /// <summary>
        /// Gets or sets the Behavior that we execute this strategy
        /// </summary>
        public CommandBehaviorBinding Behavior { get; set; }

        /// <summary>
        /// Executes an Action delegate
        /// </summary>
        /// <param name="parameter">The parameter to pass to the Action</param>
        public void Execute(object parameter)
        {
            this.Behavior.Action(parameter);
        }

        #endregion
    }

}

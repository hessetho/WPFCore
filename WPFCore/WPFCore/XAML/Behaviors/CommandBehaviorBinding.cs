using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using WPFCore.XAML.Behaviors.Internal;

namespace WPFCore.XAML.Behaviors
{
    /// <summary>
    ///     Defines the command behavior binding
    /// </summary>
    public class CommandBehaviorBinding : IDisposable
    {
        /// <summary>
        /// Creates an EventHandler at runtime and registers that handler to the Event specified
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="eventName"></param>
        public void BindEvent(DependencyObject owner, string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
                throw new InvalidOperationException("Need to specify a name for the event.");

            this.EventName = eventName;
            this.Owner = owner;
            this.Event = this.Owner.GetType()
                .GetEvent(this.EventName, BindingFlags.Public | BindingFlags.Instance);
            if (this.Event == null)
                throw new InvalidOperationException(String.Format("Could not resolve event name {0}", this.EventName));

            //Create an event handler for the event that will call the ExecuteCommand method
            this.EventHandler = EventHandlerGenerator.CreateDelegate(this.Event.EventHandlerType,
                typeof (CommandBehaviorBinding).GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance), this);
            //Register the handler to the Event
            this.Event.AddEventHandler(this.Owner, this.EventHandler);
        }

        /// <summary>
        ///     Executes the strategy
        /// </summary>
        public void Execute()
        {
            this.strategy.Execute(this.CommandParameter);
        }

        #region Properties

        /// <summary>
        ///     Get the owner of the CommandBinding ex: a Button
        ///     This property can only be set from the BindEvent Method
        /// </summary>
        public DependencyObject Owner { get; private set; }

        /// <summary>
        ///     The event name to hook up to
        ///     This property can only be set from the BindEvent Method
        /// </summary>
        public string EventName { get; private set; }

        /// <summary>
        ///     The event info of the event
        /// </summary>
        public EventInfo Event { get; private set; }

        /// <summary>
        ///     Gets the EventHandler for the binding with the event
        /// </summary>
        public Delegate EventHandler { get; private set; }

        #region Execution

        //stores the strategy of how to execute the event handler
        private IExecutionStrategy strategy;

        /// <summary>
        ///     Gets or sets a CommandParameter
        /// </summary>
        public object CommandParameter { get; set; }

        private ICommand command;

        /// <summary>
        ///     The command to execute when the specified event is raised
        /// </summary>
        public ICommand Command
        {
            get { return this.command; }
            set
            {
                this.command = value;
                //set the execution strategy to execute the command
                this.strategy = new CommandExecutionStrategy {Behavior = this};
            }
        }

        private Action<object> action;

        /// <summary>
        ///     Gets or sets the Action
        /// </summary>
        public Action<object> Action
        {
            get { return this.action; }
            set
            {
                this.action = value;
                // set the execution strategy to execute the action
                this.strategy = new ActionExecutionStrategy {Behavior = this};
            }
        }

        #endregion

        #endregion

        #region IDisposable Members

        private bool disposed;

        /// <summary>
        ///     Unregisters the EventHandler from the Event
        /// </summary>
        public void Dispose()
        {
            if (!this.disposed)
            {
                this.Event.RemoveEventHandler(this.Owner, this.EventHandler);
                this.disposed = true;
            }
        }

        #endregion
    }
}
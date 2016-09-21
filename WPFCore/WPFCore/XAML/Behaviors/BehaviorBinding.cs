using System;
using System.Windows;
using System.Windows.Input;

namespace WPFCore.XAML.Behaviors
{
    /// <summary>
    ///     Defines a Command Binding
    ///     This inherits from freezable so that it gets inheritance context for DataBinding to work
    /// </summary>
    public class BehaviorBinding : Freezable
    {
        private CommandBehaviorBinding behavior;

        private DependencyObject owner;

        /// <summary>
        ///     Stores the Command Behavior Binding
        /// </summary>
        internal CommandBehaviorBinding Behavior
        {
            get { return this.behavior ?? (this.behavior = new CommandBehaviorBinding()); }
        }

        /// <summary>
        ///     Gets or sets the Owner of the binding
        /// </summary>
        public DependencyObject Owner
        {
            get { return this.owner; }
            set
            {
                this.owner = value;
                this.ResetEventBinding();
            }
        }

        #region Command

        /// <summary>
        ///     Command Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof (ICommand), typeof (BehaviorBinding),
                                        new FrameworkPropertyMetadata(null,
                                                                      OnCommandChanged));

        /// <summary>
        ///     Gets or sets the Command property.
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand) this.GetValue(CommandProperty); }
            set { this.SetValue(CommandProperty, value); }
        }

        /// <summary>
        ///     Handles changes to the Command property.
        /// </summary>
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BehaviorBinding) d).OnCommandChanged(e);
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to the Command property.
        /// </summary>
        protected virtual void OnCommandChanged(DependencyPropertyChangedEventArgs e)
        {
            this.Behavior.Command = this.Command;
        }

        #endregion

        #region Action

        /// <summary>
        ///     Action Dependency Property
        /// </summary>
        public static readonly DependencyProperty ActionProperty =
            DependencyProperty.Register("Action", typeof (Action<object>), typeof (BehaviorBinding),
                                        new FrameworkPropertyMetadata(null,
                                                                      OnActionChanged));

        /// <summary>
        ///     Gets or sets the Action property.
        /// </summary>
        public Action<object> Action
        {
            get { return (Action<object>) this.GetValue(ActionProperty); }
            set { this.SetValue(ActionProperty, value); }
        }

        /// <summary>
        ///     Handles changes to the Action property.
        /// </summary>
        private static void OnActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BehaviorBinding) d).OnActionChanged(e);
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to the Action property.
        /// </summary>
        protected virtual void OnActionChanged(DependencyPropertyChangedEventArgs e)
        {
            this.Behavior.Action = this.Action;
        }

        #endregion

        #region CommandParameter

        /// <summary>
        ///     CommandParameter Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof (object), typeof (BehaviorBinding),
                                        new FrameworkPropertyMetadata(null,
                                                                      OnCommandParameterChanged));

        /// <summary>
        ///     Gets or sets the CommandParameter property.
        /// </summary>
        public object CommandParameter
        {
            get { return this.GetValue(CommandParameterProperty); }
            set { this.SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        ///     Handles changes to the CommandParameter property.
        /// </summary>
        private static void OnCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BehaviorBinding) d).OnCommandParameterChanged(e);
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to the CommandParameter property.
        /// </summary>
        protected virtual void OnCommandParameterChanged(DependencyPropertyChangedEventArgs e)
        {
            this.Behavior.CommandParameter = this.CommandParameter;
        }

        #endregion

        #region Event

        /// <summary>
        ///     Event Dependency Property
        /// </summary>
        public static readonly DependencyProperty EventProperty =
            DependencyProperty.Register("Event", typeof (string), typeof (BehaviorBinding),
                                        new FrameworkPropertyMetadata(null,
                                                                      OnEventChanged));

        /// <summary>
        ///     Gets or sets the Event property.
        /// </summary>
        public string Event
        {
            get { return (string) this.GetValue(EventProperty); }
            set { this.SetValue(EventProperty, value); }
        }

        /// <summary>
        ///     Handles changes to the Event property.
        /// </summary>
        private static void OnEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BehaviorBinding) d).OnEventChanged(e);
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to the Event property.
        /// </summary>
        protected virtual void OnEventChanged(DependencyPropertyChangedEventArgs e)
        {
            this.ResetEventBinding();
        }

        #endregion

        private static void OwnerReset(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BehaviorBinding) d).ResetEventBinding();
        }

        private void ResetEventBinding()
        {
            if (this.Owner != null) //only do this when the Owner is set
            {
                //check if the Event is set. If yes we need to rebind the Command to the new event and unregister the old one
                if (this.Behavior.Event != null && this.Behavior.Owner != null)
                    this.Behavior.Dispose();

                //bind the new event to the command
                this.Behavior.BindEvent(this.Owner, this.Event);
            }
        }

        /// <summary>
        ///     This is not actually used. This is just a trick so that this object gets WPF Inheritance Context
        /// </summary>
        /// <returns></returns>
        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
    }
}

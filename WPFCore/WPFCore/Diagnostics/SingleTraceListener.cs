using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using WPFCore.ViewModelSupport;

namespace WPFCore.Diagnostics
{
    /// <summary>
    /// A <see cref="System.Diagnostics.TraceListener"/> that puts the messages received from a <see cref="System.Diagnostics.TraceSource"/>
    /// in an observable collection that can be bound to the UI.
    /// </summary>
    public class SingleTraceListener : TraceListener
    {
        // stores my dispatcher
        private readonly Dispatcher myDispatcher = Dispatcher.CurrentDispatcher;

        /// <summary>
        /// The trace source this listener is assigned to
        /// </summary>
        private readonly TraceSource traceSource;

        /// <summary>
        /// Bindable list of messages
        /// </summary>
        private readonly ObservableCollection<TraceMessage> messages = new ObservableCollection<TraceMessage>();

        /// <summary>
        /// The last (partial) message
        /// </summary>
        private TraceMessage partialMessage;

        /// <summary>
        /// Internally queued messages
        /// </summary>
        private readonly Queue<TraceMessage> queuedMessages = new Queue<TraceMessage>();

        /// <summary>
        /// The timer that is used to empty the internal queue into the observable message queue
        /// </summary>
        private DispatcherTimer timer;

        private ICommand commandClearAll;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleTraceListener"/> class.
        /// </summary>
        /// <param name="traceSource">The trace source.</param>
        public SingleTraceListener(TraceSource traceSource)
        {
            this.traceSource = traceSource;
            traceSource.Listeners.Add(this);

            this.timer = new DispatcherTimer(new TimeSpan(1000), DispatcherPriority.Background, PumpMessages, Dispatcher.CurrentDispatcher);
            timer.Start();
        }

        /// <summary>
        /// Pumps the messages in the queue in the observable (and thus bindable) list of messages.
        /// </summary>
        /// <remarks>
        /// As this method is called from a <see cref="DispatcherTimer"/> it is guaranteed to run on the 
        /// dispatcher specified by that timer. In this case this is the dispatcher that owns the <c>SingleTraceListener</c>
        /// (usually the application's dispatcher).
        /// </remarks>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void PumpMessages(object sender, EventArgs e)
        {
            const string exclude = "System.Windows.Data Warning: 40 : BindingExpression path error: 'IsDropDownOpen' property not found on 'object' ''RibbonContentPresenter' (Name='PART_ContentPresenter')'. BindingExpression:Path=IsDropDownOpen; DataItem='RibbonContentPresenter' (Name='PART_ContentPresenter'); target element is 'RibbonButton' (Name=''); target property is 'NoTarget' (type 'Object')";
            while (this.queuedMessages.Count > 0)
            {
                var msg =this.queuedMessages.Dequeue();
         //       if (!msg.Message.Equals(exclude))
                    this.messages.Add(msg);
            }
        }

        /// <summary>
        /// Gets the trace source this instance is assigned to.
        /// </summary>
        public TraceSource TraceSource
        {
            get { return traceSource; }
        }

        /// <summary>
        /// Gets the messages received from the <see cref="TraceSource"/>
        /// </summary>
        public ObservableCollection<TraceMessage> Messages
        {
            get { return messages; }
        }

        /// <summary>
        /// Writes the specified message to the internal queue of messages.
        /// </summary>
        /// <remarks>
        /// In order to avoid deadlocks, messages are put on an internal
        /// message queue. Accessing the queue does not require a context
        /// switch to the dispatcher that owns this listener (usu. the app
        /// dispatcher), so the application does not get blocked if two
        /// dispatcher treads are writing trace messages at the same time.
        /// </remarks>
        /// <param name="message">A message to write.</param>
        public override void Write(string message)
        {
            Debug.WriteLine(string.Format("SingleTraceListener.Write: {0}", message));

            if (this.partialMessage == null)
            {
                this.partialMessage = new TraceMessage(message);
                this.queuedMessages.Enqueue(this.partialMessage);
            }
            else
                this.partialMessage.AppendMessage(message);
        }

        /// <summary>
        /// Writes the specified message to the internal queue of messages.
        /// </summary>
        /// <remarks>
        /// In order to avoid deadlocks, messages are put on an internal
        /// message queue. Accessing the queue does not require a context
        /// switch to the dispatcher that owns this listener (usu. the app
        /// dispatcher), so the application does not get blocked if two
        /// dispatcher treads are writing trace messages at the same time.
        /// </remarks>
        /// <param name="message">A message to write.</param>
        public override void WriteLine(string message)
        {
            this.Write(message);
            this.partialMessage = null;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.traceSource.Listeners.Remove(this);
        }


        #region CommandClearAll
        public ICommand CommandClearAll
        {
            get { return commandClearAll ?? (commandClearAll = new RelayCommand(ClearAll, CanClearAll)); }
        }

        private void ClearAll()
        {
            this.messages.Clear();
        }

        private bool CanClearAll()
        {
            return true;
        }
        #endregion CommandClearAll

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using WPFCore.Helper;

namespace WPFCore.ViewModelSupport
{
    /// <summary>
    ///     This is the base class for single item view models. It provides the methods and events to
    ///     give feedback about changes to the objects properties and changes to the objects initialization
    ///     status.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, ISupportInitializeNotification
    {
        // Thread synchronisation object
        protected object LockObj = new object();

        // stores the objects dispatcher
        protected readonly Dispatcher MyDispatcher = Dispatcher.CurrentDispatcher;

        /// <summary>
        ///     List of properties which are decorated with the <see cref="DoesNotAffectChangesFlagAttribute" />
        /// </summary>
        protected readonly List<string> PropertiesDoNotAffectChangesFlag;

        /// <summary>
        ///     Indictor to suppress tracing for the current object.
        /// This is set be decorating the class with the <see cref="DontTraceMeAttribute"/>
        /// </summary>
        private readonly bool dontTraceMe;

        /// <summary>
        /// Represents the <see cref="TraceSource"/> to use for tracing. This is set to
        /// <see cref="Constants.CoreTraceSource"/> by default, but may be set to a different
        /// value for fine grained trace filtering.
        /// </summary>
        protected TraceSource MyTraceSource { get; set; }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <remarks>
        ///     The constructor collects all properties which are decorated with the
        ///     <see cref="DoesNotAffectChangesFlagAttribute" />.
        /// </remarks>
        [DebuggerStepThrough]
        public ViewModelBase()
        {
            this.PropertiesDoNotAffectChangesFlag = this.GetType()
                .GetProperties()
                .Where(p => GetDoesNotAffectChangesFlag(p).Length != 0)
                .Select(p => p.Name)
                .ToList();

            this.dontTraceMe = this.GetType().GetCustomAttributes(typeof (DontTraceMeAttribute), true).Count()!=0;

            this.MyTraceSource = Constants.CoreTraceSource;
        }

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Occurs when any property value changed.
        /// </summary>
        //public event HasChangedEventHandler HasChanged;

        public event EventHandler<string> HasChanged;

        /// <summary>
        ///     Helper to retrieve the <see cref="DoesNotAffectChangesFlagAttribute" /> for a
        ///     specific property.
        /// </summary>
        /// <param name="property">Name of the property.</param>
        /// <returns>An array, which is empty if the property is not decorated.</returns>
        [DebuggerStepThrough]
        private static DoesNotAffectChangesFlagAttribute[] GetDoesNotAffectChangesFlag(PropertyInfo property)
        {
            return
                (DoesNotAffectChangesFlagAttribute[])
                    property.GetCustomAttributes(typeof (DoesNotAffectChangesFlagAttribute), true);
        }

        /// <summary>
        ///     This helper verifies that the provided property name
        ///     refers to an existing property of the class. (Debug only)
        /// </summary>
        /// <param name="propertyName"></param>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        protected void VerifyPropertyName(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return;

            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
                throw new ArgumentException(string.Format("({0}) Invalid property name: {1}", this.GetType().Name, propertyName));
        }

        /// <summary>
        ///     Ensures that an action is performed on the dispatcher/thread
        ///     that owns the current instance.
        /// </summary>
        /// <param name="methodCall"></param>
        protected void InvokeIfRequired(Action methodCall)
        {
            DispatcherHelper.InvokeIfRequired(this.MyDispatcher, methodCall);
        }

        /// <summary>
        ///     Ensures that an action is performed on the main dispatcher/thread
        ///     of the application.
        /// </summary>
        /// <param name="methodCall"></param>
        protected static void InvokeOnAppDispatcher(Action methodCall)
        {
            DispatcherHelper.InvokeOnAppDispatcher(methodCall);
        }

        #region property changed notification

        /// <summary>
        ///     Set's the <see cref="HasChanges" /> property to true unless
        ///     the item is currently initialized or the property has the
        ///     <see cref="DoesNotAffectChangesFlagAttribute" />.
        ///     Triggers the <see cref="PropertyChanged" /> event.
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        /// <seealso cref="BeginInit"/>
        /// <seealso cref="EndInit"/>
        /// <seealso cref="IsInitialized"/>
        /// <seealso cref="IsInitializing"/>
        /// <see cref="DoesNotAffectChangesFlagAttribute"/>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName="")
        {
            // we'll never modify the HasChanges flag while initializing (any property)
            // or changing properties which shall not affect the HasChanges flag
            if (this.IsInitializing == false && this.PropertiesDoNotAffectChangesFlag.Contains(propertyName) == false)
            {
                this.LastChangedProperty = propertyName;

                if(!string.IsNullOrEmpty(propertyName))
                    this.HasChanges = true;
            }

            this.FirePropertyChanged(propertyName);
        }

        /// <summary>
        ///     Triggers the <see cref="PropertyChanged" /> event.
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        protected void FirePropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);

            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

            this.PropertyChangedCompleted(propertyName);
        }

        /// <summary>
        ///     This virtual method is called after the <c>PropertyChanged</c> event is triggered
        ///     (see <see cref="OnPropertyChanged" /> and <see cref="FirePropertyChanged" />). It has no
        ///     effect unless overridden.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to implement specific post processing when a property
        ///     has been changed.
        /// </remarks>
        /// <param name="propertyName">Name of the property</param>
        protected virtual void PropertyChangedCompleted(string propertyName)
        {
        }

        #endregion property changed notification

        #region Abortion signalling

        /// <summary>
        ///     This method may be invoked from a (closing) window to signal
        ///     the view model to abort all work.
        /// </summary>
        /// <remarks>
        ///     The method makes a call to <see cref="AbortEverything" /> which
        ///     can be overriden to perform specific abortion tasks.
        /// </remarks>
        public void AbortEverything()
        {
            this.AbortSignalReceived();
        }

        /// <summary>
        ///     Any deriving class should override this method if it likes or
        ///     needs to clean up or shutdown background processes.
        /// </summary>
        protected virtual void AbortSignalReceived()
        {
        }

        #endregion

        #region ISupportInitialize, ISupportInitializeNotification

        private bool isInitializing;
        private bool isInitialized;

        /// <summary>
        ///     Returns <c>True</c> while the instance is being initialized, <c>False</c> otherwise.
        /// </summary>
        /// <remarks>
        ///     This flag is set by calling <see cref="BeginInit" /> and reset
        ///     by <see cref="EndInit" />.
        /// </remarks>
        /// <seealso cref="BeginInit"/>
        /// <seealso cref="EndInit"/>
        [DoesNotAffectChangesFlag]
        [XmlIgnore]
        public bool IsInitializing
        {
            get { return this.isInitializing; }
            private set
            {
                this.isInitializing = value;
                this.OnPropertyChanged("IsInitializing");
            }
        }

        /// <summary>
        ///     Returns <c>True</c> if the instance is fully initialized, <c>False</c> otherwise.
        /// </summary>
        /// <remarks>
        ///     This flag is set by calling <see cref="EndInit" /> and reset
        ///     by <see cref="BeginInit" />.
        /// </remarks>
        /// <seealso cref="BeginInit"/>
        /// <seealso cref="EndInit"/>
        [DoesNotAffectChangesFlag]
        [XmlIgnore]
        public bool IsInitialized
        {
            get { return this.isInitialized; }
            protected set
            {
                this.isInitialized = value;
                this.OnPropertyChanged("IsInitialized");
            }
        }

        /// <summary>
        ///     Occurs when the initialization has been finished.
        /// </summary>
        /// <remarks>
        ///     This event is triggered only when <see cref="EndInit" /> was called
        /// </remarks>
        /// <seealso cref="EndInit"/>
        public event EventHandler Initialized;

        private readonly object lockObj = new object();
        private int initCount;

        /// <summary>
        ///     Signals the beginning of the initialization of the object instance.
        /// </summary>
        /// <remarks>
        /// <para>
        ///     The method increases an internal counter to allow nesting of calls to
        ///     <c>BeginInit</c> and <c>EndInit</c>
        /// </para>
        /// <para>
        ///     While initializing, no <see cref="HasChanges"/> events will be triggered.
        /// </para>
        /// </remarks>
        /// <seealso cref="OnPropertyChanged"/>
        /// <seealso cref="EndInit"/>
        /// <seealso cref="IsInitialized"/>
        /// <seealso cref="IsInitializing"/>
        /// <seealso cref="HasChanges"/>
        //[DebuggerStepThrough]
        public void BeginInit()
        {
            Monitor.Enter(this.lockObj);
            this.initCount++;
            this.IsInitializing = true;
            this.IsInitialized = false;
            Monitor.Exit(this.lockObj);
        }

        /// <summary>
        ///     Signals the end of the initialization of the object instance. If all initializations
        ///     are finished, the "<see cref="Initialized" />" event is raised.
        /// </summary>
        /// <remarks>
        /// <para>
        ///     The method decreases an internal counter to allow nesting of calls to
        ///     <c>BeginInit</c> and <c>EndInit</c>. When this counter reaches zero,
        ///     the <see cref="IsInitializing" /> and <see cref="IsInitialized" /> flags
        ///     are set and the <see cref="Initialized" /> event is raised.
        /// </para>
        /// <para>
        ///     Ending the initialization of the object instance will re-enable the triggering
        ///     of the <see cref="HasChanges"/> event.
        /// </para>
        /// <para>
        ///     Note: In debug mode, an assertion failure is thrown if the counter falls below zero.
        /// </para>
        /// </remarks>
        /// <seealso cref="OnPropertyChanged"/>
        /// <seealso cref="BeginInit"/>
        /// <seealso cref="IsInitialized"/>
        /// <seealso cref="IsInitializing"/>
        /// <seealso cref="HasChanges"/>
        //[DebuggerStepThrough]
        public void EndInit()
        {
            Monitor.Enter(this.lockObj);
            this.initCount--;
            if (this.initCount == 0)
            {
                this.IsInitializing = false;
                this.IsInitialized = true;
                this.OnInitialized();
            }
#if DEBUG
            if (this.initCount < 0)
                DiagnosticsHelper.WriteLine(string.Format("WARNING: initCount falls below 0 for {0}", this.GetType().Name));
#endif
            Monitor.Exit(this.lockObj);
        }

        /// <summary>
        ///     Raises the <see cref="Initialized" /> event.
        /// </summary>
        /// <remarks>
        ///     The event is raised on the dispatcher thread of the object.
        /// </remarks>
        protected void OnInitialized()
        {
            if (this.Initialized != null)
                // ensure that this event occurs on the thread that created this instance
                this.InvokeIfRequired(() =>
                {
                    this.Initialized(this, new EventArgs());
                });
        }

        #endregion ISupportInitialize, ISupportInitializeNotification

        #region HasChanges

        private bool hasChanges;

        /// <summary>
        ///     Returns <c>True</c> if any of the object's properties has been changed, <c>False</c> otherwise.
        ///     Setting the value of this property to <c>True</c> raises the <see cref="HasChanged" /> event.
        /// </summary>
        [DoesNotAffectChangesFlag]
        [XmlIgnore]
        public bool HasChanges
        {
            get { return this.hasChanges; }
            private set
            {
                this.hasChanges = value;
                this.OnPropertyChanged("HasChanges");

                if (!this.dontTraceMe && this.hasChanges)
                {
                    // note: derived classes which do not implement ITraceInformationProvider should override ToString() to return a meaningful item description
                    var itemDescription = this.ToString();
                    if (this is ITraceInformationProvider)
                        itemDescription = ((ITraceInformationProvider)this).ItemDescription;

                    this.MyTraceSource.TraceDebug(string.Format("[{0} ({1})] changed \"{2}\"", this.GetType().Name, itemDescription, this.LastChangedProperty));
                }

                if (this.hasChanges && this.HasChanged != null)
                    this.HasChanged(this, this.LastChangedProperty);
            }
        }

        /// <summary>
        /// Set's the <see cref="HasChanges"/> flag to true.
        /// </summary>
        public void SetChangeFlag()
        {
            this.LastChangedProperty = "unknown (flag forced)";
            this.HasChanges = true;
        }

        /// <summary>
        /// Set's the <see cref="HasChanges"/> flag to true.
        /// </summary>
        public void SetChangeFlag(string propertyName)
        {
            this.LastChangedProperty = string.Format("{0} (flag forced)", propertyName);
            this.HasChanges = true;
        }

        /// <summary>
        /// Set's the <see cref="HasChanges"/> flag to false.
        /// </summary>
        public void ResetChangeFlag()
        {
            if (this.HasChanges == false) return;

            this.HasChanges = false;

            if (!this.dontTraceMe)
                this.MyTraceSource.TraceDebug(string.Format("[{0} ({1})] reset change flag", this.GetType().Name, this)); // note: derived classes should override ToString()
        }

        /// <summary>
        ///     Returns the name of the property that was changed lastly
        ///     and which triggered the <see cref="HasChanged" /> event.
        /// </summary>
        /// <remarks>
        ///     This property may be used in conjunction with the handling
        ///     of the <see cref="HasChanged" /> event to determine the property
        ///     that was changed.
        ///     Changes to properties that have the <see cref="DoesNotAffectChangesFlagAttribute" />
        ///     will not trigger the event and thus the <c>LastChangedProperty</c> will
        ///     not be set.
        /// </remarks>
        [XmlIgnore]
        public string LastChangedProperty { get; private set; }

        #endregion HasChanges
    }
}
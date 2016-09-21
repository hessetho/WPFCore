using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using ThreadState = System.Threading.ThreadState;

namespace WPFCore.Helper
{
    /// <summary>
    ///     Die <c>ThreadStarter"</c>-Klasse erlaubt es, einen <see cref="Thread" /> so zu starten,
    ///     dass <see cref="Exception" />s die darin auftreten kontrolliert an die Anwendung zurück
    ///     zu liefern.
    /// </summary>
    public class ThreadStarter
    {
        /// <summary>
        ///     Der Delegat definiert eine Callback-Methode der Anwendung, welche <see cref="Exception" />s
        ///     aus dem gestarteten Thread entgegen nimmt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Die ausgelöste Exception</param>
        public delegate void ExceptionCallbackDelegate(object sender, Exception e);

        /// <summary>
        ///     Der Delegat definiert eine Callback-Methode der Anwendung, welche bei einer Stop()
        ///     Anforderungen an den Thread ausgeführt wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public delegate void StopThreadCallbackDelegate(object sender, object data);

        /// <summary>
        ///     Der <see cref="Thread" /> in welchem die eigentliche Programmlogik ausgeführt wird
        /// </summary>
        private readonly Thread thread;

        /// <summary>
        ///     ExceptionCallback für diese Instanz
        /// </summary>
        private ExceptionCallbackDelegate exceptionCallback;

        //private object lockObj = new object();

        /// <summary>
        ///     Der eigene Dispatcher. Erforderlich um Thread-übergreifend zu arbeiten
        /// </summary>
        protected Dispatcher MyDispatcher = Dispatcher.CurrentDispatcher;

        private bool stoppable;

        static ThreadStarter()
        {
            Threads = new List<ThreadStarter>();
        }

        /// <summary>
        ///     Konstruktor. Der <c>Action</c>-Parameter definiert den im Thread auszuführenden Code.
        /// </summary>
        /// <example>
        ///     void Main()
        ///     {
        ///     var ts = new Helper.ThreadStarter(() => DoSomething());
        ///     ts.Start();
        ///     var ts2 = new Helper.ThreadStarter(DoSomething);
        ///     ts2.Start();
        ///     }
        ///     private void DoSomething() { /* tu etwas */ }
        /// </example>
        /// <param name="action">Der auszuführende Code-Block</param>
        /// <param name="threadName">Bezeichnung des Threads</param>
        public ThreadStarter(Action action, string threadName)
        {
            this.thread = new Thread(delegate()
            {
                try
                {
                    this.NotifyThreadStarted();
                    action();
                }
                catch (ThreadAbortException tae)
                {
                    // Sicherstellen, dass ThreadAbortException nicht selbst ausgelöst worden ist
                    if (this.StopRequested == false)
                        if (this.ExceptionCallback != null)
                            this.ExceptionCallback(this, tae);
                        else
                            throw;
                }
                catch (Exception e)
                {
                    if (this.ExceptionCallback != null)
                        this.ExceptionCallback(this, e);
                    else
                        throw;
                }
                finally
                {
                    UnregisterThread(this);
                    this.NotifyThreadFinished();
                }
            }
                ) {Name = threadName};
            this.stoppable = true;
        }

        /// <summary>
        ///     Konstruktor. Der <c>Action</c>-Parameter definiert den im Thread auszuführenden Code,
        ///     dem ein zusätzliches Datenobjekt übergeben werden kann.
        /// </summary>
        /// <example>
        ///     void Main()
        ///     {
        ///     string msg='Hello World';
        ///     var ts = new Helper.ThreadStarter(() => DoSomething(), msg);
        ///     ts.Start();
        ///     var ts2 = new Helper.ThreadStarter(DoSomething, msg);
        ///     ts2.Start();
        ///     }
        ///     private void ShowMessage(object msg) { /* tu etwas */ }
        /// </example>
        /// <param name="action"></param>
        /// <param name="data"></param>
        public ThreadStarter(Action<object> action, object data)
        {
            this.thread = new Thread(delegate()
            {
                try
                {
                    this.NotifyThreadStarted();
                    action(data);
                }
                catch (ThreadAbortException tae)
                {
                    // Sicherstellen, dass ThreadAbortException nicht selbst ausgelöst worden ist
                    if (this.StopRequested == false)
                        if (this.ExceptionCallback != null)
                            this.ExceptionCallback(this, tae);
                        else
                            throw;
                }
                catch (Exception e)
                {
                    if (this.ExceptionCallback != null)
                        this.ExceptionCallback(this, e);
                    else
                        throw;
                }
                finally
                {
                    UnregisterThread(this);
                    this.NotifyThreadFinished();
                }
            }
                );
            this.stoppable = true;
        }

        /// <summary>
        ///     Statische Liste, welche die aktuell laufenden Threads enthält.
        /// </summary>
        public static List<ThreadStarter> Threads { get; private set; }

        /// <summary>
        ///     Liefert bzw. setzt den Standard-ExceptionCallback für die Anwendung.
        /// </summary>
        public static ExceptionCallbackDelegate DefaultExceptionCallback { get; set; }

        /// <summary>
        ///     Setzt einen Delegaten, welcher beim Auftreten einer <c>Exception</c> aufgerufen wird.
        ///     Liefert entweder den gesetzten Delegaten oder, falls eingerichtet, den Standard-Delegaten
        ///     (<see cref="DefaultExceptionCallback" />).
        /// </summary>
        public ExceptionCallbackDelegate ExceptionCallback
        {
            get { return (this.exceptionCallback ?? DefaultExceptionCallback); }
            set { this.exceptionCallback = value; }
        }

        /// <summary>
        ///     Setzt einen Delegaten, welcher bei einer <see cref="ThreadStarter.Stop" />-Anforderung ausgeführt wird.
        /// </summary>
        public StopThreadCallbackDelegate StopThreadCallback { get; set; }

        /// <summary>
        ///     Liefert den Namen des Thread bzw. setzt diesen
        /// </summary>
        public string ThreadName
        {
            set { this.thread.Name = value; }
            get { return this.thread.Name; }
        }

        /// <summary>
        ///     Liefert den echten <see cref="Thread" />, welcher für die Ausführung zuständig ist.
        /// </summary>
        public Thread Thread
        {
            get { return this.thread; }
        }

        /// <summary>
        ///     Liefert den Status des echten <see cref="Thread" />s bzw. <see cref="System.Threading.ThreadState.Unstarted" />
        ///     wenn es noch keinen echten Thread gibt.
        /// </summary>
        public ThreadState ThreadState
        {
            get
            {
                if (this.thread == null)
                    return ThreadState.Unstarted;

                return this.Thread.ThreadState;
            }
        }

        /// <summary>
        ///     Liefert Zusatzdaten bzw. legt diese fest
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        ///     Liefert <c>True</c> wenn über <see cref="Stop()" /> ein Abbruch angefordert wurde, ansonsten <c>False</c>.
        /// </summary>
        public bool StopRequested { get; private set; }

        /// <summary>
        ///     Ereignis. Wird ausgelöst, wenn der Thread gestartet worden ist.
        ///     (Zu beachten: Der EventHandler befindet sich stets im Kontext des Threads, d.h. nicht im dem der Anwendung)
        /// </summary>
        public event EventHandler ThreadStarted;

        /// <summary>
        ///     Ereignis. Wird ausgelöst, wenn der Thread beendet worden ist.
        ///     (Zu beachten: Der EventHandler befindet sich stets im Kontext des Threads, d.h. nicht im dem der Anwendung)
        /// </summary>
        public event EventHandler<bool> ThreadFinished;

        /// <summary>
        ///     Event is invoked when a new thread has been started and added to the internal list
        /// </summary>
        public static event EventHandler<ThreadStarter> ThreadAdded;

        /// <summary>
        ///     Event is invoked when a thread has finished and was removed from the internal list
        /// </summary>
        public static event EventHandler<ThreadStarter> ThreadRemoved;

        /// <summary>
        ///     Create a new <see cref="ThreadStarter" /> instance and start it immediately
        /// </summary>
        /// <param name="action">Der auszuführende Code-Block</param>
        /// <param name="threadName">Bezeichnung des Threads</param>
        /// <returns>The new <c>ThreadStarter</c></returns>
        [DebuggerStepThrough]
        public static ThreadStarter StartImmediate(Action action, string threadName)
        {
            var thread = new ThreadStarter(action, threadName);
            thread.Start();
            return thread;
        }

        /// <summary>
        ///     Beginnt mit der Ausführung des Threads.
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Thread Start()
        {
            Debug.Assert(this.thread != null,
                "Internal error! ThreadStart.Start() invoked, but there is no thread to start!");
            if (this.StopRequested)
            {
                TraceHelper.TraceDebug("ThreadStarter.Start() overruled by stop request. Thread will not be started.");
            }
            else
            {
                RegisterThread(this);
                this.Thread.Start();
            }

            return this.Thread;
        }

        /// <summary>
        ///     Bricht die Ausführung ab
        /// </summary>
        public void Abort()
        {
            Debug.Assert(this.thread != null,
                "Internal error! ThreadStart.Abort() invoked, but there is no thread to start!");

            this.Stop(this.Data);

            this.LockMe();
            this.Thread.Abort();
            this.UnlockMe();
        }

        /// <summary>
        ///     Erlaubt es, den Thread unter der Kontrolle der Anwendung zu beenden. Dazu ist es erforderlich,
        ///     dass ein <see cref="StopThreadCallback" /> angegeben wurde, welcher seitens der Anwendung die
        ///     Beendigung das Threads initiiert.
        /// </summary>
        /// <remarks>
        ///     <c>Stop</c> verwendet in dieser Überladung den Inhalt von <see cref="Data" /> als Parameter,
        ///     welcher der Anwendung über den <see cref="StopThreadCallback" /> übergeben wird.
        /// </remarks>
        public void Stop()
        {
            this.Stop(this.Data);
        }

        /// <summary>
        ///     Erlaubt es, den Thread unter der Kontrolle der Anwendung zu beenden. Dazu ist es erforderlich,
        ///     dass ein <see cref="StopThreadCallback" /> angegeben wurde, welcher seitens der Anwendung die
        ///     Beendigung das Threads initiiert.
        /// </summary>
        /// <param name="data">zusätzliche Daten, welche an die Anwendung übergeben werden</param>
        public void Stop(object data)
        {
            Debug.Assert(this.thread != null,
                "Internal error! ThreadStart.Stop() invoked, but there is no thread to start!");

            //LockMe();
            if (this.stoppable)
            {
                this.StopRequested = true;
                TraceHelper.TraceDebug(string.Format("Requesting stop (current state: {0}): {1}", this.Thread.ThreadState,
                    this.thread.Name));

                if (this.StopThreadCallback != null)
                    this.StopThreadCallback(this, data);
            }
            //UnlockMe();
        }

        private void NotifyThreadStarted()
        {
            TraceHelper.TraceDebug(string.Format("Thread started: {1}", DateTime.Now, this.thread.Name));
            if (this.ThreadStarted != null)
            {
                this.LockMe();
                DispatcherHelper.InvokeIfRequired(this.MyDispatcher, () => this.ThreadStarted(this, new EventArgs()));
                this.UnlockMe();
            }
        }

        private void NotifyThreadFinished()
        {
            TraceHelper.TraceDebug(string.Format("Thread finished: {1}", DateTime.Now, this.thread.Name));
            if (this.ThreadFinished == null) return;

            this.LockMe();
            DispatcherHelper.InvokeIfRequired(this.MyDispatcher, () =>
                    {
                        this.ThreadFinished(this, this.StopRequested);
                    }
                );
            this.UnlockMe();
        }

        private void LockMe()
        {
            //Monitor.Enter(lockObj);
            this.stoppable = false;
        }

        private void UnlockMe()
        {
            //Monitor.Exit(lockObj);
            this.stoppable = true;
        }

        #region Self-management

        /// <summary>
        ///     Add's a <c>ThreadStarter</c> element to the internal thread list
        /// </summary>
        /// <remarks>
        ///     Raises the <see cref="ThreadAdded" /> event.
        /// </remarks>
        /// <param name="thread">The ThreadStarter element to add</param>
        private static void RegisterThread(ThreadStarter thread)
        {
            Threads.Add(thread);
            if (ThreadAdded != null)
                ThreadAdded(null, thread);
        }

        /// <summary>
        ///     Remove a ThreadStarter element from the internal thread list
        /// </summary>
        /// <remarks>
        ///     Raises the <see cref="ThreadRemoved" /> event.
        /// </remarks>
        /// <param name="thread">The ThreadStarter element to remove</param>
        private static void UnregisterThread(ThreadStarter thread)
        {
            try
            {
                Threads.Remove(thread);
            }
            catch (ArgumentOutOfRangeException e)
            {
                TraceHelper.TraceError(string.Format("ThreadStarter.UnregisterThread encountered an exception: {0}", e.Message));
            }
            finally
            {
                if (ThreadRemoved != null)
                    ThreadRemoved(null, thread);
            }
        }

        /// <summary>
        ///     Returns the number of running threads
        /// </summary>
        public static int RunningThreadCount
        {
            get { return Threads.Count; }
        }

        /// <summary>
        ///     Stops all running threads
        /// </summary>
        public static void StopAllThreads()
        {
            while (Threads.Count > 0)
            {
                try
                {
                    var t = Threads[0];
                    if (t == null)
                        Threads.RemoveAt(0);
                    else if (EnumHelper.IsFlagSet<ThreadState>(t.Thread.ThreadState, ThreadState.AbortRequested))
                        Threads.Remove(t);
                    else if (EnumHelper.IsFlagSet<ThreadState>(t.Thread.ThreadState, ThreadState.Aborted))
                        Threads.RemoveAt(0);
                    else if (!EnumHelper.IsFlagSet<ThreadState>(t.Thread.ThreadState, ThreadState.AbortRequested))
                        t.Abort();

                    System.Threading.Thread.Sleep(500);
                }
                catch
                {
                    // ignore
                }
            }
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using WPFCore.ViewModelSupport;

namespace WPFCore.Diagnostics
{
    public class AllTracesListener : ViewModelBase, IDisposable
    {
        private readonly List<TraceSource> traceSources = new List<TraceSource>();
        private readonly List<SingleTraceListener> traceListeners = new List<SingleTraceListener>();

        public AllTracesListener()
        {
        }

        public void AddSystemTraces()
        {
            //this.ListenToTraceSource(PresentationTraceSources.AnimationSource);
            this.ListenToTraceSource(PresentationTraceSources.DataBindingSource);
            //this.ListenToTraceSource(PresentationTraceSources.DependencyPropertySource);
            //this.ListenToTraceSource(PresentationTraceSources.DocumentsSource);
            //this.ListenToTraceSource(PresentationTraceSources.FreezableSource);
            //this.ListenToTraceSource(PresentationTraceSources.HwndHostSource);
            //this.ListenToTraceSource(PresentationTraceSources.MarkupSource);
            //this.ListenToTraceSource(PresentationTraceSources.NameScopeSource);
            //this.ListenToTraceSource(PresentationTraceSources.ResourceDictionarySource);
            //this.ListenToTraceSource(PresentationTraceSources.RoutedEventSource);
            //this.ListenToTraceSource(PresentationTraceSources.ShellSource);
        }

        public List<TraceSource> TraceSources
        {
            get { return traceSources; }
        }

        public List<SingleTraceListener> TraceListeners
        {
            get { return traceListeners; }
        } 

        public void Dispose()
        {
            foreach (var traceListener in this.traceListeners)
                traceListener.Dispose();

            this.traceSources.Clear();
            this.traceListeners.Clear();
        }

        public void ListenToTraceSource(TraceSource traceSource)
        {
            if (!this.traceSources.Contains(traceSource))
            {
                this.traceSources.Add(traceSource);
                this.traceListeners.Add(new SingleTraceListener(traceSource));
            }
        }
    }
}

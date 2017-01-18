using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFCore.ViewModelSupport
{
    public interface IDialogSupport
    {
        event EventHandler ChangesAccepted;
        event EventHandler ChangesRejected;

        ICommand CommandAcceptChanges { get; }
        ICommand CommandRejectChanges { get; }

        void HandleCancellableEvent(CancelEventArgs e);

        string WindowTitle { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore.ViewModelSupport
{
    public interface IInstanceUpdatedDeleted
    {
        event EventHandler<object> InstanceUpdated;
        void FireInstanceUpdated(object updatingInstance);

        event EventHandler<object> InstanceDeleted;
        void FireInstanceDeleted(object deletingInstance);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore.MySql
{
    public struct MultiItemResult<T>
    {
        public List<T> Result { get; private set; }
        public Exception Exception { get; private set; }

        public MultiItemResult(List<T> result) : this(result, null)
        {
        }

        public MultiItemResult(List<T> result, Exception e) : this()
        {
            this.Result = result;
            this.Exception = e;
        }
    }
}

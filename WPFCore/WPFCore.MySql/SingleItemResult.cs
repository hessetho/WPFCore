using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore.MySql
{
    public struct SingleItemResult<T>
    {
        public T Result { get; private set; }
        public Exception Exception { get; private set; }

        public SingleItemResult(T result) : this(result, null)
        {
        }

        public SingleItemResult(T result, Exception e) : this()
        {
            this.Result = result;
            this.Exception = e;
        }
    }
}

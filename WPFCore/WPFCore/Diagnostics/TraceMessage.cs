using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore.Diagnostics
{
    public class TraceMessage
    {
        private string message;
        public TraceMessage(string message)
        {
            this.message = message;
        }

        public void AppendMessage(string message)
        {
            this.message += message;
        }

        public string Message
        {
            get { return message; }
        }

        public override string ToString()
        {
            return this.message;
        }
    }
}

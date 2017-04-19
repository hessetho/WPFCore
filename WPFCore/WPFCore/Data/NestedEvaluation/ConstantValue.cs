using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore.Data.NestedEvaluation
{
    public abstract class ConstantValue<T> : IEvaluationNode
    {
        private T constantValue;

        public ConstantValue() {}

        public ConstantValue(T constantValue)
        {
            this.constantValue = constantValue;
        }

        public Type ResultType
        {
            get { return typeof(T); }
        }

        public string GetNodeAsString()
        {
            return constantValue.ToString();
        }

        public object GetResult()
        {
            return this.constantValue;
        }

        public void SetArgument1(object arg)
        {
            this.constantValue = (T)arg;
        }

        public void SetArgument2(object arg)
        {
            throw new InvalidOperationException();
        }
    }
}

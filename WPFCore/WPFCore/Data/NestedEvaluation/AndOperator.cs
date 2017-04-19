using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore.Data.NestedEvaluation
{
    public class AndOperator : IBooleanNode
    {
        private IBooleanNode a1;
        private IBooleanNode a2;

        public AndOperator(IBooleanNode arg1, IBooleanNode arg2)
        {
            this.SetArgument1(arg1);
            this.SetArgument2(arg2);
        }

        public bool GetBooleanResult()
        {
            return (bool)this.GetResult();
        }

        public object GetResult()
        {
            return this.a1.GetBooleanResult() && this.a2.GetBooleanResult();
        }

        public void SetArgument1(object arg)
        {
            this.a1 = (IBooleanNode)arg;
        }

        public void SetArgument2(object arg)
        {
            this.a2 = (IBooleanNode)arg;
        }

        public string GetNodeAsString()
        {
            var left = a1 is IEvaluationNode ? string.Format("( {0} )", ((IEvaluationNode)a1).GetNodeAsString()) : a1.ToString();
            var right = a2 is IEvaluationNode ? string.Format("( {0} )", ((IEvaluationNode)a2).GetNodeAsString()) : a2.ToString();

            return string.Format("{0} AND {1}", left, right);
        }

        public Type ResultType
        {
            get { return typeof(bool); }
        }
    }
}

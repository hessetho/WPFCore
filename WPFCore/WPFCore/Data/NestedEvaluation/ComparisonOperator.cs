using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore.Data.NestedEvaluation
{
    public enum BooleanOperatorTypeEnum
    {
        Equal,
        Less,
        EqualOrLess,
        EqualOrGreater,
        Greater,
        NotEqual
    }

    public class ComparisonOperator<T> : IBooleanNode
    {
        private IComparable a1;
        private IComparable a2;
        private BooleanOperatorTypeEnum booleanNodeType;

        public ComparisonOperator(BooleanOperatorTypeEnum booleanNodeType)
        {
            this.booleanNodeType = booleanNodeType;
        }

        public ComparisonOperator(T arg1, T arg2, BooleanOperatorTypeEnum booleanNodeType): this(booleanNodeType)
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
            var r1 = (IComparable)this.Evaluate(a1);
            var r2 = (IComparable)this.Evaluate(a2);

            var result = r1.CompareTo(r2);
            var equal = r1.Equals(r2);

            switch(this.booleanNodeType)
            {
                case BooleanOperatorTypeEnum.Equal:
                    return equal;
                case BooleanOperatorTypeEnum.Less:
                    return result < 0;
                case BooleanOperatorTypeEnum.EqualOrLess:
                    return result < 0 || equal;
                case BooleanOperatorTypeEnum.EqualOrGreater:
                    return result > 0 || equal;
                case BooleanOperatorTypeEnum.Greater:
                    return result > 0;
                case BooleanOperatorTypeEnum.NotEqual:
                    return !equal;
            }

            return false;
        }

        private T Evaluate(object arg)
        {
            if (arg is IBooleanNode)
                return (T)((IBooleanNode)arg).GetResult();

            return (T)arg;
        }

        public void SetArgument1(object arg)
        {
            if(!CheckType(arg))
                throw new ArgumentException("arg must implement IComparable or IEvaluationNode returning the correct type.");

            a1 = (IComparable)arg;
        }

        public void SetArgument2(object arg)
        {
            if (!CheckType(arg))
                throw new ArgumentException("arg must implement IComparable or IEvaluationNode returning the correct type.");

            a2 = (IComparable)arg;
        }

        public Type ResultType
        {
            get { return typeof(bool); }
        }

        private bool CheckType(object arg)
        {
            var argType = arg is IBooleanNode ? ((IBooleanNode)arg).ResultType: arg.GetType();

            // Der Typ muss sowohl <T> entsprechen als auc IComparable implementieren
            if (argType == typeof(T) && (arg is IComparable))
                return true;

            return false;
        }

        public string GetNodeAsString()
        {
            string op = string.Empty;
            switch (this.booleanNodeType)
            {
                case BooleanOperatorTypeEnum.Equal:
                    op = " = ";
                    break;
                case BooleanOperatorTypeEnum.Less:
                    op = " < ";
                    break;
                case BooleanOperatorTypeEnum.EqualOrLess:
                    op = " <= ";
                    break;
                case BooleanOperatorTypeEnum.EqualOrGreater:
                    op = " >= ";
                    break;
                case BooleanOperatorTypeEnum.Greater:
                    op = " > ";
                    break;
                case BooleanOperatorTypeEnum.NotEqual:
                    op = " <> ";
                    break;
            }

            var left = a1 is IEvaluationNode ? string.Format("( {0} )", ((IEvaluationNode)a1).GetNodeAsString()) : a1.ToString();
            var right = a2 is IEvaluationNode ? string.Format("( {0} )", ((IEvaluationNode)a2).GetNodeAsString()) : a2.ToString();

            return string.Format("{0} {1} {2}", left, op, right);
        }
    }
}

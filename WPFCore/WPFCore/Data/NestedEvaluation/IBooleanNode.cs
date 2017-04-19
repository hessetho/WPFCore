using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore.Data.NestedEvaluation
{
    public interface IBooleanNode : IEvaluationNode
    {
        bool GetBooleanResult();
    }
}

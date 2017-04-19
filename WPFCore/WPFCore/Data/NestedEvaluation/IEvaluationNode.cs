using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore.Data.NestedEvaluation
{
    /// <summary>
    /// Stellt Methoden zur Verfügung, um ...
    /// </summary>
    public interface IEvaluationNode
    {
        void SetArgument1(object arg);
        void SetArgument2(object arg);

        object GetResult();

        Type ResultType { get; }

        string GetNodeAsString();
    }
}

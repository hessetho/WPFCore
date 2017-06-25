using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFCore.Data.NestedEvaluation;
using WPFCore.ViewModelSupport;

namespace WPFCoreTest
{
    public class EvaluationTreeTestViewModel : ViewModelCore
    {

        public EvaluationTreeTestViewModel()
        {
            var node1 = new ComparisonOperator<string>("a","b", BooleanOperatorTypeEnum.EqualOrLess);
            var node2 = new ComparisonOperator<string>("c", "d", BooleanOperatorTypeEnum.EqualOrGreater);
            var node3 = new AndOperator(node1, node2);

            Debug.WriteLine(string.Format("{0} = {1}", node1.GetNodeAsString(), node1.GetResult()));
            Debug.WriteLine(string.Format("{0} = {1}", node2.GetNodeAsString(), node2.GetResult()));
            Debug.WriteLine(string.Format("{0} = {1}", node3.GetNodeAsString(), node3.GetResult()));

        }
    }
}

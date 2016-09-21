using System.Collections.Generic;
using System.Linq;

namespace WPFCore.SqlClient
{
    /// <summary>
    /// Represents a stored procedure's definition: Name and parameters
    /// </summary>
    public class SqlProcedure
    {
        /// <summary>
        /// Returns the name of the stpored procedure
        /// </summary>
        public string ProcedureName { get; private set; }
        /// <summary>
        /// Returns the list of the stored procedure's parameters
        /// </summary>
        public List<SqlProcedureParameter> Parameters { get; private set; }

        /// <summary>
        /// Constructor. Prepares an empty parameter list.
        /// </summary>
        /// <param name="name"></param>
        public SqlProcedure(string name)
        {
            this.ProcedureName = name;
            this.Parameters = new List<SqlProcedureParameter>();
        }

        /// <summary>
        /// Checks if a stored procedure's parameter signature matches
        /// a given pattern. The signature is defined by the type (not the name!)
        /// of the parameters, the type is represented by an integer (is this
        /// is used in sql server)
        /// </summary>
        /// <param name="signature"></param>
        /// <returns></returns>
        public bool CheckSignature(List<int> signature)
        {
            if (this.Parameters.Count == signature.Count())
            {
                for (int i = 0; i < signature.Count(); i++)
                    if (this.Parameters[i].ParameterType != signature[i])
                        return false;

                return true;
            }

            return false;
        }
    }
}

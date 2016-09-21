namespace WPFCore.SqlClient
{
    /// <summary>
    /// Represents a parameter for a stored procedure
    /// </summary>
    public class SqlProcedureParameter
    {
        public string ParameterName { get; private set; }
        public int ParameterOrder { get; private set; }
        public int ParameterType { get; private set; }

        public SqlProcedureParameter(string name, int order, int type)
        {
            this.ParameterName = name;
            this.ParameterOrder = order;
            this.ParameterType = type;
        }
    }
}

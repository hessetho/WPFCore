using System.Data.SqlClient;

namespace WPFCore.SqlClient
{
    /// <summary>
    /// This static class provides methods to access internal structures of a <c>TableAdapter</c> and is used
    /// as back end for the classes that implement the interface <see cref="ISqlDataAdapterExtension"/>
    /// </summary>
    /// <remarks>
    /// In order to use the methods of this class with a TableAdapter, extend the TableAdapter
    /// to implement ISqlDataAdapterExtender like this:
    /// <code>
    ///    public partial class ExpressionSetsTableAdapter : AMRisk.SqlClient.ISqlDataAdapterExtension
    ///    {
    ///        public SqlDataAdapter SqlDataAdapter
    ///        {
    ///            get { return this.Adapter; }
    ///        }
    ///
    ///        public SqlCommand SelectCommand
    ///        {
    ///            get { return AMRisk.SqlClient.SqlDataAdapterExtender.GetSelectCommand(this); }
    ///        }
    ///
    ///        public SqlTransaction SqlTransaction
    ///        {
    ///            get { return AMRisk.SqlClient.SqlDataAdapterExtender.GetSqlTransaction(this); }
    ///            set { AMRisk.SqlClient.SqlDataAdapterExtender.SetSqlTransaction(this, value); }
    ///        }
    ///
    ///        SqlCommand[] AMRisk.SqlClient.ISqlDataAdapterExtension.CommandCollection
    ///        {
    ///            get { return this.CommandCollection; }
    ///        }
    ///    }
    /// </code>
    /// Note: In order to extend a TableAdapter, open the data set with the adapter, right-click on the adapter
    /// and select "View Code". VS opens a new editor window with a new "partial class" for the adapter. Be aware
    /// that VS will always create a new partial whenever you select "View Code". It's save to remove empty class
    /// definitions.
    /// </remarks>
    public static class SqlDataAdapterExtender
    {
        /// <summary>
        /// Returns the <c>SelectCommand</c> of a table adapter (which surprisingly is missing on the TableAdapter's interface)
        /// </summary>
        /// <param name="adapter"></param>
        /// <returns></returns>
        public static SqlCommand GetSelectCommand(ISqlDataAdapterExtension adapter)
        {
            // Note: we never need to check whether CommanCollection is initialized as 
            // this is done automatically upon the first call to this property!
            return adapter.CommandCollection[0];
        }

        /// <summary>
        /// Returns the database transaction assigned to a table adapter
        /// </summary>
        /// <param name="adapter"></param>
        /// <returns></returns>
        public static SqlTransaction GetSqlTransaction(ISqlDataAdapterExtension adapter)
        {
            if (adapter.SqlDataAdapter.UpdateCommand != null && adapter.SqlDataAdapter.UpdateCommand.Transaction != null)
                return adapter.SqlDataAdapter.UpdateCommand.Transaction;
            else if (adapter.SqlDataAdapter.InsertCommand != null && adapter.SqlDataAdapter.InsertCommand.Transaction != null)
                return adapter.SqlDataAdapter.InsertCommand.Transaction;
            else if (adapter.SqlDataAdapter.DeleteCommand != null && adapter.SqlDataAdapter.DeleteCommand.Transaction != null)
                return adapter.SqlDataAdapter.DeleteCommand.Transaction;
            else
                return null;
        }

        /// <summary>
        /// Sets a database transaction on a table adapter
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="transaction"></param>
        public static void SetSqlTransaction(ISqlDataAdapterExtension adapter, SqlTransaction transaction)
        {
            if (adapter.SqlDataAdapter.UpdateCommand != null)
            {
                adapter.SqlDataAdapter.UpdateCommand.Transaction = transaction;
                adapter.SqlDataAdapter.UpdateCommand.Connection = transaction.Connection;
            }
            if (adapter.SqlDataAdapter.InsertCommand != null)
            {
                adapter.SqlDataAdapter.InsertCommand.Transaction = transaction;
                adapter.SqlDataAdapter.InsertCommand.Connection = transaction.Connection;
            }
            if (adapter.SqlDataAdapter.DeleteCommand != null)
            {
                adapter.SqlDataAdapter.DeleteCommand.Transaction = transaction;
                adapter.SqlDataAdapter.DeleteCommand.Connection = transaction.Connection;
            }
            if (adapter.SqlDataAdapter.SelectCommand != null)
            {
                adapter.SqlDataAdapter.SelectCommand.Transaction = transaction;
                adapter.SqlDataAdapter.SelectCommand.Connection = transaction.Connection;
            }
        }

    }
}

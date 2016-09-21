using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

namespace WPFCore.SqlClient
{
    /// <summary>
    /// Implements a SQL transaction that is shared among a list of table adapters.
    /// </summary>
    /// <remarks>
    /// A table adapter is represented by the interface <see cref="ISqlDataAdapterExtension"/> that allows
    /// access to internal functionalities of the adapter.
    /// </remarks>
    public class SharedSqlTransaction : IDisposable
    {
        private SqlTransaction transaction;
        private List<ISqlDataAdapterExtension> adapters = new List<ISqlDataAdapterExtension>();

        /// <summary>
        /// Constructor. Creates a named shared transaction for a database.
        /// </summary>
        /// <param name="transactionName">Name of the shared transaction.</param>
        /// <param name="connection">Connection to the database.</param>
        public SharedSqlTransaction(string transactionName, SqlConnection connection)
        {
            this.TransactionName = transactionName;
            this.Connection = connection;
        }

        #region Properties
        /// <summary>
        /// Returns the name of this transaction
        /// </summary>
        public string TransactionName { get; private set; }
        /// <summary>
        /// Returns the database connection assigned to this transaction
        /// </summary>
        public SqlConnection Connection { get; private set; }

        /// <summary>
        /// Returns the list of table adapters which share this transaction
        /// </summary>
        public List<ISqlDataAdapterExtension> Adapters
        {
            get { return this.adapters; }
        }

        /// <summary>
        /// Returns <c>True</c> if a physical database transaction is assigned to this 
        /// <c>SharedSqlTransaction</c> instance, i.e. there's an open transaction registered
        /// on the database
        /// </summary>
        public bool IsInTransaction
        {
            get { return this.transaction != null; }
        }
        #endregion

        /// <summary>
        /// Adds a table adapter to the shared transaction.
        /// </summary>
        /// <remarks>
        /// This effectively opens a database transaction if this is the first table adapter
        /// in the list of this shared transaction group
        /// </remarks>
        /// <param name="adapter"></param>
        public void BeginTransaction(ISqlDataAdapterExtension adapter)
        {
            if (this.adapters.Contains(adapter))
                throw new ArgumentException("adapter is already sharing this transaction.");

            if (this.transaction == null)
                this.transaction = this.Connection.BeginTransaction(this.TransactionName);

            this.adapters.Add(adapter);
            adapter.SqlTransaction = this.transaction;
        }

        /// <summary>
        /// Rolls back any changes and ends the transaction
        /// </summary>
        public void Rollback()
        {
            if (this.transaction == null)
                throw new InvalidOperationException("No transaction opened. Rollback failed");

            this.transaction.Rollback();
            this.EndTransaction();
        }

        /// <summary>
        /// Commits all changes to the database and ends the transaction
        /// </summary>
        public void Commit()
        {
            if (this.transaction == null)
                throw new InvalidOperationException("No transaction opened. Commit failed");

            this.transaction.Commit();
            this.EndTransaction();
        }

        /// <summary>
        /// Disposes the database transaction which effectively closes it
        /// </summary>
        private void EndTransaction()
        {
            if (this.transaction != null)
                this.transaction.Dispose();
            this.transaction = null;
        }

        /// <summary>
        /// Clean up, i.e. close the database transaction
        /// </summary>
        public void Dispose()
        {
            this.EndTransaction();
        }

        /// <summary>
        /// Used for debugging: show all table adapters assigned to this shared transaction
        /// </summary>
        public void DumpStatus()
        {
            foreach (var adapter in this.adapters)
            {
                Debug.WriteLine(string.Format("Adapter: {0}", adapter.GetType().Name));
                Debug.WriteLine(string.Format("  transaction is {0}", adapter.SqlTransaction == null ? "not set" : "set"));

                // adapter.SelectCommand.Transaction == null
            }
        }
    }
}

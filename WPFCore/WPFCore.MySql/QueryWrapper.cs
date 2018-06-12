using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using WPFCore.Helper;
using WPFCore.StatusText;

namespace WPFCore.MySql
{
    public class QueryWrapper
    {
        public delegate MySqlConnection GetOpenConnectionDelegate();
        public delegate T SingleItemQueryFunc<T>(MySqlConnection conn, params object[] parms);
        public delegate List<T> MultiItemQueryFunc<T>(MySqlConnection conn, params object[] parms);
        public delegate TOut SingleItemExecute<T, TOut>(MySqlConnection conn, T item);

        public QueryWrapper(string databaseChannel, GetOpenConnectionDelegate getOpenConnectionDelegate)
        {
            this.DatabaseChannel = databaseChannel;
            this.GetOpenConnection = getOpenConnectionDelegate;
        }

        public string DatabaseChannel { get; private set; }
        public GetOpenConnectionDelegate GetOpenConnection { get; private set; }

        public SingleItemResult<T> RunSingleItemQuery<T>(SingleItemQueryFunc<T> f, params object[] parms)
        {
            try
            {
                using (var conn = this.GetOpenConnection())
                    return new SingleItemResult<T>(f(conn, parms));
            }
            catch (Exception e)
            {
                return new SingleItemResult<T>(default(T), e);
            }
        }

        public MultiItemResult<T> RunMultiItemQuery<T>(MultiItemQueryFunc<T> f, params object[] parms)
        {
            var rdrName = GetThreadName();

            return this.RunMultiItemQueryNamed(f, rdrName, parms);
        }

        public MultiItemResult<T> RunMultiItemQueryNamed<T>(MultiItemQueryFunc<T> f, string readerName, params object[] parms)
        {
            var sw = StopWatch.Start();

            try
            {
                StatusTextBroker.UpdateStatusText(DatabaseChannel, this, string.Format("{0} start reading.", readerName));

                using (var conn = this.GetOpenConnection())
                    return new MultiItemResult<T>(f(conn, parms));
            }
            catch (Exception e)
            {
                return new MultiItemResult<T>(default(List<T>), e);
            }
            finally
            {
                sw.Stop();
                StatusTextBroker.UpdateStatusText(DatabaseChannel, this, string.Format("{0} finished reading, duration {1}", readerName, sw.Elapsed));
            }
        }

        public SingleItemResult<TOut> ExecuteSingleItem<T, TOut>(SingleItemExecute<T, TOut> f, T item)
        {
            try
            {
                using (var conn = this.GetOpenConnection())
                    return new SingleItemResult<TOut>(f(conn, item));
            }
            catch (Exception e)
            {
                return new SingleItemResult<TOut>(default(TOut), e);
            }
        }

        private static string GetThreadName()
        {
            var rdrName = System.Threading.Thread.CurrentThread.Name;
            if (string.IsNullOrEmpty(rdrName))
                rdrName = string.Format("[{0}]", System.Threading.Thread.CurrentThread.ManagedThreadId);
            return rdrName;
        }
    }
}

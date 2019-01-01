using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore.MySql
{
    public static class MySqlHelper
    {
        private const int ER_UNKNOWN_ERROR = -1;
        private const int ER_BAD_HOST_ERROR = 1042;
        private const int ER_ACCESS_DENIED_ERROR = 1045;
        private const int ER_BAD_DB_ERROR = 1049;

        public enum ExceptionReasonEnum
        {
            OtherError,
            UnknownHost,
            AccessDenied,
            UnknownDatabase
        }

        public static ExceptionReasonEnum GetExceptionReason(MySqlException e)
        {
            if (e.Number == 0)
            {
                if (e.InnerException is MySqlException)
                    return GetExceptionReason(e.InnerException as MySqlException);
                return ExceptionReasonEnum.OtherError;
            }

            switch (e.Number)
            {
                case ER_BAD_HOST_ERROR:
                    return ExceptionReasonEnum.UnknownHost;
                case ER_ACCESS_DENIED_ERROR:
                    return ExceptionReasonEnum.AccessDenied;
                case ER_BAD_DB_ERROR:
                    return ExceptionReasonEnum.UnknownDatabase;
            }

            return ExceptionReasonEnum.OtherError;
        }
    }
}

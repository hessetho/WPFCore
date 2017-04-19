using System;
using System.Data.OleDb;

namespace WPFCore.Helper
{
    public static class NumberHelper
    {
        public static Boolean IsNumeric(this Object expression)
        {
            if (expression == null || expression is DateTime)
                return false;

            if (expression is Int16 || expression is Int32 || expression is Int64 || expression is Decimal ||
                expression is Single || expression is Double || expression is Boolean)
                return true;

            double dbl;
            return Double.TryParse(expression.ToString(), out dbl);
        }
    }
}
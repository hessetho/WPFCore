using System.Data.SqlClient;

namespace WPFCore.SqlClient
{
    public interface ISqlDataAdapterExtension
    {
        SqlDataAdapter SqlDataAdapter { get; }
        SqlCommand SelectCommand { get; }
        SqlTransaction SqlTransaction { get; set;  }

        SqlConnection Connection { get; set; }
        SqlCommand[] CommandCollection { get; }
    }
}

using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace LEReports.Utils
{
    public class DBHelper
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        public DBHelper()
        {
        }

        public static DataTable RunSQL(string sql, object parameters = null)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    if (parameters != null)
                    {
                        foreach (PropertyInfo prop in parameters.GetType().GetProperties())
                        {
                            cmd.Parameters.AddWithValue("@" + prop.Name, prop.GetValue(parameters, null) ?? DBNull.Value);
                        }
                    }
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    da.Fill(ds, "ds");                    
                    return ds.Tables["ds"];
                }
            }
        }
    }
}
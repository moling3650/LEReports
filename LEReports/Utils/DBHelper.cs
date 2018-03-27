using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace LEReports.Utils
{
    public class DBHelper
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        public DBHelper()
        {
        }

        public static DataTable getDataTable(string sql, JToken parameters = null, bool isProcedure = false)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = isProcedure ? CommandType.StoredProcedure : CommandType.Text;
                    cmd.CommandText = sql;
                    if (parameters != null)
                    {
                        foreach (JProperty item in parameters)
                        {
                            switch (item.Value.Type)
                            {
                                case JTokenType.Boolean:
                                    break;
                                case JTokenType.Date:
                                    var dateValue = DateTime.SpecifyKind(item.Value.ToObject<DateTime>(), DateTimeKind.Utc).ToLocalTime();
                                    cmd.Parameters.AddWithValue("@" + item.Name, dateValue);
                                    break;
                                case JTokenType.Float:
                                    cmd.Parameters.AddWithValue("@" + item.Name, item.Value.ToObject<decimal>());
                                    break;
                                case JTokenType.Integer:
                                    cmd.Parameters.AddWithValue("@" + item.Name, item.Value.ToObject<int>());
                                    break;
                                case JTokenType.String:
                                    cmd.Parameters.AddWithValue("@" + item.Name, item.Value.ToString());
                                    break;
                                default:
                                    cmd.Parameters.AddWithValue("@" + item.Name, DBNull.Value);
                                    break;
                            }
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
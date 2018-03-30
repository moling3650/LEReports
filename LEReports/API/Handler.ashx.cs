using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LEReports.Utils;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace LEReports.API
{
    /// <summary>
    /// Handler 的摘要说明
    /// </summary>
    public class Handler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            // 获取json对象
            Stream content = context.Request.InputStream;
            byte[] byteContent = new byte[content.Length];
            content.Read(byteContent, 0, (int)content.Length);
            var strJson = Encoding.UTF8.GetString(byteContent);
            var json = string.IsNullOrEmpty(strJson) ? null : JObject.Parse(strJson);

            var method = context.Request.QueryString["method"].ToString();
            var sql = string.Empty;
            var isProcedure = false;
            switch (method)
            {
                case "GetReports":
                    sql = "SELECT * FROM B_Report";
                    break;
                case "DeleteReport":
                    sql = "DELETE B_Report_Field WHERE report_code = @reportCode;DELETE B_Report WHERE report_code = @reportCode";
                    break;
                case "ValidReport":
                    sql = "SELECT (SELECT COUNT(*) FROM B_Report WHERE report_code = @reportCode) AS hasReport, OBJECT_ID(@reportCode) AS hasObject";
                    break;
                case "SaveReport":
                    sql = "DELETE B_Report WHERE report_code = @report_code;INSERT INTO B_Report (report_code,report_name,query_type,query_sql) VALUES (@report_code,@report_name,1,@query_sql)";
                    break;
                case "UpdateReportField":
                    sql = "UPDATE B_Report_Field SET label = @label,width = @width,idx = @idx,align = @align,state = @state,is_check = @is_check,control_code = @control_code,control_span = @control_span,required = @required,options_api = @options_api WHERE report_code = @report_code AND prop = @prop";
                    break;
                case "GetQueryControls":
                    sql = "SELECT control_code AS value, control_name AS label FROM B_Report_Query_Control";
                    break;
                case "GetRecords":
                    sql = json != null ? json["sql"].ToString() : string.Empty;
                    break;
                case "GetReportFields":
                    sql = "Z_GetReportFields";
                    isProcedure = true;
                    break;
                case "GetChartsByReport":
                    sql = "SELECT * FROM B_Chart WHERE report_code = @reportCode";
                    break;
                case "SaveChart":
                    var id = json != null ? json["id"].ToObject<int>() : -1;
                    if (id <= 0)
                    {
                        sql = "INSERT INTO B_Chart(type, report_code, title, label, value) VALUES(@type, @report_code, @title, @label, @value)";
                    }
                    else
                    {
                        sql = "UPDATE B_Chart SET type = @type, title = @title, label = @label, value = @value WHERE id = @id";
                    }
                    break;
                case "GetChartTypes":
                    sql = "SELECT * FROM B_ChartType";
                    break;
                case "DeleteChartById":
                    sql = "DELETE FROM B_Chart WHERE id = @id";
                    break;   
                default:
                    break;
            }
            context.Response.Write(JSONHelper.ToJson(DBHelper.getDataTable(sql, json, isProcedure)));
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
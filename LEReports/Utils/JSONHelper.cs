using Newtonsoft.Json;

namespace LEReports.Utils
{
    public class JSONHelper
    {
        /// <summary>
        /// 将JSON转换为指定类型的对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="strJson">json字符串</param>
        /// <returns>json对象</returns>
        public static T ConvertToObject<T>(string strJson)
        {
            var setting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.DeserializeObject<T>(strJson, setting);
        }

        /// <summary>
        /// 生成压缩的json 字符串
        /// </summary>
        /// <param name="obj">json的对象</param>
        /// <returns>json字符串</returns>
        public static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
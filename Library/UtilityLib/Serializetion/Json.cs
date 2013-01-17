using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web.Script.Serialization;
using System.Web;

namespace UtilityLib.Serializetion
{
    /// <summary>
    /// JSON 扩展
    /// </summary>
    public static class Json
    {
        /// <summary>
        /// 转换成 JSON
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ToJson(this object o)
        {
            var serializer = new JavaScriptSerializer();
            
            return serializer.Serialize(o);
        }

        /// <summary>
        /// 转换成 JSON
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ToUrlEncodeJson(this object o)
        {
            var serializer = new JavaScriptSerializer();
            var jason = serializer.Serialize(o);
            return HttpUtility.UrlEncode(jason, Encoding.UTF8);
        }

        /// <summary>
        /// 将 JSON 转换为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public static T FromJson<T>(this string s)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(s);
        }

        /// <summary>
        /// 将 JSON 转换为对象
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static object FromJson(this string s)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.DeserializeObject(s);
        }
    }
}

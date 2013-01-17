using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilityLib.Extensions
{
    public static class String
    {
        /// <summary>
        /// 对base64的编码进行解码
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string DecodeBase64(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            try
            {
                var base64 = Convert.FromBase64String(text);
                return System.Text.Encoding.UTF8.GetString(base64);
            }
            catch (Exception ex)
            {
                Console.WriteLine("base64解码错误");
            }

            return text;
        }

        /// <summary>
        /// 把字符串转换成Base64编码
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string EncodeBase64(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var bytes = System.Text.Encoding.UTF8.GetBytes(text);

            var base64 = Convert.ToBase64String(bytes);

            return base64;
        }
    }
}

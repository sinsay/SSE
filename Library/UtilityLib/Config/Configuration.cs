using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace UtilityLib.Config
{
    public class Configuration
    {
        /// <summary>
        /// 获取配置文件中当前键值对应的数据库连接
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns>数据库连接字符串</returns>
        public static string ConnectionStrings(string key)
        {
            return ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }

        /// <summary>
        /// 获取配置文件中当前键值对应的数据库设置
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns>数据库设置</returns>
        public static ConnectionStringSettings ConnectionStringSettings(string key)
        {
            return ConfigurationManager.ConnectionStrings[key];
        }

        /// <summary>
        /// <para>获取配置文件中当前键值对应的值，并转换为相应的类型</para>
        /// <para>当配置项为空时，自动转换为该类型的默认值</para>
        /// </summary>
        /// <typeparam name="T">想要转换的类型</typeparam>
        /// <param name="key">键值</param>
        /// <returns>配置项值</returns>
        public static T AppSettings<T>(string key)
        {
            return AppSettings<T>(key, default(T));
        }

        /// <summary>
        /// <para>获取配置文件中当前键值对应的值，并转换为相应的类型</para>
        /// <para>当配置项为空，返回传入的默认值</para>
        /// </summary>
        /// <typeparam name="T">想要转换的类型</typeparam>
        /// <param name="key">键值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>配置项值</returns>
        public static T AppSettings<T>(string key, T defaultValue)
        {
            var v = ConfigurationManager.AppSettings[key];
            return String.IsNullOrEmpty(v) ? defaultValue : (T)Convert.ChangeType(v, typeof(T));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilityLib.Converter
{
    public class DateTimeConverter
    {
        private static readonly DateTime DefaultDT = DateTime.Parse("1970-1-1");

        /// <summary>
        /// 获取 dt 到默认时间的间隔（精确到秒）
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long TimeSpan4Second(DateTime dt)
        {
            return TimeSpan4Second(DefaultDT, dt);
        }

        /// <summary>
        /// 获取 dt 到默认时间的间隔（精确到分钟）
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long TimeSpan4Minute(DateTime dt)
        {
            return TimeSpan4Second(dt) / 60;
        }

        /// <summary>
        /// 获取 dt1 到 dt2 的时间间隔（精确到秒）
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        public static long TimeSpan4Second(DateTime dt1, DateTime dt2)
        {
            return (long)(dt1 - dt2).TotalSeconds;
        }

        /// <summary>
        /// 获取 dt1 到 dt2 的时间间隔（精确到分钟）
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        public static long TimeSpan4Minute(DateTime dt1, DateTime dt2)
        {
            return TimeSpan4Second(dt1, dt2) / 60;
        }
    }
}

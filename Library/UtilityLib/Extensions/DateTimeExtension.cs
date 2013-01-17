using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilityLib.Extensions
{
    public static class DateTimeExtension
    {
        private static DateTime DefaultDateTime = DateTime.Parse("2000-1-1");

        /// <summary>
        /// 获取默认时间，时间默认从2K年开始。
        /// </summary>
        /// <returns></returns>
        public static DateTime DefaultTime()
        {
            return DefaultDateTime;
        }

        /// <summary>
        /// 获取与 DefaultTime 之间的间隔（天数）
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static double GetCompareTime(this DateTime self)
        {
            return (self.ToShortDate() - DefaultDateTime).TotalDays;
        }

        /// <summary>
        /// 获取与 DefaultTime 之间的间隔（毫秒数）
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static double GetCompareTimeExtractMill(this DateTime self)
        {
            return (self - DefaultDateTime).TotalMilliseconds;
        }

        /// <summary>
        /// 获取与 DefaultTime 之间的间隔（秒数）
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static double GetCompareTimeExtractSec(this DateTime self)
        {
            return (self - DefaultDateTime).TotalSeconds;
        }

        /// <summary>
        /// 获取与 DefaultTime 之间的间隔（分钟数）
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static double GetCompareTimeExtractMin(this DateTime self)
        {
            return (self - DefaultDateTime).TotalMinutes;
        }

        /// <summary>
        /// 获取与 DefaultTime 之间的间隔（小时数）
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static double GetCompareTimeExtractHour(this DateTime self)
        {
            return (self - DefaultDateTime).TotalHours;
        }

        /// <summary>
        /// 转换到日期模式（即把小时、分钟数。秒数都清空）
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static DateTime ToShortDate(this DateTime self)
        {
            return new DateTime(self.Year, self.Month, self.Day);
        }

        public static DateTime RestoreFromSec(double l)
        {
            return DefaultDateTime.AddSeconds(l);
        }
    }
}

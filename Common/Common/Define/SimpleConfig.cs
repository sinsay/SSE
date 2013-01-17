using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilityLib.Config;

namespace Common.Define
{
    /// <summary>
    /// 通用配置项
    /// </summary>
    public static class SimpleConfig
    {
        /// <summary>
        /// Shell 名称
        /// </summary>
        public static string SimpleShellName
        {
            get { return Configuration.AppSettings<string>("SimpleShellName", "Shell.exe"); }
        }

        /// <summary>
        /// 进程报告的间隔，精确到秒数
        /// </summary>
        public static int ReportInterval
        {
            get { return Configuration.AppSettings<int>("ReportInterval", 30); }
        }

        /// <summary>
        /// 保存已运行进程信息的目录
        /// </summary>
        public static string ProcessStatePath 
        {
            get { return Configuration.AppSettings<string>("ProcessesStatePath", "Process"); }
        }

        /// <summary>
        /// 索引的磁盘路径
        /// </summary>
        public static string IndexRootPath
        {
            get { return Configuration.AppSettings<string>("IndexRootPath"); }
        }
    }
}

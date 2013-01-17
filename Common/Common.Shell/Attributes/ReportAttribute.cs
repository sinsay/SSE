using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;
using Common.Define;
using Common.Attributes;
using Common;

namespace Common.Shell.Attributes
{
    public class ReportAttribute : BaseAttribute
    {
        /// <summary>
        /// 自定义报告操作
        /// </summary>
        public Action<object> ReportAction { get; set; }

        /// <summary>
        /// 报告操作所需的上下文信息
        /// </summary>
        public IContext ReportContext { get; set; }

        /// <summary>
        /// 父进程ID
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 报告器的名称
        /// </summary>
        public string Name { get; set; }

        #region Constructor

        public ReportAttribute(Action<object> reportAction, IContext reportContext)
            : this()
        {
            this.ReportAction = reportAction;
            this.ReportContext = reportContext;
        }

        public ReportAttribute() 
        {
            this.Name = "default";
        }

        #endregion

        public override void Execute(IContext context)
        {
            StartupContext sc = context as StartupContext;
            if (sc == null)
                return;

            var parentMatch = Regex.Match(sc.GetCmd(), @"ParentId=(?<ParentId>[\d]+)");
            if (parentMatch.Success)
            {
                sc.ParentProcId = int.Parse(parentMatch.Groups["ParentId"].Value);

                // 如果自定义了报告操作，则执行自定义操作
                if (this.ReportAction != null)
                {
                    this.ReportAction(this.ReportContext);
                }
                else // 否则执行默认操作，定时发送心跳，仅为测试用
                {
                    new Thread(new ThreadStart(() =>
                    {
                        while (true)
                        {
                            try
                            {
                                var msg = string.Format("{1} Send report to {0}, Means im ok!",
                                    sc.ParentProcId, this.GetReportInfo());
                                Console.Error.WriteLine(msg); // 写到标准错误流
                            }
                            catch (Exception e)
                            {
                                // DO STH
                            }

                            Thread.Sleep(TimeSpan.FromSeconds(SimpleConfig.ReportInterval)); // 每隔一分钟像父进程报告一次
                        }
                    }))
                    {
                        IsBackground = true,
                        Name = this.GetReportInfo()
                    }.Start();
                }
            }
        }

        private string GetReportInfo()
        {
            return string.Format("Reporter.ProcId.{0}.{1}",
                            Process.GetCurrentProcess().Id, this.Name);
        }
    }
}

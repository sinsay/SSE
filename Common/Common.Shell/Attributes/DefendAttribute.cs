using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Attributes;
using UtilityLib.Reflection;
using System.Diagnostics;
using Common.Shell.Processor;
using Common;

namespace Common.Shell.Attributes
{
    /// <summary>
    /// 任务守护特性
    /// 增加此特性的任务会由其他的守护程序对其进行保护
    /// </summary>
    [Priority(-1)]  // 保证守护进程在处理完所有事情才启动
    public class DefendAttribute: BaseAttribute
    {
 
        /// <summary>
        /// 负责任务进程心跳消息的处理器，只有当任务进程启用了 Report 特性才会开启
        /// Process 是发送消息的进程， string 是收到的消息
        /// </summary>
        public Action<Process, string> MessageProcessor { get; set; }
        
        /// <summary>
        /// action 是负责任务进程心跳消息的处理器，只有当任务进程启用了 Report 特性才会开启
        /// Process 是发送消息的进程， string 是收到的消息
        /// </summary>
        /// <param name="action"></param>
        public DefendAttribute(Action<Process, string> action)
        {
            this.MessageProcessor = action;
        }

        public DefendAttribute() { }

        public override void Execute(IContext context)
        {
            StartupContext c = context as StartupContext;
            if (c != null)
            {
                // 为子进程提供父进程信息
                c.AddArg("ParentId=" + Process.GetCurrentProcess().Id);
                c.ParentProcId = Process.GetCurrentProcess().Id;
                new DefendProcessor(this.MessageProcessor).Process(c);
            }
        }
    }
}

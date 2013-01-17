using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace Common.Shell
{
    [Serializable]
    public class StartupContext : IContext, IEquatable<StartupContext>
    {
        /// <summary>
        /// 程序启动命令
        /// </summary>
        public string StartCmd { get; set; }

        /// <summary>
        /// 执行的任务名称
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 父进程ID
        /// </summary>
        public int ParentProcId { get; set; }

        /// <summary>
        /// 附加的启动命令
        /// </summary>
        private List<string> AdditionArgs { get; set; }

        public StartupContext(string startCmd, string taskName)
        {
            this.StartCmd = startCmd;
            this.TaskName = taskName;
            this.AdditionArgs =  new List<string>();
        }

        /// <summary>
        /// 添加附加的启动参数
        /// </summary>
        /// <param name="arg"></param>
        public void AddArg(string arg)
        {
            this.AdditionArgs.Add(arg);
        }

        public void AddArgRange(IEnumerable<string> args)
        {
            foreach (var arg in args)
            {
                this.AddArg(arg);
            }
        }

        public bool ContainArg(string arg)
        {
            return string.IsNullOrEmpty(arg) || this.StartCmd == arg || this.AdditionArgs.Contains(arg);
        }

        /// <summary>
        /// 获取当前上下文配置的启动命令
        /// </summary>
        /// <returns></returns>
        public string GetCmd()
        {
            string cmd = string.Empty;

            if (!string.IsNullOrEmpty(this.StartCmd))
            {
                var args = new List<string>();
                args.Add(this.StartCmd);
                args.AddRange(this.AdditionArgs);
                cmd = string.Join(" ", args.Distinct().ToArray());
            }

            return cmd;
        }


        public bool Equals(StartupContext other)
        {
            bool equal = false;
            if (other != null)
            {
                equal = string.Format("{0}-{1}", this.GetCmd(), this.TaskName) ==
                    string.Format("{0}-{1}", other.GetCmd(), other.TaskName);
            }
            return equal;
        }
    }
}

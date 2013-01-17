using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Attributes;
using UtilityLib;
using System.IO;
using Common.Define;
using Common;

namespace Common.Shell.Processor
{
    public class DefendProcessor: IProcessor
    {
        /// <summary>
        /// 默认的消息处理，测试用
        /// </summary>
        private static Action<System.Diagnostics.Process, string> DefaultProcessor = new Action<System.Diagnostics.Process,string> ( (proc, data) =>
            {
                string msg = string.IsNullOrEmpty(data) ? "Empty Msg" : data;
                Console.WriteLine("Current ProcId.{3}, receive from process Id.{0} send msg.{1}, time.{2}", 
                    proc.Id.ToString(), msg, DateTime.Now.ToString(), System.Diagnostics.Process.GetCurrentProcess().Id);
            });

        #region Constructor

        public DefendProcessor()
            : this(DefaultProcessor)
        { }

        public DefendProcessor(Action<System.Diagnostics.Process, string> messageProcessor)
        {
            if (messageProcessor != null)
            {
                this.MessageProcessor = messageProcessor;
            }
            else
            {
                this.MessageProcessor = DefendProcessor.DefaultProcessor;
            }
        }

        #endregion

        #region Property

        /// <summary>
        /// 守护的进程ID
        /// </summary>
        public int ProcId { get; set; }

        /// <summary>
        /// 正在执行的进程
        /// </summary>
        public System.Diagnostics.Process CurrentProc { get; set; }

        /// <summary>
        /// 进程的执行时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 进程通信处理器
        /// </summary>
        public Action<System.Diagnostics.Process, string> MessageProcessor { get; set; }

        private string WithoutDefendCmd = "--WithoutDefend";

        #endregion

        #region Defend Method

        /// <summary>
        /// 开始守护
        /// </summary>
        /// <param name="icontext"></param>
        public void Process(IContext icontext)
        {
            StartupContext context = icontext as StartupContext;
            if (context == null || context.ContainArg(WithoutDefendCmd))  // 如果上下文为空，或者不需要守护则退出
            {
                // 初始化未处理异常处理器
                this.InitialUnHandlerExceptionHandler();
                return;
            }

            string cmd = context.GetCmd();
            if (string.IsNullOrEmpty(cmd))
            {
                Console.WriteLine("没有启动信息，程序即将退出。");
            }

            this.StartAndDefend(context);
        }

        /// <summary>
        /// 初始化未处理异常的处理器
        /// </summary>
        private void InitialUnHandlerExceptionHandler()
        {
            System.AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler((o, e) =>
            {

            });
        }

        /// <summary>
        /// 检查是否有迷失的任务进程
        /// </summary>
        /// <param name="context"></param>
        private void CheckLostChild(StartupContext context)
        {
            if (!Directory.Exists(SimpleConfig.ProcessStatePath))
            {
                return;
            }

            foreach (var procInfo in Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, SimpleConfig.ProcessStatePath)))
            {
                System.Diagnostics.Process p;
                int pid = 0;
                if (!int.TryParse( // 如果格式化为整型失败
                    procInfo.Substring(procInfo.LastIndexOf(Path.DirectorySeparatorChar) + 1), 
                    out pid))
                {
                    
                    try
                    {
                        File.Delete(procInfo);
                    }
                    catch { }
                    continue;
                }
                try
                {
                    p = System.Diagnostics.Process.GetProcessById(pid);
                }
                catch (Exception notFoundEx)
                {
                    // 说明该进程已退出
                    File.Delete(procInfo);
                    continue;
                }

                StartupContext pContext = null;
                try
                {
                    pContext =UtilityLib.Serializetion.Binary<StartupContext>.ReadObject(
                        Path.Combine(Environment.CurrentDirectory, SimpleConfig.ProcessStatePath, pid.ToString()));
                }
                catch (Exception formatEx)
                {
                    // 序列化失败，文件破坏，删除进程文件，退出迷失的进程
                    File.Delete(procInfo);
                    p.Kill();
                }

                if (context.Equals(pContext))
                {
                    // 重新接管该进程
                    this.TakeOverProc(pContext, p);
                    return;
                }
            }
        }

        /// <summary>
        /// 重新接管迷失的进程
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="proc"></param>
        private void TakeOverProc(StartupContext pContext, System.Diagnostics.Process proc)
        {
            this.ProcId = proc.Id;
            this.CurrentProc = proc;
            this.StartTime = proc.StartTime;
        }

        /// <summary>
        /// 开始守护过程
        /// </summary>
        /// <param name="process"></param>
        /// <param name="context"></param>
        private void StartAndDefend(StartupContext context)
        {
            // 启动失败或出错的时候会尝试继续守护
            // 开始守护
            while (true)
            {
                bool failed = false;
                if (this.StartProcess(context) && !failed) // 如果进程已退出，则重启进程
                {
                    try
                    {
                        // 如果进程已退出，则failed为true
                        failed = this.CurrentProc.WaitForExit((int)TimeSpan.FromSeconds(30).TotalMilliseconds); // 等待程序退出30秒
                    }
                    catch { }
                }
                
                if(failed)
                {
                    // 程序启动失败，等待10秒后重试
                    try
                    {
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(10));
                        this.BuildProcess(context); // 启动失败则重置进程信息
                    }
                    catch(Exception e) 
                    {

                    }
                }
            }
        }

        #endregion

        #region Handle Process

        /// <summary>
        /// 启动进程
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool StartProcess(StartupContext context)
        {
            // 检查是否有丢失的进程（即失去守护进程的）
            this.CheckLostChild(context);

            bool success = false;
            try
            {
                // 查看任务进程的状态
                success = !this.CurrentProc.HasExited;
            }
            catch { }

            if (!success) // 如果任务进程非正常执行则重新执行任务进程
            {
                try
                {
                    this.BuildProcess(context);
                    success = this.CurrentProc.Start();
                    if (success)
                    {
                        this.CurrentProc.BeginErrorReadLine();
                        this.ProcId = this.CurrentProc.Id;
                        this.StartTime = this.CurrentProc.StartTime;

                        // 保存正在运行的进程信息
                        StoreProcInfo(context, this.ProcId);

                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2)); // 等待两秒，待任务进程启动
                        Console.Title = string.Format("守护进程.For.ProcId.{0}.{1} At.{2}",
                            this.ProcId, context.TaskName, Environment.CurrentDirectory);
                    }
                }
                catch (Exception e)
                {
                    // DO STH
                }
            }
            return success;
        }

        /// <summary>
        /// 保存进程信息到磁盘
        /// </summary>
        /// <param name="context"></param>
        /// <param name="p"></param>
        private void StoreProcInfo(StartupContext context, int pid)
        {
            // 说明进程不存在
            if (context == null || pid <= 0)
            {
                return;
            }

            if (!Directory.Exists(SimpleConfig.ProcessStatePath))
            {
                Directory.CreateDirectory(SimpleConfig.ProcessStatePath);
            }

            string path = Path.Combine(Environment.CurrentDirectory, SimpleConfig.ProcessStatePath, pid.ToString());
            if(File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch { }
            }

            
            try
            {
                UtilityLib.Serializetion.Binary<StartupContext>.WriteObject(context, path);
            }
            catch (Exception e)
            {
                // DO STH
            }
        }

        /// <summary>
        /// 构建任务进程
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private void BuildProcess(StartupContext context)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();

            // 为子任务进程增加免守护标识
            context.AddArg(WithoutDefendCmd);

            // 配置进程信息，父子进程通过重定向错误输出来通信
            process.StartInfo = new System.Diagnostics.ProcessStartInfo(
                System.IO.Path.Combine(Environment.CurrentDirectory, SimpleConfig.SimpleShellName),
                context.GetCmd())
                {
                    WorkingDirectory = System.Environment.CurrentDirectory ,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };

            // 为子进程增加事件处理
            process.EnableRaisingEvents = true;
            process.Exited += new EventHandler(process_Exited); // 注册退出处理事件
            process.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler(process_OutputDataReceived); // 注册通信处理事件

            this.CurrentProc = process;
        }

        /// <summary>
        /// 关闭进程信息
        /// </summary>
        private void CloseProcess()
        {
            try
            {
                this.CurrentProc.CancelErrorRead();
                this.CurrentProc.Refresh();

            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Event Handler

        /// <summary>
        /// 进程退出处理方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void process_Exited(object sender, EventArgs e)
        {
            try
            {
                string msg = string.Format("process Id.{2} ended, process start time.{0}, endtime.{1}, start args.{3}",
                    this.StartTime, DateTime.Now, this.ProcId, this.CurrentProc.StartInfo.Arguments);
                try
                {
                    this.CloseProcess();
                }
                catch (Exception ee)
                {
                    // DO STH
                }
            }
            catch(Exception msgE)
            {
                Console.WriteLine("error occur while exit process, msg:{0}", msgE.Message);
            }
        }

        /// <summary>
        /// 进程通信处理方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void process_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {          
            this.MessageProcessor(sender as System.Diagnostics.Process, e.Data);
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilityLib;
using UtilityLib.Reflection;
using Common.Attributes;

namespace Common.Shell
{
    public class Application
    {
        IDictionary<string, Type> taskDic = new Dictionary<string, Type>();
        IDictionary<string, string> describes = new Dictionary<string, string>();

        public string Name { get; set; }

        public void Execute(string task, List<string> args)
        {
            if (!String.IsNullOrEmpty(task) && taskDic.ContainsKey(task.ToUpper()))
            {
                IExecutable exe = null;
                try
                {
                    var type = taskDic[task.ToUpper()] as Type;
                    exe = UtilityLib.Reflection.Reflection.CreateInstance(null, type) as IExecutable;

                    // 获取任务的特性，根据特性执行程序
                    ExecuteByAttribute(exe, task, args == null ? new string[0] : args.ToArray());

                    Console.Title = describes[task.ToUpper()] + " : " + Environment.CurrentDirectory;
                    try
                    {
                        exe.Execute(args);
                    }
                    catch (Exception e)
                    {
                        // DO STH
                    }
                }
                catch { }
                finally
                {
                    if (exe != null && exe is IDisposable) (exe as IDisposable).Dispose();
                }
            }
            else
            {
                ShowTasks();
            }
        }

        /// <summary>
        /// 根据特性来执行任务
        /// </summary>
        /// <param name="exe"></param>
        private void ExecuteByAttribute(IExecutable exe, string cmd, string[] args)
        {
            StartupContext context = new StartupContext(cmd, exe.Name);
            context.AddArgRange(args);

            ObjectProcessor<BaseAttribute> op = new ObjectProcessor<BaseAttribute>();
            op.ProcessAttr += attr => 
            {
                try
                {
                    attr.Execute(context);
                }
                catch (Exception e)
                {
                    // DO STH
                }
            };
            op.Process(exe); // 开始处理任务特性
        }

        public void RegisterTask<T>(string key, string name) where T : IExecutable
        {
            var type = typeof(T);
            this.RegisterTask(key, name, type);
        }

        public void RegisterTask(string key, string name, Type type)
        {
            key = key.ToUpper();
            this.taskDic.Add(key, type);
            this.describes.Add(key, name);
        }

        private void ShowTasks()
        {
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(this.Name);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("-----------------------------------------");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("用法: {0} 参数", AppDomain.CurrentDomain.FriendlyName);
            Console.WriteLine("参数说明:");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("-----------------------------------------");

            Console.ForegroundColor = ConsoleColor.Green;

            foreach (var item in describes)
            {
                Console.WriteLine("-{0} : {1} ", item.Key, item.Value);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("-----------------------------------------");

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("请选择:");

            Console.ForegroundColor = ConsoleColor.White;
            var task = Console.ReadLine();
            if (task != null)
            {
                var args = task.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                var list = new List<string>(args);
                var taskname = list[0];
                list.RemoveAt(0);
                Execute(taskname, list);
            }
        }
    }
}
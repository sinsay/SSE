using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace Indexer
{
    /// <summary>
    /// 索引管理器，支持按时间划分索引文件，限制每个文件夹最大的索引文章数
    /// </summary>
    public class IndexDirectoryManager
    {
        /// <summary>
        /// 索引的根目录
        /// </summary>
        public string IndexDirectory { get; private set; }

        /// <summary>
        /// 每个索引块的最大文档数
        /// </summary>
        public int MaxDocPerDirectory { get; private set; }


        #region Constructor

        public IndexDirectoryManager(string indexDirectory) : this(indexDirectory, 5000000) { }

        public IndexDirectoryManager(string indexDirectory, int maxDocPerDirectory)
        {
            this.MaxDocPerDirectory = maxDocPerDirectory;
            this.IndexDirectory = indexDirectory;
        }

        #endregion

        #region Publish Method

        /// <summary>
        /// 返回当前索引目录的文档数量
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private int readIndexCount(string path)
        {
            var count = 0;

            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(path);
                if (lines.Length != 0)
                    int.TryParse(lines[0], out count);
            }

            return count;
        }

        public int ReadIndexCountOnDir(string path)
        {
            var count = 0;
            if (Directory.Exists(path))
            {
                var infoFile = Path.Combine(path, "DirInfo.info");
                if (File.Exists(infoFile))
                {
                    count = readIndexCount(infoFile);
                }
            }
            else if (File.Exists(path))
            {
                count = readIndexCount(path);
            }

            return count;
        }

        /// <summary>
        /// 写入本次索引的信息
        /// </summary>
        /// <param name="count"></param>
        public void WriteIndexInfo(int count)
        {
            WriteIndexInfo(count, DateTime.Now);
        }

        /// <summary>
        /// 将本次索引的信息写入date月份的索引文件夹中
        /// </summary>
        /// <param name="count"></param>
        /// <param name="date"></param>
        public void WriteIndexInfo(int count, DateTime date)
        {
            var indexPath = GetIndexPathByDate(date);
            var parentDirectory = indexPath.Substring(0, indexPath.LastIndexOf(Path.DirectorySeparatorChar));
            WriteIndexInfoOnDir(indexPath, count, string.Empty);
            WriteIndexInfoOnDir(parentDirectory, count, indexPath);
        }

        /// <summary>
        /// 根据日期获取索引目录
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string GetIndexPathByDate(DateTime date)
        {
            var path = Path.Combine(this.IndexDirectory, date.ToString("yyyy-MM"));

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                this.WriteIndexInfoOnDir(path, 0, Path.Combine(path, "0"));
            }

            //获取当前目录的信息
            var count = 0;
            var lastIndex = DateTime.Parse("1970-1-1");
            var lastModifySubDir = string.Empty;
            getIndexDirInfo(Path.Combine(path, "DirInfo.info"), ref count, ref lastIndex, ref lastModifySubDir);

            //如果文件夹里尚没有索引
            if (!Directory.Exists(lastModifySubDir))
            {
                lastModifySubDir = Path.Combine(path, "0");
                Directory.CreateDirectory(lastModifySubDir);
            }

            //获取正确的目录
            var indexPath = string.Empty;
            getIndexDirInfo(Path.Combine(lastModifySubDir, "DirInfo.info"), ref count, ref lastIndex, ref lastModifySubDir);
            if (count > this.MaxDocPerDirectory)
            {
                var fileName = new DirectoryInfo(lastModifySubDir).Name;
                var newid = int.Parse(fileName) + 1;
                var newPath = Path.Combine(path, newid.ToString());

                while (Directory.Exists(newPath))
                {
                    var newCount = 0;
                    var newDate = DateTime.Now;
                    var newLastDir = string.Empty;
                    getIndexDirInfo(newPath, ref newCount, ref newDate, ref newLastDir);
                    if (newCount > this.MaxDocPerDirectory)
                    {
                        newid++;
                        newPath = Path.Combine(path, newid.ToString());
                    }
                    else
                    {
                        break;
                    }
                }
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                    indexPath = newPath;
                }
                if (string.IsNullOrEmpty(indexPath))
                {
                    indexPath = newPath;
                }
            }
            else
            {
                indexPath = lastModifySubDir;
            }

            return indexPath;
        }

        /// <summary>
        /// 获取最新的索引目录
        /// </summary>
        /// <returns></returns>
        public string GetIndexPath()
        {
            return GetIndexPathByDate(DateTime.Now);
        }

        /// <summary>
        /// 返回所有时间段的索引目录
        /// </summary>
        /// <returns></returns>
        public string[] GetIndexPath4Query()
        {
            List<string> directories = new List<string>();
            try
            {
                var dirs = Directory.GetDirectories(this.IndexDirectory); //获取所有月份目录
                foreach (var item in dirs)
                {
                    var subDirs = Directory.GetDirectories(item);
                    if (subDirs != null && subDirs.Length > 0)
                    {
                        directories.AddRange(subDirs);
                    }
                }
            }
            catch { }

            return directories.ToArray();
        }

        /// <summary>
        /// 返回指定时间段内的索引目录
        /// 上下区间均闭合
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public string[] GetIndexPath4Query(DateTime beginDate, DateTime endDate)
        {
            List<string> directories = new List<string>();
            var refStr = string.Empty;
            try
            {
                var begin = new DateTime(beginDate.Year, beginDate.Month, 1);
                var end = new DateTime(endDate.Year, endDate.Month, 1);
                var dirs = Directory.GetDirectories(this.IndexDirectory)
                    .Where(s =>
                    {
                        var dateStr = s.Substring(s.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                        var dt = DateTime.Parse(dateStr);
                        return dt <= end && dt >= begin;
                    });

                foreach (var item in dirs)
                {
                    var subDirs = Directory.GetDirectories(item);
                    if (subDirs != null && subDirs.Length > 0)
                    {
                        directories.AddRange(subDirs);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("获取检索路径出错，错误信息:{0}", e.Message);
            }

            return directories.ToArray();
        }

        public void ResetIndex(DateTime date)
        {
            var dir = this.GetIndexPathByDate(date);
            var parentDirectory = dir.Substring(0, dir.LastIndexOf(Path.DirectorySeparatorChar));
            System.IO.Directory.Delete(parentDirectory, true);
        }

        public void ReCaculateInfo(DateTime dt)
        {
            var dir = this.GetIndexPathByDate(dt);
            var parentDir = dir.Substring(0, dir.LastIndexOf(Path.DirectorySeparatorChar));
            var count = 0;
            foreach (var item in Directory.GetDirectories(parentDir))
            {
                count += this.ReadIndexCountOnDir(item);
            }
            var lastUpdatePath = string.Empty;
            var nouse = 0;
            var lastUpdate = DateTime.Now;

            parentDir = Path.Combine(parentDir, "DirInfo.info");

            this.getIndexDirInfo(parentDir, ref nouse, ref lastUpdate, ref lastUpdatePath);



            var info = string.Format("{0}{2}{1}", count, DateTime.Now, Environment.NewLine);
            if (!string.IsNullOrEmpty(lastUpdatePath))
            {
                info = string.Concat(info, Environment.NewLine, lastUpdatePath);
            }
            File.WriteAllText(parentDir, info);
        }

        /// <summary>
        /// 写入当前索引目录的文档数量
        /// </summary>
        /// <param name="path"></param>
        /// <param name="count"></param>
        /// <param name="addition"></param>
        public void WriteIndexInfoOnDir(string path, int count, string addition)
        {
            path = Path.Combine(path, "DirInfo.info");
            if (File.Exists(path))
                count += ReadIndexCountOnDir(path);

            var info = string.Format("{0}{2}{1}", count, DateTime.Now, Environment.NewLine);
            if (!string.IsNullOrEmpty(addition))
            {
                info = string.Concat(info, Environment.NewLine, addition);
            }
            File.WriteAllText(path, info);
        }

        #endregion

        #region Private Method


        /// <summary>
        /// 获取信息文件保存的索引信息
        /// </summary>
        /// <param name="infoPath"></param>
        /// <param name="count"></param>
        /// <param name="lastIndex"></param>
        /// <param name="lastModifySubDir"></param>
        private bool getIndexDirInfo(string infoPath, ref int count, ref DateTime lastIndex, ref string lastModifySubDir)
        {
            if (!File.Exists(infoPath))
            {
                return false;
            }

            try
            {
                var infos = File.ReadAllLines(infoPath);
                if (infos.Length != 2 && infos.Length != 3)
                {
                    throw new Exception("索引信息文件出错。");
                }

                count = int.Parse(infos[0]);
                lastIndex = DateTime.Parse(infos[1]);
                if (infos.Length == 3) //表示是月份目录
                {
                    lastModifySubDir = infos[2];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("索引信息文件出错。文件路径: {0}, 错误信息: {1}, 错误堆栈: {2},错误时间: {3}", infoPath, e.Message, e.StackTrace, DateTime.Now);
                throw e;
            }

            return true;
        }

        /// <summary>
        /// 加锁
        /// </summary>
        private void lockOperation()
        {
            while (File.Exists("IndexManager.lock"))
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }

            var file = File.Create("IndexManager.lock");
            file.Close();
        }

        /// <summary>
        /// 解锁
        /// </summary>
        private void unlockOperation()
        {
            File.Delete("IndexManager.lock");
        }

        #endregion
    }
}
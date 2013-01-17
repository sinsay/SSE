using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Indexer.IndexInterface
{
    public static class IndexDirectoryCache
    {
        /// <summary>
        /// 索引目录缓存
        /// </summary>
        private static Dictionary<string, IIndexDirectory> DirCache;

        /// <summary>
        /// 缓存的同步锁
        /// </summary>
        private static readonly object Locker;

        static IndexDirectoryCache()
        {
            DirCache = new Dictionary<string, IIndexDirectory>();
            Locker = new object();
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="dir"></param>
        public static void Set(IIndexDirectory dir)
        {
            lock (Locker)
            {
                dir.Init();
                DirCache[dir.IndexPath] = dir;
            }
        }

        /// <summary>
        /// 设置一批索引目录进缓存
        /// </summary>
        /// <param name="dirs"></param>
        public static void SetRange(IIndexDirectory[] dirs)
        {
            if (dirs != null && dirs.Length != 0)
            {
                foreach (IIndexDirectory dir in dirs)
                {
                    Set(dir);
                }
            }
        }

        /// <summary>
        /// 根据 path 路径获取对应的索引目录缓存
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IIndexDirectory Get(string path)
        {
            IIndexDirectory dir = null;
            lock (Locker)
            {
                if (DirCache.ContainsKey(path.ToLower()))
                {
                    dir = DirCache[path];
                }
            }
            return dir;
        }

        /// <summary>
        /// 批量获取索引缓存
        /// </summary>
        /// <param name="pathes"></param>
        /// <returns></returns>
        public static PathInfo GetRange(string[] pathes)
        {
            PathInfo info = new PathInfo();
            if (pathes != null && pathes.Length != 0)
            {
                List<string> useless = new List<string>();
                List<IIndexDirectory> useful = new List<IIndexDirectory>();
                foreach (string  path in pathes)
                {
                    IIndexDirectory dir = Get(path);
                    if (dir == null)
                    {
                        useless.Add(path);
                    }
                    else
                    {
                        useful.Add(dir);
                    }
                }

                info.Useful = useful.ToArray();
                info.Useless = useless.ToArray();
            }
            else
            {
                info.Useless = pathes;
            }

            return info;
        }

        /// <summary>
        /// 释放一个索引缓存
        /// </summary>
        /// <param name="path"></param>
        public static void Release(string path)
        {
            lock (Locker)
            {
                if (!string.IsNullOrEmpty(path) && DirCache.ContainsKey(path))
                {
                    DirCache[path].Close();
                    DirCache.Remove(path);
                }
            }
        }

        /// <summary>
        /// 批量释放索引缓存
        /// </summary>
        /// <param name="pathes"></param>
        public static void ReleaseRange(string[] pathes)
        {
            if (pathes != null && pathes.Length != 0)
            {
                foreach (string path in pathes)
                {
                    Release(path);
                }
            }
        }
    }

    public class PathInfo
    {
        /// <summary>
        /// 有效索引
        /// </summary>
        public IIndexDirectory[] Useful { get; set; }

        /// <summary>
        /// 无效的索引，可能是因为不在缓存中
        /// </summary>
        public string[] Useless { get; set; }
    }
}

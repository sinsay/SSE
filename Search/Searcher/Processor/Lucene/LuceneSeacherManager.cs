using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuceneAddIn = Lucene.Net;
using Indexer.IndexInterface.LuceneInterface;
using Indexer.IndexInterface;

namespace Searcher.Processor.Lucene
{
    /// <summary>
    /// Lucene 的检索器缓存，包括自动缓存索引目录
    /// </summary>
    public static class LuceneSearcherManager
    {
        private static Dictionary<string, LuceneAddIn.Search.IndexSearcher> SearcherCache;

        private static readonly object Locker;

        static LuceneSearcherManager()
        {
            SearcherCache = new Dictionary<string, LuceneAddIn.Search.IndexSearcher>();
            Locker = new object();
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="dir"></param>
        public static void Set(LuceneDirectory dir)
        {
            lock (Locker)
            {
                SearcherCache[dir.IndexPath] = new LuceneAddIn.Search.IndexSearcher(dir.Directory, true);
            }
        }

        /// <summary>
        /// 设置一批搜索器进缓存
        /// </summary>
        /// <param name="dirs"></param>
        public static void SetRange(LuceneDirectory[] dirs)
        {
            if (dirs != null && dirs.Length != 0)
            {
                foreach (LuceneDirectory dir in dirs)
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
        public static LuceneAddIn.Search.IndexSearcher Get(string path)
        {
            LuceneAddIn.Search.IndexSearcher searcher = null;

            // 如果不在缓存中，尝试读取进缓存
            IIndexDirectory dir = IndexDirectoryCache.Get(path);
            if (dir == null && System.IO.Directory.Exists(path))
            {
                try
                {
                    LuceneAddIn.Store.FSDirectory directory = LuceneAddIn.Store.MMapDirectory.Open(new System.IO.DirectoryInfo(path));
                    dir = new LuceneDirectory(path, false) { Directory = directory };
                    IndexDirectoryCache.Set(dir);
                    LuceneSearcherManager.Set(dir as LuceneDirectory);
                }
                catch { }
            }

            lock (Locker)
            {
                if (SearcherCache.ContainsKey(path))
                {
                    searcher = SearcherCache[path] ;
                }
            }
            return searcher;
        }

        /// <summary>
        /// 批量获取检索器缓存
        /// </summary>
        /// <param name="pathes"></param>
        /// <returns></returns>
        public static SearcherInfo GetRange(string[] pathes)
        {
            SearcherInfo info = new SearcherInfo();
            if (pathes != null && pathes.Length != 0)
            {
                List<string> useless = new List<string>();
                List<LuceneAddIn.Search.IndexSearcher> useful = new List<LuceneAddIn.Search.IndexSearcher>();
                foreach (string path in pathes)
                {
                    LuceneAddIn.Search.IndexSearcher searcher = Get(path);
                    if (searcher == null)
                    {
                        useless.Add(path);
                    }
                    else
                    {
                        useful.Add(searcher);
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
                if (!string.IsNullOrEmpty(path) && SearcherCache.ContainsKey(path))
                {
                    SearcherCache[path].Close();
                    SearcherCache.Remove(path);
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

    public class SearcherInfo
    {
        /// <summary>
        /// 有效索引
        /// </summary>
        public LuceneAddIn.Search.IndexSearcher[] Useful { get; set; }

        /// <summary>
        /// 无效的索引，可能是因为不在缓存中
        /// </summary>
        public string[] Useless { get; set; }
    }
}

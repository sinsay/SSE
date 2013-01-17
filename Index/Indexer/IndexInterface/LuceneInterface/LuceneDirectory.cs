using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Indexer.IndexInterface.LuceneInterface
{
    /// <summary>
    /// the directory for lucene
    /// </summary>
    public class LuceneDirectory: IIndexDirectory
    {
        public LuceneDirectory(string indexPath, bool ram)
        {
            this.IndexPath = indexPath;
            this.Ram = ram;
        }

        /// <summary>
        /// path of index
        /// </summary>
        public string IndexPath { get; set; }

        public bool Ram { get; set; }

        private long lastModify = -1;

        private Lucene.Net.Store.Directory directory;

        public Lucene.Net.Store.Directory Directory
        {
            get
            {
                // 获取索引目录
                if (this.directory == null || 
                    (Lucene.Net.Index.IndexReader.IndexExists(this.directory) &&
                    Lucene.Net.Index.IndexReader.LastModified(this.directory) != this.lastModify)) // 如果已缓存且索引发生改变，则重新读取
                {
                    this.Init();
                }
                return this.directory;
            }
            set { this.directory = value; }
        }

        /// <summary>
        /// init dir before truly use this instance
        /// </summary>
        public void Init()
        {
            Lucene.Net.Store.Directory dir;

            if (!System.IO.Directory.Exists(this.IndexPath))
            {
                System.IO.Directory.CreateDirectory(this.IndexPath);
            }

            if (this.Ram)
            {
                if (string.IsNullOrEmpty(this.IndexPath))
                {
                    this.IndexPath = "RAM";
                }

                dir = new Lucene.Net.Store.RAMDirectory(this.IndexPath);
            }
            else
            {
                if (this.directory != null)
                {
                    try
                    {
                        this.directory.Close();
                    }
                    catch { }
                }
                dir = Lucene.Net.Store.SimpleFSDirectory.Open(new System.IO.DirectoryInfo(this.IndexPath));
            }

            this.lastModify = Lucene.Net.Index.IndexReader.IndexExists(dir) ? Lucene.Net.Index.IndexReader.LastModified(dir) : -1;
            this.directory = dir;
        }

        public bool Exists()
        {
            // TODO: 实现索引是否存在的判断
            return true;
        }

        public void Close()
        {
            if (this.directory != null)
            {
                try
                {
                    this.directory.Close();
                }
                catch { }
            }
        }


        #region static method

        public static LuceneDirectory CreateRAMDir(string indexPath)
        {
            return new LuceneDirectory(indexPath, true);
        }

        public static LuceneDirectory CreateRAMDir()
        {
            return CreateRAMDir(string.Empty);
        }

        #endregion
    }
}

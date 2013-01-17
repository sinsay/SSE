using System;
using System.Collections.Generic;
using Indexer.IndexInterface;

namespace Indexer
{
    /// <summary>
    /// the index context, which contains a batch of document cache, processing info
    /// </summary>
    public class IndexContext : IDisposable, Common.IContext
    {
        #region Constructor 

        public IndexContext()
        {
            this.AdditionInfo = new Dictionary<string, object>();
        }

        #endregion

        #region Index Document Info

        private object document;
        /// <summary>
        /// current document which need to process
        /// </summary>
        public object Document
        {
            get
            {
                return this.document;
            }
            set
            {
                this.document = value;

                if (value == null)
                {
                    return;
                }

                if (this.DocumentCache == null)
                {
                    this.DocumentCache = new Dictionary<int, object>();
                }
                this.DocumentCache[value.GetHashCode()] = value;
            }
        }

        /// <summary>
        /// the field current processing
        /// </summary>
        public FieldAnalyseInfo CurrentFieldInfo { get; set; }

        /// <summary>
        /// object's hashcode as key and the object as value,for current index cache
        /// </summary>
        private Dictionary<int, object> DocumentCache;

        #endregion

        /// <summary>
        /// process operation need to be cancle
        /// </summary>
        public bool Cancle { get; set; }

        /// <summary>
        /// the directory of index path info
        /// </summary>
        public IIndexDirectory Directory { get; set; }

        /// <summary>
        /// addition index info for customer processor
        /// </summary>
        public Dictionary<string, object> AdditionInfo { get; set; }

        public void Dispose()
        {
            foreach (var key in this.DocumentCache.Keys)
            {
                this.DocumentCache[key] = null;
            }

            this.DocumentCache.Clear();
            this.DocumentCache = null;
        }
    }
}
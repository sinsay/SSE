using System;
using Indexer.IndexInterface;
using Indexer.IndexInterface.LuceneInterface;

namespace Indexer.Processor.LuceneProcessor
{
    /// <summary>
    /// Lucene Indexwriter
    /// </summary>
    public class LuceneIndexWriter : IIndexWriter, IDisposable
    {
        /// <summary>
        /// current indexer's directory info
        /// </summary>
        public IIndexDirectory Directory { get; set; }

        private Lucene.Net.Index.IndexWriter indexWriter;

        public LuceneIndexWriter(IIndexDirectory directory)
        {
            this.Directory = directory;

            this.InitIndexWriter();
        }

        private void InitIndexWriter()
        {
            this.Directory.Init();
            this.indexWriter = new Lucene.Net.Index.IndexWriter(
                (this.Directory as LuceneDirectory).Directory, 
                new LuceneExtension.LuceneAnalyser(), 
                new Lucene.Net.Index.IndexWriter.MaxFieldLength(int.MaxValue));

            this.indexWriter.SetMaxBufferedDocs(100000);      // 最大缓存文档数 10W
            this.indexWriter.SetMaxFieldLength(10000);          // 最大字段长度 1W, 过大的情况可能会导致OOM
            this.indexWriter.SetMaxMergeDocs(500000);         // 最大合并文档数，当文档达到 50W 即开始合并为一个文件
            this.indexWriter.SetUseCompoundFile(true);          // 合并文件
        }

        /// <summary>
        /// index current document
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public bool AddDocument(Document doc)
        {
            var status = true;

            var fields = doc.GetFields();
            if (fields == null || fields.Length == 0)
            {
                status = false;
            }
            else
            {
                var luceneDoc = new Lucene.Net.Documents.Document();
                foreach (var field in fields)
                {
                    luceneDoc.Add(new Lucene.Net.Documents.Field(
                        field.FieldName,                    // field name
                        field.FieldValue.ToString(),     // field value
                        field.Store ? Lucene.Net.Documents.Field.Store.YES : Lucene.Net.Documents.Field.Store.NO,
                        field.Analyse ? Lucene.Net.Documents.Field.Index.ANALYZED : Lucene.Net.Documents.Field.Index.NOT_ANALYZED));
                }

                this.indexWriter.AddDocument(luceneDoc);
            }

            return status;
        }

        /// <summary>
        /// close the indexwriter and the directory
        /// </summary>
        public void Close()
        {
            this.Dispose();
        }

        /// <summary>
        /// flush data 
        /// </summary>
        public void Commit()
        {
            this.indexWriter.Commit();
        }

        public void Dispose()
        {
            try
            {
                this.indexWriter.Close();
                (this.Directory as LuceneDirectory).Directory.Close();
            }
            catch { }
        }
    }
}
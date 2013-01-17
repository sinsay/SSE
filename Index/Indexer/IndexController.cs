using System;
using Common.Attributes;
using Indexer.Processor;
using UtilityLib.Extensions;
using UtilityLib.Reflection;

namespace Indexer
{
    public class IndexController
    {
        #region CallBack

        /// <summary>
        /// index callback handler
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public delegate object IndexCallBackHandler(IndexContext context);

        /// <summary>
        /// index callback event
        /// </summary>
        public event IndexCallBackHandler IndexEvent;

        #endregion

        /// <summary>
        /// the archive indexer
        /// </summary>
        public IIndexWriter Processor { get; set; }

        /// <summary>
        /// init the index flow controller
        /// </summary>
        /// <param name="processor"></param>
        public IndexController(IIndexWriter processor)
        {
            this.Processor = processor;
        }

        /// <summary>
        /// initialize the current document's index info
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Document InitDocument(IndexContext context)
        {
            if (context == null || context.Document == null)
                throw new ArgumentNullException("context");

            Document doc = new Document();

            var properties = Property.GetProperties(context.Document.GetType());
            foreach (var property in properties)
            {
                // get all the attribute witch add for build index info
                var attributes = UtilityLib.Reflection.Attribute.GetAttributes<BaseAttribute>(context.Document, property);
                if (attributes == null) // means this property no need to be index
                    continue;

                // TODO: 将 FieldInfo 改成缓存
                context.CurrentFieldInfo = new FieldAnalyseInfo
                {
                    FieldName = property.Name,
                    FieldValue = UtilityLib.Reflection.Property.GetValue(context.Document, property)
                };

                // fill the property info, order by attribute's priority before fill
                foreach (var attribute in attributes.OrderByPriority())
                {
                    attribute.Execute(context);
                }

                // save the property witch was processed
                doc.SetField(context.CurrentFieldInfo);
            }

            return doc;
        }

        /// <summary>
        /// index the current document
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool Index(IndexContext context)
        {
            var doc = this.InitDocument(context);

            if (!this.Processor.AddDocument(doc))
                throw new Exception("index current archive failed");

            // do something after index current document
            if (this.IndexEvent != null)
            {
                this.IndexEvent(context);
            }

            return true;
        }

        /// <summary>
        /// close the index controller
        /// </summary>
        public void Close()
        {
            this.Processor.Close();
        }

        /// <summary>
        /// commit the processor to flush data
        /// </summary>
        public void Commit()
        {
            this.Processor.Commit();
        }
    }
}

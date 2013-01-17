using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Indexer
{
    /// <summary>
    /// definition of document
    /// </summary>
    public  class Document
    {
        private List<FieldAnalyseInfo> fieldList;

        public Document()
        {
            fieldList = new List<FieldAnalyseInfo>();
        }

        /// <summary>
        /// set the field info which need's to be index
        /// </summary>
        /// <param name="field"></param>
        public void SetField(FieldAnalyseInfo field)
        {
            this.fieldList.Add(field);
        }

        /// <summary>
        /// get all the field's info
        /// </summary>
        /// <returns></returns>
        public FieldAnalyseInfo[] GetFields()
        {
            return this.fieldList.ToArray();
        }
    }
}

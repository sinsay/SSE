using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Indexer
{
    /// <summary>
    /// field info, to store the index info, like need's to be analyse or store and so on
    /// </summary>
    public class FieldAnalyseInfo
    {
        /// <summary>
        /// field name
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// field value
        /// </summary>
        public object FieldValue { get; set; }

        /// <summary>
        /// it's need's to be store on index file
        /// </summary>
        public bool Store { get; set; }

        /// <summary>
        /// need's to be analyse
        /// </summary>
        public bool Analyse { get; set; }

        /// <summary>
        /// use term veter
        /// </summary>
        public bool TermVeter { get; set; }

        /// <summary>
        /// type of this field( NO USE!!! )
        /// </summary>
        public string FieldType { get; set; }
    }
}

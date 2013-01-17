using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis;

namespace LuceneExtension
{
    public class LuceneAnalyser : Analyzer
    {
        public LuceneAnalyser()
        {
            this.overridesTokenStreamMethod = true;
        }

		public override TokenStream TokenStream(string fieldName, System.IO.TextReader reader)
		{
            TokenStream result = new LuceneTokenizer(reader, reader.ReadToEnd());
			result = new LowerCaseFilter(result);
			return result;
		}
    }
}

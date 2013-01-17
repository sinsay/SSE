using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis;
using System.IO;

namespace LuceneExtension
{
    public class LuceneTokenizer : Tokenizer
    {
        protected string input_text;
		protected string[] words;
		protected int index;
        protected int offset;

        public LuceneTokenizer(TextReader input, string inputText)
            : base(input)
        {
            this.input = input;
            this.index = 0;
            this.input_text = inputText;
        }

		public override Token Next()
		{
			if (words == null)
			{
				words = MCSegment.Segment.Seg(this.input_text);
			}

			if (words == null || index >= words.Length) return null;

			string word = words[index++];
			Token token = new Token(word, offset, offset + word.Length);
            offset += word.Length;
			return token;
		}
	}
}

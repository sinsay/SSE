using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Indexer.Attributes;

namespace Indexer.TinyBlog
{
    /// <summary>
    /// TinyBlog Entity For Test
    /// </summary>
    public class TinyBlogEntity
    {
        [Stroe]
        public int ID { get; set; }

        [Analyse]
        public string Content { get; set; }

        [Stroe]
        [Date]
        public DateTime Publish { get; set; }

        [Analyse]
        public string Author { get; set; }
    }
}

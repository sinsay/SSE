using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCSegment
{
    public class Segment
    {
        public static string[] Seg(string text)
        {
            return text.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}

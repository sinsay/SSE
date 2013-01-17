
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilityLib.Reflection;

namespace UtilityLib.Extensions
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<T> OrderByPriority<T>(this IEnumerable<T> self)
        {
            if (self == null)
                return null;

            return orderbyPriority<T>(self, false);
        }

        public static IEnumerable<T> OrderByPriorityDescending<T>(IEnumerable<T> self)
        {
            if (self == null)
                return null;

            return orderbyPriority<T>(self, true);
        }

        private static IEnumerable<T> orderbyPriority<T>(IEnumerable<T> self, bool desc)
        {
            if (!desc)
            {
                return self.OrderBy<T, int>(t =>
                {
                    var priority = -1;
                    var attrs = Attribute.GetAttributes<PriorityAttribute>(t);
                    if (attrs != null && attrs.Length != 0)
                    {
                        //if implet too many priority, choose the max one
                        priority = attrs.Max(p => p.Priotiry);
                    }
                    return priority;
                });
            }
            else
            {
                return self.OrderByDescending<T, int>(t =>
                {
                    var priority = -1;
                    var attrs = Attribute.GetAttributes<PriorityAttribute>(t);
                    if (attrs != null && attrs.Length != 0)
                    {
                        //if implet too many priority, choose the max one
                        priority = attrs.Max(p => p.Priotiry);
                    }
                    return priority;
                });
            }
        }

    }
}

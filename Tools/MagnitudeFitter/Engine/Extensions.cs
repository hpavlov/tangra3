using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagnitudeFitter.Engine
{
    public static class Extensions
    {
        public static T Median<T>(this List<T> list)
        {
            if (list.Count == 0) return default(T);
            if (list.Count == 1) return list[0];

            var copy = new List<T>();
            copy.AddRange(list);
            copy.Sort();

            return copy[copy.Count / 2];
        }
    }
}

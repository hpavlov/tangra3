using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OcrTester
{
    public static class Extensions
    {
        public static T Median<T>(this IList<T> list)
        {
            if (list.Count == 0)
                return default(T);

            T[] arrayList = list.ToArray();
            Array.Sort(arrayList);

            return arrayList[list.Count / 2];
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Helpers
{
    public static class Extensions
    {
        public static string FullExceptionInfo(this Exception ex)
        {
            var output = new StringBuilder();
            Exception currEx = ex;
            while (currEx != null)
            {
                output.AppendFormat("{0} : {1}\r\n{2}\r\n-------------------------------------\r\n\r\n", currEx.GetType(), currEx.Message, currEx.StackTrace);
                currEx = currEx.InnerException;
            }

            return output.ToString();
        }
    }
}

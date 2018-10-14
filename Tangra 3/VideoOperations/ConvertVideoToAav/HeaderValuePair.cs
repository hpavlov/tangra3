using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tangra.VideoOperations.ConvertVideoToAav
{

    public class HeaderValuePair
    {
        public HeaderValuePair()
        { }

        public HeaderValuePair(string header, string value)
        {
            Header = header;
            Value = value;
        }

        public string Header { get; set; }
        public string Value { get; set; }
    }
}

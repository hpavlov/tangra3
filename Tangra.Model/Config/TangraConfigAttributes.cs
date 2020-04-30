using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Config
{
    public class ProvidesUtcTimeAttribute : Attribute
    {
    }

    public class GenericCameraSystemAttribute : Attribute
    {
    }

    public class RequiresTimingCorrectionsAttribute : Attribute
    {
    }

    public class EnumValDisplayNameAttribute : Attribute
    {
        public EnumValDisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }

        public string DisplayName { get; private set; }
    }
}

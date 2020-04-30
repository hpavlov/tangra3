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

    public class EnumValDetailsAttribute : Attribute
    {
        public EnumValDetailsAttribute(string description, string url)
        {
            Description = description;
            Url = url;
        }

        public string Description { get; private set; }

        public string Url { get; private set; }
    }
}

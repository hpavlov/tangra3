using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tangra.Video.AstroDigitalVideo
{
    public class AdvSectionFactory
    {
        public static IAdvDataSection ConstructSection(string sectionType, BinaryReader fileReader)
        {
            if (sectionType == AdvSectionTypes.SECTION_IMAGE)
                return new AdvImageSection(fileReader);
            else if (sectionType == AdvSectionTypes.SECTION_SYSTEM_STATUS)
                return new AdvStatusSection(fileReader);
            else
                throw new NotSupportedException();
        }
    }
}

/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Tangra.Model.Config
{
    public enum OSDTimestampFormat
    {
        IOTAVideoTimeInserter
    }

    public class OCRConfig
    {
        [XmlArrayItem("Entry", typeof(OCRConfigEntry))]
        public List<OCRConfigEntry> SavedConfigurations = new List<OCRConfigEntry>();
    }

    public class OCRConfigEntry
    {
        public OSDTimestampFormat ORCEngine;
        public int Width;
        public int Height;
        public OCRParameters Parameters = new OCRParameters();
    }

    public class OCRParameters
    {
        public float SymbolWidthCoeff;
        public float SymbolHeightCoeff;
        public float SymbolGapCoeff;

        public int DeltaXAdj;
        public int DeltaYAdj;

        public int HorizontalPos;
        public int VerticalPos;
        public int XAxisPos;
        public int YAxisPos;
    }
}

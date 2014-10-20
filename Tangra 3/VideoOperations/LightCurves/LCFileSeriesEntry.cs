/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.VideoOperations.LightCurves
{
	public class LCFileSeriesEntry
	{
		public uint FrameNumber;
		public int NumObjects;

		public uint Signal1
		{
			get { return NumObjects > 0 ? m_Measurement1.TotalReading : 0; }
			set
			{
				if (NumObjects > 0) m_Measurement1.TotalReading = value;
			}
		}
		public uint Background1
		{
			get { return NumObjects > 0 ? m_Measurement1.TotalBackground : 0; }
			set { if (NumObjects > 0) m_Measurement1.TotalBackground = value; }
		}

		public uint Signal2
		{
			get { return NumObjects > 1 ? m_Measurement2.TotalReading : 0; }
			set { if (NumObjects > 1) m_Measurement2.TotalReading = value; }
		}
		public uint Background2
		{
			get { return NumObjects > 1 ? m_Measurement2.TotalBackground : 0; }
			set { if (NumObjects > 1) m_Measurement2.TotalBackground = value; }
		}


		public uint Signal3
		{
			get { return NumObjects > 2 ? m_Measurement3.TotalReading : 0; }
			set { if (NumObjects > 2) m_Measurement3.TotalReading = value; }
		}
		public uint Background3
		{
			get { return NumObjects > 2 ? m_Measurement3.TotalBackground : 0; }
			set { if (NumObjects > 2) m_Measurement3.TotalBackground = value; }
		}

		public uint Signal4
		{
			get { return NumObjects > 3 ? m_Measurement4.TotalReading : 0; }
			set { if (NumObjects > 3) m_Measurement4.TotalReading = value; }
		}
		public uint Background4
		{
			get { return NumObjects > 3 ? m_Measurement4.TotalBackground : 0; }
			set { if (NumObjects > 3) m_Measurement4.TotalBackground = value; }
		}
		

		private LCMeasurement m_Measurement1;
		private LCMeasurement m_Measurement2;
		private LCMeasurement m_Measurement3;
		private LCMeasurement m_Measurement4;

		internal LCMeasurement Measurement1 { get { return m_Measurement1; } }
		internal LCMeasurement Measurement2 { get { return m_Measurement2; } }
		internal LCMeasurement Measurement3 { get { return m_Measurement3; } }
		internal LCMeasurement Measurement4 { get { return m_Measurement4; } }

		internal LCFileSeriesEntry(int numObjects, uint frameNo, LCMeasurement measurement1, LCMeasurement measurement2, LCMeasurement measurement3, LCMeasurement measurement4)
		{
			NumObjects = numObjects;
			FrameNumber = frameNo;
			m_Measurement1 = measurement1;
			m_Measurement2 = measurement2;
			m_Measurement3 = measurement3;
			m_Measurement4 = measurement4;
		}
	}
}

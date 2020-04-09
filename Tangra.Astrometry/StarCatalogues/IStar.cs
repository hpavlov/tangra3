/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Tangra.StarCatalogues;

namespace Tangra.StarCatalogues
{
    public interface IStar
    {
        ulong StarNo { get; }
        double RADeg { get;}
        double DEDeg { get; }
        double Mag { get; }
        double MagR { get; }
        double MagB { get; }
        double MagV { get; }
		double MagJ { get; }
		double MagK { get; }
        bool IsForInitialPlateSolve { get; }
        string GetStarDesignation(int alternativeId);
        double GetMagnitudeForBand(Guid magBandId);
    }

	[Serializable()]
    public class Star : IStar, ISerializable
    {
        private readonly ulong m_StarNo;
        private readonly double m_RADeg;
        private readonly double m_DEDeg;
        private readonly double m_Mag;

        public Star(ulong id, double raDeg, double deDeg, double mag)
        {
            m_StarNo = id;
            m_RADeg = raDeg;
            m_DEDeg = deDeg;
            m_Mag = mag;
        }

		public Star(IStar copyFrom)
			: this(copyFrom.StarNo, copyFrom.RADeg, copyFrom.DEDeg, copyFrom.Mag)
		{ }

        #region IStar Members

        public ulong StarNo
        {
            get { return m_StarNo; }
        }

        public string GetStarDesignation(int alternativeId)
        {
            return m_StarNo.ToString();
        }

        public double RADeg
        {
            get { return m_RADeg; }
        }

        public double DEDeg
        {
            get { return m_DEDeg; }
        }

        public double Mag
        {
            get { return m_Mag; }
        }

        public double MagR
        {
            get { return m_Mag; }
        }

        public double MagB
        {
            get { return m_Mag; }
        }

        public double MagV
        {
            get { return m_Mag; }
        }

		public double MagJ
		{
			get { return double.NaN; }
		}

		public double MagK
		{
			get { return double.NaN; }
		}

        public bool IsForInitialPlateSolve
        {
            get { return true; }
        }

        public double GetMagnitudeForBand(Guid magBandId)
        {
            return m_Mag;
        }
        #endregion

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("m_StarNo", m_StarNo);
			info.AddValue("m_RADeg", m_RADeg);
			info.AddValue("m_DEDeg", m_DEDeg);
			info.AddValue("m_Mag", m_Mag);
		}

		public Star(SerializationInfo info, StreamingContext context)
        {
			m_StarNo = info.GetUInt32("m_StarNo");
			m_RADeg = info.GetDouble("m_RADeg");
			m_DEDeg = info.GetDouble("m_DEDeg");
			m_Mag = info.GetDouble("m_Mag");
        }
    }
}

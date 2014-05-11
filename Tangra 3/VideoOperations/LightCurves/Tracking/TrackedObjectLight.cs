using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;

namespace Tangra.VideoOperations.LightCurves.Tracking
{
	internal class TrackedObjectLight : TrackedObjectBase
	{
		public float RefinedFWHM;
		public float RefinedIMAX;

		public List<double> m_RecentFWHMs = new List<double>();
		public List<double> m_RecentIMAXs = new List<double>();
 
		public TrackedObjectLight(byte targetNo, TrackedObjectConfig originalObject)
			: base(targetNo, originalObject)
		{
			Center = new ImagePixel(originalObject.OriginalFieldCenterX, originalObject.OriginalFieldCenterY);
		}

		public void InitializeNewTracking()
		{
			RefinedFWHM = float.NaN;
			m_RecentFWHMs.Clear();
			m_RecentIMAXs.Clear();

			LastKnownGoodPosition = OriginalObject.AsImagePixel;
			IsLocated = false;
		}

		public void NewMatchEvaluation()
		{
			IsLocated = false;
			NotMeasuredReasons = NotMeasuredReasons.UnknownReason;
		}

		public void NextFrame()
		{
			if (IsLocated && PSFFit != null)
			{
				if (m_RecentFWHMs.Count > 25) m_RecentFWHMs.RemoveAt(0);
				m_RecentFWHMs.Add(PSFFit.FWHM);
				RefinedFWHM = (float)m_RecentFWHMs.Average();

				if (OriginalObject.TrackingType != TrackingType.OccultedStar)
				{
					if (m_RecentIMAXs.Count > 25) m_RecentIMAXs.RemoveAt(0);
					m_RecentIMAXs.Add(PSFFit.IMax);
					RefinedIMAX = (float)m_RecentIMAXs.Average();					
				}
			}

			IsLocated = false;
		}

		public void SetTrackedObjectMatch(PSFFit psfFitMatched)
		{
			if (PSFFit != null && PSFFit.IsSolved)
                LastKnownGoodPosition = new ImagePixel(psfFitMatched.Brightness, Center.XDouble, Center.YDouble);

			PSFFit = psfFitMatched;
			Center = new ImagePixel((int)psfFitMatched.IMax, psfFitMatched.XCenter, psfFitMatched.YCenter);
			IsLocated = true;
			NotMeasuredReasons = NotMeasuredReasons.TrackedSuccessfully;
		}
	}
}

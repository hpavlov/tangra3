using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;
using Tangra.VideoOperations.LightCurves.Tracking;

namespace Tangra.VideoOperations.Spectroscopy.Tracking
{
	internal class SpectroscopyStarTracker
	{
		private bool m_IsTrackedSuccessfully = false;
		private int m_FrameNo;

		public TrackedObject TrackedStar;

		public SpectroscopyStarTracker(TrackedObjectConfig starConfig)
		{
			TrackedStar = new TrackedObject(0, starConfig);
			TrackedStar.LastKnownGoodPosition = new ImagePixel(starConfig.ApertureStartingX, starConfig.ApertureStartingY);
		}

		public bool IsTrackedSuccessfully
		{
			get { return m_IsTrackedSuccessfully; }
		}

		public void NextFrame(int frameNo, IAstroImage astroImage)
		{
			m_FrameNo = frameNo;

			TrackSingleStar(frameNo, astroImage);

			m_IsTrackedSuccessfully = TrackedStar.IsLocated;

			if (TrackedStar.IsLocated)
			{
				TrackedStar.LastKnownGoodPosition = new ImagePixel(TrackedStar.Center);
				TrackedStar.LastKnownGoodPsfCertainty = TrackedStar.PSFFit != null ? TrackedStar.PSFFit.Certainty : 0;
			}
		}

		private void GetExpectedXY(out float expectedX, out float expectedY)
		{
			expectedX = (float)TrackedStar.LastKnownGoodPosition.XDouble;
			expectedY = (float)TrackedStar.LastKnownGoodPosition.YDouble;
		}

		private void TrackSingleStar(int frameNo, IAstroImage astroImage)
		{
			TrackedStar.NewFrame();
			float expectedX;
			float expectedY;

			GetExpectedXY(out expectedX, out expectedY);

			uint[,] pixels = astroImage.GetPixelsArea((int)expectedX, (int)expectedY, 17);

			// There is only one object in the area, just do a wide fit followed by a fit with the selected matrix size
			PSFFit gaussian = new PSFFit((int)expectedX, (int)expectedY);
			gaussian.Fit(pixels, 17);

			IImagePixel firstCenter = new ImagePixel((int)gaussian.XCenter, (int)gaussian.YCenter);

			pixels = astroImage.GetPixelsArea(firstCenter.X, firstCenter.Y, 17);
			gaussian = new PSFFit(firstCenter.X, firstCenter.Y);
			gaussian.Fit(pixels, TrackedStar.PsfFitMatrixSize);
			if (gaussian.IsSolved)
			{
				TrackedStar.PSFFit = gaussian;
				TrackedStar.ThisFrameX = (float)gaussian.XCenter;
				TrackedStar.ThisFrameY = (float)gaussian.YCenter;
				TrackedStar.ThisSignalLevel = (float)(gaussian.IMax - gaussian.I0);
				TrackedStar.ThisFrameCertainty = (float)gaussian.Certainty;
				TrackedStar.SetIsLocated(true, NotMeasuredReasons.TrackedSuccessfully);
			}
			else
			{
				TrackedStar.ThisFrameX = expectedX;
				TrackedStar.ThisFrameY = expectedY;
				TrackedStar.ThisFrameCertainty = (float)gaussian.Certainty;

				Trace.WriteLine(string.Format("Frame {0}: Cannot confirm target {1} [SingleStar]. Cannot solve second PSF", m_FrameNo, TrackedStar.TargetNo));
				TrackedStar.SetIsLocated(false, NotMeasuredReasons.PSFFittingFailed);
			}
		}
	}
}

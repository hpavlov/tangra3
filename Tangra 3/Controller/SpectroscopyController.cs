using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Helpers;
using Tangra.Model.Astro;
using Tangra.VideoOperations.Spectroscopy.Helpers;

namespace Tangra.Controller
{
    public class SpectroscopyController
    {
		private object m_SyncLock = new object();

		private VideoController m_VideoController;

        internal SpectroscopyController(VideoController videoController)
		{
			m_VideoController = videoController;
		}

        internal float LocateSpectraAngle(PSFFit selectedStar)
        {
            float x0 = (float)selectedStar.XCenter;
            float y0 = (float)selectedStar.YCenter;
            
            int minDistance = (int)(10 * selectedStar.FWHM);

            AstroImage image = m_VideoController.GetCurrentAstroImage(false);

            float width = image.Width;
            float height = image.Height;

            uint[] angles = new uint[360];
            uint[] sums = new uint[360];

            int diagonnalPixels = (int)Math.Ceiling(Math.Sqrt(image.Width * image.Width + image.Height * image.Height));
            for (int i = 0; i < 360; i++)
            {
                var mapper = new RotationMapper(image.Width, image.Height, i);
                PointF p1 = mapper.GetDestCoords(x0, y0);
                float x1 = p1.X;
                float y1 = p1.Y;

                uint rowSum = 0;

                for (int d = minDistance; d < diagonnalPixels; d++)
                {
                    PointF p = mapper.GetSourceCoords(x1 + d, y1);

                    if (p.X >= 0 && p.X < width && p.Y >= 0 && p.Y < height)
                    {
                        rowSum += image.Pixelmap[(int)p.X, (int)p.Y];
                    }
                }

                angles[i] = (uint)i;
                sums[i] = rowSum;
            }

            Array.Sort(sums, angles);

            uint roughAngle = angles[359];

            uint bestSum = 0;
            float bestAngle = 0f;

            for (float a = roughAngle - 1; a < roughAngle + 1; a += 0.02f)
            {
                var mapper = new RotationMapper(image.Width, image.Height, a);
                PointF p1 = mapper.GetDestCoords(x0, y0);
                float x1 = p1.X;
                float y1 = p1.Y;

                uint rowSum = 0;

                for (int d = minDistance; d < diagonnalPixels; d++)
                {
                    PointF p = mapper.GetSourceCoords(x1 + d, y1);

                    if (p.X >= 0 && p.X < width && p.Y >= 0 && p.Y < height)
                    {
                        uint pixVal = image.Pixelmap[(int)p.X, (int)p.Y];
                        rowSum += pixVal;
                    }
                }

                if (rowSum > bestSum)
                {
                    bestSum = rowSum;
                    bestAngle = a;
                }
            }

            return bestAngle;
        }
    }
}

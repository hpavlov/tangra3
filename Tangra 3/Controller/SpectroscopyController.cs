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
            uint brigthness20 = (uint)(selectedStar.Brightness / 5);
            uint bgFromPsf = (uint)(selectedStar.I0); 

            int minDistance = (int)(10 * selectedStar.FWHM);
            int clearDist =  (int)(2 * selectedStar.FWHM);

            AstroImage image = m_VideoController.GetCurrentAstroImage(false);

            float width = image.Width;
            float height = image.Height;

            uint[] angles = new uint[360];
            uint[] sums = new uint[360];
            uint[] pixAbove50Perc = new uint[360];

            int diagonnalPixels = (int)Math.Ceiling(Math.Sqrt(image.Width * image.Width + image.Height * image.Height));
            for (int i = 0; i < 360; i++)
            {
                var mapper = new RotationMapper(image.Width, image.Height, i);
                PointF p1 = mapper.GetDestCoords(x0, y0);
                float x1 = p1.X;
                float y1 = p1.Y;

                uint rowSum = 0;
                uint pixAbove50 = 0;
                uint pixAbove50Max = 0;
                bool prevPixAbove50 = false;

                for (int d = minDistance; d < diagonnalPixels; d++)
                {
                    PointF p = mapper.GetSourceCoords(x1 + d, y1);

                    if (p.X >= 0 && p.X < width && p.Y >= 0 && p.Y < height)
                    {
                        uint value = image.Pixelmap[(int) p.X, (int) p.Y];
                        rowSum += value;
                        PointF pu = mapper.GetSourceCoords(x1 + d, y1 + clearDist);
                        PointF pd = mapper.GetSourceCoords(x1 + d, y1 - clearDist);
                        if (pu.X >= 0 && pu.X < width && pu.Y >= 0 && pu.Y < height &&
                            pd.X >= 0 && pd.X < width && pd.Y >= 0 && pd.Y < height)
                        {
                            uint value_u = image.Pixelmap[(int)pu.X, (int)pu.Y];
                            uint value_d = image.Pixelmap[(int)pd.X, (int)pd.Y];
                            if ((value - bgFromPsf) > brigthness20 && value > value_u && value > value_d)
                            {
                                if (prevPixAbove50) pixAbove50++;
                                prevPixAbove50 = true;
                            }
                            else
                            {
                                prevPixAbove50 = false;
                                if (pixAbove50Max < pixAbove50) pixAbove50Max = pixAbove50;
                                pixAbove50 = 0;
                            }                            
                        }
                        else
                        {
                            prevPixAbove50 = false;
                            if (pixAbove50Max < pixAbove50) pixAbove50Max = pixAbove50;
                            pixAbove50 = 0;
                        }
                    }
                }

                angles[i] = (uint)i;
                sums[i] = rowSum;
                pixAbove50Perc[i] = pixAbove50Max;
            }

            Array.Sort(pixAbove50Perc, angles);

            uint roughAngle = angles[359];

            if (pixAbove50Perc[358] * 2 > pixAbove50Perc[359]) 
                return float.NaN;

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

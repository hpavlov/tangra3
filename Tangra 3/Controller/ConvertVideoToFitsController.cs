using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using nom.tam.fits;
using nom.tam.util;
using Tangra.Helpers;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.VideoOperations.LightCurves;

namespace Tangra.Controller
{
    public class ConvertVideoToFitsController
    {
		private Form m_MainFormView;
		private VideoController m_VideoController;
        private Rectangle m_RegionOfInterest;

        private int m_FrameWidth;
        private int m_FrameHeight;
        private bool m_UsesROI;
        private string m_FolderName;
        private bool m_FitsCube;

        private string m_DateObsComment;
        private string m_TimeObsComment;
        private string m_Note;

        public ConvertVideoToFitsController(Form mainFormView, VideoController videoController)
		{
			m_MainFormView = mainFormView;
			m_VideoController = videoController;
		}

        internal void StartExport(string fileName, bool fitsCube, Rectangle roi, UsedTimeBase timeBase, bool usesOCR)
        {
            m_FrameWidth = TangraContext.Current.FrameWidth;
            m_FrameHeight = TangraContext.Current.FrameHeight;

            m_RegionOfInterest = roi;
            m_UsesROI = roi.Width != m_FrameWidth || roi.Height != m_FrameHeight;

            m_FitsCube = fitsCube;
            if (!fitsCube)
            {
                m_FolderName = fileName;

                if (!Directory.Exists(m_FolderName)) 
                    Directory.CreateDirectory(m_FolderName);
            }

            m_Note = string.Format("Converted from {0} file.", m_VideoController.GetVideoFileFormat());
            if (m_Note.Length > HeaderCard.MAX_VALUE_LENGTH) m_Note = m_Note.Substring(0, HeaderCard.MAX_VALUE_LENGTH);
            m_DateObsComment = "Date (yyyy-MM-dd)";
            m_TimeObsComment = "Time (HH:mm:ss.fff)";

            if (m_UsesROI)
            {
                m_Note += string.Format(" Selected ROI: ({0},{1},{2},{3})", roi.Left, roi.Top, roi.Right, roi.Bottom);
                if (m_Note.Length > HeaderCard.MAX_VALUE_LENGTH) m_Note = m_Note.Substring(0, HeaderCard.MAX_VALUE_LENGTH);                
            }

            if (timeBase == UsedTimeBase.UserEnterred)
            {
                m_DateObsComment = "Date based on user entered VTI-OSD of a reference frame (yyyy-MM-dd)";
                m_TimeObsComment = "Time based on user entered VTI-OSD of a reference frame (HH:mm:ss.fff)";
            }
            else if (timeBase == UsedTimeBase.EmbeddedTimeStamp)
            {
                if (usesOCR)
                {
                    m_DateObsComment = "Date based on the ORC-ed VTI-OSD timestamp of the frame (yyyy-MM-dd)";
                    m_TimeObsComment = "Time based on the ORC-ed VTI-OSD timestamp of the frame (HH:mm:ss.fff)";                    
                }
                else
                {
                    m_DateObsComment = "Date of the frame as saved by the video recorder (yyyy-MM-dd)";
                    m_TimeObsComment = "Time of the frame as saved by the video recorder (HH:mm:ss.fff)";
                }
            }
        }

        internal void ProcessFrame(int frameNo, AstroImage astroImage, DateTime timeStamp, float exposureSeconds)
        {
            string fileName = Path.GetFullPath(string.Format("{0}\\{1}_{2}.fit", m_FolderName, frameNo.ToString().PadLeft(5, '0'), timeStamp.ToString("yyyy-MMM-dd_HHmmss_fff")));
            if (m_UsesROI)
            {
                uint[] subpixels = new uint[m_RegionOfInterest.Width * m_RegionOfInterest.Height];

                for (int y = 0; y < m_RegionOfInterest.Height; y++)
                {
                    for (int x = 0; x < m_RegionOfInterest.Width; x++)
                    {
                        subpixels[y * m_RegionOfInterest.Width + x] = astroImage.Pixelmap.Pixels[(m_RegionOfInterest.Top + y) * m_FrameWidth + m_RegionOfInterest.Left + x];
                    }
                }

                SaveFitsFrame(fileName, m_RegionOfInterest.Width, m_RegionOfInterest.Height, subpixels, timeStamp, exposureSeconds);
            }
            else
            {
                SaveFitsFrame(fileName, m_RegionOfInterest.Width, m_RegionOfInterest.Height, astroImage.Pixelmap.Pixels, timeStamp, exposureSeconds);
            }            
        }

        private static int[][] SaveImageData(int width, int height, uint[] data)
        {
            int[][] bimg = new int[height][];

            for (int y = 0; y < height; y++)
            {
                bimg[y] = new int[width];

                for (int x = 0; x < width; x++)
                {
                    bimg[y][x] = (int)Math.Max(0, data[x + (height - y - 1) * width]);
                }
            }

            return bimg;
        }

        internal void SaveFitsFrame(string fileName, int width, int height, uint[] framePixels, DateTime timeStamp, float exposureSeconds)
        {
            Fits f = new Fits();

            object data = SaveImageData(width, height, framePixels);

            BasicHDU imageHDU = Fits.MakeHDU(data);

            nom.tam.fits.Header hdr = imageHDU.Header;
            hdr.AddValue("SIMPLE", "T", null);

            hdr.AddValue("BITPIX", 32, null);
            hdr.AddValue("NAXIS", 2, null);
            hdr.AddValue("NAXIS1", width, null);
            hdr.AddValue("NAXIS2", height, null);

            hdr.AddValue("NOTES", m_Note, null);

            if (exposureSeconds > 0)
                hdr.AddValue("EXPOSURE", exposureSeconds.ToString("0.000", CultureInfo.InvariantCulture), "Exposure, seconds");

            hdr.AddValue("DATEOBS", timeStamp.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), m_DateObsComment);
            hdr.AddValue("TIMEOBS", timeStamp.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture), m_TimeObsComment);

            hdr.AddValue("TANGRAVE", string.Format("{0} v{1}", VersionHelper.AssemblyProduct, VersionHelper.AssemblyFileVersion), "Tangra version");

            f.AddHDU(imageHDU);

            // Write a FITS file.
            using (BufferedFile bf = new BufferedFile(fileName, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                f.Write(bf);
                bf.Flush();
            }
        }

        internal void FinishExport()
        {
            
        }
    }
}

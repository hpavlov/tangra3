using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangra.Model.Context;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.PInvoke;
using Tangra.Video;
using Tangra.Video.SER;

namespace Tangra.Automation
{
    public class AutomationVideoPlayer : IImagePixelProvider, IDisposable
    {
        protected IFramePlayer m_FramePlayer;

        public AutomationVideoPlayer()
        {
            m_FramePlayer = new FramePlayer();
        }

        public bool OpenVideo(string fileName)
        {
            string fileExtension = Path.GetExtension(fileName);

            if (fileExtension != null)
                fileExtension = fileExtension.ToLower();

            IFrameStream frameStream = null;

            if (fileExtension == ".adv" || fileExtension == ".aav")
            {
                AdvFileMetadataInfo fileMetadataInfo;
                GeoLocationInfo geoLocation;
                frameStream = AstroDigitalVideoStream.OpenFile(fileName, out fileMetadataInfo, out geoLocation);
            }
            else if (fileExtension == ".avi")
            {
                frameStream = VideoStream.OpenFileForAutomation(fileName, 0);
                frameStream = ReInterlacingVideoStream.Create(frameStream, ReInterlaceMode.None);
            }
            else
            {
                Console.Error.WriteLine("{0} files are not supported.", fileExtension);
                Console.WriteLine(string.Format("{0} files are not supported.", fileExtension));

                return false;
            }

            if (frameStream != null && m_FramePlayer != null)
            {
                m_FramePlayer.OpenVideo(frameStream);
                return true;
            }

            return false;
        }

        #region IImagePixelProvider Implementation
        public int Width
        {
            get { return m_FramePlayer.Video.Width; }
        }

        public int Height
        {
            get { return m_FramePlayer.Video.Height; }
        }

        public int FirstFrame
        {
            get { return m_FramePlayer.Video.FirstFrame; }
        }

        public int LastFrame
        {
            get { return m_FramePlayer.Video.LastFrame; }
        }

        public int[,] GetPixelArray(int frameNo, Rectangle rect)
        {
            if (rect.Width != 32 || rect.Height != 32)
                throw new NotSupportedException();

            int[,] rv = new int[32, 32];

            Pixelmap pixelmap = m_FramePlayer.GetFrame(frameNo, true);

            for (int x = 0; x < 32; x++)
                for (int y = 0; y < 32; y++)
                {
                    rv[x, y] = (int)pixelmap.UnprocessedPixels[(x + rect.Left) + (y + rect.Top) * pixelmap.Width];
                }

            return rv;
        }
        #endregion

        public void Dispose()
        {
            m_FramePlayer.CloseVideo();
        }
    }
}

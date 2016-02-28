using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Image;
using Tangra.PInvoke;

namespace Tangra.Video.AstroDigitalVideo
{
    public enum AdvToAviConverter
    {
        VideoForWindowsAviSaver,
        DirectShowAviSaver
    }

    internal interface IAviSaver
    {
        void CloseAviFile();
        bool StartNewAviFile(string fileName, int width, int height, int bpp, double fps, bool tryCodec);
        bool AddAviVideoFrame(Pixelmap pixmap, double addedGamma, int? adv16NormalisationValue);
    }

    internal class AdvToAviConverterFactory
    {
        public static IAviSaver CreateConverter(AdvToAviConverter converter)
        {
            switch (converter)
            {
                case AdvToAviConverter.VideoForWindowsAviSaver:
                    return new VideoForWindowsAviSaver();

                case AdvToAviConverter.DirectShowAviSaver:
                    return new DirectShowAviSaver();

                default:
                    throw new NotSupportedException();
            }
        }
    }

    internal class DirectShowAviSaver : IAviSaver
    {

        public void CloseAviFile()
        {
            throw new NotImplementedException();
        }

        public bool StartNewAviFile(string fileName, int width, int height, int bpp, double fps, bool tryCodec)
        {
            throw new NotImplementedException();
        }

        public bool AddAviVideoFrame(Pixelmap pixmap, double addedGamma, int? adv16NormalisationValue)
        {
            throw new NotImplementedException();
        }
    }

    internal class VideoForWindowsAviSaver : IAviSaver
    {

        public void CloseAviFile()
        {
            TangraVideo.CloseAviFile();
        }

        public bool StartNewAviFile(string fileName, int width, int height, int bpp, double fps, bool tryCodec)
        {
            if (!TangraVideo.StartNewAviFile(fileName, width, height, bpp, fps, tryCodec))
            {
                MessageBox.Show("There was an error calling AddAviVideoFrame:\r\n\r\n" + TangraVideo.GetLastAviErrorMessage(), "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        public bool AddAviVideoFrame(Pixelmap pixmap, double addedGamma, int? adv16NormalisationValue)
        {
            if (!TangraVideo.AddAviVideoFrame(pixmap, addedGamma, adv16NormalisationValue))
            {
                MessageBox.Show("There was an error calling AddAviVideoFrame:\r\n\r\n" + TangraVideo.GetLastAviErrorMessage(), "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
    }
}

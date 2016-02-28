using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        void StartNewAviFile(string fileName, int width, int height, int bpp, double fps, bool tryCodec);
        void AddAviVideoFrame(Pixelmap pixmap, double addedGamma, int? adv16NormalisationValue);
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

        public void StartNewAviFile(string fileName, int width, int height, int bpp, double fps, bool tryCodec)
        {
            throw new NotImplementedException();
        }

        public void AddAviVideoFrame(Pixelmap pixmap, double addedGamma, int? adv16NormalisationValue)
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

        public void StartNewAviFile(string fileName, int width, int height, int bpp, double fps, bool tryCodec)
        {
            TangraVideo.StartNewAviFile(fileName, width, height, bpp, fps, tryCodec);
        }

        public void AddAviVideoFrame(Pixelmap pixmap, double addedGamma, int? adv16NormalisationValue)
        {
            TangraVideo.AddAviVideoFrame(pixmap, addedGamma, adv16NormalisationValue);
        }
    }
}

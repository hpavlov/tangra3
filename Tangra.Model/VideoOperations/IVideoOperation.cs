using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Astro;
using Tangra.Model.Image;
using Tangra.Model.ImageTools;
using Tangra.Model.Video;


namespace Tangra.Model.VideoOperations
{
    public interface IVideoOperation
    {
        bool InitializeOperation(IVideoController videoContoller, Panel controlPanel, IFramePlayer framePlayer, Form topForm);
        void FinalizeOperation();

        void PlayerStarted();
        void NextFrame(int frameNo, MovementType movementType, bool isLastFrame, AstroImage astroImage, int firstFrameInIntegrationPeriod, string fileName);
        void ImageToolChanged(ImageTool newTool, ImageTool oldTool);

        void PreDraw(Graphics g);
        void PostDraw(Graphics g);

        void MouseLeave();
        void MouseMove(Point location);
        void MouseClick(ObjectClickEventArgs e);
        void MouseDown(Point location);
        void MouseUp(Point location);
        void MouseDoubleClick(Point location);

        bool HasCustomZoomImage { get; }
        bool DrawCustomZoomImage(Graphics g, int width, int height);
        bool AvoidImageOverlays { get; }
    }

    public interface IFramePreProcessor
    {
        void OnPreProcess(Pixelmap newFrame);
		uint[,] OnPreProcessPixels(uint[,] pixels);
    }
}

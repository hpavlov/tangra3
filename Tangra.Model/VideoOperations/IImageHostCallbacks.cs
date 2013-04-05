using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Image;
using Tangra.Model.ImageTools;

namespace Tangra.Model.VideoOperations
{
    public interface IImageHostCallbacksDepricated
    {
        void ShowExtraInfo(string info);
        void StatusChanged(string newStatus);
		void ShowToolInfo(string toolInfo);

        Panel Panel { get; }
        PictureBox DisplayControl { get; }
        IWin32Window MainFormWindow { get; }
        CompositeFramePreProcessor FramePreProcessor { get; }

        int CurrentFrameId { get; }

        void SetCanPlayVideo(bool canPlayVideo);
        void AbortCurrentOperation();
        void RefreshCurrentFrame();
        void RedrawCurrentFrame(bool showFields, bool shalowRedraw);

        void SetPictureBoxCursor(Cursor cur);
        void InvalidatePictureBox();
		void InvalidatePictureBox(Bitmap newBmp);
        void PlayVideo();

        void ApplyPreProcessing(Pixelmap bmp);

        void ChangeImageTool(ImageTool newTool);
        ImageTool CurrentImageTool { get; }

        Rectangle MainFormPosition { get; }

    	void ResetDelayedIndicatorForm();
    }
}

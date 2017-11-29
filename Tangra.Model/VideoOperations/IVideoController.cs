using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Astro;
using Tangra.Model.Image;
using Tangra.Model.Video;

namespace Tangra.Model.VideoOperations
{
    public interface IVideoController
    {
        void StatusChanged(string displayName);
        
		DialogResult ShowMessageBox(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon);
		DialogResult ShowMessageBox(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton);
		DialogResult ShowSaveFileDialog(string title, string filter, ref string fileName, IWin32Window ownerWindow = null);
		DialogResult ShowOpenFileDialog(string title, string filter, out string fileName);
        
		void UpdateViews();
        
		void RefreshCurrentFrame();
        void RedrawCurrentFrame(bool showFields, bool reloadImage = false, bool reprocess = true);
        void ToggleShowFieldsMode(bool showFields);

		void UpdateZoomedImage(Bitmap zoomedBitmap, ImagePixel center, Pixelmap displayPixelmap = null);
        void ClearZoomedImage();
        
		AstroImage GetCurrentAstroImage(bool integrated);
		FrameStateData GetCurrentFrameState();

        double VideoFrameRate { get; }
		int VideoBitPix { get; }
		int VideoCountFrames { get; }
		int VideoFirstFrame { get; }
		int VideoLastFrame { get; }

		void PlayVideo(int? startAtFrame = null, uint step = 1);
	    void StopVideo(Action<int, bool> callback = null);
	    void MoveToFrame(int frameId);
	    void StepForward();
		void StepBackward();

	    bool IsAstroDigitalVideo { get; }	 
		bool HasAstroImageState { get; }

	    string CurrentVideoFileName { get; }
		string CurrentVideoFileType { get; }

        void RegisterExtractingOcrTimestamps();
		void RegisterOcrError();

        IFramePlayer FramePlayer { get; }
    }
}

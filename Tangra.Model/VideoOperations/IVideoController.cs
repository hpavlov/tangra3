using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Astro;
using Tangra.Model.Image;

namespace Tangra.Model.VideoOperations
{
    public interface IVideoController
    {
        void StatusChanged(string displayName);
        
		DialogResult ShowMessageBox(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon);
		DialogResult ShowMessageBox(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton);
		DialogResult ShowSaveFileDialog(string title, string filter, ref string fileName);
		DialogResult ShowOpenFileDialog(string title, string filter, out string fileName);
        
		void UpdateViews();
        
		void RefreshCurrentFrame();
	    void RedrawCurrentFrame(bool showFields);
        void ToggleShowFieldsMode(bool showFields);

		void UpdateZoomedImage(Bitmap zoomedBitmap);
        void ClearZoomedImage();
        
		AstroImage GetCurrentAstroImage(bool integrated);
		FrameStateData GetCurrentFrameState();

        double VideoFrameRate { get; }
		int VideoBitPix { get; }
		int VideoCountFrames { get; }
		int VideoFirstFrame { get; }
		int VideoLastFrame { get; }

		void PlayVideo();
	    void StopVideo();
	    void MoveToFrame(int frameId);
	    void StepForward();
		void StepBackward();

	    bool IsAstroDigitalVideo { get; }	 
		bool HasAstroImageState { get; }

	    string CurrentVideoFileName { get; }
		string CurrentVideoFileType { get; }

        void RegisterExtractingOcrTimestamps();
        void RegisterOcrError();
    }
}

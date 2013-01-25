using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Astro;

namespace Tangra.Model.VideoOperations
{
    public interface IVideoController
    {
        void StatusChanged(string displayName);
        DialogResult ShowMessageBox(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon);
        void UpdateViews();
        void RefreshCurrentFrame();
        void UpdateZoomedImage(Bitmap zoomedBitmap);
        void ClearZoomedImage();
        AstroImage GetCurrentAstroImage(bool integrated);
        double VideoFrameRate { get; }
    }
}

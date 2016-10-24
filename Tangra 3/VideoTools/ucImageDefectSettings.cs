using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Model.Config;
using Tangra.Model.Video;
using Tangra.Video;
using Tangra.VideoOperations;
using Tangra.VideoOperations.Astrometry.Engine;

namespace Tangra.VideoTools
{
    public partial class ucImageDefectSettings : UserControl
    {
        private IFrameStream m_VideoStream;
        private ReInterlacingVideoStream m_ReInterlacedStream;

        public ucImageDefectSettings()
        {
            InitializeComponent();
        }

        public void Initialize(IFrameStream videoStream, int currentFrameNo)
        {
            m_VideoStream = videoStream;
            m_ReInterlacedStream = videoStream as ReInterlacingVideoStream;
            
            SetFilterMode();

            gbxInterlacedSettings.Enabled = m_ReInterlacedStream != null;
            rbReInterlaceNon.Checked = true;
        }

        private void btnPreProcessingFilter_Click(object sender, EventArgs e)
        {
            contextMenuFilter.Show(
                btnPreProcessingFilter,
                new Point(0, 0),
                ToolStripDropDownDirection.BelowRight);
        }

        private void contextMenuFilter_Opening(object sender, CancelEventArgs e)
        {
            if (AstrometryContext.Current.FieldSolveContext.UseFilter == TangraConfig.PreProcessingFilter.NoFilter)
                miNoFilter.Checked = true;
            else if (AstrometryContext.Current.FieldSolveContext.UseFilter == TangraConfig.PreProcessingFilter.LowPassFilter)
                miLowPass.Checked = true;
        }

        private bool m_NoFilterSetting = false;

        private void miFilter_CheckedChanged(object sender, EventArgs e)
        {
            if (m_NoFilterSetting) return;

            ToolStripMenuItem clickedItem = sender as ToolStripMenuItem;
            if (clickedItem == miNoFilter)
                AstrometryContext.Current.FieldSolveContext.UseFilter = TangraConfig.PreProcessingFilter.NoFilter;
            else if (clickedItem == miLowPass)
                AstrometryContext.Current.FieldSolveContext.UseFilter = TangraConfig.PreProcessingFilter.LowPassFilter;

            SetFilterMode();
        }

        private void SetFilterMode()
        {
            m_NoFilterSetting = true;
            try
            {
                miNoFilter.Checked = false;
                miLowPass.Checked = false;

                if (AstrometryContext.Current.FieldSolveContext.UseFilter == TangraConfig.PreProcessingFilter.NoFilter)
                {
                    btnPreProcessingFilter.Text = "No Filter";
                    miNoFilter.Checked = true;
                }
                else if (AstrometryContext.Current.FieldSolveContext.UseFilter == TangraConfig.PreProcessingFilter.LowPassFilter)
                {
                    btnPreProcessingFilter.Text = "Low-Pass Filter";
                    miLowPass.Checked = true;
                }
            }
            finally
            {
                m_NoFilterSetting = false;
            }
        }

        private void ReInterlacedModeChanged(object sender, EventArgs e)
        {
            ReInterlaceMode mode = ReInterlaceMode.None;

            if (rbReInterlaceNon.Checked)
                mode = ReInterlaceMode.None;
            else if (rbReInterlaceSwapFields.Checked)
                mode = ReInterlaceMode.SwapFields;
            else if (rbReInterlaceShiftForward.Checked)
                mode = ReInterlaceMode.ShiftOneField;
            else if (rbReInterlaceShiftAndSwap.Checked)
                mode = ReInterlaceMode.SwapAndShiftOneField;

            if (m_ReInterlacedStream.ChangeReInterlaceMode(mode))
            {
                FrameAdjustmentsPreview.Instance.ReInterlace(mode);
            }
        }

        private void cbxUseHotPixelsCorrection_CheckedChanged(object sender, EventArgs e)
        {
            FrameAdjustmentsPreview.Instance.CorrectHotPixels(cbxUseHotPixelsCorrection.Checked);
        }
    }
}

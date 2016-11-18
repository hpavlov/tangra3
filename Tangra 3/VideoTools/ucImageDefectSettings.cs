using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Image;
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

            frmFullSizePreview.OnDrawOverlays += frmFullSizePreview_OnDrawOverlays;
            frmFullSizePreview.OnMouseClicked += frmFullSizePreview_OnMouseClicked;
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if (disposing)
            {
                frmFullSizePreview.OnMouseClicked -= frmFullSizePreview_OnMouseClicked;
                frmFullSizePreview.OnDrawOverlays -= frmFullSizePreview_OnDrawOverlays;                
            }
            
            base.Dispose(disposing);
        }

        public void Initialize(IFrameStream videoStream, int currentFrameNo)
        {
            m_VideoStream = videoStream;
            m_ReInterlacedStream = videoStream as ReInterlacingVideoStream;
            
            SetFilterMode();

            gbxInterlacedSettings.Enabled = m_ReInterlacedStream != null;
            rbReInterlaceNon.Checked = true;

            HotPixelCorrector.ConfigurePreProcessing(false);
            tbDepth.Value = 20;
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

        public bool m_ExpectHotPixelDefinition = false;

        private void cbxUseHotPixelsCorrection_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxUseHotPixelsCorrection.Checked)
            {
                HotPixelCorrector.Initialize();

                FrameAdjustmentsPreview.Instance.ExpectHotPixelClick(true, true);

                MessageBox.Show(ParentForm, 
                    "Please select an existing hot pixel by clicking on it.",
                    "Tangra", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                m_ExpectHotPixelDefinition = true;
            }
            else
            {
                m_ExpectHotPixelDefinition = false;
                FrameAdjustmentsPreview.Instance.ExpectHotPixelClick(false, false);

                HotPixelCorrector.Cleanup();
            }

            pnlHotPixelControls.Enabled = false;
        }


        void frmFullSizePreview_OnMouseClicked(MouseEventArgs e)
        {
            if (cbxUseHotPixelsCorrection.Checked && m_ExpectHotPixelDefinition && frmFullSizePreview.CurrFrame != null)
            {
                var pixels = frmFullSizePreview.CurrFrame.GetPixelsArea(e.X, e.Y, 35); 
                PSFFit fit = new PSFFit(e.X, e.Y);
                fit.Fit(pixels);
                if (fit.IsSolved && fit.Certainty > 1 && fit.IMax > frmFullSizePreview.CurrFrame.MaxSignalValue/2.0)
                {
                    var sample = frmFullSizePreview.CurrFrame.GetPixelsArea((int) Math.Round(fit.XCenter), (int) Math.Round(fit.YCenter), 7);
                    HotPixelCorrector.RegisterHotPixelSample(sample, frmFullSizePreview.CurrFrame.Pixelmap.MaxSignalValue);

                    m_ExpectHotPixelDefinition = false;
                    FrameAdjustmentsPreview.Instance.ExpectHotPixelClick(false, true);
                    pnlHotPixelControls.Enabled = true;

                    HotPixelCorrector.LocateHotPixels(frmFullSizePreview.CurrFrame, Math.Max(0, tbDepth.Value));

                    FrameAdjustmentsPreview.Instance.Update();
                }
                else
                {
                    MessageBox.Show(
                        ParentForm, 
                        "The location you clicked doesn't appear to contain a hot pixel. Please try again.\r\n\r\nIf necessary adjust the brightness and contrast of the image first.", 
                        "Tangra", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        void frmFullSizePreview_OnDrawOverlays(Graphics g)
        {
            if (cbxUseHotPixelsCorrection.Checked)
                HotPixelCorrector.DrawOverlay(g, Math.Max(1, tbDepth.Value), cbxPlotPeakPixels.Checked);                
        }

        private void ucImageDefectSettings_Load(object sender, EventArgs e)
        {

        }

        private void tbDepth_Scroll(object sender, EventArgs e)
        {
            int depth = tbDepth.Value;
            if (depth > 0)
            {
                lblDepthValue.Text = depth.ToString();

                HotPixelCorrector.LocateHotPixels(frmFullSizePreview.CurrFrame, depth);

                FrameAdjustmentsPreview.Instance.Update();
            }            
        }

        private void rbHotPixelsShowPositions_CheckedChanged(object sender, EventArgs e)
        {
            FrameAdjustmentsPreview.Instance.Update();
        }

        private void cbxPlotPeakPixels_CheckedChanged(object sender, EventArgs e)
        {
            FrameAdjustmentsPreview.Instance.Update();
        }

        private void rbHotPixelsPreviewRemove_CheckedChanged(object sender, EventArgs e)
        {
            FrameAdjustmentsPreview.Instance.RemoveHotPixels(rbHotPixelsPreviewRemove.Checked);
        }

        internal void ApplyHotPixelSettings()
        {
            HotPixelCorrector.ConfigurePreProcessing(cbxUseHotPixelsCorrection.Checked);
        }
    }
}

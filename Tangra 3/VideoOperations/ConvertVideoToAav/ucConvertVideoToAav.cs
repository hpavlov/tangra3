using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Helpers;
using Tangra.ImageTools;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.MotionFitting;
using Tangra.Properties;
using Tangra.Video;
using Tangra.VideoTools;

namespace Tangra.VideoOperations.ConvertVideoToAav
{
    public enum AavConfigState
    {
        ConfirmingVtiOsdPosition,
        DetectingIntegrationRate,
        ReadyToConvert,
        Converting,
        FinishedConverting

    }
    public partial class ucConvertVideoToAav : UserControl
    {
        private ConvertVideoToAavOperation m_Operation;
        private VideoController m_VideoController;

        private AavConfigState m_State;
        private RoiSelector m_RoiSelector;

        public ucConvertVideoToAav()
        {
            InitializeComponent();
        }

        public ucConvertVideoToAav(ConvertVideoToAavOperation operation, VideoController videoController)
            : this()
        {
            m_Operation = operation;
            m_VideoController = videoController;

            m_RoiSelector = m_VideoController.CurrentImageTool as RoiSelector;

            nudFirstFrame.Minimum = videoController.VideoFirstFrame;
            nudFirstFrame.Maximum = videoController.VideoLastFrame - 1;
            nudFirstFrame.Value = nudFirstFrame.Minimum;

            nudLastFrame.Minimum = videoController.VideoFirstFrame;
            nudLastFrame.Maximum = videoController.VideoLastFrame - 1;
            nudLastFrame.Value = nudLastFrame.Maximum;

            nudStartingAtFrame.Minimum = videoController.VideoFirstFrame;
            nudStartingAtFrame.Maximum = videoController.VideoLastFrame - 1;
            nudStartingAtFrame.Value = nudFirstFrame.Minimum;

            AutoDetectVTIOSDPosition();

            m_State = AavConfigState.ConfirmingVtiOsdPosition;
            UpdateControlState();
        }

        private void UpdateVtiOsdPositions(int fromLine, int toLine)
        {
            m_RoiSelector.SetUserFrame(new Rectangle(0, fromLine, TangraContext.Current.FrameWidth, toLine - fromLine));
            m_VideoController.RefreshCurrentFrame();
        }

        private void AutoDetectVTIOSDPosition()
        {
            var pixelMap = m_VideoController.GetCurrentAstroImage(false).Pixelmap;
            if (!LocateTimestampPosition(pixelMap.Pixels, pixelMap.Width, pixelMap.Height))
            {
                var lastChoise = TangraConfig.Settings.AAV.GetLastOsdPositionForFrameSize(m_VideoController.FramePlayer.Video.Width, m_VideoController.FramePlayer.Video.Height);
                if (lastChoise != null)
                {
                    m_RoiSelector.SetUserFrame(new Rectangle(lastChoise.Item3, lastChoise.Item1, lastChoise.Item4 - lastChoise.Item3, lastChoise.Item2 - lastChoise.Item1));
                    m_VideoController.RefreshCurrentFrame();
                }
                m_VideoController.ShowMessageBox("Cannot locate the VTI-OSD position. Please specify it manually.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LocateTopAndBottomLineOfTimestamp(uint[] preProcessedPixels, int imageWidth, int fromHeight, int toHeight, out int bestTopPosition, out int bestBottomPosition)
        {
            int bestTopScope = -1;
            bestBottomPosition = -1;
            bestTopPosition = -1;
            int bestBottomScope = -1;

            for (int y = fromHeight + 1; y < toHeight - 1; y++)
            {
                int topScore = 0;
                int bottomScore = 0;

                for (int x = 0; x < imageWidth; x++)
                {
                    if (preProcessedPixels[x + imageWidth * (y + 1)] < 127 && preProcessedPixels[x + imageWidth * y] > 127)
                    {
                        topScore++;
                    }

                    if (preProcessedPixels[x + imageWidth * (y - 1)] < 127 && preProcessedPixels[x + imageWidth * y] > 127)
                    {
                        bottomScore++;
                    }
                }

                if (topScore > bestTopScope)
                {
                    bestTopScope = topScore;
                    bestTopPosition = y;
                }

                if (bottomScore > bestBottomScope)
                {
                    bestBottomScope = bottomScore;
                    bestBottomPosition = y;
                }
            }
        }

        private bool LocateTimestampPosition(uint[] data, int frameWidth, int frameHeight)
        {
            uint[] preProcessedPixels = new uint[data.Length];
            Array.Copy(data, preProcessedPixels, data.Length);

            // Process the image
            uint median = preProcessedPixels.Median();
            for (int i = 0; i < preProcessedPixels.Length; i++)
            {
                int darkCorrectedValue = (int)preProcessedPixels[i] - (int)median;
                if (darkCorrectedValue < 0) darkCorrectedValue = 0;
                preProcessedPixels[i] = (uint)darkCorrectedValue;
            }

            if (median > 250)
            {
                //InitiazliationError = "The background is too bright.";
                return false;
            }

            uint[] blurResult = BitmapFilter.GaussianBlur(preProcessedPixels, 8, frameWidth, frameHeight);
            uint average = 128;
            uint[] sharpenResult = BitmapFilter.Sharpen(blurResult, 8, frameWidth, frameHeight, out average);

            // Binerize and Inverse
            for (int i = 0; i < sharpenResult.Length; i++)
            {
                sharpenResult[i] = sharpenResult[i] > average ? (uint)0 : (uint)255;
            }
            uint[] denoised = BitmapFilter.Denoise(sharpenResult, 8, frameWidth, frameHeight, out average, false);

            for (int i = 0; i < denoised.Length; i++)
            {
                preProcessedPixels[i] = denoised[i] < 127 ? (uint)0 : (uint)255;
            }

            int bestBottomPosition = -1;
            int bestTopPosition = -1;
            LocateTopAndBottomLineOfTimestamp(
                preProcessedPixels,
                frameWidth,
                frameHeight / 2 + 1,
                frameHeight,
                out bestTopPosition,
                out bestBottomPosition);

            if (bestBottomPosition - bestTopPosition < 10 || bestBottomPosition - bestTopPosition > 60 || bestTopPosition < 0 || bestBottomPosition > frameHeight)
            {
                return false;
            }

            int fromLine = bestTopPosition - 1;
            int toLine = bestBottomPosition + 3;
            if (toLine > frameHeight)
                toLine = frameHeight - 2;

            if ((toLine - fromLine) % 2 == 1)
            {
                if (fromLine % 2 == 1)
                    fromLine--;
                else
                    toLine++;
            }

            #region We need to make sure that the two fields have the same top and bottom lines

            // Create temporary arrays so the top/bottom position per field can be further refined
            int fieldAreaHeight = (toLine - fromLine) / 2;
            int fieldAreaWidth = frameWidth;
            uint[] oddFieldPixelsPreProcessed = new uint[frameWidth * fieldAreaHeight];
            uint[] evenFieldPixelsPreProcessed = new uint[frameWidth * fieldAreaHeight];

            int[] DELTAS = new int[] { 0, -1, 1 };
            int fromLineBase = fromLine;
            int toLineBase = toLine;
            bool matchFound = false;

            for (int deltaIdx = 0; deltaIdx < DELTAS.Length; deltaIdx++)
            {
                fromLine = fromLineBase + DELTAS[deltaIdx];
                toLine = toLineBase + DELTAS[deltaIdx];

                int bestBottomPositionOdd = -1;
                int bestTopPositionOdd = -1;
                int bestBottomPositionEven = -1;
                int bestTopPositionEven = -1;

                LocateTopAndBottomLineOfTimestamp(
                    oddFieldPixelsPreProcessed,
                    fieldAreaWidth, 1, fieldAreaHeight - 1, 
                    out bestTopPositionOdd, out bestBottomPositionOdd);

                LocateTopAndBottomLineOfTimestamp(
                    evenFieldPixelsPreProcessed,
                    fieldAreaWidth, 1, fieldAreaHeight - 1, 
                    out bestTopPositionEven, out bestBottomPositionEven);

                if (bestBottomPositionOdd == bestBottomPositionEven &&
                    bestTopPositionOdd == bestTopPositionEven)
                {
                    matchFound = true;
                    fromLine = fromLineBase;
                    toLine = toLineBase;

                    break;
                }
            }
            #endregion

            if (matchFound)
            {
                UpdateVtiOsdPositions(fromLine, toLine);                
            }

            return matchFound;
        }

        private void UpdateControlState()
        {
            if (m_State == AavConfigState.ConfirmingVtiOsdPosition)
            {
                gbxVTIPosition.Enabled = true;
                btnConfirmPosition.Enabled = true;
                gbxSection.Visible = false;
                gbxIntegrationRate.Visible = false;
                btnDetectIntegrationRate.Visible = false;
                btnConvert.Enabled = false;
                pbar.Visible = false;
                btnCancel.Enabled = false;
                m_RoiSelector.Enabled = true;
            }
            else if (m_State == AavConfigState.DetectingIntegrationRate)
            {
                gbxVTIPosition.Enabled = false;
                btnConfirmPosition.Enabled = false;
                gbxSection.Visible = true;
                btnDetectIntegrationRate.Visible = true;
                btnConvert.Enabled = false;
                m_RoiSelector.Enabled = false;
                m_RoiSelector.DisplayOnlyMode = true;
            }
            else if (m_State == AavConfigState.ReadyToConvert)
            {
                btnConvert.Enabled = true;
                btnDetectIntegrationRate.Enabled = false;
                nudFirstFrame.Enabled = false;
                gbxIntegrationRate.Visible = true;
                gbxIntegrationRate.Enabled = false;
                gbxCameraInfo.Visible = true;
                pnlFirstField.Visible = true;
                m_RoiSelector.Enabled = false;
                m_RoiSelector.DisplayOnlyMode = false;
                m_VideoController.RedrawCurrentFrame(true);
            }
            else if (m_State == AavConfigState.Converting)
            {
                pbar.Visible = true;
                btnConvert.Enabled = false;
                gbxSection.Enabled = false;
                gbxIntegrationRate.Enabled = false;
                gbxCameraInfo.Enabled = false;
                pnlFirstField.Enabled = false;
                btnDetectIntegrationRate.Enabled = false;
                btnCancel.Enabled = true;
                m_RoiSelector.Enabled = false;
                m_RoiSelector.DisplayOnlyMode = false;
            }
            else if (m_State == AavConfigState.FinishedConverting)
            {
                pbar.Value = pbar.Maximum;
                btnCancel.Enabled = false;
                m_RoiSelector.Enabled = false;
                m_RoiSelector.DisplayOnlyMode = true;
            }
        }

        private void btnConfirmPosition_Click(object sender, EventArgs e)
        {
            m_State = AavConfigState.DetectingIntegrationRate;
            nudFirstFrame.SetNUDValue(m_VideoController.CurrentFrameIndex);

            var vtiOsdPosition = m_RoiSelector.SelectedROI;

            TangraConfig.Settings.AAV.RegisterOsdPosition(
                m_VideoController.FramePlayer.Video.Width,
                m_VideoController.FramePlayer.Video.Height,
                vtiOsdPosition.Top,
                vtiOsdPosition.Bottom,
                vtiOsdPosition.Left,
                vtiOsdPosition.Right);

            TangraConfig.Settings.Save();

            UpdateControlState();
        }

        private void btnDetectIntegrationRate_Click(object sender, EventArgs e)
        {
            var frm = new frmIntegrationDetection(m_VideoController, (int)nudFirstFrame.Value);
            frm.StartPosition = FormStartPosition.CenterParent;
            DialogResult res = frm.ShowDialog(this);

            if (res == DialogResult.OK)
            {
                if (frm.IntegratedFrames != null)
                {
                    nudIntegratedFrames.SetNUDValue(frm.IntegratedFrames.Interval);
                    nudStartingAtFrame.SetNUDValue(frm.IntegratedFrames.StartingAtFrame);

                    m_State = AavConfigState.ReadyToConvert;
                    var reinterlacedStream = m_VideoController.FramePlayer.Video as ReInterlacingVideoStream;
                    if (reinterlacedStream != null && (reinterlacedStream.Mode == ReInterlaceMode.ShiftOneField || reinterlacedStream.Mode == ReInterlaceMode.SwapFields))
                        rbFirstFieldBottom.Checked = true;
                    UpdateControlState();
                }
            }
            else if (res == DialogResult.Cancel)
            {
                m_VideoController.ShowMessageBox("This must be an integrated video with consistent and known integration rate in order to convert it to AAV.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void nudStartingAtFrame_ValueChanged(object sender, EventArgs e)
        {
            nudFirstFrame.Minimum = nudStartingAtFrame.Value;
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cbxCameraModel.Text))
            {
                m_VideoController.ShowMessageBox(
                    "Camera model must be specified. Use 'Unknown' if the camera model is not known.", 
                    "Tangra", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);

                cbxCameraModel.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(cbxSensorInfo.Text))
            {
                m_VideoController.ShowMessageBox(
                    "Sensor info must be specified. Use 'Unknown' if the camera sensor is not known.",
                    "Tangra",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                cbxSensorInfo.Focus();
                return;
            }

            saveFileDialog.FileName = Path.GetFileName(Path.ChangeExtension(m_VideoController.CurrentVideoFileName, ".aav"));

            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                m_State = AavConfigState.Converting;
                UpdateControlState();

                m_VideoController.MoveToFrame((int) nudFirstFrame.Value);

                var vtiOsdPosition = m_RoiSelector.SelectedROI;

                m_Operation.StartConversion(
                    saveFileDialog.FileName,
                    vtiOsdPosition.Top,
                    vtiOsdPosition.Bottom,
                    vtiOsdPosition.Left,
                    vtiOsdPosition.Right,
                    (int)nudStartingAtFrame.Value,
                    (int)nudIntegratedFrames.Value,
                    (int)nudLastFrame.Value,
                    cbxCameraModel.Text,
                    cbxSensorInfo.Text,
                    rbFirstFieldBottom.Checked);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            m_State = AavConfigState.FinishedConverting;
            UpdateControlState();

            m_Operation.EndConversion();
        }

        internal void EndConversion()
        {
            m_State = AavConfigState.FinishedConverting;
            UpdateControlState();
        }

        internal void UpdateProgress(int currentFrame, int maxFrames)
        {
            pbar.Maximum = maxFrames;
            pbar.Value = Math.Max(pbar.Minimum, Math.Min(currentFrame, pbar.Maximum));
            pbar.Update();
        }
    }
}

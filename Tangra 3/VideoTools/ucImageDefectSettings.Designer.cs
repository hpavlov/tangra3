namespace Tangra.VideoTools
{
    partial class ucImageDefectSettings
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.gbxInterlacedSettings = new System.Windows.Forms.GroupBox();
            this.rbReInterlaceShiftAndSwap = new System.Windows.Forms.RadioButton();
            this.rbReInterlaceShiftForward = new System.Windows.Forms.RadioButton();
            this.rbReInterlaceNon = new System.Windows.Forms.RadioButton();
            this.rbReInterlaceSwapFields = new System.Windows.Forms.RadioButton();
            this.btnPreProcessingFilter = new System.Windows.Forms.Button();
            this.contextMenuFilter = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miNoFilter = new System.Windows.Forms.ToolStripMenuItem();
            this.miLowPass = new System.Windows.Forms.ToolStripMenuItem();
            this.cbxUseHotPixelsCorrection = new System.Windows.Forms.CheckBox();
            this.gbxHotPixels = new System.Windows.Forms.GroupBox();
            this.pnlHotPixelControls = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbHotPixelsShowPositions = new System.Windows.Forms.RadioButton();
            this.rbHotPixelsPreviewRemove = new System.Windows.Forms.RadioButton();
            this.tbDepth = new System.Windows.Forms.TrackBar();
            this.cbxPlotPeakPixels = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblDepthValue = new System.Windows.Forms.Label();
            this.cbxMaskContamination = new System.Windows.Forms.CheckBox();
            this.gbxInterlacedSettings.SuspendLayout();
            this.contextMenuFilter.SuspendLayout();
            this.gbxHotPixels.SuspendLayout();
            this.pnlHotPixelControls.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbDepth)).BeginInit();
            this.SuspendLayout();
            // 
            // gbxInterlacedSettings
            // 
            this.gbxInterlacedSettings.Controls.Add(this.rbReInterlaceShiftAndSwap);
            this.gbxInterlacedSettings.Controls.Add(this.rbReInterlaceShiftForward);
            this.gbxInterlacedSettings.Controls.Add(this.rbReInterlaceNon);
            this.gbxInterlacedSettings.Controls.Add(this.rbReInterlaceSwapFields);
            this.gbxInterlacedSettings.Location = new System.Drawing.Point(3, 3);
            this.gbxInterlacedSettings.Name = "gbxInterlacedSettings";
            this.gbxInterlacedSettings.Size = new System.Drawing.Size(249, 121);
            this.gbxInterlacedSettings.TabIndex = 48;
            this.gbxInterlacedSettings.TabStop = false;
            this.gbxInterlacedSettings.Text = "Interlaced Video Frame Grabbing Corrections";
            // 
            // rbReInterlaceShiftAndSwap
            // 
            this.rbReInterlaceShiftAndSwap.AutoSize = true;
            this.rbReInterlaceShiftAndSwap.Location = new System.Drawing.Point(24, 93);
            this.rbReInterlaceShiftAndSwap.Name = "rbReInterlaceShiftAndSwap";
            this.rbReInterlaceShiftAndSwap.Size = new System.Drawing.Size(186, 17);
            this.rbReInterlaceShiftAndSwap.TabIndex = 50;
            this.rbReInterlaceShiftAndSwap.Text = "Swap and Shift One Field Forward";
            this.rbReInterlaceShiftAndSwap.UseVisualStyleBackColor = true;
            this.rbReInterlaceShiftAndSwap.CheckedChanged += new System.EventHandler(this.ReInterlacedModeChanged);
            // 
            // rbReInterlaceShiftForward
            // 
            this.rbReInterlaceShiftForward.AutoSize = true;
            this.rbReInterlaceShiftForward.Location = new System.Drawing.Point(24, 70);
            this.rbReInterlaceShiftForward.Name = "rbReInterlaceShiftForward";
            this.rbReInterlaceShiftForward.Size = new System.Drawing.Size(135, 17);
            this.rbReInterlaceShiftForward.TabIndex = 49;
            this.rbReInterlaceShiftForward.Text = "Shift One Field Forward";
            this.rbReInterlaceShiftForward.UseVisualStyleBackColor = true;
            this.rbReInterlaceShiftForward.CheckedChanged += new System.EventHandler(this.ReInterlacedModeChanged);
            // 
            // rbReInterlaceNon
            // 
            this.rbReInterlaceNon.AutoSize = true;
            this.rbReInterlaceNon.Checked = true;
            this.rbReInterlaceNon.Location = new System.Drawing.Point(24, 24);
            this.rbReInterlaceNon.Name = "rbReInterlaceNon";
            this.rbReInterlaceNon.Size = new System.Drawing.Size(95, 17);
            this.rbReInterlaceNon.TabIndex = 47;
            this.rbReInterlaceNon.TabStop = true;
            this.rbReInterlaceNon.Text = "No Corrections";
            this.rbReInterlaceNon.UseVisualStyleBackColor = true;
            this.rbReInterlaceNon.CheckedChanged += new System.EventHandler(this.ReInterlacedModeChanged);
            // 
            // rbReInterlaceSwapFields
            // 
            this.rbReInterlaceSwapFields.AutoSize = true;
            this.rbReInterlaceSwapFields.Location = new System.Drawing.Point(24, 47);
            this.rbReInterlaceSwapFields.Name = "rbReInterlaceSwapFields";
            this.rbReInterlaceSwapFields.Size = new System.Drawing.Size(154, 17);
            this.rbReInterlaceSwapFields.TabIndex = 48;
            this.rbReInterlaceSwapFields.Text = "Swap Even and Odd Fields";
            this.rbReInterlaceSwapFields.UseVisualStyleBackColor = true;
            this.rbReInterlaceSwapFields.CheckedChanged += new System.EventHandler(this.ReInterlacedModeChanged);
            // 
            // btnPreProcessingFilter
            // 
            this.btnPreProcessingFilter.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPreProcessingFilter.Location = new System.Drawing.Point(258, 9);
            this.btnPreProcessingFilter.Name = "btnPreProcessingFilter";
            this.btnPreProcessingFilter.Size = new System.Drawing.Size(132, 21);
            this.btnPreProcessingFilter.TabIndex = 77;
            this.btnPreProcessingFilter.Text = "Low Pass Diff Filter";
            this.btnPreProcessingFilter.UseVisualStyleBackColor = true;
            this.btnPreProcessingFilter.Click += new System.EventHandler(this.btnPreProcessingFilter_Click);
            // 
            // contextMenuFilter
            // 
            this.contextMenuFilter.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miNoFilter,
            this.miLowPass});
            this.contextMenuFilter.Name = "contextMenuFilter";
            this.contextMenuFilter.Size = new System.Drawing.Size(154, 48);
            this.contextMenuFilter.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuFilter_Opening);
            // 
            // miNoFilter
            // 
            this.miNoFilter.Checked = true;
            this.miNoFilter.CheckOnClick = true;
            this.miNoFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.miNoFilter.Name = "miNoFilter";
            this.miNoFilter.Size = new System.Drawing.Size(153, 22);
            this.miNoFilter.Text = "&No Filter";
            this.miNoFilter.CheckedChanged += new System.EventHandler(this.miFilter_CheckedChanged);
            // 
            // miLowPass
            // 
            this.miLowPass.CheckOnClick = true;
            this.miLowPass.Name = "miLowPass";
            this.miLowPass.Size = new System.Drawing.Size(153, 22);
            this.miLowPass.Text = "&Low-Pass Filter";
            this.miLowPass.CheckedChanged += new System.EventHandler(this.miFilter_CheckedChanged);
            // 
            // cbxUseHotPixelsCorrection
            // 
            this.cbxUseHotPixelsCorrection.AutoSize = true;
            this.cbxUseHotPixelsCorrection.Location = new System.Drawing.Point(17, 135);
            this.cbxUseHotPixelsCorrection.Name = "cbxUseHotPixelsCorrection";
            this.cbxUseHotPixelsCorrection.Size = new System.Drawing.Size(124, 17);
            this.cbxUseHotPixelsCorrection.TabIndex = 79;
            this.cbxUseHotPixelsCorrection.Text = "Hot Pixels Correction";
            this.cbxUseHotPixelsCorrection.UseVisualStyleBackColor = true;
            this.cbxUseHotPixelsCorrection.CheckedChanged += new System.EventHandler(this.cbxUseHotPixelsCorrection_CheckedChanged);
            // 
            // gbxHotPixels
            // 
            this.gbxHotPixels.Controls.Add(this.pnlHotPixelControls);
            this.gbxHotPixels.Location = new System.Drawing.Point(3, 135);
            this.gbxHotPixels.Name = "gbxHotPixels";
            this.gbxHotPixels.Size = new System.Drawing.Size(387, 129);
            this.gbxHotPixels.TabIndex = 80;
            this.gbxHotPixels.TabStop = false;
            this.gbxHotPixels.Text = "                                             ";
            // 
            // pnlHotPixelControls
            // 
            this.pnlHotPixelControls.Controls.Add(this.groupBox1);
            this.pnlHotPixelControls.Controls.Add(this.tbDepth);
            this.pnlHotPixelControls.Controls.Add(this.cbxPlotPeakPixels);
            this.pnlHotPixelControls.Controls.Add(this.label1);
            this.pnlHotPixelControls.Controls.Add(this.lblDepthValue);
            this.pnlHotPixelControls.Enabled = false;
            this.pnlHotPixelControls.Location = new System.Drawing.Point(5, 19);
            this.pnlHotPixelControls.Name = "pnlHotPixelControls";
            this.pnlHotPixelControls.Size = new System.Drawing.Size(377, 109);
            this.pnlHotPixelControls.TabIndex = 81;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbHotPixelsShowPositions);
            this.groupBox1.Controls.Add(this.rbHotPixelsPreviewRemove);
            this.groupBox1.Location = new System.Drawing.Point(5, 62);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(239, 42);
            this.groupBox1.TabIndex = 86;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Hot Pixels Display";
            // 
            // rbHotPixelsShowPositions
            // 
            this.rbHotPixelsShowPositions.AutoSize = true;
            this.rbHotPixelsShowPositions.Checked = true;
            this.rbHotPixelsShowPositions.Location = new System.Drawing.Point(9, 18);
            this.rbHotPixelsShowPositions.Name = "rbHotPixelsShowPositions";
            this.rbHotPixelsShowPositions.Size = new System.Drawing.Size(97, 17);
            this.rbHotPixelsShowPositions.TabIndex = 84;
            this.rbHotPixelsShowPositions.TabStop = true;
            this.rbHotPixelsShowPositions.Text = "Show Positions";
            this.rbHotPixelsShowPositions.UseVisualStyleBackColor = true;
            this.rbHotPixelsShowPositions.CheckedChanged += new System.EventHandler(this.rbHotPixelsShowPositions_CheckedChanged);
            // 
            // rbHotPixelsPreviewRemove
            // 
            this.rbHotPixelsPreviewRemove.AutoSize = true;
            this.rbHotPixelsPreviewRemove.Location = new System.Drawing.Point(135, 17);
            this.rbHotPixelsPreviewRemove.Name = "rbHotPixelsPreviewRemove";
            this.rbHotPixelsPreviewRemove.Size = new System.Drawing.Size(65, 17);
            this.rbHotPixelsPreviewRemove.TabIndex = 85;
            this.rbHotPixelsPreviewRemove.Text = "Remove";
            this.rbHotPixelsPreviewRemove.UseVisualStyleBackColor = true;
            this.rbHotPixelsPreviewRemove.CheckedChanged += new System.EventHandler(this.rbHotPixelsPreviewRemove_CheckedChanged);
            // 
            // tbDepth
            // 
            this.tbDepth.Location = new System.Drawing.Point(44, 16);
            this.tbDepth.Maximum = 200;
            this.tbDepth.Name = "tbDepth";
            this.tbDepth.Size = new System.Drawing.Size(325, 45);
            this.tbDepth.TabIndex = 80;
            this.tbDepth.TickFrequency = 10;
            this.tbDepth.Scroll += new System.EventHandler(this.tbDepth_Scroll);
            // 
            // cbxPlotPeakPixels
            // 
            this.cbxPlotPeakPixels.AutoSize = true;
            this.cbxPlotPeakPixels.Location = new System.Drawing.Point(267, 79);
            this.cbxPlotPeakPixels.Name = "cbxPlotPeakPixels";
            this.cbxPlotPeakPixels.Size = new System.Drawing.Size(102, 17);
            this.cbxPlotPeakPixels.TabIndex = 83;
            this.cbxPlotPeakPixels.Text = "Plot Peak Pixels";
            this.cbxPlotPeakPixels.UseVisualStyleBackColor = true;
            this.cbxPlotPeakPixels.CheckedChanged += new System.EventHandler(this.cbxPlotPeakPixels_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 81;
            this.label1.Text = "Depth";
            // 
            // lblDepthValue
            // 
            this.lblDepthValue.AutoSize = true;
            this.lblDepthValue.Location = new System.Drawing.Point(6, 32);
            this.lblDepthValue.Name = "lblDepthValue";
            this.lblDepthValue.Size = new System.Drawing.Size(19, 13);
            this.lblDepthValue.TabIndex = 82;
            this.lblDepthValue.Text = "20";
            // 
            // cbxMaskContamination
            // 
            this.cbxMaskContamination.AutoSize = true;
            this.cbxMaskContamination.Location = new System.Drawing.Point(17, 278);
            this.cbxMaskContamination.Name = "cbxMaskContamination";
            this.cbxMaskContamination.Size = new System.Drawing.Size(149, 17);
            this.cbxMaskContamination.TabIndex = 81;
            this.cbxMaskContamination.Text = "Ignore Contaminated Area";
            this.cbxMaskContamination.UseVisualStyleBackColor = true;
            this.cbxMaskContamination.CheckedChanged += new System.EventHandler(this.cbxMaskContamination_CheckedChanged);
            // 
            // ucImageDefectSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbxMaskContamination);
            this.Controls.Add(this.cbxUseHotPixelsCorrection);
            this.Controls.Add(this.gbxHotPixels);
            this.Controls.Add(this.btnPreProcessingFilter);
            this.Controls.Add(this.gbxInterlacedSettings);
            this.Name = "ucImageDefectSettings";
            this.Size = new System.Drawing.Size(410, 329);
            this.Load += new System.EventHandler(this.ucImageDefectSettings_Load);
            this.gbxInterlacedSettings.ResumeLayout(false);
            this.gbxInterlacedSettings.PerformLayout();
            this.contextMenuFilter.ResumeLayout(false);
            this.gbxHotPixels.ResumeLayout(false);
            this.pnlHotPixelControls.ResumeLayout(false);
            this.pnlHotPixelControls.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbDepth)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxInterlacedSettings;
        private System.Windows.Forms.RadioButton rbReInterlaceShiftAndSwap;
        private System.Windows.Forms.RadioButton rbReInterlaceShiftForward;
        private System.Windows.Forms.RadioButton rbReInterlaceNon;
        private System.Windows.Forms.RadioButton rbReInterlaceSwapFields;
        private System.Windows.Forms.Button btnPreProcessingFilter;
        private System.Windows.Forms.ContextMenuStrip contextMenuFilter;
        private System.Windows.Forms.ToolStripMenuItem miNoFilter;
        private System.Windows.Forms.CheckBox cbxUseHotPixelsCorrection;
        protected internal System.Windows.Forms.ToolStripMenuItem miLowPass;
        private System.Windows.Forms.GroupBox gbxHotPixels;
        private System.Windows.Forms.TrackBar tbDepth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblDepthValue;
        private System.Windows.Forms.RadioButton rbHotPixelsPreviewRemove;
        private System.Windows.Forms.RadioButton rbHotPixelsShowPositions;
        private System.Windows.Forms.CheckBox cbxPlotPeakPixels;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel pnlHotPixelControls;
        private System.Windows.Forms.CheckBox cbxMaskContamination;
    }
}

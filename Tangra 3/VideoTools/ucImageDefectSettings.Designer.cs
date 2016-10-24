namespace Tangra.VideoTools
{
    partial class ucImageDefectSettings
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            base.Dispose(disposing);
        }

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
            this.gbxInterlacedSettings.SuspendLayout();
            this.contextMenuFilter.SuspendLayout();
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
            this.gbxInterlacedSettings.Size = new System.Drawing.Size(371, 135);
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
            this.btnPreProcessingFilter.Location = new System.Drawing.Point(3, 195);
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
            // 
            // cbxUseHotPixelsCorrection
            // 
            this.cbxUseHotPixelsCorrection.AutoSize = true;
            this.cbxUseHotPixelsCorrection.Location = new System.Drawing.Point(3, 158);
            this.cbxUseHotPixelsCorrection.Name = "cbxUseHotPixelsCorrection";
            this.cbxUseHotPixelsCorrection.Size = new System.Drawing.Size(124, 17);
            this.cbxUseHotPixelsCorrection.TabIndex = 79;
            this.cbxUseHotPixelsCorrection.Text = "Hot Pixels Correction";
            this.cbxUseHotPixelsCorrection.UseVisualStyleBackColor = true;
            this.cbxUseHotPixelsCorrection.CheckedChanged += new System.EventHandler(this.cbxUseHotPixelsCorrection_CheckedChanged);
            // 
            // ucImageDefectSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbxUseHotPixelsCorrection);
            this.Controls.Add(this.btnPreProcessingFilter);
            this.Controls.Add(this.gbxInterlacedSettings);
            this.Name = "ucImageDefectSettings";
            this.Size = new System.Drawing.Size(400, 261);
            this.gbxInterlacedSettings.ResumeLayout(false);
            this.gbxInterlacedSettings.PerformLayout();
            this.contextMenuFilter.ResumeLayout(false);
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
    }
}

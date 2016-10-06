/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

namespace Tangra.VideoOperations.ConvertVideoToFits
{
    partial class ucConvertVideoToFits
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
            this.rbCube = new System.Windows.Forms.RadioButton();
            this.rbSequence = new System.Windows.Forms.RadioButton();
            this.nudFirstFrame = new System.Windows.Forms.NumericUpDown();
            this.label26 = new System.Windows.Forms.Label();
            this.nudLastFrame = new System.Windows.Forms.NumericUpDown();
            this.label27 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pbar = new System.Windows.Forms.ProgressBar();
            this.btnExport = new System.Windows.Forms.Button();
            this.gbxFormat = new System.Windows.Forms.GroupBox();
            this.gbxFrameSize = new System.Windows.Forms.GroupBox();
            this.rbFullFrame = new System.Windows.Forms.RadioButton();
            this.rbROI = new System.Windows.Forms.RadioButton();
            this.gbxSection = new System.Windows.Forms.GroupBox();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.nudFirstFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLastFrame)).BeginInit();
            this.gbxFormat.SuspendLayout();
            this.gbxFrameSize.SuspendLayout();
            this.gbxSection.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbCube
            // 
            this.rbCube.AutoSize = true;
            this.rbCube.Enabled = false;
            this.rbCube.Location = new System.Drawing.Point(23, 47);
            this.rbCube.Name = "rbCube";
            this.rbCube.Size = new System.Drawing.Size(76, 17);
            this.rbCube.TabIndex = 12;
            this.rbCube.Text = "FITS Cube";
            this.rbCube.UseVisualStyleBackColor = true;
            // 
            // rbSequence
            // 
            this.rbSequence.AutoSize = true;
            this.rbSequence.Checked = true;
            this.rbSequence.Location = new System.Drawing.Point(23, 24);
            this.rbSequence.Name = "rbSequence";
            this.rbSequence.Size = new System.Drawing.Size(119, 17);
            this.rbSequence.TabIndex = 11;
            this.rbSequence.TabStop = true;
            this.rbSequence.Text = "FITS File Sequence";
            this.rbSequence.UseVisualStyleBackColor = true;
            // 
            // nudFirstFrame
            // 
            this.nudFirstFrame.Location = new System.Drawing.Point(89, 21);
            this.nudFirstFrame.Name = "nudFirstFrame";
            this.nudFirstFrame.Size = new System.Drawing.Size(58, 20);
            this.nudFirstFrame.TabIndex = 22;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(16, 50);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(59, 13);
            this.label26.TabIndex = 25;
            this.label26.Text = "Last Frame";
            // 
            // nudLastFrame
            // 
            this.nudLastFrame.Location = new System.Drawing.Point(89, 48);
            this.nudLastFrame.Name = "nudLastFrame";
            this.nudLastFrame.Size = new System.Drawing.Size(58, 20);
            this.nudLastFrame.TabIndex = 23;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(16, 23);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(58, 13);
            this.label27.TabIndex = 24;
            this.label27.Text = "First Frame";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(126, 258);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(101, 23);
            this.btnCancel.TabIndex = 27;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // pbar
            // 
            this.pbar.Location = new System.Drawing.Point(13, 292);
            this.pbar.Name = "pbar";
            this.pbar.Size = new System.Drawing.Size(214, 15);
            this.pbar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbar.TabIndex = 28;
            this.pbar.Visible = false;
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(13, 258);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(101, 23);
            this.btnExport.TabIndex = 26;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // gbxFormat
            // 
            this.gbxFormat.Controls.Add(this.rbSequence);
            this.gbxFormat.Controls.Add(this.rbCube);
            this.gbxFormat.Location = new System.Drawing.Point(9, 0);
            this.gbxFormat.Name = "gbxFormat";
            this.gbxFormat.Size = new System.Drawing.Size(218, 78);
            this.gbxFormat.TabIndex = 29;
            this.gbxFormat.TabStop = false;
            this.gbxFormat.Text = "Export Format";
            // 
            // gbxFrameSize
            // 
            this.gbxFrameSize.Controls.Add(this.rbFullFrame);
            this.gbxFrameSize.Controls.Add(this.rbROI);
            this.gbxFrameSize.Location = new System.Drawing.Point(11, 84);
            this.gbxFrameSize.Name = "gbxFrameSize";
            this.gbxFrameSize.Size = new System.Drawing.Size(216, 78);
            this.gbxFrameSize.TabIndex = 30;
            this.gbxFrameSize.TabStop = false;
            this.gbxFrameSize.Text = "Frame Size";
            // 
            // rbFullFrame
            // 
            this.rbFullFrame.AutoSize = true;
            this.rbFullFrame.Checked = true;
            this.rbFullFrame.Location = new System.Drawing.Point(21, 24);
            this.rbFullFrame.Name = "rbFullFrame";
            this.rbFullFrame.Size = new System.Drawing.Size(73, 17);
            this.rbFullFrame.TabIndex = 11;
            this.rbFullFrame.TabStop = true;
            this.rbFullFrame.Text = "Full Frame";
            this.rbFullFrame.UseVisualStyleBackColor = true;
            // 
            // rbROI
            // 
            this.rbROI.AutoSize = true;
            this.rbROI.Location = new System.Drawing.Point(21, 47);
            this.rbROI.Name = "rbROI";
            this.rbROI.Size = new System.Drawing.Size(109, 17);
            this.rbROI.TabIndex = 12;
            this.rbROI.Text = "Region of Interest";
            this.rbROI.UseVisualStyleBackColor = true;
            this.rbROI.CheckedChanged += new System.EventHandler(this.rbROI_CheckedChanged);
            // 
            // gbxSection
            // 
            this.gbxSection.Controls.Add(this.nudFirstFrame);
            this.gbxSection.Controls.Add(this.label27);
            this.gbxSection.Controls.Add(this.nudLastFrame);
            this.gbxSection.Controls.Add(this.label26);
            this.gbxSection.Location = new System.Drawing.Point(13, 168);
            this.gbxSection.Name = "gbxSection";
            this.gbxSection.Size = new System.Drawing.Size(214, 78);
            this.gbxSection.TabIndex = 31;
            this.gbxSection.TabStop = false;
            this.gbxSection.Text = "Export Section";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "fits";
            // 
            // ucConvertVideoToFits
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbxSection);
            this.Controls.Add(this.gbxFrameSize);
            this.Controls.Add(this.gbxFormat);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.pbar);
            this.Controls.Add(this.btnExport);
            this.Name = "ucConvertVideoToFits";
            this.Size = new System.Drawing.Size(259, 336);
            ((System.ComponentModel.ISupportInitialize)(this.nudFirstFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLastFrame)).EndInit();
            this.gbxFormat.ResumeLayout(false);
            this.gbxFormat.PerformLayout();
            this.gbxFrameSize.ResumeLayout(false);
            this.gbxFrameSize.PerformLayout();
            this.gbxSection.ResumeLayout(false);
            this.gbxSection.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rbCube;
        private System.Windows.Forms.RadioButton rbSequence;
        private System.Windows.Forms.NumericUpDown nudFirstFrame;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.NumericUpDown nudLastFrame;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ProgressBar pbar;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.GroupBox gbxFormat;
        private System.Windows.Forms.GroupBox gbxFrameSize;
        private System.Windows.Forms.RadioButton rbFullFrame;
        private System.Windows.Forms.RadioButton rbROI;
        private System.Windows.Forms.GroupBox gbxSection;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    }
}

namespace Tangra.VideoTools
{
    partial class frmExportVideoToFITS
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmExportVideoToFITS));
            this.gbxExport = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.rbCube = new System.Windows.Forms.RadioButton();
            this.rbSequence = new System.Windows.Forms.RadioButton();
            this.pbar1 = new System.Windows.Forms.ProgressBar();
            this.pnlCropChooseFrames = new System.Windows.Forms.Panel();
            this.pnlCalibrControls = new System.Windows.Forms.Panel();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label37 = new System.Windows.Forms.Label();
            this.rbCalibStrToCSV = new System.Windows.Forms.RadioButton();
            this.rbMainStrToCSV = new System.Windows.Forms.RadioButton();
            this.nudFirstFrame = new System.Windows.Forms.NumericUpDown();
            this.label26 = new System.Windows.Forms.Label();
            this.nudLastFrame = new System.Windows.Forms.NumericUpDown();
            this.label27 = new System.Windows.Forms.Label();
            this.gbxExport.SuspendLayout();
            this.pnlCropChooseFrames.SuspendLayout();
            this.pnlCalibrControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFirstFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLastFrame)).BeginInit();
            this.SuspendLayout();
            // 
            // gbxExport
            // 
            this.gbxExport.Controls.Add(this.btnBrowse);
            this.gbxExport.Controls.Add(this.textBox1);
            this.gbxExport.Controls.Add(this.rbCube);
            this.gbxExport.Controls.Add(this.rbSequence);
            this.gbxExport.Controls.Add(this.pbar1);
            this.gbxExport.Controls.Add(this.pnlCropChooseFrames);
            this.gbxExport.Location = new System.Drawing.Point(12, 12);
            this.gbxExport.Name = "gbxExport";
            this.gbxExport.Size = new System.Drawing.Size(450, 203);
            this.gbxExport.TabIndex = 1;
            this.gbxExport.TabStop = false;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(392, 123);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(42, 23);
            this.btnBrowse.TabIndex = 7;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(16, 126);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(370, 20);
            this.textBox1.TabIndex = 11;
            // 
            // rbCube
            // 
            this.rbCube.AutoSize = true;
            this.rbCube.Location = new System.Drawing.Point(141, 19);
            this.rbCube.Name = "rbCube";
            this.rbCube.Size = new System.Drawing.Size(76, 17);
            this.rbCube.TabIndex = 10;
            this.rbCube.Text = "FITS Cube";
            this.rbCube.UseVisualStyleBackColor = true;
            // 
            // rbSequence
            // 
            this.rbSequence.AutoSize = true;
            this.rbSequence.Checked = true;
            this.rbSequence.Location = new System.Drawing.Point(16, 20);
            this.rbSequence.Name = "rbSequence";
            this.rbSequence.Size = new System.Drawing.Size(119, 17);
            this.rbSequence.TabIndex = 9;
            this.rbSequence.TabStop = true;
            this.rbSequence.Text = "FITS File Sequence";
            this.rbSequence.UseVisualStyleBackColor = true;
            // 
            // pbar1
            // 
            this.pbar1.Location = new System.Drawing.Point(16, 165);
            this.pbar1.Name = "pbar1";
            this.pbar1.Size = new System.Drawing.Size(418, 23);
            this.pbar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbar1.TabIndex = 8;
            this.pbar1.Visible = false;
            // 
            // pnlCropChooseFrames
            // 
            this.pnlCropChooseFrames.Controls.Add(this.nudFirstFrame);
            this.pnlCropChooseFrames.Controls.Add(this.label26);
            this.pnlCropChooseFrames.Controls.Add(this.nudLastFrame);
            this.pnlCropChooseFrames.Controls.Add(this.pnlCalibrControls);
            this.pnlCropChooseFrames.Controls.Add(this.label27);
            this.pnlCropChooseFrames.Location = new System.Drawing.Point(6, 40);
            this.pnlCropChooseFrames.Name = "pnlCropChooseFrames";
            this.pnlCropChooseFrames.Size = new System.Drawing.Size(428, 77);
            this.pnlCropChooseFrames.TabIndex = 7;
            // 
            // pnlCalibrControls
            // 
            this.pnlCalibrControls.Controls.Add(this.label37);
            this.pnlCalibrControls.Controls.Add(this.rbMainStrToCSV);
            this.pnlCalibrControls.Controls.Add(this.rbCalibStrToCSV);
            this.pnlCalibrControls.Location = new System.Drawing.Point(83, 7);
            this.pnlCalibrControls.Name = "pnlCalibrControls";
            this.pnlCalibrControls.Size = new System.Drawing.Size(225, 25);
            this.pnlCalibrControls.TabIndex = 13;
            this.pnlCalibrControls.Visible = false;
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(134, 221);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(93, 23);
            this.btnExport.TabIndex = 6;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(233, 221);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(93, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(5, 6);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(40, 13);
            this.label37.TabIndex = 24;
            this.label37.Text = "Stream";
            // 
            // rbCalibStrToCSV
            // 
            this.rbCalibStrToCSV.AutoSize = true;
            this.rbCalibStrToCSV.Location = new System.Drawing.Point(112, 4);
            this.rbCalibStrToCSV.Name = "rbCalibStrToCSV";
            this.rbCalibStrToCSV.Size = new System.Drawing.Size(96, 17);
            this.rbCalibStrToCSV.TabIndex = 23;
            this.rbCalibStrToCSV.Text = "CALIBRATION";
            this.rbCalibStrToCSV.UseVisualStyleBackColor = true;
            // 
            // rbMainStrToCSV
            // 
            this.rbMainStrToCSV.AutoSize = true;
            this.rbMainStrToCSV.Checked = true;
            this.rbMainStrToCSV.Location = new System.Drawing.Point(54, 4);
            this.rbMainStrToCSV.Name = "rbMainStrToCSV";
            this.rbMainStrToCSV.Size = new System.Drawing.Size(52, 17);
            this.rbMainStrToCSV.TabIndex = 22;
            this.rbMainStrToCSV.TabStop = true;
            this.rbMainStrToCSV.Text = "MAIN";
            this.rbMainStrToCSV.UseVisualStyleBackColor = true;
            // 
            // nudFirstFrame
            // 
            this.nudFirstFrame.Location = new System.Drawing.Point(139, 38);
            this.nudFirstFrame.Name = "nudFirstFrame";
            this.nudFirstFrame.Size = new System.Drawing.Size(58, 20);
            this.nudFirstFrame.TabIndex = 18;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(234, 40);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(59, 13);
            this.label26.TabIndex = 21;
            this.label26.Text = "Last Frame";
            // 
            // nudLastFrame
            // 
            this.nudLastFrame.Location = new System.Drawing.Point(299, 38);
            this.nudLastFrame.Name = "nudLastFrame";
            this.nudLastFrame.Size = new System.Drawing.Size(58, 20);
            this.nudLastFrame.TabIndex = 19;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(72, 40);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(58, 13);
            this.label27.TabIndex = 20;
            this.label27.Text = "First Frame";
            // 
            // frmExportVideoToFITS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 305);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.gbxExport);
            this.Controls.Add(this.btnExport);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmExportVideoToFITS";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export Video";
            this.gbxExport.ResumeLayout(false);
            this.gbxExport.PerformLayout();
            this.pnlCropChooseFrames.ResumeLayout(false);
            this.pnlCropChooseFrames.PerformLayout();
            this.pnlCalibrControls.ResumeLayout(false);
            this.pnlCalibrControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFirstFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLastFrame)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxExport;
        private System.Windows.Forms.ProgressBar pbar1;
        private System.Windows.Forms.Panel pnlCropChooseFrames;
        private System.Windows.Forms.Panel pnlCalibrControls;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.RadioButton rbCube;
        private System.Windows.Forms.RadioButton rbSequence;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.NumericUpDown nudFirstFrame;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.NumericUpDown nudLastFrame;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.RadioButton rbMainStrToCSV;
        private System.Windows.Forms.RadioButton rbCalibStrToCSV;
        private System.Windows.Forms.Label label27;
    }
}
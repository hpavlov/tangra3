namespace Tangra.VideoOperations.ConvertVideoToAav
{
    partial class ucConvertVideoToAav
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
            this.gbxSection = new System.Windows.Forms.GroupBox();
            this.nudFirstFrame = new System.Windows.Forms.NumericUpDown();
            this.label27 = new System.Windows.Forms.Label();
            this.nudLastFrame = new System.Windows.Forms.NumericUpDown();
            this.label26 = new System.Windows.Forms.Label();
            this.btnDetectIntegrationRate = new System.Windows.Forms.Button();
            this.btnConvert = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbxVTIPosition = new System.Windows.Forms.GroupBox();
            this.btnConfirmPosition = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.gbxIntegrationRate = new System.Windows.Forms.GroupBox();
            this.lblFrames = new System.Windows.Forms.Label();
            this.nudIntegratedFrames = new System.Windows.Forms.NumericUpDown();
            this.lblIntegration = new System.Windows.Forms.Label();
            this.nudStartingAtFrame = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.pbar = new System.Windows.Forms.ProgressBar();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.cbxCameraModel = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.gbxCameraInfo = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbxSensorInfo = new System.Windows.Forms.ComboBox();
            this.rbFirstFieldBottom = new System.Windows.Forms.RadioButton();
            this.rbFirstFieldTop = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.pnlFirstField = new System.Windows.Forms.Panel();
            this.btnUseIntegrationRate = new System.Windows.Forms.Button();
            this.btnUseNoIntegration = new System.Windows.Forms.Button();
            this.gbxSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFirstFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLastFrame)).BeginInit();
            this.gbxVTIPosition.SuspendLayout();
            this.gbxIntegrationRate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudIntegratedFrames)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStartingAtFrame)).BeginInit();
            this.gbxCameraInfo.SuspendLayout();
            this.pnlFirstField.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxSection
            // 
            this.gbxSection.Controls.Add(this.nudFirstFrame);
            this.gbxSection.Controls.Add(this.label27);
            this.gbxSection.Controls.Add(this.nudLastFrame);
            this.gbxSection.Controls.Add(this.label26);
            this.gbxSection.Location = new System.Drawing.Point(15, 102);
            this.gbxSection.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbxSection.Name = "gbxSection";
            this.gbxSection.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbxSection.Size = new System.Drawing.Size(283, 65);
            this.gbxSection.TabIndex = 32;
            this.gbxSection.TabStop = false;
            this.gbxSection.Text = "Video Frames to Convert";
            // 
            // nudFirstFrame
            // 
            this.nudFirstFrame.Location = new System.Drawing.Point(64, 28);
            this.nudFirstFrame.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.nudFirstFrame.Name = "nudFirstFrame";
            this.nudFirstFrame.ReadOnly = true;
            this.nudFirstFrame.Size = new System.Drawing.Size(73, 22);
            this.nudFirstFrame.TabIndex = 22;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(17, 31);
            this.label27.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(39, 17);
            this.label27.TabIndex = 24;
            this.label27.Text = "First:";
            // 
            // nudLastFrame
            // 
            this.nudLastFrame.Location = new System.Drawing.Point(191, 28);
            this.nudLastFrame.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.nudLastFrame.Name = "nudLastFrame";
            this.nudLastFrame.Size = new System.Drawing.Size(73, 22);
            this.nudLastFrame.TabIndex = 23;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(152, 31);
            this.label26.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(39, 17);
            this.label26.TabIndex = 25;
            this.label26.Text = "Last:";
            // 
            // btnDetectIntegrationRate
            // 
            this.btnDetectIntegrationRate.Location = new System.Drawing.Point(15, 185);
            this.btnDetectIntegrationRate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnDetectIntegrationRate.Name = "btnDetectIntegrationRate";
            this.btnDetectIntegrationRate.Size = new System.Drawing.Size(207, 28);
            this.btnDetectIntegrationRate.TabIndex = 44;
            this.btnDetectIntegrationRate.Text = "Detect Integration Rate";
            this.btnDetectIntegrationRate.Visible = false;
            this.btnDetectIntegrationRate.Click += new System.EventHandler(this.btnDetectIntegrationRate_Click);
            // 
            // btnConvert
            // 
            this.btnConvert.Enabled = false;
            this.btnConvert.Location = new System.Drawing.Point(15, 430);
            this.btnConvert.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(123, 28);
            this.btnConvert.TabIndex = 46;
            this.btnConvert.Text = "Convert";
            this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(171, 430);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(123, 28);
            this.btnCancel.TabIndex = 47;
            this.btnCancel.Text = "Stop";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gbxVTIPosition
            // 
            this.gbxVTIPosition.Controls.Add(this.btnConfirmPosition);
            this.gbxVTIPosition.Controls.Add(this.label1);
            this.gbxVTIPosition.Location = new System.Drawing.Point(15, 0);
            this.gbxVTIPosition.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbxVTIPosition.Name = "gbxVTIPosition";
            this.gbxVTIPosition.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbxVTIPosition.Size = new System.Drawing.Size(283, 98);
            this.gbxVTIPosition.TabIndex = 48;
            this.gbxVTIPosition.TabStop = false;
            this.gbxVTIPosition.Text = "VTI-OSD Position";
            // 
            // btnConfirmPosition
            // 
            this.btnConfirmPosition.Location = new System.Drawing.Point(21, 60);
            this.btnConfirmPosition.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnConfirmPosition.Name = "btnConfirmPosition";
            this.btnConfirmPosition.Size = new System.Drawing.Size(175, 28);
            this.btnConfirmPosition.TabIndex = 48;
            this.btnConfirmPosition.Text = "Confirm Position";
            this.btnConfirmPosition.Click += new System.EventHandler(this.btnConfirmPosition_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 28);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(251, 17);
            this.label1.TabIndex = 24;
            this.label1.Text = "Select the VTI-OSD position on the left";
            // 
            // gbxIntegrationRate
            // 
            this.gbxIntegrationRate.Controls.Add(this.lblFrames);
            this.gbxIntegrationRate.Controls.Add(this.nudIntegratedFrames);
            this.gbxIntegrationRate.Controls.Add(this.lblIntegration);
            this.gbxIntegrationRate.Controls.Add(this.nudStartingAtFrame);
            this.gbxIntegrationRate.Controls.Add(this.label4);
            this.gbxIntegrationRate.Location = new System.Drawing.Point(15, 175);
            this.gbxIntegrationRate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbxIntegrationRate.Name = "gbxIntegrationRate";
            this.gbxIntegrationRate.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbxIntegrationRate.Size = new System.Drawing.Size(283, 94);
            this.gbxIntegrationRate.TabIndex = 49;
            this.gbxIntegrationRate.TabStop = false;
            this.gbxIntegrationRate.Text = "Integration Rate";
            // 
            // lblFrames
            // 
            this.lblFrames.AutoSize = true;
            this.lblFrames.Location = new System.Drawing.Point(181, 27);
            this.lblFrames.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFrames.Name = "lblFrames";
            this.lblFrames.Size = new System.Drawing.Size(51, 17);
            this.lblFrames.TabIndex = 51;
            this.lblFrames.Text = "frames";
            // 
            // nudIntegratedFrames
            // 
            this.nudIntegratedFrames.Location = new System.Drawing.Point(99, 23);
            this.nudIntegratedFrames.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.nudIntegratedFrames.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.nudIntegratedFrames.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudIntegratedFrames.Name = "nudIntegratedFrames";
            this.nudIntegratedFrames.ReadOnly = true;
            this.nudIntegratedFrames.Size = new System.Drawing.Size(75, 22);
            this.nudIntegratedFrames.TabIndex = 50;
            this.nudIntegratedFrames.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblIntegration
            // 
            this.lblIntegration.AutoSize = true;
            this.lblIntegration.Location = new System.Drawing.Point(17, 27);
            this.lblIntegration.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblIntegration.Name = "lblIntegration";
            this.lblIntegration.Size = new System.Drawing.Size(79, 17);
            this.lblIntegration.TabIndex = 49;
            this.lblIntegration.Text = "Integration:";
            // 
            // nudStartingAtFrame
            // 
            this.nudStartingAtFrame.Location = new System.Drawing.Point(100, 57);
            this.nudStartingAtFrame.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.nudStartingAtFrame.Name = "nudStartingAtFrame";
            this.nudStartingAtFrame.ReadOnly = true;
            this.nudStartingAtFrame.Size = new System.Drawing.Size(73, 22);
            this.nudStartingAtFrame.TabIndex = 47;
            this.nudStartingAtFrame.ValueChanged += new System.EventHandler(this.nudStartingAtFrame_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 59);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 17);
            this.label4.TabIndex = 48;
            this.label4.Text = "Starting At:";
            // 
            // pbar
            // 
            this.pbar.Location = new System.Drawing.Point(15, 405);
            this.pbar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pbar.Name = "pbar";
            this.pbar.Size = new System.Drawing.Size(283, 17);
            this.pbar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbar.TabIndex = 50;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "aav";
            this.saveFileDialog.Filter = "Astro Analogue Video (*.aav)|*.aav";
            this.saveFileDialog.Title = "Select output aav file ...";
            // 
            // cbxCameraModel
            // 
            this.cbxCameraModel.FormattingEnabled = true;
            this.cbxCameraModel.Items.AddRange(new object[] {
            "WAT-120N",
            "WAT-910HX",
            "WAT-910BD",
            "Mintron 12V1C-EX",
            "G-Star",
            "Samsung SCB-2000",
            "PC165-DNR",
            "Unknown"});
            this.cbxCameraModel.Location = new System.Drawing.Point(80, 23);
            this.cbxCameraModel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbxCameraModel.Name = "cbxCameraModel";
            this.cbxCameraModel.Size = new System.Drawing.Size(183, 24);
            this.cbxCameraModel.TabIndex = 52;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 28);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 17);
            this.label3.TabIndex = 51;
            this.label3.Text = "Model:";
            // 
            // gbxCameraInfo
            // 
            this.gbxCameraInfo.Controls.Add(this.label5);
            this.gbxCameraInfo.Controls.Add(this.cbxSensorInfo);
            this.gbxCameraInfo.Controls.Add(this.label3);
            this.gbxCameraInfo.Controls.Add(this.cbxCameraModel);
            this.gbxCameraInfo.Location = new System.Drawing.Point(15, 306);
            this.gbxCameraInfo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbxCameraInfo.Name = "gbxCameraInfo";
            this.gbxCameraInfo.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbxCameraInfo.Size = new System.Drawing.Size(283, 91);
            this.gbxCameraInfo.TabIndex = 53;
            this.gbxCameraInfo.TabStop = false;
            this.gbxCameraInfo.Text = "Camera Info";
            this.gbxCameraInfo.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 62);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 17);
            this.label5.TabIndex = 53;
            this.label5.Text = "Sensor:";
            // 
            // cbxSensorInfo
            // 
            this.cbxSensorInfo.FormattingEnabled = true;
            this.cbxSensorInfo.Items.AddRange(new object[] {
            "Unknown"});
            this.cbxSensorInfo.Location = new System.Drawing.Point(80, 57);
            this.cbxSensorInfo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbxSensorInfo.Name = "cbxSensorInfo";
            this.cbxSensorInfo.Size = new System.Drawing.Size(183, 24);
            this.cbxSensorInfo.TabIndex = 54;
            // 
            // rbFirstFieldBottom
            // 
            this.rbFirstFieldBottom.AutoSize = true;
            this.rbFirstFieldBottom.Location = new System.Drawing.Point(177, 4);
            this.rbFirstFieldBottom.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbFirstFieldBottom.Name = "rbFirstFieldBottom";
            this.rbFirstFieldBottom.Size = new System.Drawing.Size(73, 21);
            this.rbFirstFieldBottom.TabIndex = 26;
            this.rbFirstFieldBottom.Text = "Bottom";
            this.rbFirstFieldBottom.UseVisualStyleBackColor = true;
            // 
            // rbFirstFieldTop
            // 
            this.rbFirstFieldTop.AutoSize = true;
            this.rbFirstFieldTop.Checked = true;
            this.rbFirstFieldTop.Location = new System.Drawing.Point(113, 5);
            this.rbFirstFieldTop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbFirstFieldTop.Name = "rbFirstFieldTop";
            this.rbFirstFieldTop.Size = new System.Drawing.Size(54, 21);
            this.rbFirstFieldTop.TabIndex = 25;
            this.rbFirstFieldTop.TabStop = true;
            this.rbFirstFieldTop.Text = "Top";
            this.rbFirstFieldTop.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 7);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(103, 17);
            this.label6.TabIndex = 24;
            this.label6.Text = "First field is on:";
            // 
            // pnlFirstField
            // 
            this.pnlFirstField.Controls.Add(this.rbFirstFieldBottom);
            this.pnlFirstField.Controls.Add(this.label6);
            this.pnlFirstField.Controls.Add(this.rbFirstFieldTop);
            this.pnlFirstField.Location = new System.Drawing.Point(12, 270);
            this.pnlFirstField.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlFirstField.Name = "pnlFirstField";
            this.pnlFirstField.Size = new System.Drawing.Size(285, 33);
            this.pnlFirstField.TabIndex = 54;
            this.pnlFirstField.Visible = false;
            // 
            // btnUseIntegrationRate
            // 
            this.btnUseIntegrationRate.Location = new System.Drawing.Point(15, 273);
            this.btnUseIntegrationRate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnUseIntegrationRate.Name = "btnUseIntegrationRate";
            this.btnUseIntegrationRate.Size = new System.Drawing.Size(207, 28);
            this.btnUseIntegrationRate.TabIndex = 55;
            this.btnUseIntegrationRate.Text = "Use this Integration Rate";
            this.btnUseIntegrationRate.Visible = false;
            this.btnUseIntegrationRate.Click += new System.EventHandler(this.btnUseIntegrationRate_Click);
            // 
            // btnUseNoIntegration
            // 
            this.btnUseNoIntegration.Location = new System.Drawing.Point(230, 185);
            this.btnUseNoIntegration.Name = "btnUseNoIntegration";
            this.btnUseNoIntegration.Size = new System.Drawing.Size(65, 28);
            this.btnUseNoIntegration.TabIndex = 56;
            this.btnUseNoIntegration.Text = "Use x1";
            this.btnUseNoIntegration.UseVisualStyleBackColor = true;
            this.btnUseNoIntegration.Click += new System.EventHandler(this.btnUseNoIntegration_Click);
            // 
            // ucConvertVideoToAav
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbxIntegrationRate);
            this.Controls.Add(this.btnUseIntegrationRate);
            this.Controls.Add(this.pnlFirstField);
            this.Controls.Add(this.gbxCameraInfo);
            this.Controls.Add(this.pbar);
            this.Controls.Add(this.gbxVTIPosition);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConvert);
            this.Controls.Add(this.gbxSection);
            this.Controls.Add(this.btnUseNoIntegration);
            this.Controls.Add(this.btnDetectIntegrationRate);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ucConvertVideoToAav";
            this.Size = new System.Drawing.Size(315, 480);
            this.gbxSection.ResumeLayout(false);
            this.gbxSection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFirstFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLastFrame)).EndInit();
            this.gbxVTIPosition.ResumeLayout(false);
            this.gbxVTIPosition.PerformLayout();
            this.gbxIntegrationRate.ResumeLayout(false);
            this.gbxIntegrationRate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudIntegratedFrames)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStartingAtFrame)).EndInit();
            this.gbxCameraInfo.ResumeLayout(false);
            this.gbxCameraInfo.PerformLayout();
            this.pnlFirstField.ResumeLayout(false);
            this.pnlFirstField.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxSection;
        private System.Windows.Forms.NumericUpDown nudFirstFrame;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.NumericUpDown nudLastFrame;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Button btnDetectIntegrationRate;
        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox gbxVTIPosition;
        private System.Windows.Forms.Button btnConfirmPosition;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gbxIntegrationRate;
        private System.Windows.Forms.NumericUpDown nudStartingAtFrame;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblFrames;
        private System.Windows.Forms.NumericUpDown nudIntegratedFrames;
        private System.Windows.Forms.Label lblIntegration;
        private System.Windows.Forms.ProgressBar pbar;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ComboBox cbxCameraModel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox gbxCameraInfo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbxSensorInfo;
        private System.Windows.Forms.RadioButton rbFirstFieldBottom;
        private System.Windows.Forms.RadioButton rbFirstFieldTop;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel pnlFirstField;
        private System.Windows.Forms.Button btnUseIntegrationRate;
        private System.Windows.Forms.Button btnUseNoIntegration;
    }
}

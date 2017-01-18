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
            this.nudPreserveVTIBottomRow = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.nudPreserveVTITopRow = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
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
            this.gbxSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFirstFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLastFrame)).BeginInit();
            this.gbxVTIPosition.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPreserveVTIBottomRow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPreserveVTITopRow)).BeginInit();
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
            this.gbxSection.Location = new System.Drawing.Point(11, 83);
            this.gbxSection.Name = "gbxSection";
            this.gbxSection.Size = new System.Drawing.Size(212, 53);
            this.gbxSection.TabIndex = 32;
            this.gbxSection.TabStop = false;
            this.gbxSection.Text = "Video Frames to Convert";
            // 
            // nudFirstFrame
            // 
            this.nudFirstFrame.Location = new System.Drawing.Point(48, 23);
            this.nudFirstFrame.Name = "nudFirstFrame";
            this.nudFirstFrame.ReadOnly = true;
            this.nudFirstFrame.Size = new System.Drawing.Size(55, 20);
            this.nudFirstFrame.TabIndex = 22;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(13, 25);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(29, 13);
            this.label27.TabIndex = 24;
            this.label27.Text = "First:";
            // 
            // nudLastFrame
            // 
            this.nudLastFrame.Location = new System.Drawing.Point(143, 23);
            this.nudLastFrame.Name = "nudLastFrame";
            this.nudLastFrame.Size = new System.Drawing.Size(55, 20);
            this.nudLastFrame.TabIndex = 23;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(114, 25);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(30, 13);
            this.label26.TabIndex = 25;
            this.label26.Text = "Last:";
            // 
            // btnDetectIntegrationRate
            // 
            this.btnDetectIntegrationRate.Location = new System.Drawing.Point(11, 150);
            this.btnDetectIntegrationRate.Name = "btnDetectIntegrationRate";
            this.btnDetectIntegrationRate.Size = new System.Drawing.Size(155, 23);
            this.btnDetectIntegrationRate.TabIndex = 44;
            this.btnDetectIntegrationRate.Text = "Detect Integration Rate";
            this.btnDetectIntegrationRate.Visible = false;
            this.btnDetectIntegrationRate.Click += new System.EventHandler(this.btnDetectIntegrationRate_Click);
            // 
            // btnConvert
            // 
            this.btnConvert.Enabled = false;
            this.btnConvert.Location = new System.Drawing.Point(11, 349);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(92, 23);
            this.btnConvert.TabIndex = 46;
            this.btnConvert.Text = "Convert";
            this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(128, 349);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(92, 23);
            this.btnCancel.TabIndex = 47;
            this.btnCancel.Text = "Stop";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gbxVTIPosition
            // 
            this.gbxVTIPosition.Controls.Add(this.btnConfirmPosition);
            this.gbxVTIPosition.Controls.Add(this.nudPreserveVTIBottomRow);
            this.gbxVTIPosition.Controls.Add(this.label1);
            this.gbxVTIPosition.Controls.Add(this.nudPreserveVTITopRow);
            this.gbxVTIPosition.Controls.Add(this.label2);
            this.gbxVTIPosition.Location = new System.Drawing.Point(11, 0);
            this.gbxVTIPosition.Name = "gbxVTIPosition";
            this.gbxVTIPosition.Size = new System.Drawing.Size(212, 80);
            this.gbxVTIPosition.TabIndex = 48;
            this.gbxVTIPosition.TabStop = false;
            this.gbxVTIPosition.Text = "VTI-OSD Position";
            // 
            // btnConfirmPosition
            // 
            this.btnConfirmPosition.Location = new System.Drawing.Point(16, 49);
            this.btnConfirmPosition.Name = "btnConfirmPosition";
            this.btnConfirmPosition.Size = new System.Drawing.Size(131, 23);
            this.btnConfirmPosition.TabIndex = 48;
            this.btnConfirmPosition.Text = "Confirm Position";
            this.btnConfirmPosition.Click += new System.EventHandler(this.btnConfirmPosition_Click);
            // 
            // nudPreserveVTIBottomRow
            // 
            this.nudPreserveVTIBottomRow.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nudPreserveVTIBottomRow.Location = new System.Drawing.Point(143, 19);
            this.nudPreserveVTIBottomRow.Maximum = new decimal(new int[] {
            4096,
            0,
            0,
            0});
            this.nudPreserveVTIBottomRow.Name = "nudPreserveVTIBottomRow";
            this.nudPreserveVTIBottomRow.Size = new System.Drawing.Size(55, 20);
            this.nudPreserveVTIBottomRow.TabIndex = 50;
            this.nudPreserveVTIBottomRow.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.nudPreserveVTIBottomRow.ValueChanged += new System.EventHandler(this.nudPreserveVTITopRow_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "From:";
            // 
            // nudPreserveVTITopRow
            // 
            this.nudPreserveVTITopRow.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nudPreserveVTITopRow.Location = new System.Drawing.Point(48, 19);
            this.nudPreserveVTITopRow.Maximum = new decimal(new int[] {
            4096,
            0,
            0,
            0});
            this.nudPreserveVTITopRow.Name = "nudPreserveVTITopRow";
            this.nudPreserveVTITopRow.Size = new System.Drawing.Size(55, 20);
            this.nudPreserveVTITopRow.TabIndex = 49;
            this.nudPreserveVTITopRow.Value = new decimal(new int[] {
            542,
            0,
            0,
            0});
            this.nudPreserveVTITopRow.ValueChanged += new System.EventHandler(this.nudPreserveVTITopRow_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(117, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 25;
            this.label2.Text = "To:";
            // 
            // gbxIntegrationRate
            // 
            this.gbxIntegrationRate.Controls.Add(this.lblFrames);
            this.gbxIntegrationRate.Controls.Add(this.nudIntegratedFrames);
            this.gbxIntegrationRate.Controls.Add(this.lblIntegration);
            this.gbxIntegrationRate.Controls.Add(this.nudStartingAtFrame);
            this.gbxIntegrationRate.Controls.Add(this.label4);
            this.gbxIntegrationRate.Location = new System.Drawing.Point(11, 142);
            this.gbxIntegrationRate.Name = "gbxIntegrationRate";
            this.gbxIntegrationRate.Size = new System.Drawing.Size(212, 76);
            this.gbxIntegrationRate.TabIndex = 49;
            this.gbxIntegrationRate.TabStop = false;
            this.gbxIntegrationRate.Text = "Integration Rate";
            // 
            // lblFrames
            // 
            this.lblFrames.AutoSize = true;
            this.lblFrames.Location = new System.Drawing.Point(136, 22);
            this.lblFrames.Name = "lblFrames";
            this.lblFrames.Size = new System.Drawing.Size(38, 13);
            this.lblFrames.TabIndex = 51;
            this.lblFrames.Text = "frames";
            // 
            // nudIntegratedFrames
            // 
            this.nudIntegratedFrames.Location = new System.Drawing.Point(74, 19);
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
            this.nudIntegratedFrames.Size = new System.Drawing.Size(56, 20);
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
            this.lblIntegration.Location = new System.Drawing.Point(13, 22);
            this.lblIntegration.Name = "lblIntegration";
            this.lblIntegration.Size = new System.Drawing.Size(60, 13);
            this.lblIntegration.TabIndex = 49;
            this.lblIntegration.Text = "Integration:";
            // 
            // nudStartingAtFrame
            // 
            this.nudStartingAtFrame.Location = new System.Drawing.Point(75, 46);
            this.nudStartingAtFrame.Name = "nudStartingAtFrame";
            this.nudStartingAtFrame.ReadOnly = true;
            this.nudStartingAtFrame.Size = new System.Drawing.Size(55, 20);
            this.nudStartingAtFrame.TabIndex = 47;
            this.nudStartingAtFrame.ValueChanged += new System.EventHandler(this.nudStartingAtFrame_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 48;
            this.label4.Text = "Starting At:";
            // 
            // pbar
            // 
            this.pbar.Location = new System.Drawing.Point(11, 329);
            this.pbar.Name = "pbar";
            this.pbar.Size = new System.Drawing.Size(212, 14);
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
            this.cbxCameraModel.Location = new System.Drawing.Point(60, 19);
            this.cbxCameraModel.Name = "cbxCameraModel";
            this.cbxCameraModel.Size = new System.Drawing.Size(138, 21);
            this.cbxCameraModel.TabIndex = 52;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 51;
            this.label3.Text = "Model:";
            // 
            // gbxCameraInfo
            // 
            this.gbxCameraInfo.Controls.Add(this.label5);
            this.gbxCameraInfo.Controls.Add(this.cbxSensorInfo);
            this.gbxCameraInfo.Controls.Add(this.label3);
            this.gbxCameraInfo.Controls.Add(this.cbxCameraModel);
            this.gbxCameraInfo.Location = new System.Drawing.Point(11, 249);
            this.gbxCameraInfo.Name = "gbxCameraInfo";
            this.gbxCameraInfo.Size = new System.Drawing.Size(212, 74);
            this.gbxCameraInfo.TabIndex = 53;
            this.gbxCameraInfo.TabStop = false;
            this.gbxCameraInfo.Text = "Camera Info";
            this.gbxCameraInfo.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 53;
            this.label5.Text = "Sensor:";
            // 
            // cbxSensorInfo
            // 
            this.cbxSensorInfo.FormattingEnabled = true;
            this.cbxSensorInfo.Items.AddRange(new object[] {
            "Unknown"});
            this.cbxSensorInfo.Location = new System.Drawing.Point(60, 46);
            this.cbxSensorInfo.Name = "cbxSensorInfo";
            this.cbxSensorInfo.Size = new System.Drawing.Size(138, 21);
            this.cbxSensorInfo.TabIndex = 54;
            // 
            // rbFirstFieldBottom
            // 
            this.rbFirstFieldBottom.AutoSize = true;
            this.rbFirstFieldBottom.Location = new System.Drawing.Point(133, 3);
            this.rbFirstFieldBottom.Name = "rbFirstFieldBottom";
            this.rbFirstFieldBottom.Size = new System.Drawing.Size(58, 17);
            this.rbFirstFieldBottom.TabIndex = 26;
            this.rbFirstFieldBottom.Text = "Bottom";
            this.rbFirstFieldBottom.UseVisualStyleBackColor = true;
            // 
            // rbFirstFieldTop
            // 
            this.rbFirstFieldTop.AutoSize = true;
            this.rbFirstFieldTop.Checked = true;
            this.rbFirstFieldTop.Location = new System.Drawing.Point(85, 4);
            this.rbFirstFieldTop.Name = "rbFirstFieldTop";
            this.rbFirstFieldTop.Size = new System.Drawing.Size(44, 17);
            this.rbFirstFieldTop.TabIndex = 25;
            this.rbFirstFieldTop.TabStop = true;
            this.rbFirstFieldTop.Text = "Top";
            this.rbFirstFieldTop.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 6);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(76, 13);
            this.label6.TabIndex = 24;
            this.label6.Text = "First field is on:";
            // 
            // pnlFirstField
            // 
            this.pnlFirstField.Controls.Add(this.rbFirstFieldBottom);
            this.pnlFirstField.Controls.Add(this.label6);
            this.pnlFirstField.Controls.Add(this.rbFirstFieldTop);
            this.pnlFirstField.Location = new System.Drawing.Point(9, 219);
            this.pnlFirstField.Name = "pnlFirstField";
            this.pnlFirstField.Size = new System.Drawing.Size(214, 27);
            this.pnlFirstField.TabIndex = 54;
            this.pnlFirstField.Visible = false;
            // 
            // ucConvertVideoToAav
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlFirstField);
            this.Controls.Add(this.gbxCameraInfo);
            this.Controls.Add(this.pbar);
            this.Controls.Add(this.gbxIntegrationRate);
            this.Controls.Add(this.gbxVTIPosition);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnDetectIntegrationRate);
            this.Controls.Add(this.btnConvert);
            this.Controls.Add(this.gbxSection);
            this.Name = "ucConvertVideoToAav";
            this.Size = new System.Drawing.Size(242, 390);
            this.gbxSection.ResumeLayout(false);
            this.gbxSection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFirstFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLastFrame)).EndInit();
            this.gbxVTIPosition.ResumeLayout(false);
            this.gbxVTIPosition.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPreserveVTIBottomRow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPreserveVTITopRow)).EndInit();
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudPreserveVTIBottomRow;
        private System.Windows.Forms.NumericUpDown nudPreserveVTITopRow;
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
    }
}

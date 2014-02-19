namespace Tangra.Config.SettingPannels
{
	partial class ucAAV8bit
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
			this.groupControl1 = new System.Windows.Forms.GroupBox();
			this.cbxADVEngine = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.nudSaturation8bit = new System.Windows.Forms.NumericUpDown();
			this.lblSaturation = new System.Windows.Forms.Label();
			this.groupControl2 = new System.Windows.Forms.GroupBox();
			this.cbxAdvsOsdSystemInfo = new System.Windows.Forms.CheckBox();
			this.cbxAdvsOsdCameraInfo = new System.Windows.Forms.CheckBox();
			this.cbxAdvsOsdMessages = new System.Windows.Forms.CheckBox();
			this.cbxAdvsOsdTimeStamp = new System.Windows.Forms.CheckBox();
			this.groupControl3 = new System.Windows.Forms.GroupBox();
			this.cbxAdvsPopupExposure = new System.Windows.Forms.CheckBox();
			this.cbxAdvsPopupGPSFix = new System.Windows.Forms.CheckBox();
			this.cbxAdvsPopupAlmanac = new System.Windows.Forms.CheckBox();
			this.cbxAdvsPopupSatellites = new System.Windows.Forms.CheckBox();
			this.cbxAdvsPopupSystemTime = new System.Windows.Forms.CheckBox();
			this.cbxAdvsPopupTimeStamp = new System.Windows.Forms.CheckBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.linkLabelAAV = new System.Windows.Forms.LinkLabel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.cbxAavSplitFieldsOSD = new System.Windows.Forms.CheckBox();
			this.cbxNtpDebugFlag = new System.Windows.Forms.CheckBox();
			this.groupControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudSaturation8bit)).BeginInit();
			this.groupControl2.SuspendLayout();
			this.groupControl3.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupControl1
			// 
			this.groupControl1.Controls.Add(this.cbxADVEngine);
			this.groupControl1.Controls.Add(this.label2);
			this.groupControl1.Controls.Add(this.nudSaturation8bit);
			this.groupControl1.Controls.Add(this.lblSaturation);
			this.groupControl1.Location = new System.Drawing.Point(3, 3);
			this.groupControl1.Name = "groupControl1";
			this.groupControl1.Size = new System.Drawing.Size(207, 95);
			this.groupControl1.TabIndex = 41;
			this.groupControl1.TabStop = false;
			this.groupControl1.Text = "OccuRec  AAV Format";
			// 
			// cbxADVEngine
			// 
			this.cbxADVEngine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxADVEngine.Items.AddRange(new object[] {
            "Native"});
			this.cbxADVEngine.Location = new System.Drawing.Point(92, 18);
			this.cbxADVEngine.Name = "cbxADVEngine";
			this.cbxADVEngine.Size = new System.Drawing.Size(97, 21);
			this.cbxADVEngine.TabIndex = 46;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 21);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(67, 13);
			this.label2.TabIndex = 44;
			this.label2.Text = "AAV Engine:";
			// 
			// nudSaturation8bit
			// 
			this.nudSaturation8bit.Enabled = false;
			this.nudSaturation8bit.Location = new System.Drawing.Point(133, 53);
			this.nudSaturation8bit.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.nudSaturation8bit.Name = "nudSaturation8bit";
			this.nudSaturation8bit.Size = new System.Drawing.Size(54, 20);
			this.nudSaturation8bit.TabIndex = 42;
			this.nudSaturation8bit.Value = new decimal(new int[] {
            250,
            0,
            0,
            0});
			// 
			// lblSaturation
			// 
			this.lblSaturation.AutoSize = true;
			this.lblSaturation.Location = new System.Drawing.Point(11, 56);
			this.lblSaturation.Name = "lblSaturation";
			this.lblSaturation.Size = new System.Drawing.Size(110, 13);
			this.lblSaturation.TabIndex = 43;
			this.lblSaturation.Text = "8 bit Saturation Level:";
			this.toolTip1.SetToolTip(this.lblSaturation, "Change this value from the \"Analogue Video\" section");
			// 
			// groupControl2
			// 
			this.groupControl2.Controls.Add(this.cbxAdvsOsdSystemInfo);
			this.groupControl2.Controls.Add(this.cbxAdvsOsdCameraInfo);
			this.groupControl2.Controls.Add(this.cbxAdvsOsdMessages);
			this.groupControl2.Controls.Add(this.cbxAdvsOsdTimeStamp);
			this.groupControl2.Location = new System.Drawing.Point(3, 104);
			this.groupControl2.Name = "groupControl2";
			this.groupControl2.Size = new System.Drawing.Size(207, 154);
			this.groupControl2.TabIndex = 44;
			this.groupControl2.TabStop = false;
			this.groupControl2.Text = "On Screen Display (Overlay)";
			// 
			// cbxAdvsOsdSystemInfo
			// 
			this.cbxAdvsOsdSystemInfo.Location = new System.Drawing.Point(14, 98);
			this.cbxAdvsOsdSystemInfo.Name = "cbxAdvsOsdSystemInfo";
			this.cbxAdvsOsdSystemInfo.Size = new System.Drawing.Size(120, 19);
			this.cbxAdvsOsdSystemInfo.TabIndex = 49;
			this.cbxAdvsOsdSystemInfo.Text = "System Info";
			// 
			// cbxAdvsOsdCameraInfo
			// 
			this.cbxAdvsOsdCameraInfo.Location = new System.Drawing.Point(14, 76);
			this.cbxAdvsOsdCameraInfo.Name = "cbxAdvsOsdCameraInfo";
			this.cbxAdvsOsdCameraInfo.Size = new System.Drawing.Size(120, 19);
			this.cbxAdvsOsdCameraInfo.TabIndex = 48;
			this.cbxAdvsOsdCameraInfo.Text = "Camera Info";
			// 
			// cbxAdvsOsdMessages
			// 
			this.cbxAdvsOsdMessages.Location = new System.Drawing.Point(14, 51);
			this.cbxAdvsOsdMessages.Name = "cbxAdvsOsdMessages";
			this.cbxAdvsOsdMessages.Size = new System.Drawing.Size(120, 19);
			this.cbxAdvsOsdMessages.TabIndex = 47;
			this.cbxAdvsOsdMessages.Text = "Messages";
			// 
			// cbxAdvsOsdTimeStamp
			// 
			this.cbxAdvsOsdTimeStamp.Location = new System.Drawing.Point(14, 26);
			this.cbxAdvsOsdTimeStamp.Name = "cbxAdvsOsdTimeStamp";
			this.cbxAdvsOsdTimeStamp.Size = new System.Drawing.Size(115, 19);
			this.cbxAdvsOsdTimeStamp.TabIndex = 44;
			this.cbxAdvsOsdTimeStamp.Text = "Timestamp";
			// 
			// groupControl3
			// 
			this.groupControl3.Controls.Add(this.cbxAdvsPopupExposure);
			this.groupControl3.Controls.Add(this.cbxAdvsPopupGPSFix);
			this.groupControl3.Controls.Add(this.cbxAdvsPopupAlmanac);
			this.groupControl3.Controls.Add(this.cbxAdvsPopupSatellites);
			this.groupControl3.Controls.Add(this.cbxAdvsPopupSystemTime);
			this.groupControl3.Controls.Add(this.cbxAdvsPopupTimeStamp);
			this.groupControl3.Location = new System.Drawing.Point(216, 3);
			this.groupControl3.Name = "groupControl3";
			this.groupControl3.Size = new System.Drawing.Size(208, 180);
			this.groupControl3.TabIndex = 45;
			this.groupControl3.TabStop = false;
			this.groupControl3.Text = "Frame Details Display (Pop-up)";
			// 
			// cbxAdvsPopupExposure
			// 
			this.cbxAdvsPopupExposure.Location = new System.Drawing.Point(14, 57);
			this.cbxAdvsPopupExposure.Name = "cbxAdvsPopupExposure";
			this.cbxAdvsPopupExposure.Size = new System.Drawing.Size(161, 19);
			this.cbxAdvsPopupExposure.TabIndex = 52;
			this.cbxAdvsPopupExposure.Text = "Exposure Duration";
			this.cbxAdvsPopupExposure.CheckedChanged += new System.EventHandler(this.OnAdvPopupSettingChanged);
			// 
			// cbxAdvsPopupGPSFix
			// 
			this.cbxAdvsPopupGPSFix.Location = new System.Drawing.Point(14, 125);
			this.cbxAdvsPopupGPSFix.Name = "cbxAdvsPopupGPSFix";
			this.cbxAdvsPopupGPSFix.Size = new System.Drawing.Size(120, 19);
			this.cbxAdvsPopupGPSFix.TabIndex = 51;
			this.cbxAdvsPopupGPSFix.Text = "GPS Fix";
			this.cbxAdvsPopupGPSFix.CheckedChanged += new System.EventHandler(this.OnAdvPopupSettingChanged);
			// 
			// cbxAdvsPopupAlmanac
			// 
			this.cbxAdvsPopupAlmanac.Location = new System.Drawing.Point(14, 102);
			this.cbxAdvsPopupAlmanac.Name = "cbxAdvsPopupAlmanac";
			this.cbxAdvsPopupAlmanac.Size = new System.Drawing.Size(120, 19);
			this.cbxAdvsPopupAlmanac.TabIndex = 50;
			this.cbxAdvsPopupAlmanac.Text = "Almanac Status";
			this.cbxAdvsPopupAlmanac.CheckedChanged += new System.EventHandler(this.OnAdvPopupSettingChanged);
			// 
			// cbxAdvsPopupSatellites
			// 
			this.cbxAdvsPopupSatellites.Location = new System.Drawing.Point(14, 79);
			this.cbxAdvsPopupSatellites.Name = "cbxAdvsPopupSatellites";
			this.cbxAdvsPopupSatellites.Size = new System.Drawing.Size(120, 19);
			this.cbxAdvsPopupSatellites.TabIndex = 49;
			this.cbxAdvsPopupSatellites.Text = "Tracked Satellites";
			this.cbxAdvsPopupSatellites.CheckedChanged += new System.EventHandler(this.OnAdvPopupSettingChanged);
			// 
			// cbxAdvsPopupSystemTime
			// 
			this.cbxAdvsPopupSystemTime.Location = new System.Drawing.Point(14, 148);
			this.cbxAdvsPopupSystemTime.Name = "cbxAdvsPopupSystemTime";
			this.cbxAdvsPopupSystemTime.Size = new System.Drawing.Size(120, 19);
			this.cbxAdvsPopupSystemTime.TabIndex = 47;
			this.cbxAdvsPopupSystemTime.Text = "System Time";
			this.cbxAdvsPopupSystemTime.CheckedChanged += new System.EventHandler(this.OnAdvPopupSettingChanged);
			// 
			// cbxAdvsPopupTimeStamp
			// 
			this.cbxAdvsPopupTimeStamp.Location = new System.Drawing.Point(14, 36);
			this.cbxAdvsPopupTimeStamp.Name = "cbxAdvsPopupTimeStamp";
			this.cbxAdvsPopupTimeStamp.Size = new System.Drawing.Size(161, 19);
			this.cbxAdvsPopupTimeStamp.TabIndex = 44;
			this.cbxAdvsPopupTimeStamp.Text = "Central Exposure Time";
			this.cbxAdvsPopupTimeStamp.CheckedChanged += new System.EventHandler(this.OnAdvPopupSettingChanged);
			// 
			// linkLabelAAV
			// 
			this.linkLabelAAV.AutoSize = true;
			this.linkLabelAAV.Location = new System.Drawing.Point(11, 2);
			this.linkLabelAAV.Name = "linkLabelAAV";
			this.linkLabelAAV.Size = new System.Drawing.Size(53, 13);
			this.linkLabelAAV.TabIndex = 46;
			this.linkLabelAAV.TabStop = true;
			this.linkLabelAAV.Text = "OccuRec";
			this.linkLabelAAV.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelAAV_LinkClicked);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.cbxAavSplitFieldsOSD);
			this.groupBox1.Location = new System.Drawing.Point(216, 189);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(207, 69);
			this.groupBox1.TabIndex = 49;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Analogue Video Fields";
			// 
			// cbxAavSplitFieldsOSD
			// 
			this.cbxAavSplitFieldsOSD.Location = new System.Drawing.Point(14, 29);
			this.cbxAavSplitFieldsOSD.Name = "cbxAavSplitFieldsOSD";
			this.cbxAavSplitFieldsOSD.Size = new System.Drawing.Size(120, 19);
			this.cbxAavSplitFieldsOSD.TabIndex = 44;
			this.cbxAavSplitFieldsOSD.Text = "Split Field OSD";
			// 
			// cbxNtpDebugFlag
			// 
			this.cbxNtpDebugFlag.Location = new System.Drawing.Point(14, 264);
			this.cbxNtpDebugFlag.Name = "cbxNtpDebugFlag";
			this.cbxNtpDebugFlag.Size = new System.Drawing.Size(256, 19);
			this.cbxNtpDebugFlag.TabIndex = 50;
			this.cbxNtpDebugFlag.Text = "Prefer NTP over OCR Timestamps (Debug Only)";
			// 
			// ucAAV8bit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.cbxNtpDebugFlag);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.linkLabelAAV);
			this.Controls.Add(this.groupControl3);
			this.Controls.Add(this.groupControl2);
			this.Controls.Add(this.groupControl1);
			this.Name = "ucAAV8bit";
			this.Size = new System.Drawing.Size(454, 325);
			this.groupControl1.ResumeLayout(false);
			this.groupControl1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudSaturation8bit)).EndInit();
			this.groupControl2.ResumeLayout(false);
			this.groupControl3.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupControl1;
		private System.Windows.Forms.NumericUpDown nudSaturation8bit;
		private System.Windows.Forms.Label lblSaturation;
		private System.Windows.Forms.GroupBox groupControl2;
		private System.Windows.Forms.CheckBox cbxAdvsOsdTimeStamp;
		private System.Windows.Forms.CheckBox cbxAdvsOsdMessages;
		private System.Windows.Forms.GroupBox groupControl3;
		private System.Windows.Forms.CheckBox cbxAdvsPopupGPSFix;
		private System.Windows.Forms.CheckBox cbxAdvsPopupAlmanac;
		private System.Windows.Forms.CheckBox cbxAdvsPopupSatellites;
		private System.Windows.Forms.CheckBox cbxAdvsPopupSystemTime;
		private System.Windows.Forms.CheckBox cbxAdvsPopupTimeStamp;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cbxADVEngine;
		private System.Windows.Forms.CheckBox cbxAdvsPopupExposure;
        private System.Windows.Forms.CheckBox cbxAdvsOsdCameraInfo;
		private System.Windows.Forms.CheckBox cbxAdvsOsdSystemInfo;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.LinkLabel linkLabelAAV;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox cbxAavSplitFieldsOSD;
		private System.Windows.Forms.CheckBox cbxNtpDebugFlag;

	}
}

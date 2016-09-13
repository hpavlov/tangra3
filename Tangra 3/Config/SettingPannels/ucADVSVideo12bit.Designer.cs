namespace Tangra.Config.SettingPannels
{
	partial class ucADVSVideo12bit
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
            this.groupControl1 = new System.Windows.Forms.GroupBox();
            this.nudSaturation16bit = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.linkLabelADVS = new System.Windows.Forms.LinkLabel();
            this.nudSaturation14bit = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.cbxADVEngine = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nudSaturation12bit = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupControl2 = new System.Windows.Forms.GroupBox();
            this.cbxAdvsOsdGeoLocation = new System.Windows.Forms.CheckBox();
            this.cbxAdvsOsdSystemInfo = new System.Windows.Forms.CheckBox();
            this.cbxAdvsOsdCameraInfo = new System.Windows.Forms.CheckBox();
            this.cbxAdvsOsdMessages = new System.Windows.Forms.CheckBox();
            this.cbxAdvsOsdTimeStamp = new System.Windows.Forms.CheckBox();
            this.groupControl3 = new System.Windows.Forms.GroupBox();
            this.cbxAdvsPopupVideoCameraFrameId = new System.Windows.Forms.CheckBox();
            this.cbxAdvsPopupExposure = new System.Windows.Forms.CheckBox();
            this.cbxAdvsPopupGPSFix = new System.Windows.Forms.CheckBox();
            this.cbxAdvsPopupAlmanac = new System.Windows.Forms.CheckBox();
            this.cbxAdvsPopupSatellites = new System.Windows.Forms.CheckBox();
            this.cbxAdvsPopupOffset = new System.Windows.Forms.CheckBox();
            this.cbxAdvsPopupSystemTime = new System.Windows.Forms.CheckBox();
            this.cbxAdvsPopupGamma = new System.Windows.Forms.CheckBox();
            this.cbxAdvsPopupGain = new System.Windows.Forms.CheckBox();
            this.cbxAdvsPopupTimeStamp = new System.Windows.Forms.CheckBox();
            this.cbxAdvsOsdObjName = new System.Windows.Forms.CheckBox();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSaturation16bit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSaturation14bit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSaturation12bit)).BeginInit();
            this.groupControl2.SuspendLayout();
            this.groupControl3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.nudSaturation16bit);
            this.groupControl1.Controls.Add(this.label4);
            this.groupControl1.Controls.Add(this.linkLabelADVS);
            this.groupControl1.Controls.Add(this.nudSaturation14bit);
            this.groupControl1.Controls.Add(this.label3);
            this.groupControl1.Controls.Add(this.cbxADVEngine);
            this.groupControl1.Controls.Add(this.label2);
            this.groupControl1.Controls.Add(this.nudSaturation12bit);
            this.groupControl1.Controls.Add(this.label1);
            this.groupControl1.Location = new System.Drawing.Point(3, 3);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(207, 131);
            this.groupControl1.TabIndex = 41;
            this.groupControl1.TabStop = false;
            this.groupControl1.Text = "ADVS";
            // 
            // nudSaturation16bit
            // 
            this.nudSaturation16bit.Location = new System.Drawing.Point(133, 105);
            this.nudSaturation16bit.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudSaturation16bit.Name = "nudSaturation16bit";
            this.nudSaturation16bit.Size = new System.Drawing.Size(54, 20);
            this.nudSaturation16bit.TabIndex = 56;
            this.nudSaturation16bit.Value = new decimal(new int[] {
            65000,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 108);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(116, 13);
            this.label4.TabIndex = 57;
            this.label4.Text = "16 bit Saturation Level:";
            // 
            // linkLabelADVS
            // 
            this.linkLabelADVS.AutoSize = true;
            this.linkLabelADVS.Location = new System.Drawing.Point(8, -1);
            this.linkLabelADVS.Name = "linkLabelADVS";
            this.linkLabelADVS.Size = new System.Drawing.Size(36, 13);
            this.linkLabelADVS.TabIndex = 55;
            this.linkLabelADVS.TabStop = true;
            this.linkLabelADVS.Text = "ADVS";
            this.linkLabelADVS.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelADVS_LinkClicked);
            // 
            // nudSaturation14bit
            // 
            this.nudSaturation14bit.Location = new System.Drawing.Point(133, 79);
            this.nudSaturation14bit.Maximum = new decimal(new int[] {
            16383,
            0,
            0,
            0});
            this.nudSaturation14bit.Name = "nudSaturation14bit";
            this.nudSaturation14bit.Size = new System.Drawing.Size(54, 20);
            this.nudSaturation14bit.TabIndex = 47;
            this.nudSaturation14bit.Value = new decimal(new int[] {
            16000,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 13);
            this.label3.TabIndex = 48;
            this.label3.Text = "14 bit Saturation Level:";
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
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 44;
            this.label2.Text = "ADV Engine:";
            // 
            // nudSaturation12bit
            // 
            this.nudSaturation12bit.Location = new System.Drawing.Point(133, 53);
            this.nudSaturation12bit.Maximum = new decimal(new int[] {
            4095,
            0,
            0,
            0});
            this.nudSaturation12bit.Name = "nudSaturation12bit";
            this.nudSaturation12bit.Size = new System.Drawing.Size(54, 20);
            this.nudSaturation12bit.TabIndex = 42;
            this.nudSaturation12bit.Value = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 13);
            this.label1.TabIndex = 43;
            this.label1.Text = "12 bit Saturation Level:";
            // 
            // groupControl2
            // 
            this.groupControl2.Controls.Add(this.cbxAdvsOsdObjName);
            this.groupControl2.Controls.Add(this.cbxAdvsOsdGeoLocation);
            this.groupControl2.Controls.Add(this.cbxAdvsOsdSystemInfo);
            this.groupControl2.Controls.Add(this.cbxAdvsOsdCameraInfo);
            this.groupControl2.Controls.Add(this.cbxAdvsOsdMessages);
            this.groupControl2.Controls.Add(this.cbxAdvsOsdTimeStamp);
            this.groupControl2.Location = new System.Drawing.Point(3, 140);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Size = new System.Drawing.Size(207, 189);
            this.groupControl2.TabIndex = 44;
            this.groupControl2.TabStop = false;
            this.groupControl2.Text = "On Screen Display (Overlay)";
            // 
            // cbxAdvsOsdGeoLocation
            // 
            this.cbxAdvsOsdGeoLocation.Location = new System.Drawing.Point(14, 117);
            this.cbxAdvsOsdGeoLocation.Name = "cbxAdvsOsdGeoLocation";
            this.cbxAdvsOsdGeoLocation.Size = new System.Drawing.Size(120, 19);
            this.cbxAdvsOsdGeoLocation.TabIndex = 50;
            this.cbxAdvsOsdGeoLocation.Text = "Geo Location";
            // 
            // cbxAdvsOsdSystemInfo
            // 
            this.cbxAdvsOsdSystemInfo.Location = new System.Drawing.Point(14, 95);
            this.cbxAdvsOsdSystemInfo.Name = "cbxAdvsOsdSystemInfo";
            this.cbxAdvsOsdSystemInfo.Size = new System.Drawing.Size(120, 19);
            this.cbxAdvsOsdSystemInfo.TabIndex = 49;
            this.cbxAdvsOsdSystemInfo.Text = "System Info";
            // 
            // cbxAdvsOsdCameraInfo
            // 
            this.cbxAdvsOsdCameraInfo.Location = new System.Drawing.Point(14, 73);
            this.cbxAdvsOsdCameraInfo.Name = "cbxAdvsOsdCameraInfo";
            this.cbxAdvsOsdCameraInfo.Size = new System.Drawing.Size(120, 19);
            this.cbxAdvsOsdCameraInfo.TabIndex = 48;
            this.cbxAdvsOsdCameraInfo.Text = "Camera Info";
            // 
            // cbxAdvsOsdMessages
            // 
            this.cbxAdvsOsdMessages.Location = new System.Drawing.Point(14, 48);
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
            this.groupControl3.Controls.Add(this.cbxAdvsPopupVideoCameraFrameId);
            this.groupControl3.Controls.Add(this.cbxAdvsPopupExposure);
            this.groupControl3.Controls.Add(this.cbxAdvsPopupGPSFix);
            this.groupControl3.Controls.Add(this.cbxAdvsPopupAlmanac);
            this.groupControl3.Controls.Add(this.cbxAdvsPopupSatellites);
            this.groupControl3.Controls.Add(this.cbxAdvsPopupOffset);
            this.groupControl3.Controls.Add(this.cbxAdvsPopupSystemTime);
            this.groupControl3.Controls.Add(this.cbxAdvsPopupGamma);
            this.groupControl3.Controls.Add(this.cbxAdvsPopupGain);
            this.groupControl3.Controls.Add(this.cbxAdvsPopupTimeStamp);
            this.groupControl3.Location = new System.Drawing.Point(216, 3);
            this.groupControl3.Name = "groupControl3";
            this.groupControl3.Size = new System.Drawing.Size(208, 323);
            this.groupControl3.TabIndex = 45;
            this.groupControl3.TabStop = false;
            this.groupControl3.Text = "Frame Details Display (Pop-up)";
            // 
            // cbxAdvsPopupVideoCameraFrameId
            // 
            this.cbxAdvsPopupVideoCameraFrameId.Location = new System.Drawing.Point(14, 76);
            this.cbxAdvsPopupVideoCameraFrameId.Name = "cbxAdvsPopupVideoCameraFrameId";
            this.cbxAdvsPopupVideoCameraFrameId.Size = new System.Drawing.Size(161, 19);
            this.cbxAdvsPopupVideoCameraFrameId.TabIndex = 53;
            this.cbxAdvsPopupVideoCameraFrameId.Text = "Camera Frame #";
            this.cbxAdvsPopupVideoCameraFrameId.CheckedChanged += new System.EventHandler(this.OnAdvPopupSettingChanged);
            // 
            // cbxAdvsPopupExposure
            // 
            this.cbxAdvsPopupExposure.Location = new System.Drawing.Point(14, 56);
            this.cbxAdvsPopupExposure.Name = "cbxAdvsPopupExposure";
            this.cbxAdvsPopupExposure.Size = new System.Drawing.Size(161, 19);
            this.cbxAdvsPopupExposure.TabIndex = 52;
            this.cbxAdvsPopupExposure.Text = "Exposure Duration";
            this.cbxAdvsPopupExposure.CheckedChanged += new System.EventHandler(this.OnAdvPopupSettingChanged);
            // 
            // cbxAdvsPopupGPSFix
            // 
            this.cbxAdvsPopupGPSFix.Location = new System.Drawing.Point(14, 214);
            this.cbxAdvsPopupGPSFix.Name = "cbxAdvsPopupGPSFix";
            this.cbxAdvsPopupGPSFix.Size = new System.Drawing.Size(120, 19);
            this.cbxAdvsPopupGPSFix.TabIndex = 51;
            this.cbxAdvsPopupGPSFix.Text = "GPS Fix";
            this.cbxAdvsPopupGPSFix.CheckedChanged += new System.EventHandler(this.OnAdvPopupSettingChanged);
            // 
            // cbxAdvsPopupAlmanac
            // 
            this.cbxAdvsPopupAlmanac.Location = new System.Drawing.Point(14, 191);
            this.cbxAdvsPopupAlmanac.Name = "cbxAdvsPopupAlmanac";
            this.cbxAdvsPopupAlmanac.Size = new System.Drawing.Size(120, 19);
            this.cbxAdvsPopupAlmanac.TabIndex = 50;
            this.cbxAdvsPopupAlmanac.Text = "Almanac Status";
            this.cbxAdvsPopupAlmanac.CheckedChanged += new System.EventHandler(this.OnAdvPopupSettingChanged);
            // 
            // cbxAdvsPopupSatellites
            // 
            this.cbxAdvsPopupSatellites.Location = new System.Drawing.Point(14, 168);
            this.cbxAdvsPopupSatellites.Name = "cbxAdvsPopupSatellites";
            this.cbxAdvsPopupSatellites.Size = new System.Drawing.Size(120, 19);
            this.cbxAdvsPopupSatellites.TabIndex = 49;
            this.cbxAdvsPopupSatellites.Text = "Tracked Satellites";
            this.cbxAdvsPopupSatellites.CheckedChanged += new System.EventHandler(this.OnAdvPopupSettingChanged);
            // 
            // cbxAdvsPopupOffset
            // 
            this.cbxAdvsPopupOffset.Location = new System.Drawing.Point(14, 145);
            this.cbxAdvsPopupOffset.Name = "cbxAdvsPopupOffset";
            this.cbxAdvsPopupOffset.Size = new System.Drawing.Size(75, 19);
            this.cbxAdvsPopupOffset.TabIndex = 48;
            this.cbxAdvsPopupOffset.Text = "Offset";
            this.cbxAdvsPopupOffset.CheckedChanged += new System.EventHandler(this.OnAdvPopupSettingChanged);
            // 
            // cbxAdvsPopupSystemTime
            // 
            this.cbxAdvsPopupSystemTime.Location = new System.Drawing.Point(14, 237);
            this.cbxAdvsPopupSystemTime.Name = "cbxAdvsPopupSystemTime";
            this.cbxAdvsPopupSystemTime.Size = new System.Drawing.Size(120, 19);
            this.cbxAdvsPopupSystemTime.TabIndex = 47;
            this.cbxAdvsPopupSystemTime.Text = "PC Clock Time";
            this.cbxAdvsPopupSystemTime.CheckedChanged += new System.EventHandler(this.OnAdvPopupSettingChanged);
            // 
            // cbxAdvsPopupGamma
            // 
            this.cbxAdvsPopupGamma.Location = new System.Drawing.Point(14, 122);
            this.cbxAdvsPopupGamma.Name = "cbxAdvsPopupGamma";
            this.cbxAdvsPopupGamma.Size = new System.Drawing.Size(120, 19);
            this.cbxAdvsPopupGamma.TabIndex = 46;
            this.cbxAdvsPopupGamma.Text = "Gamma";
            this.cbxAdvsPopupGamma.CheckedChanged += new System.EventHandler(this.OnAdvPopupSettingChanged);
            // 
            // cbxAdvsPopupGain
            // 
            this.cbxAdvsPopupGain.Location = new System.Drawing.Point(14, 99);
            this.cbxAdvsPopupGain.Name = "cbxAdvsPopupGain";
            this.cbxAdvsPopupGain.Size = new System.Drawing.Size(120, 19);
            this.cbxAdvsPopupGain.TabIndex = 45;
            this.cbxAdvsPopupGain.Text = "Gain";
            this.cbxAdvsPopupGain.CheckedChanged += new System.EventHandler(this.OnAdvPopupSettingChanged);
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
            // cbxAdvsOsdObjName
            // 
            this.cbxAdvsOsdObjName.Location = new System.Drawing.Point(14, 139);
            this.cbxAdvsOsdObjName.Name = "cbxAdvsOsdObjName";
            this.cbxAdvsOsdObjName.Size = new System.Drawing.Size(120, 19);
            this.cbxAdvsOsdObjName.TabIndex = 51;
            this.cbxAdvsOsdObjName.Text = "Observed Object";
            // 
            // ucADVSVideo12bit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.groupControl3);
            this.Controls.Add(this.groupControl2);
            this.Controls.Add(this.groupControl1);
            this.Name = "ucADVSVideo12bit";
            this.Size = new System.Drawing.Size(454, 343);
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSaturation16bit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSaturation14bit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSaturation12bit)).EndInit();
            this.groupControl2.ResumeLayout(false);
            this.groupControl3.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupControl1;
		private System.Windows.Forms.NumericUpDown nudSaturation12bit;
		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupControl2;
		private System.Windows.Forms.CheckBox cbxAdvsOsdTimeStamp;
        private System.Windows.Forms.CheckBox cbxAdvsOsdMessages;
		private System.Windows.Forms.GroupBox groupControl3;
		private System.Windows.Forms.CheckBox cbxAdvsPopupGPSFix;
		private System.Windows.Forms.CheckBox cbxAdvsPopupAlmanac;
		private System.Windows.Forms.CheckBox cbxAdvsPopupSatellites;
		private System.Windows.Forms.CheckBox cbxAdvsPopupOffset;
		private System.Windows.Forms.CheckBox cbxAdvsPopupSystemTime;
		private System.Windows.Forms.CheckBox cbxAdvsPopupGamma;
		private System.Windows.Forms.CheckBox cbxAdvsPopupGain;
		private System.Windows.Forms.CheckBox cbxAdvsPopupTimeStamp;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cbxADVEngine;
		private System.Windows.Forms.CheckBox cbxAdvsPopupExposure;
		private System.Windows.Forms.CheckBox cbxAdvsPopupVideoCameraFrameId;
        private System.Windows.Forms.CheckBox cbxAdvsOsdCameraInfo;
        private System.Windows.Forms.CheckBox cbxAdvsOsdSystemInfo;
		private System.Windows.Forms.NumericUpDown nudSaturation14bit;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox cbxAdvsOsdGeoLocation;
		private System.Windows.Forms.LinkLabel linkLabelADVS;
		private System.Windows.Forms.NumericUpDown nudSaturation16bit;
		private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cbxAdvsOsdObjName;

	}
}

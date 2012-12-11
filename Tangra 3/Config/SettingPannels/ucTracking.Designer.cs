namespace Tangra.Config.SettingPannels
{
	partial class ucTracking
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
			this.pnlRecoverySettings = new System.Windows.Forms.Panel();
			this.label24 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label23 = new System.Windows.Forms.Label();
			this.tbRecoveryTolerance = new System.Windows.Forms.TrackBar();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label19 = new System.Windows.Forms.Label();
			this.cbxInterlacedCorrection = new System.Windows.Forms.CheckBox();
			this.label15 = new System.Windows.Forms.Label();
			this.nudInterlacedCorrection = new System.Windows.Forms.NumericUpDown();
			this.label16 = new System.Windows.Forms.Label();
			this.label18 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.groupControl1 = new System.Windows.Forms.GroupBox();
			this.nudMinAboveMedian = new System.Windows.Forms.NumericUpDown();
			this.nudRefiningFrames = new System.Windows.Forms.NumericUpDown();
			this.nudMinSNRatio = new System.Windows.Forms.NumericUpDown();
			this.cbWarnOnUnsatisfiedGuidingRequirements = new System.Windows.Forms.CheckBox();
			this.cbxPlaySound = new System.Windows.Forms.CheckBox();
			this.cbxRecoverFromLostTracking = new System.Windows.Forms.CheckBox();
			this.pnlRecoverySettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tbRecoveryTolerance)).BeginInit();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudInterlacedCorrection)).BeginInit();
			this.groupControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudMinAboveMedian)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudRefiningFrames)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinSNRatio)).BeginInit();
			this.SuspendLayout();
			// 
			// pnlRecoverySettings
			// 
			this.pnlRecoverySettings.Controls.Add(this.label24);
			this.pnlRecoverySettings.Controls.Add(this.label10);
			this.pnlRecoverySettings.Controls.Add(this.label23);
			this.pnlRecoverySettings.Controls.Add(this.tbRecoveryTolerance);
			this.pnlRecoverySettings.Location = new System.Drawing.Point(250, 30);
			this.pnlRecoverySettings.Name = "pnlRecoverySettings";
			this.pnlRecoverySettings.Size = new System.Drawing.Size(201, 80);
			this.pnlRecoverySettings.TabIndex = 39;
			// 
			// label24
			// 
			this.label24.Location = new System.Drawing.Point(123, 44);
			this.label24.Name = "label24";
			this.label24.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label24.Size = new System.Drawing.Size(75, 29);
			this.label24.TabIndex = 41;
			this.label24.Text = "Slower && more tolerant";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(45, -1);
			this.label10.Name = "label10";
			this.label10.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label10.Size = new System.Drawing.Size(104, 13);
			this.label10.TabIndex = 38;
			this.label10.Text = "Recovery Tolerance";
			// 
			// label23
			// 
			this.label23.Location = new System.Drawing.Point(-3, 46);
			this.label23.Name = "label23";
			this.label23.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label23.Size = new System.Drawing.Size(79, 31);
			this.label23.TabIndex = 40;
			this.label23.Text = "Faster && less tolerant";
			// 
			// tbRecoveryTolerance
			// 
			this.tbRecoveryTolerance.Location = new System.Drawing.Point(19, 12);
			this.tbRecoveryTolerance.Maximum = 4;
			this.tbRecoveryTolerance.Name = "tbRecoveryTolerance";
			this.tbRecoveryTolerance.Size = new System.Drawing.Size(161, 42);
			this.tbRecoveryTolerance.TabIndex = 42;
			this.tbRecoveryTolerance.Value = 2;
			this.tbRecoveryTolerance.ValueChanged += new System.EventHandler(this.tbRecoveryTolerance_ValueChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label19);
			this.groupBox2.Controls.Add(this.cbxInterlacedCorrection);
			this.groupBox2.Controls.Add(this.label15);
			this.groupBox2.Controls.Add(this.nudInterlacedCorrection);
			this.groupBox2.Controls.Add(this.label16);
			this.groupBox2.Location = new System.Drawing.Point(373, 169);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(358, 54);
			this.groupBox2.TabIndex = 33;
			this.groupBox2.TabStop = false;
			this.groupBox2.Visible = false;
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.Location = new System.Drawing.Point(23, 20);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(185, 13);
			this.label19.TabIndex = 30;
			this.label19.Text = "Interlaced video movement correction";
			// 
			// cbxInterlacedCorrection
			// 
			this.cbxInterlacedCorrection.AutoSize = true;
			this.cbxInterlacedCorrection.Enabled = false;
			this.cbxInterlacedCorrection.Location = new System.Drawing.Point(6, 19);
			this.cbxInterlacedCorrection.Name = "cbxInterlacedCorrection";
			this.cbxInterlacedCorrection.Size = new System.Drawing.Size(15, 14);
			this.cbxInterlacedCorrection.TabIndex = 29;
			this.cbxInterlacedCorrection.UseVisualStyleBackColor = true;
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(207, 20);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(36, 13);
			this.label15.TabIndex = 26;
			this.label15.Text = "Up to:";
			// 
			// nudInterlacedCorrection
			// 
			this.nudInterlacedCorrection.Location = new System.Drawing.Point(249, 16);
			this.nudInterlacedCorrection.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.nudInterlacedCorrection.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudInterlacedCorrection.Name = "nudInterlacedCorrection";
			this.nudInterlacedCorrection.Size = new System.Drawing.Size(39, 20);
			this.nudInterlacedCorrection.TabIndex = 27;
			this.nudInterlacedCorrection.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(291, 20);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(28, 13);
			this.label16.TabIndex = 28;
			this.label16.Text = "lines";
			// 
			// label18
			// 
			this.label18.AutoSize = true;
			this.label18.Location = new System.Drawing.Point(90, 197);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(134, 13);
			this.label18.TabIndex = 31;
			this.label18.Text = "Minimum signal/noise ratio:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label5.Location = new System.Drawing.Point(24, 149);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(168, 13);
			this.label5.TabIndex = 25;
			this.label5.Text = "Guiding Stars Requirements:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(24, 47);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(86, 13);
			this.label4.TabIndex = 21;
			this.label4.Text = "Refining Frames:";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(24, 171);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(200, 13);
			this.label11.TabIndex = 23;
			this.label11.Text = "Minimum brightness above median noise:";
			// 
			// groupControl1
			// 
			this.groupControl1.Controls.Add(this.nudMinAboveMedian);
			this.groupControl1.Controls.Add(this.nudRefiningFrames);
			this.groupControl1.Controls.Add(this.nudMinSNRatio);
			this.groupControl1.Controls.Add(this.cbWarnOnUnsatisfiedGuidingRequirements);
			this.groupControl1.Controls.Add(this.cbxPlaySound);
			this.groupControl1.Controls.Add(this.cbxRecoverFromLostTracking);
			this.groupControl1.Controls.Add(this.pnlRecoverySettings);
			this.groupControl1.Controls.Add(this.label4);
			this.groupControl1.Controls.Add(this.label11);
			this.groupControl1.Controls.Add(this.groupBox2);
			this.groupControl1.Controls.Add(this.label5);
			this.groupControl1.Controls.Add(this.label18);
			this.groupControl1.Location = new System.Drawing.Point(3, 3);
			this.groupControl1.Name = "groupControl1";
			this.groupControl1.Size = new System.Drawing.Size(464, 308);
			this.groupControl1.TabIndex = 5;
			this.groupControl1.TabStop = false;
			this.groupControl1.Text = "Object Tracking";
			// 
			// nudMinAboveMedian
			// 
			this.nudMinAboveMedian.Location = new System.Drawing.Point(229, 168);
			this.nudMinAboveMedian.Name = "nudMinAboveMedian";
			this.nudMinAboveMedian.Size = new System.Drawing.Size(47, 20);
			this.nudMinAboveMedian.TabIndex = 44;
			// 
			// nudRefiningFrames
			// 
			this.nudRefiningFrames.Location = new System.Drawing.Point(116, 44);
			this.nudRefiningFrames.Name = "nudRefiningFrames";
			this.nudRefiningFrames.Size = new System.Drawing.Size(47, 20);
			this.nudRefiningFrames.TabIndex = 43;
			// 
			// nudMinSNRatio
			// 
			this.nudMinSNRatio.Location = new System.Drawing.Point(229, 194);
			this.nudMinSNRatio.Name = "nudMinSNRatio";
			this.nudMinSNRatio.Size = new System.Drawing.Size(47, 20);
			this.nudMinSNRatio.TabIndex = 42;
			// 
			// cbWarnOnUnsatisfiedGuidingRequirements
			// 
			this.cbWarnOnUnsatisfiedGuidingRequirements.Location = new System.Drawing.Point(25, 221);
			this.cbWarnOnUnsatisfiedGuidingRequirements.Name = "cbWarnOnUnsatisfiedGuidingRequirements";
			this.cbWarnOnUnsatisfiedGuidingRequirements.Size = new System.Drawing.Size(282, 19);
			this.cbWarnOnUnsatisfiedGuidingRequirements.TabIndex = 41;
			this.cbWarnOnUnsatisfiedGuidingRequirements.Text = "Warn on unsatisfied guiding star requirements";
			// 
			// cbxPlaySound
			// 
			this.cbxPlaySound.Location = new System.Drawing.Point(25, 116);
			this.cbxPlaySound.Name = "cbxPlaySound";
			this.cbxPlaySound.Size = new System.Drawing.Size(282, 19);
			this.cbxPlaySound.TabIndex = 40;
			this.cbxPlaySound.Text = "Play sound on lost tracking and end of measurement";
			// 
			// cbxRecoverFromLostTracking
			// 
			this.cbxRecoverFromLostTracking.Location = new System.Drawing.Point(25, 84);
			this.cbxRecoverFromLostTracking.Name = "cbxRecoverFromLostTracking";
			this.cbxRecoverFromLostTracking.Size = new System.Drawing.Size(226, 19);
			this.cbxRecoverFromLostTracking.TabIndex = 6;
			this.cbxRecoverFromLostTracking.Text = "Try automatic recovery from lost tracking";
			this.cbxRecoverFromLostTracking.CheckedChanged += new System.EventHandler(this.cbxRecoverFromLostTracking_CheckedChanged);
			// 
			// ucTracking
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.groupControl1);
			this.Name = "ucTracking";
			this.Size = new System.Drawing.Size(949, 366);
			this.pnlRecoverySettings.ResumeLayout(false);
			this.pnlRecoverySettings.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.tbRecoveryTolerance)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudInterlacedCorrection)).EndInit();
			this.groupControl1.ResumeLayout(false);
			this.groupControl1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudMinAboveMedian)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudRefiningFrames)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinSNRatio)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel pnlRecoverySettings;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.CheckBox cbxInterlacedCorrection;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.NumericUpDown nudInterlacedCorrection;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.GroupBox groupControl1;
		private System.Windows.Forms.CheckBox cbxRecoverFromLostTracking;
		private System.Windows.Forms.CheckBox cbWarnOnUnsatisfiedGuidingRequirements;
		private System.Windows.Forms.CheckBox cbxPlaySound;
		private System.Windows.Forms.NumericUpDown nudMinSNRatio;
		private System.Windows.Forms.NumericUpDown nudRefiningFrames;
		private System.Windows.Forms.NumericUpDown nudMinAboveMedian;
		private System.Windows.Forms.TrackBar tbRecoveryTolerance;
	}
}

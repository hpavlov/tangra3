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
            this.cbxTrackingEngine = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabRecoveryTracker = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.nudRefiningFrames = new System.Windows.Forms.NumericUpDown();
            this.cbxRecoverFromLostTracking = new System.Windows.Forms.CheckBox();
            this.cbxPlaySound = new System.Windows.Forms.CheckBox();
            this.tabSimplifiedTracker = new System.Windows.Forms.TabPage();
            this.label8 = new System.Windows.Forms.Label();
            this.nudDetectionCertainty = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.cbxTestPSFElongation = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.nudMaxElongation = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.nudMaxFWHM = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.nudMinFWHM = new System.Windows.Forms.NumericUpDown();
            this.nudMinAboveMedian = new System.Windows.Forms.NumericUpDown();
            this.nudMinSNRatio = new System.Windows.Forms.NumericUpDown();
            this.cbWarnOnUnsatisfiedGuidingRequirements = new System.Windows.Forms.CheckBox();
            this.pnlRecoverySettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbRecoveryTolerance)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudInterlacedCorrection)).BeginInit();
            this.groupControl1.SuspendLayout();
            this.tabRecoveryTracker.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRefiningFrames)).BeginInit();
            this.tabSimplifiedTracker.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDetectionCertainty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxElongation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxFWHM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinFWHM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinAboveMedian)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinSNRatio)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlRecoverySettings
            // 
            this.pnlRecoverySettings.Controls.Add(this.label24);
            this.pnlRecoverySettings.Controls.Add(this.label10);
            this.pnlRecoverySettings.Controls.Add(this.label23);
            this.pnlRecoverySettings.Controls.Add(this.tbRecoveryTolerance);
            this.pnlRecoverySettings.Location = new System.Drawing.Point(31, 31);
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
            this.tbRecoveryTolerance.Size = new System.Drawing.Size(161, 45);
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
            this.groupBox2.Location = new System.Drawing.Point(373, 265);
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
            this.label18.Location = new System.Drawing.Point(90, 294);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(134, 13);
            this.label18.TabIndex = 31;
            this.label18.Text = "Minimum signal/noise ratio:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(7, 246);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(168, 13);
            this.label5.TabIndex = 25;
            this.label5.Text = "Guiding Stars Requirements:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(276, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Refining Frames:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(24, 268);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(200, 13);
            this.label11.TabIndex = 23;
            this.label11.Text = "Minimum brightness above median noise:";
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.cbxTrackingEngine);
            this.groupControl1.Controls.Add(this.label2);
            this.groupControl1.Controls.Add(this.tabRecoveryTracker);
            this.groupControl1.Controls.Add(this.nudMinAboveMedian);
            this.groupControl1.Controls.Add(this.nudMinSNRatio);
            this.groupControl1.Controls.Add(this.cbWarnOnUnsatisfiedGuidingRequirements);
            this.groupControl1.Controls.Add(this.label11);
            this.groupControl1.Controls.Add(this.groupBox2);
            this.groupControl1.Controls.Add(this.label5);
            this.groupControl1.Controls.Add(this.label18);
            this.groupControl1.Location = new System.Drawing.Point(3, 3);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(464, 348);
            this.groupControl1.TabIndex = 5;
            this.groupControl1.TabStop = false;
            this.groupControl1.Text = "Object Tracking";
            // 
            // cbxTrackingEngine
            // 
            this.cbxTrackingEngine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTrackingEngine.Items.AddRange(new object[] {
            "Let Tangra choose automatically",
            "Tracking with automatic recovery",
            "Simplified tracking"});
            this.cbxTrackingEngine.Location = new System.Drawing.Point(102, 18);
            this.cbxTrackingEngine.Name = "cbxTrackingEngine";
            this.cbxTrackingEngine.Size = new System.Drawing.Size(233, 21);
            this.cbxTrackingEngine.TabIndex = 48;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 47;
            this.label2.Text = "Tracking Engine:";
            // 
            // tabRecoveryTracker
            // 
            this.tabRecoveryTracker.Controls.Add(this.tabPage1);
            this.tabRecoveryTracker.Controls.Add(this.tabSimplifiedTracker);
            this.tabRecoveryTracker.Location = new System.Drawing.Point(6, 54);
            this.tabRecoveryTracker.Name = "tabRecoveryTracker";
            this.tabRecoveryTracker.SelectedIndex = 0;
            this.tabRecoveryTracker.Size = new System.Drawing.Size(451, 178);
            this.tabRecoveryTracker.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.nudRefiningFrames);
            this.tabPage1.Controls.Add(this.pnlRecoverySettings);
            this.tabPage1.Controls.Add(this.cbxRecoverFromLostTracking);
            this.tabPage1.Controls.Add(this.cbxPlaySound);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(443, 152);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Tracking with automatic recovery";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // nudRefiningFrames
            // 
            this.nudRefiningFrames.Location = new System.Drawing.Point(368, 27);
            this.nudRefiningFrames.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.nudRefiningFrames.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRefiningFrames.Name = "nudRefiningFrames";
            this.nudRefiningFrames.Size = new System.Drawing.Size(47, 20);
            this.nudRefiningFrames.TabIndex = 43;
            this.nudRefiningFrames.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // cbxRecoverFromLostTracking
            // 
            this.cbxRecoverFromLostTracking.Location = new System.Drawing.Point(6, 6);
            this.cbxRecoverFromLostTracking.Name = "cbxRecoverFromLostTracking";
            this.cbxRecoverFromLostTracking.Size = new System.Drawing.Size(226, 19);
            this.cbxRecoverFromLostTracking.TabIndex = 6;
            this.cbxRecoverFromLostTracking.Text = "Try automatic recovery from lost tracking";
            this.cbxRecoverFromLostTracking.CheckedChanged += new System.EventHandler(this.cbxRecoverFromLostTracking_CheckedChanged);
            // 
            // cbxPlaySound
            // 
            this.cbxPlaySound.Location = new System.Drawing.Point(6, 121);
            this.cbxPlaySound.Name = "cbxPlaySound";
            this.cbxPlaySound.Size = new System.Drawing.Size(282, 19);
            this.cbxPlaySound.TabIndex = 40;
            this.cbxPlaySound.Text = "Play sound on lost tracking and end of measurement";
            // 
            // tabSimplifiedTracker
            // 
            this.tabSimplifiedTracker.Controls.Add(this.label8);
            this.tabSimplifiedTracker.Controls.Add(this.nudDetectionCertainty);
            this.tabSimplifiedTracker.Controls.Add(this.label7);
            this.tabSimplifiedTracker.Controls.Add(this.cbxTestPSFElongation);
            this.tabSimplifiedTracker.Controls.Add(this.label6);
            this.tabSimplifiedTracker.Controls.Add(this.nudMaxElongation);
            this.tabSimplifiedTracker.Controls.Add(this.label3);
            this.tabSimplifiedTracker.Controls.Add(this.nudMaxFWHM);
            this.tabSimplifiedTracker.Controls.Add(this.label1);
            this.tabSimplifiedTracker.Controls.Add(this.nudMinFWHM);
            this.tabSimplifiedTracker.Location = new System.Drawing.Point(4, 22);
            this.tabSimplifiedTracker.Name = "tabSimplifiedTracker";
            this.tabSimplifiedTracker.Padding = new System.Windows.Forms.Padding(3);
            this.tabSimplifiedTracker.Size = new System.Drawing.Size(443, 152);
            this.tabSimplifiedTracker.TabIndex = 1;
            this.tabSimplifiedTracker.Text = "Simplified tracking";
            this.tabSimplifiedTracker.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(220, 21);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(144, 13);
            this.label8.TabIndex = 52;
            this.label8.Text = "Minimum Detection Certainty:";
            // 
            // nudDetectionCertainty
            // 
            this.nudDetectionCertainty.DecimalPlaces = 1;
            this.nudDetectionCertainty.Location = new System.Drawing.Point(374, 18);
            this.nudDetectionCertainty.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudDetectionCertainty.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudDetectionCertainty.Name = "nudDetectionCertainty";
            this.nudDetectionCertainty.Size = new System.Drawing.Size(51, 20);
            this.nudDetectionCertainty.TabIndex = 53;
            this.nudDetectionCertainty.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(186, 117);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(15, 13);
            this.label7.TabIndex = 51;
            this.label7.Text = "%";
            // 
            // cbxTestPSFElongation
            // 
            this.cbxTestPSFElongation.Location = new System.Drawing.Point(6, 89);
            this.cbxTestPSFElongation.Name = "cbxTestPSFElongation";
            this.cbxTestPSFElongation.Size = new System.Drawing.Size(187, 19);
            this.cbxTestPSFElongation.TabIndex = 50;
            this.cbxTestPSFElongation.Text = "Test PSF elongation";
            this.cbxTestPSFElongation.CheckedChanged += new System.EventHandler(this.cbxTestPSFElongation_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 116);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(83, 13);
            this.label6.TabIndex = 48;
            this.label6.Text = "Max Elongation:";
            // 
            // nudMaxElongation
            // 
            this.nudMaxElongation.Location = new System.Drawing.Point(125, 114);
            this.nudMaxElongation.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.nudMaxElongation.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudMaxElongation.Name = "nudMaxElongation";
            this.nudMaxElongation.Size = new System.Drawing.Size(57, 20);
            this.nudMaxElongation.TabIndex = 49;
            this.nudMaxElongation.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 46;
            this.label3.Text = "Maximum FWHM:";
            // 
            // nudMaxFWHM
            // 
            this.nudMaxFWHM.DecimalPlaces = 1;
            this.nudMaxFWHM.Location = new System.Drawing.Point(125, 44);
            this.nudMaxFWHM.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.nudMaxFWHM.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudMaxFWHM.Name = "nudMaxFWHM";
            this.nudMaxFWHM.Size = new System.Drawing.Size(57, 20);
            this.nudMaxFWHM.TabIndex = 47;
            this.nudMaxFWHM.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 44;
            this.label1.Text = "Minimum FWHM:";
            // 
            // nudMinFWHM
            // 
            this.nudMinFWHM.DecimalPlaces = 1;
            this.nudMinFWHM.Location = new System.Drawing.Point(125, 18);
            this.nudMinFWHM.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudMinFWHM.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.nudMinFWHM.Name = "nudMinFWHM";
            this.nudMinFWHM.Size = new System.Drawing.Size(57, 20);
            this.nudMinFWHM.TabIndex = 45;
            this.nudMinFWHM.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // nudMinAboveMedian
            // 
            this.nudMinAboveMedian.Location = new System.Drawing.Point(229, 265);
            this.nudMinAboveMedian.Name = "nudMinAboveMedian";
            this.nudMinAboveMedian.Size = new System.Drawing.Size(47, 20);
            this.nudMinAboveMedian.TabIndex = 44;
            // 
            // nudMinSNRatio
            // 
            this.nudMinSNRatio.DecimalPlaces = 1;
            this.nudMinSNRatio.Location = new System.Drawing.Point(229, 291);
            this.nudMinSNRatio.Name = "nudMinSNRatio";
            this.nudMinSNRatio.Size = new System.Drawing.Size(47, 20);
            this.nudMinSNRatio.TabIndex = 42;
            // 
            // cbWarnOnUnsatisfiedGuidingRequirements
            // 
            this.cbWarnOnUnsatisfiedGuidingRequirements.Location = new System.Drawing.Point(25, 318);
            this.cbWarnOnUnsatisfiedGuidingRequirements.Name = "cbWarnOnUnsatisfiedGuidingRequirements";
            this.cbWarnOnUnsatisfiedGuidingRequirements.Size = new System.Drawing.Size(282, 19);
            this.cbWarnOnUnsatisfiedGuidingRequirements.TabIndex = 41;
            this.cbWarnOnUnsatisfiedGuidingRequirements.Text = "Warn on unsatisfied guiding star requirements";
            // 
            // ucTracking
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.groupControl1);
            this.Name = "ucTracking";
            this.Size = new System.Drawing.Size(473, 366);
            this.pnlRecoverySettings.ResumeLayout(false);
            this.pnlRecoverySettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbRecoveryTolerance)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudInterlacedCorrection)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            this.tabRecoveryTracker.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRefiningFrames)).EndInit();
            this.tabSimplifiedTracker.ResumeLayout(false);
            this.tabSimplifiedTracker.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDetectionCertainty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxElongation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxFWHM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinFWHM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinAboveMedian)).EndInit();
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
		private System.Windows.Forms.TabControl tabRecoveryTracker;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabSimplifiedTracker;
		private System.Windows.Forms.ComboBox cbxTrackingEngine;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown nudMaxFWHM;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown nudMinFWHM;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.NumericUpDown nudDetectionCertainty;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.CheckBox cbxTestPSFElongation;
		private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudMaxElongation;
	}
}

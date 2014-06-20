namespace Tangra.VideoTools
{
	partial class frmGenerateVideoModel
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGenerateVideoModel));
			this.btnGenerateVideo = new System.Windows.Forms.Button();
			this.pbar = new System.Windows.Forms.ProgressBar();
			this.pnlConfig = new System.Windows.Forms.Panel();
			this.label15 = new System.Windows.Forms.Label();
			this.nudGamma = new System.Windows.Forms.NumericUpDown();
			this.label14 = new System.Windows.Forms.Label();
			this.nudPassByMag2 = new System.Windows.Forms.NumericUpDown();
			this.label13 = new System.Windows.Forms.Label();
			this.nudPassByMag1 = new System.Windows.Forms.NumericUpDown();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.nudPassByDist = new System.Windows.Forms.NumericUpDown();
			this.cbClosePassBySim = new System.Windows.Forms.CheckBox();
			this.label10 = new System.Windows.Forms.Label();
			this.nudStarFlickering = new System.Windows.Forms.NumericUpDown();
			this.nudStar5Mag = new System.Windows.Forms.NumericUpDown();
			this.cbxStar5 = new System.Windows.Forms.CheckBox();
			this.nudStar4Mag = new System.Windows.Forms.NumericUpDown();
			this.cbxStar4 = new System.Windows.Forms.CheckBox();
			this.nudStar3Mag = new System.Windows.Forms.NumericUpDown();
			this.cbxStar3 = new System.Windows.Forms.CheckBox();
			this.nudStar2Mag = new System.Windows.Forms.NumericUpDown();
			this.cbxStar2 = new System.Windows.Forms.CheckBox();
			this.label9 = new System.Windows.Forms.Label();
			this.nudStar1FWHM = new System.Windows.Forms.NumericUpDown();
			this.label8 = new System.Windows.Forms.Label();
			this.nudStar1Intensity = new System.Windows.Forms.NumericUpDown();
			this.label7 = new System.Windows.Forms.Label();
			this.nudStar1Mag = new System.Windows.Forms.NumericUpDown();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.nudNoiseStdDev = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.nudNoiseMean = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.nudTotalFrames = new System.Windows.Forms.NumericUpDown();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.cbxPolyBackground = new System.Windows.Forms.CheckBox();
			this.label16 = new System.Windows.Forms.Label();
			this.nudPolyOrder = new System.Windows.Forms.NumericUpDown();
			this.label17 = new System.Windows.Forms.Label();
			this.nudPolyFreq = new System.Windows.Forms.NumericUpDown();
			this.label18 = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.label20 = new System.Windows.Forms.Label();
			this.nudPolyShift = new System.Windows.Forms.NumericUpDown();
			this.label21 = new System.Windows.Forms.Label();
			this.nudPolyDepth = new System.Windows.Forms.NumericUpDown();
			this.pnlConfig.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudGamma)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPassByMag2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPassByMag1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPassByDist)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudStarFlickering)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudStar5Mag)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudStar4Mag)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudStar3Mag)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudStar2Mag)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudStar1FWHM)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudStar1Intensity)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudStar1Mag)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudNoiseStdDev)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudNoiseMean)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudTotalFrames)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPolyOrder)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPolyFreq)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPolyShift)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPolyDepth)).BeginInit();
			this.SuspendLayout();
			// 
			// btnGenerateVideo
			// 
			this.btnGenerateVideo.Location = new System.Drawing.Point(110, 287);
			this.btnGenerateVideo.Name = "btnGenerateVideo";
			this.btnGenerateVideo.Size = new System.Drawing.Size(178, 23);
			this.btnGenerateVideo.TabIndex = 12;
			this.btnGenerateVideo.Text = "Generate Video";
			this.btnGenerateVideo.UseVisualStyleBackColor = true;
			this.btnGenerateVideo.Click += new System.EventHandler(this.btnGenerateVideo_Click);
			// 
			// pbar
			// 
			this.pbar.Location = new System.Drawing.Point(26, 316);
			this.pbar.Name = "pbar";
			this.pbar.Size = new System.Drawing.Size(368, 23);
			this.pbar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.pbar.TabIndex = 13;
			this.pbar.Visible = false;
			// 
			// pnlConfig
			// 
			this.pnlConfig.Controls.Add(this.label21);
			this.pnlConfig.Controls.Add(this.nudPolyDepth);
			this.pnlConfig.Controls.Add(this.label19);
			this.pnlConfig.Controls.Add(this.label20);
			this.pnlConfig.Controls.Add(this.nudPolyShift);
			this.pnlConfig.Controls.Add(this.label18);
			this.pnlConfig.Controls.Add(this.label17);
			this.pnlConfig.Controls.Add(this.nudPolyFreq);
			this.pnlConfig.Controls.Add(this.label16);
			this.pnlConfig.Controls.Add(this.nudPolyOrder);
			this.pnlConfig.Controls.Add(this.cbxPolyBackground);
			this.pnlConfig.Controls.Add(this.label15);
			this.pnlConfig.Controls.Add(this.nudGamma);
			this.pnlConfig.Controls.Add(this.label14);
			this.pnlConfig.Controls.Add(this.nudPassByMag2);
			this.pnlConfig.Controls.Add(this.label13);
			this.pnlConfig.Controls.Add(this.nudPassByMag1);
			this.pnlConfig.Controls.Add(this.label12);
			this.pnlConfig.Controls.Add(this.label11);
			this.pnlConfig.Controls.Add(this.nudPassByDist);
			this.pnlConfig.Controls.Add(this.cbClosePassBySim);
			this.pnlConfig.Controls.Add(this.label10);
			this.pnlConfig.Controls.Add(this.nudStarFlickering);
			this.pnlConfig.Controls.Add(this.nudStar5Mag);
			this.pnlConfig.Controls.Add(this.cbxStar5);
			this.pnlConfig.Controls.Add(this.nudStar4Mag);
			this.pnlConfig.Controls.Add(this.cbxStar4);
			this.pnlConfig.Controls.Add(this.nudStar3Mag);
			this.pnlConfig.Controls.Add(this.cbxStar3);
			this.pnlConfig.Controls.Add(this.nudStar2Mag);
			this.pnlConfig.Controls.Add(this.cbxStar2);
			this.pnlConfig.Controls.Add(this.label9);
			this.pnlConfig.Controls.Add(this.nudStar1FWHM);
			this.pnlConfig.Controls.Add(this.label8);
			this.pnlConfig.Controls.Add(this.nudStar1Intensity);
			this.pnlConfig.Controls.Add(this.label7);
			this.pnlConfig.Controls.Add(this.nudStar1Mag);
			this.pnlConfig.Controls.Add(this.label6);
			this.pnlConfig.Controls.Add(this.label5);
			this.pnlConfig.Controls.Add(this.nudNoiseStdDev);
			this.pnlConfig.Controls.Add(this.label4);
			this.pnlConfig.Controls.Add(this.nudNoiseMean);
			this.pnlConfig.Controls.Add(this.label3);
			this.pnlConfig.Controls.Add(this.label2);
			this.pnlConfig.Controls.Add(this.label1);
			this.pnlConfig.Controls.Add(this.nudTotalFrames);
			this.pnlConfig.Location = new System.Drawing.Point(2, 2);
			this.pnlConfig.Name = "pnlConfig";
			this.pnlConfig.Size = new System.Drawing.Size(403, 279);
			this.pnlConfig.TabIndex = 14;
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(296, 11);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(43, 13);
			this.label15.TabIndex = 34;
			this.label15.Text = "Gamma";
			// 
			// nudGamma
			// 
			this.nudGamma.DecimalPlaces = 2;
			this.nudGamma.Location = new System.Drawing.Point(340, 9);
			this.nudGamma.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.nudGamma.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            131072});
			this.nudGamma.Name = "nudGamma";
			this.nudGamma.Size = new System.Drawing.Size(52, 20);
			this.nudGamma.TabIndex = 33;
			this.nudGamma.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(239, 258);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(37, 13);
			this.label14.TabIndex = 32;
			this.label14.Text = "Mag 2";
			// 
			// nudPassByMag2
			// 
			this.nudPassByMag2.DecimalPlaces = 2;
			this.nudPassByMag2.Location = new System.Drawing.Point(282, 254);
			this.nudPassByMag2.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
			this.nudPassByMag2.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.nudPassByMag2.Name = "nudPassByMag2";
			this.nudPassByMag2.Size = new System.Drawing.Size(52, 20);
			this.nudPassByMag2.TabIndex = 31;
			this.nudPassByMag2.Value = new decimal(new int[] {
            1327,
            0,
            0,
            131072});
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(239, 232);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(37, 13);
			this.label13.TabIndex = 30;
			this.label13.Text = "Mag 1";
			// 
			// nudPassByMag1
			// 
			this.nudPassByMag1.DecimalPlaces = 2;
			this.nudPassByMag1.Location = new System.Drawing.Point(282, 228);
			this.nudPassByMag1.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
			this.nudPassByMag1.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.nudPassByMag1.Name = "nudPassByMag1";
			this.nudPassByMag1.Size = new System.Drawing.Size(52, 20);
			this.nudPassByMag1.TabIndex = 29;
			this.nudPassByMag1.Value = new decimal(new int[] {
            1265,
            0,
            0,
            131072});
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(327, 206);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(20, 13);
			this.label12.TabIndex = 28;
			this.label12.Text = "pix";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(216, 205);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(62, 13);
			this.label11.TabIndex = 27;
			this.label11.Text = "Closest Dist";
			// 
			// nudPassByDist
			// 
			this.nudPassByDist.DecimalPlaces = 1;
			this.nudPassByDist.Location = new System.Drawing.Point(282, 202);
			this.nudPassByDist.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
			this.nudPassByDist.Name = "nudPassByDist";
			this.nudPassByDist.Size = new System.Drawing.Size(41, 20);
			this.nudPassByDist.TabIndex = 26;
			this.nudPassByDist.Value = new decimal(new int[] {
            35,
            0,
            0,
            65536});
			// 
			// cbClosePassBySim
			// 
			this.cbClosePassBySim.AutoSize = true;
			this.cbClosePassBySim.Checked = true;
			this.cbClosePassBySim.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbClosePassBySim.Location = new System.Drawing.Point(180, 174);
			this.cbClosePassBySim.Name = "cbClosePassBySim";
			this.cbClosePassBySim.Size = new System.Drawing.Size(143, 17);
			this.cbClosePassBySim.TabIndex = 25;
			this.cbClosePassBySim.Text = "Close Pass-by Simulation";
			this.cbClosePassBySim.UseVisualStyleBackColor = true;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(234, 44);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(113, 13);
			this.label10.TabIndex = 24;
			this.label10.Text = "Star Flickering StdDev";
			// 
			// nudStarFlickering
			// 
			this.nudStarFlickering.Location = new System.Drawing.Point(349, 40);
			this.nudStarFlickering.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
			this.nudStarFlickering.Name = "nudStarFlickering";
			this.nudStarFlickering.Size = new System.Drawing.Size(41, 20);
			this.nudStarFlickering.TabIndex = 23;
			// 
			// nudStar5Mag
			// 
			this.nudStar5Mag.DecimalPlaces = 2;
			this.nudStar5Mag.Location = new System.Drawing.Point(99, 254);
			this.nudStar5Mag.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
			this.nudStar5Mag.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.nudStar5Mag.Name = "nudStar5Mag";
			this.nudStar5Mag.Size = new System.Drawing.Size(52, 20);
			this.nudStar5Mag.TabIndex = 22;
			this.nudStar5Mag.Value = new decimal(new int[] {
            141,
            0,
            0,
            65536});
			// 
			// cbxStar5
			// 
			this.cbxStar5.AutoSize = true;
			this.cbxStar5.Checked = true;
			this.cbxStar5.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbxStar5.Location = new System.Drawing.Point(15, 257);
			this.cbxStar5.Name = "cbxStar5";
			this.cbxStar5.Size = new System.Drawing.Size(78, 17);
			this.cbxStar5.TabIndex = 21;
			this.cbxStar5.Text = "Star 5 Mag";
			this.cbxStar5.UseVisualStyleBackColor = true;
			// 
			// nudStar4Mag
			// 
			this.nudStar4Mag.DecimalPlaces = 2;
			this.nudStar4Mag.Location = new System.Drawing.Point(99, 225);
			this.nudStar4Mag.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
			this.nudStar4Mag.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.nudStar4Mag.Name = "nudStar4Mag";
			this.nudStar4Mag.Size = new System.Drawing.Size(52, 20);
			this.nudStar4Mag.TabIndex = 20;
			this.nudStar4Mag.Value = new decimal(new int[] {
            135,
            0,
            0,
            65536});
			// 
			// cbxStar4
			// 
			this.cbxStar4.AutoSize = true;
			this.cbxStar4.Checked = true;
			this.cbxStar4.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbxStar4.Location = new System.Drawing.Point(15, 228);
			this.cbxStar4.Name = "cbxStar4";
			this.cbxStar4.Size = new System.Drawing.Size(78, 17);
			this.cbxStar4.TabIndex = 19;
			this.cbxStar4.Text = "Star 4 Mag";
			this.cbxStar4.UseVisualStyleBackColor = true;
			// 
			// nudStar3Mag
			// 
			this.nudStar3Mag.DecimalPlaces = 2;
			this.nudStar3Mag.Location = new System.Drawing.Point(99, 199);
			this.nudStar3Mag.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
			this.nudStar3Mag.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.nudStar3Mag.Name = "nudStar3Mag";
			this.nudStar3Mag.Size = new System.Drawing.Size(52, 20);
			this.nudStar3Mag.TabIndex = 18;
			this.nudStar3Mag.Value = new decimal(new int[] {
            115,
            0,
            0,
            65536});
			// 
			// cbxStar3
			// 
			this.cbxStar3.AutoSize = true;
			this.cbxStar3.Checked = true;
			this.cbxStar3.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbxStar3.Location = new System.Drawing.Point(15, 202);
			this.cbxStar3.Name = "cbxStar3";
			this.cbxStar3.Size = new System.Drawing.Size(78, 17);
			this.cbxStar3.TabIndex = 17;
			this.cbxStar3.Text = "Star 3 Mag";
			this.cbxStar3.UseVisualStyleBackColor = true;
			// 
			// nudStar2Mag
			// 
			this.nudStar2Mag.DecimalPlaces = 2;
			this.nudStar2Mag.Location = new System.Drawing.Point(99, 173);
			this.nudStar2Mag.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
			this.nudStar2Mag.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.nudStar2Mag.Name = "nudStar2Mag";
			this.nudStar2Mag.Size = new System.Drawing.Size(52, 20);
			this.nudStar2Mag.TabIndex = 16;
			this.nudStar2Mag.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// cbxStar2
			// 
			this.cbxStar2.AutoSize = true;
			this.cbxStar2.Checked = true;
			this.cbxStar2.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbxStar2.Location = new System.Drawing.Point(15, 176);
			this.cbxStar2.Name = "cbxStar2";
			this.cbxStar2.Size = new System.Drawing.Size(78, 17);
			this.cbxStar2.TabIndex = 15;
			this.cbxStar2.Text = "Star 2 Mag";
			this.cbxStar2.UseVisualStyleBackColor = true;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(296, 82);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(41, 13);
			this.label9.TabIndex = 14;
			this.label9.Text = "FWHM";
			// 
			// nudStar1FWHM
			// 
			this.nudStar1FWHM.DecimalPlaces = 1;
			this.nudStar1FWHM.Location = new System.Drawing.Point(340, 79);
			this.nudStar1FWHM.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
			this.nudStar1FWHM.Name = "nudStar1FWHM";
			this.nudStar1FWHM.Size = new System.Drawing.Size(41, 20);
			this.nudStar1FWHM.TabIndex = 13;
			this.nudStar1FWHM.Value = new decimal(new int[] {
            35,
            0,
            0,
            65536});
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(197, 82);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(46, 13);
			this.label8.TabIndex = 12;
			this.label8.Text = "Intensity";
			// 
			// nudStar1Intensity
			// 
			this.nudStar1Intensity.Location = new System.Drawing.Point(244, 79);
			this.nudStar1Intensity.Maximum = new decimal(new int[] {
            250,
            0,
            0,
            0});
			this.nudStar1Intensity.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.nudStar1Intensity.Name = "nudStar1Intensity";
			this.nudStar1Intensity.Size = new System.Drawing.Size(41, 20);
			this.nudStar1Intensity.TabIndex = 11;
			this.nudStar1Intensity.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(105, 82);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(28, 13);
			this.label7.TabIndex = 10;
			this.label7.Text = "Mag";
			// 
			// nudStar1Mag
			// 
			this.nudStar1Mag.DecimalPlaces = 2;
			this.nudStar1Mag.Location = new System.Drawing.Point(135, 79);
			this.nudStar1Mag.Maximum = new decimal(new int[] {
            13,
            0,
            0,
            0});
			this.nudStar1Mag.Minimum = new decimal(new int[] {
            11,
            0,
            0,
            0});
			this.nudStar1Mag.Name = "nudStar1Mag";
			this.nudStar1Mag.Size = new System.Drawing.Size(52, 20);
			this.nudStar1Mag.TabIndex = 9;
			this.nudStar1Mag.Value = new decimal(new int[] {
            1200,
            0,
            0,
            131072});
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(12, 82);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(93, 13);
			this.label6.TabIndex = 8;
			this.label6.Text = "Calibration Star 1: ";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(136, 44);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(43, 13);
			this.label5.TabIndex = 7;
			this.label5.Text = "StdDev";
			// 
			// nudNoiseStdDev
			// 
			this.nudNoiseStdDev.Location = new System.Drawing.Point(180, 40);
			this.nudNoiseStdDev.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.nudNoiseStdDev.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudNoiseStdDev.Name = "nudNoiseStdDev";
			this.nudNoiseStdDev.Size = new System.Drawing.Size(41, 20);
			this.nudNoiseStdDev.TabIndex = 6;
			this.nudNoiseStdDev.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(47, 44);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(34, 13);
			this.label4.TabIndex = 5;
			this.label4.Text = "Mean";
			// 
			// nudNoiseMean
			// 
			this.nudNoiseMean.Location = new System.Drawing.Point(84, 41);
			this.nudNoiseMean.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.nudNoiseMean.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudNoiseMean.Name = "nudNoiseMean";
			this.nudNoiseMean.Size = new System.Drawing.Size(41, 20);
			this.nudNoiseMean.TabIndex = 4;
			this.nudNoiseMean.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(9, 44);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(40, 13);
			this.label3.TabIndex = 3;
			this.label3.Text = "Noise: ";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(169, 11);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(54, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "(at 25 fps)";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 11);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(93, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Number of frames:";
			// 
			// nudTotalFrames
			// 
			this.nudTotalFrames.Location = new System.Drawing.Point(108, 9);
			this.nudTotalFrames.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
			this.nudTotalFrames.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudTotalFrames.Name = "nudTotalFrames";
			this.nudTotalFrames.Size = new System.Drawing.Size(55, 20);
			this.nudTotalFrames.TabIndex = 0;
			this.nudTotalFrames.Value = new decimal(new int[] {
            4000,
            0,
            0,
            0});
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.DefaultExt = "avi";
			this.saveFileDialog.Filter = "AVI Files (*.avi)|*.avi";
			// 
			// cbxPolyBackground
			// 
			this.cbxPolyBackground.AutoSize = true;
			this.cbxPolyBackground.Checked = true;
			this.cbxPolyBackground.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbxPolyBackground.Location = new System.Drawing.Point(15, 115);
			this.cbxPolyBackground.Name = "cbxPolyBackground";
			this.cbxPolyBackground.Size = new System.Drawing.Size(192, 17);
			this.cbxPolyBackground.TabIndex = 35;
			this.cbxPolyBackground.Text = "Moving 3D-Polynomial Background";
			this.cbxPolyBackground.UseVisualStyleBackColor = true;
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(212, 115);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(33, 13);
			this.label16.TabIndex = 37;
			this.label16.Text = "Order";
			// 
			// nudPolyOrder
			// 
			this.nudPolyOrder.Location = new System.Drawing.Point(251, 111);
			this.nudPolyOrder.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
			this.nudPolyOrder.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudPolyOrder.Name = "nudPolyOrder";
			this.nudPolyOrder.Size = new System.Drawing.Size(34, 20);
			this.nudPolyOrder.TabIndex = 36;
			this.nudPolyOrder.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Location = new System.Drawing.Point(36, 141);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(57, 13);
			this.label17.TabIndex = 39;
			this.label17.Text = "Frequency";
			// 
			// nudPolyFreq
			// 
			this.nudPolyFreq.Location = new System.Drawing.Point(95, 138);
			this.nudPolyFreq.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.nudPolyFreq.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
			this.nudPolyFreq.Name = "nudPolyFreq";
			this.nudPolyFreq.Size = new System.Drawing.Size(41, 20);
			this.nudPolyFreq.TabIndex = 38;
			this.nudPolyFreq.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
			// 
			// label18
			// 
			this.label18.AutoSize = true;
			this.label18.Location = new System.Drawing.Point(142, 141);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(38, 13);
			this.label18.TabIndex = 40;
			this.label18.Text = "frames";
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.Location = new System.Drawing.Point(264, 141);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(20, 13);
			this.label19.TabIndex = 43;
			this.label19.Text = "pix";
			// 
			// label20
			// 
			this.label20.AutoSize = true;
			this.label20.Location = new System.Drawing.Point(187, 141);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(28, 13);
			this.label20.TabIndex = 42;
			this.label20.Text = "Shift";
			// 
			// nudPolyShift
			// 
			this.nudPolyShift.DecimalPlaces = 1;
			this.nudPolyShift.Location = new System.Drawing.Point(219, 137);
			this.nudPolyShift.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
			this.nudPolyShift.Name = "nudPolyShift";
			this.nudPolyShift.Size = new System.Drawing.Size(41, 20);
			this.nudPolyShift.TabIndex = 41;
			this.nudPolyShift.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
			// 
			// label21
			// 
			this.label21.AutoSize = true;
			this.label21.Location = new System.Drawing.Point(298, 141);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(36, 13);
			this.label21.TabIndex = 45;
			this.label21.Text = "Depth";
			// 
			// nudPolyDepth
			// 
			this.nudPolyDepth.DecimalPlaces = 1;
			this.nudPolyDepth.Location = new System.Drawing.Point(340, 138);
			this.nudPolyDepth.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.nudPolyDepth.Name = "nudPolyDepth";
			this.nudPolyDepth.Size = new System.Drawing.Size(50, 20);
			this.nudPolyDepth.TabIndex = 44;
			this.nudPolyDepth.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
			// 
			// frmGenerateVideoModel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(417, 351);
			this.Controls.Add(this.pnlConfig);
			this.Controls.Add(this.pbar);
			this.Controls.Add(this.btnGenerateVideo);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmGenerateVideoModel";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Controlled Model Video Generation";
			this.pnlConfig.ResumeLayout(false);
			this.pnlConfig.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudGamma)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPassByMag2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPassByMag1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPassByDist)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudStarFlickering)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudStar5Mag)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudStar4Mag)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudStar3Mag)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudStar2Mag)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudStar1FWHM)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudStar1Intensity)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudStar1Mag)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudNoiseStdDev)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudNoiseMean)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudTotalFrames)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPolyOrder)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPolyFreq)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPolyShift)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPolyDepth)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnGenerateVideo;
		private System.Windows.Forms.ProgressBar pbar;
		private System.Windows.Forms.Panel pnlConfig;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown nudTotalFrames;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown nudNoiseMean;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown nudNoiseStdDev;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.NumericUpDown nudStar1FWHM;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.NumericUpDown nudStar1Intensity;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown nudStar1Mag;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown nudStar2Mag;
		private System.Windows.Forms.CheckBox cbxStar2;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.NumericUpDown nudStarFlickering;
		private System.Windows.Forms.NumericUpDown nudStar5Mag;
		private System.Windows.Forms.CheckBox cbxStar5;
		private System.Windows.Forms.NumericUpDown nudStar4Mag;
		private System.Windows.Forms.CheckBox cbxStar4;
		private System.Windows.Forms.NumericUpDown nudStar3Mag;
		private System.Windows.Forms.CheckBox cbxStar3;
		private System.Windows.Forms.CheckBox cbClosePassBySim;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.NumericUpDown nudPassByDist;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.NumericUpDown nudPassByMag2;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.NumericUpDown nudPassByMag1;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.NumericUpDown nudGamma;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.NumericUpDown nudPolyOrder;
		private System.Windows.Forms.CheckBox cbxPolyBackground;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.NumericUpDown nudPolyDepth;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.NumericUpDown nudPolyShift;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.NumericUpDown nudPolyFreq;
	}
}
namespace Tangra.VideoTools
{
    partial class frmGenerateStarFieldVideoModel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGenerateStarFieldVideoModel));
            this.pbar = new System.Windows.Forms.ProgressBar();
            this.btnGenerateVideo = new System.Windows.Forms.Button();
            this.pnlConfig = new System.Windows.Forms.Panel();
            this.label26 = new System.Windows.Forms.Label();
            this.nudFrameHeight = new System.Windows.Forms.NumericUpDown();
            this.lblWidth = new System.Windows.Forms.Label();
            this.nudFrameWidth = new System.Windows.Forms.NumericUpDown();
            this.nudPlateFocLength = new System.Windows.Forms.NumericUpDown();
            this.nudPlatePixHeight = new System.Windows.Forms.NumericUpDown();
            this.nudPlatePixWidth = new System.Windows.Forms.NumericUpDown();
            this.label19 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.nudFirstSaturatedMag = new System.Windows.Forms.NumericUpDown();
            this.nudNonLinearity = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.nudBVSlope = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.cbxPhotometricFilter = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tbxDE = new System.Windows.Forms.TextBox();
            this.tbxRA = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cbxAAVIntegration = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbxVideoFormat = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.nudStarFlickering = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.nudStar1FWHM = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.nudNoiseStdDev = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.nudNoiseMean = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.nudTotalFrames = new System.Windows.Forms.NumericUpDown();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.pnlConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrameHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrameWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlateFocLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlatePixHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlatePixWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFirstSaturatedMag)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNonLinearity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBVSlope)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStarFlickering)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStar1FWHM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNoiseStdDev)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNoiseMean)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTotalFrames)).BeginInit();
            this.SuspendLayout();
            // 
            // pbar
            // 
            this.pbar.Location = new System.Drawing.Point(27, 368);
            this.pbar.Name = "pbar";
            this.pbar.Size = new System.Drawing.Size(368, 23);
            this.pbar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbar.TabIndex = 15;
            this.pbar.Visible = false;
            // 
            // btnGenerateVideo
            // 
            this.btnGenerateVideo.Location = new System.Drawing.Point(111, 336);
            this.btnGenerateVideo.Name = "btnGenerateVideo";
            this.btnGenerateVideo.Size = new System.Drawing.Size(178, 23);
            this.btnGenerateVideo.TabIndex = 14;
            this.btnGenerateVideo.Text = "Generate Video";
            this.btnGenerateVideo.UseVisualStyleBackColor = true;
            this.btnGenerateVideo.Click += new System.EventHandler(this.btnGenerateVideo_Click);
            // 
            // pnlConfig
            // 
            this.pnlConfig.Controls.Add(this.label26);
            this.pnlConfig.Controls.Add(this.nudFrameHeight);
            this.pnlConfig.Controls.Add(this.lblWidth);
            this.pnlConfig.Controls.Add(this.nudFrameWidth);
            this.pnlConfig.Controls.Add(this.nudPlateFocLength);
            this.pnlConfig.Controls.Add(this.nudPlatePixHeight);
            this.pnlConfig.Controls.Add(this.nudPlatePixWidth);
            this.pnlConfig.Controls.Add(this.label19);
            this.pnlConfig.Controls.Add(this.label22);
            this.pnlConfig.Controls.Add(this.label20);
            this.pnlConfig.Controls.Add(this.label24);
            this.pnlConfig.Controls.Add(this.label23);
            this.pnlConfig.Controls.Add(this.label17);
            this.pnlConfig.Controls.Add(this.label21);
            this.pnlConfig.Controls.Add(this.label16);
            this.pnlConfig.Controls.Add(this.label18);
            this.pnlConfig.Controls.Add(this.nudFirstSaturatedMag);
            this.pnlConfig.Controls.Add(this.nudNonLinearity);
            this.pnlConfig.Controls.Add(this.label14);
            this.pnlConfig.Controls.Add(this.nudBVSlope);
            this.pnlConfig.Controls.Add(this.label13);
            this.pnlConfig.Controls.Add(this.cbxPhotometricFilter);
            this.pnlConfig.Controls.Add(this.label12);
            this.pnlConfig.Controls.Add(this.tbxDE);
            this.pnlConfig.Controls.Add(this.tbxRA);
            this.pnlConfig.Controls.Add(this.label11);
            this.pnlConfig.Controls.Add(this.label8);
            this.pnlConfig.Controls.Add(this.label7);
            this.pnlConfig.Controls.Add(this.label6);
            this.pnlConfig.Controls.Add(this.cbxAAVIntegration);
            this.pnlConfig.Controls.Add(this.label2);
            this.pnlConfig.Controls.Add(this.cbxVideoFormat);
            this.pnlConfig.Controls.Add(this.label15);
            this.pnlConfig.Controls.Add(this.label10);
            this.pnlConfig.Controls.Add(this.nudStarFlickering);
            this.pnlConfig.Controls.Add(this.label9);
            this.pnlConfig.Controls.Add(this.nudStar1FWHM);
            this.pnlConfig.Controls.Add(this.label5);
            this.pnlConfig.Controls.Add(this.nudNoiseStdDev);
            this.pnlConfig.Controls.Add(this.label4);
            this.pnlConfig.Controls.Add(this.nudNoiseMean);
            this.pnlConfig.Controls.Add(this.label3);
            this.pnlConfig.Controls.Add(this.label1);
            this.pnlConfig.Controls.Add(this.nudTotalFrames);
            this.pnlConfig.Location = new System.Drawing.Point(12, 12);
            this.pnlConfig.Name = "pnlConfig";
            this.pnlConfig.Size = new System.Drawing.Size(401, 318);
            this.pnlConfig.TabIndex = 16;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(304, 7);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(38, 13);
            this.label26.TabIndex = 84;
            this.label26.Text = "Height";
            // 
            // nudFrameHeight
            // 
            this.nudFrameHeight.Location = new System.Drawing.Point(344, 3);
            this.nudFrameHeight.Maximum = new decimal(new int[] {
            5012,
            0,
            0,
            0});
            this.nudFrameHeight.Name = "nudFrameHeight";
            this.nudFrameHeight.Size = new System.Drawing.Size(49, 20);
            this.nudFrameHeight.TabIndex = 83;
            this.nudFrameHeight.Value = new decimal(new int[] {
            576,
            0,
            0,
            0});
            // 
            // lblWidth
            // 
            this.lblWidth.AutoSize = true;
            this.lblWidth.Location = new System.Drawing.Point(190, 7);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Size = new System.Drawing.Size(35, 13);
            this.lblWidth.TabIndex = 82;
            this.lblWidth.Text = "Width";
            // 
            // nudFrameWidth
            // 
            this.nudFrameWidth.Location = new System.Drawing.Point(234, 3);
            this.nudFrameWidth.Maximum = new decimal(new int[] {
            5012,
            0,
            0,
            0});
            this.nudFrameWidth.Name = "nudFrameWidth";
            this.nudFrameWidth.Size = new System.Drawing.Size(49, 20);
            this.nudFrameWidth.TabIndex = 81;
            this.nudFrameWidth.Value = new decimal(new int[] {
            720,
            0,
            0,
            0});
            // 
            // nudPlateFocLength
            // 
            this.nudPlateFocLength.DecimalPlaces = 1;
            this.nudPlateFocLength.Location = new System.Drawing.Point(303, 200);
            this.nudPlateFocLength.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudPlateFocLength.Name = "nudPlateFocLength";
            this.nudPlateFocLength.Size = new System.Drawing.Size(63, 20);
            this.nudPlateFocLength.TabIndex = 80;
            this.nudPlateFocLength.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // nudPlatePixHeight
            // 
            this.nudPlatePixHeight.DecimalPlaces = 3;
            this.nudPlatePixHeight.Location = new System.Drawing.Point(204, 199);
            this.nudPlatePixHeight.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudPlatePixHeight.Name = "nudPlatePixHeight";
            this.nudPlatePixHeight.Size = new System.Drawing.Size(58, 20);
            this.nudPlatePixHeight.TabIndex = 79;
            this.nudPlatePixHeight.Value = new decimal(new int[] {
            6224,
            0,
            0,
            196608});
            // 
            // nudPlatePixWidth
            // 
            this.nudPlatePixWidth.DecimalPlaces = 3;
            this.nudPlatePixWidth.Location = new System.Drawing.Point(102, 200);
            this.nudPlatePixWidth.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudPlatePixWidth.Name = "nudPlatePixWidth";
            this.nudPlatePixWidth.Size = new System.Drawing.Size(60, 20);
            this.nudPlatePixWidth.TabIndex = 78;
            this.nudPlatePixWidth.Value = new decimal(new int[] {
            6773,
            0,
            0,
            196608});
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(300, 183);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(69, 13);
            this.label19.TabIndex = 73;
            this.label19.Text = "Focal Length";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.label22.Location = new System.Drawing.Point(263, 199);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(13, 13);
            this.label22.TabIndex = 77;
            this.label22.Text = "m";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(368, 202);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(23, 13);
            this.label20.TabIndex = 74;
            this.label20.Text = "mm";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(273, 199);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(15, 13);
            this.label24.TabIndex = 76;
            this.label24.Text = "m";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.label23.Location = new System.Drawing.Point(164, 202);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(13, 13);
            this.label23.TabIndex = 75;
            this.label23.Text = "m";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(174, 202);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(15, 13);
            this.label17.TabIndex = 70;
            this.label17.Text = "m";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(204, 184);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(55, 13);
            this.label21.TabIndex = 69;
            this.label21.Text = "Pix Height";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(144, 103);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(150, 13);
            this.label16.TabIndex = 67;
            this.label16.Text = "Saturation starts at magnitude:";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(99, 184);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(60, 13);
            this.label18.TabIndex = 67;
            this.label18.Text = "Pixel Width";
            // 
            // nudFirstSaturatedMag
            // 
            this.nudFirstSaturatedMag.DecimalPlaces = 2;
            this.nudFirstSaturatedMag.Location = new System.Drawing.Point(300, 100);
            this.nudFirstSaturatedMag.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.nudFirstSaturatedMag.Name = "nudFirstSaturatedMag";
            this.nudFirstSaturatedMag.Size = new System.Drawing.Size(52, 20);
            this.nudFirstSaturatedMag.TabIndex = 66;
            this.nudFirstSaturatedMag.Value = new decimal(new int[] {
            1200,
            0,
            0,
            131072});
            // 
            // nudNonLinearity
            // 
            this.nudNonLinearity.DecimalPlaces = 4;
            this.nudNonLinearity.Location = new System.Drawing.Point(136, 284);
            this.nudNonLinearity.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudNonLinearity.Name = "nudNonLinearity";
            this.nudNonLinearity.Size = new System.Drawing.Size(61, 20);
            this.nudNonLinearity.TabIndex = 65;
            this.nudNonLinearity.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(9, 286);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(118, 13);
            this.label14.TabIndex = 64;
            this.label14.Text = "Modelled Non-Linearity:";
            // 
            // nudBVSlope
            // 
            this.nudBVSlope.DecimalPlaces = 4;
            this.nudBVSlope.Location = new System.Drawing.Point(136, 257);
            this.nudBVSlope.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudBVSlope.Name = "nudBVSlope";
            this.nudBVSlope.Size = new System.Drawing.Size(61, 20);
            this.nudBVSlope.TabIndex = 63;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(21, 260);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(109, 13);
            this.label13.TabIndex = 62;
            this.label13.Text = "Modelled (B-V) Slope:";
            // 
            // cbxPhotometricFilter
            // 
            this.cbxPhotometricFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPhotometricFilter.FormattingEnabled = true;
            this.cbxPhotometricFilter.Items.AddRange(new object[] {
            "B",
            "V",
            "g\'",
            "r\'",
            "i\'"});
            this.cbxPhotometricFilter.Location = new System.Drawing.Point(331, 256);
            this.cbxPhotometricFilter.Name = "cbxPhotometricFilter";
            this.cbxPhotometricFilter.Size = new System.Drawing.Size(62, 21);
            this.cbxPhotometricFilter.TabIndex = 61;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(236, 259);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(91, 13);
            this.label12.TabIndex = 60;
            this.label12.Text = "Photometric Filter:";
            // 
            // tbxDE
            // 
            this.tbxDE.Location = new System.Drawing.Point(267, 140);
            this.tbxDE.Name = "tbxDE";
            this.tbxDE.Size = new System.Drawing.Size(100, 20);
            this.tbxDE.TabIndex = 59;
            this.tbxDE.Text = "-19 44 05";
            // 
            // tbxRA
            // 
            this.tbxRA.Location = new System.Drawing.Point(135, 140);
            this.tbxRA.Name = "tbxRA";
            this.tbxRA.Size = new System.Drawing.Size(100, 20);
            this.tbxRA.TabIndex = 58;
            this.tbxRA.Text = "18 54 34";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(243, 143);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(22, 13);
            this.label11.TabIndex = 57;
            this.label11.Text = "DE";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(112, 143);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(22, 13);
            this.label8.TabIndex = 56;
            this.label8.Text = "RA";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 143);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(104, 13);
            this.label7.TabIndex = 55;
            this.label7.Text = "Field Center (J2000):";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 184);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 13);
            this.label6.TabIndex = 53;
            this.label6.Text = "Plate Constants:";
            // 
            // cbxAAVIntegration
            // 
            this.cbxAAVIntegration.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxAAVIntegration.FormattingEnabled = true;
            this.cbxAAVIntegration.Items.AddRange(new object[] {
            "x1",
            "x2",
            "x4",
            "x8",
            "x16",
            "x32",
            "x64",
            "x128",
            "x256"});
            this.cbxAAVIntegration.Location = new System.Drawing.Point(265, 32);
            this.cbxAAVIntegration.Name = "cbxAAVIntegration";
            this.cbxAAVIntegration.Size = new System.Drawing.Size(62, 21);
            this.cbxAAVIntegration.TabIndex = 52;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(209, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 51;
            this.label2.Text = "Integration";
            // 
            // cbxVideoFormat
            // 
            this.cbxVideoFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxVideoFormat.FormattingEnabled = true;
            this.cbxVideoFormat.Items.AddRange(new object[] {
            "AAV",
            "AVI (25 fps)"});
            this.cbxVideoFormat.Location = new System.Drawing.Point(100, 32);
            this.cbxVideoFormat.Name = "cbxVideoFormat";
            this.cbxVideoFormat.Size = new System.Drawing.Size(97, 21);
            this.cbxVideoFormat.TabIndex = 49;
            this.cbxVideoFormat.SelectedIndexChanged += new System.EventHandler(this.cbxVideoFormat_SelectedIndexChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(25, 35);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(69, 13);
            this.label15.TabIndex = 48;
            this.label15.Text = "Video format:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(239, 77);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(113, 13);
            this.label10.TabIndex = 46;
            this.label10.Text = "Star Flickering StdDev";
            // 
            // nudStarFlickering
            // 
            this.nudStarFlickering.Location = new System.Drawing.Point(354, 73);
            this.nudStarFlickering.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.nudStarFlickering.Name = "nudStarFlickering";
            this.nudStarFlickering.Size = new System.Drawing.Size(41, 20);
            this.nudStarFlickering.TabIndex = 45;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(45, 103);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 13);
            this.label9.TabIndex = 44;
            this.label9.Text = "FWHM";
            // 
            // nudStar1FWHM
            // 
            this.nudStar1FWHM.DecimalPlaces = 1;
            this.nudStar1FWHM.Location = new System.Drawing.Point(89, 100);
            this.nudStar1FWHM.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.nudStar1FWHM.Name = "nudStar1FWHM";
            this.nudStar1FWHM.Size = new System.Drawing.Size(41, 20);
            this.nudStar1FWHM.TabIndex = 43;
            this.nudStar1FWHM.Value = new decimal(new int[] {
            35,
            0,
            0,
            65536});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(141, 77);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 42;
            this.label5.Text = "StdDev";
            // 
            // nudNoiseStdDev
            // 
            this.nudNoiseStdDev.Location = new System.Drawing.Point(185, 73);
            this.nudNoiseStdDev.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudNoiseStdDev.Name = "nudNoiseStdDev";
            this.nudNoiseStdDev.Size = new System.Drawing.Size(41, 20);
            this.nudNoiseStdDev.TabIndex = 41;
            this.nudNoiseStdDev.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(52, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 40;
            this.label4.Text = "Mean";
            // 
            // nudNoiseMean
            // 
            this.nudNoiseMean.Location = new System.Drawing.Point(89, 74);
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
            this.nudNoiseMean.TabIndex = 39;
            this.nudNoiseMean.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 38;
            this.label3.Text = "Noise: ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 36;
            this.label1.Text = "Number of frames:";
            // 
            // nudTotalFrames
            // 
            this.nudTotalFrames.Location = new System.Drawing.Point(102, 6);
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
            this.nudTotalFrames.TabIndex = 35;
            this.nudTotalFrames.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // frmGenerateStarFieldVideoModel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 408);
            this.Controls.Add(this.pnlConfig);
            this.Controls.Add(this.pbar);
            this.Controls.Add(this.btnGenerateVideo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmGenerateStarFieldVideoModel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Controlled Star Field Model Video Generation";
            this.pnlConfig.ResumeLayout(false);
            this.pnlConfig.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrameHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrameWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlateFocLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlatePixHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlatePixWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFirstSaturatedMag)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNonLinearity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBVSlope)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStarFlickering)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStar1FWHM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNoiseStdDev)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNoiseMean)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTotalFrames)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar pbar;
        private System.Windows.Forms.Button btnGenerateVideo;
        private System.Windows.Forms.Panel pnlConfig;
        private System.Windows.Forms.ComboBox cbxAAVIntegration;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbxVideoFormat;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown nudStarFlickering;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown nudStar1FWHM;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudNoiseStdDev;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudNoiseMean;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudTotalFrames;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.NumericUpDown nudFirstSaturatedMag;
        private System.Windows.Forms.NumericUpDown nudNonLinearity;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown nudBVSlope;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cbxPhotometricFilter;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox tbxDE;
        private System.Windows.Forms.TextBox tbxRA;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.NumericUpDown nudFrameHeight;
        private System.Windows.Forms.Label lblWidth;
        private System.Windows.Forms.NumericUpDown nudFrameWidth;
        private System.Windows.Forms.NumericUpDown nudPlateFocLength;
        private System.Windows.Forms.NumericUpDown nudPlatePixHeight;
        private System.Windows.Forms.NumericUpDown nudPlatePixWidth;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}
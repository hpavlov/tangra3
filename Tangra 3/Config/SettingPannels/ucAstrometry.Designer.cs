namespace Tangra.Config.SettingPannels
{
	partial class ucAstrometry
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
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label62 = new System.Windows.Forms.Label();
            this.cbxAstrFitMethod = new System.Windows.Forms.ComboBox();
            this.nudAstrMaximumStars = new System.Windows.Forms.NumericUpDown();
            this.label49 = new System.Windows.Forms.Label();
            this.label48 = new System.Windows.Forms.Label();
            this.nudAstrMinimumStars = new System.Windows.Forms.NumericUpDown();
            this.label51 = new System.Windows.Forms.Label();
            this.nudAstrMaxResidual = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.nudAstrAssumedUncertainty = new System.Windows.Forms.NumericUpDown();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nudNumberOfPivots = new System.Windows.Forms.NumericUpDown();
            this.label61 = new System.Windows.Forms.Label();
            this.nudZoneStarIndex = new System.Windows.Forms.NumericUpDown();
            this.label55 = new System.Windows.Forms.Label();
            this.label56 = new System.Windows.Forms.Label();
            this.nudAstrAttemptTimeout = new System.Windows.Forms.NumericUpDown();
            this.label46 = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.nudAstrFocLenVariation = new System.Windows.Forms.NumericUpDown();
            this.label36 = new System.Windows.Forms.Label();
            this.nudAstrOptimumStars = new System.Windows.Forms.NumericUpDown();
            this.label35 = new System.Windows.Forms.Label();
            this.nudAstrDistTolerance = new System.Windows.Forms.NumericUpDown();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.label60 = new System.Windows.Forms.Label();
            this.nudTargetVRColour = new System.Windows.Forms.NumericUpDown();
            this.label59 = new System.Windows.Forms.Label();
            this.cbxMagBand = new System.Windows.Forms.ComboBox();
            this.label54 = new System.Windows.Forms.Label();
            this.label57 = new System.Windows.Forms.Label();
            this.nudMagResidual = new System.Windows.Forms.NumericUpDown();
            this.label53 = new System.Windows.Forms.Label();
            this.cbxDebugOutput = new System.Windows.Forms.CheckBox();
            this.cbxExportUncertainties = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbxExportHigherPositionAccuracy = new System.Windows.Forms.CheckBox();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAstrMaximumStars)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAstrMinimumStars)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAstrMaxResidual)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAstrAssumedUncertainty)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumberOfPivots)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudZoneStarIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAstrAttemptTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAstrFocLenVariation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAstrOptimumStars)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAstrDistTolerance)).BeginInit();
            this.groupBox11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTargetVRColour)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMagResidual)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.label1);
            this.groupBox8.Controls.Add(this.label62);
            this.groupBox8.Controls.Add(this.cbxAstrFitMethod);
            this.groupBox8.Controls.Add(this.nudAstrMaximumStars);
            this.groupBox8.Controls.Add(this.label49);
            this.groupBox8.Controls.Add(this.label48);
            this.groupBox8.Controls.Add(this.nudAstrMinimumStars);
            this.groupBox8.Controls.Add(this.label51);
            this.groupBox8.Controls.Add(this.nudAstrMaxResidual);
            this.groupBox8.Location = new System.Drawing.Point(3, 3);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(225, 134);
            this.groupBox8.TabIndex = 32;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Astrometric Fit";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(203, 101);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 13);
            this.label1.TabIndex = 40;
            this.label1.Text = "px";
            // 
            // label62
            // 
            this.label62.AutoSize = true;
            this.label62.Location = new System.Drawing.Point(15, 73);
            this.label62.Name = "label62";
            this.label62.Size = new System.Drawing.Size(126, 13);
            this.label62.TabIndex = 39;
            this.label62.Text = "Max Number of Ref Stars";
            // 
            // cbxAstrFitMethod
            // 
            this.cbxAstrFitMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxAstrFitMethod.FormattingEnabled = true;
            this.cbxAstrFitMethod.Items.AddRange(new object[] {
            "Automatic Fit",
            "Linear Fit",
            "Quadratic Fit"});
            this.cbxAstrFitMethod.Location = new System.Drawing.Point(71, 18);
            this.cbxAstrFitMethod.Name = "cbxAstrFitMethod";
            this.cbxAstrFitMethod.Size = new System.Drawing.Size(133, 21);
            this.cbxAstrFitMethod.TabIndex = 32;
            // 
            // nudAstrMaximumStars
            // 
            this.nudAstrMaximumStars.Location = new System.Drawing.Point(151, 71);
            this.nudAstrMaximumStars.Maximum = new decimal(new int[] {
            250,
            0,
            0,
            0});
            this.nudAstrMaximumStars.Minimum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.nudAstrMaximumStars.Name = "nudAstrMaximumStars";
            this.nudAstrMaximumStars.Size = new System.Drawing.Size(50, 20);
            this.nudAstrMaximumStars.TabIndex = 38;
            this.nudAstrMaximumStars.Value = new decimal(new int[] {
            150,
            0,
            0,
            0});
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Location = new System.Drawing.Point(15, 21);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(45, 13);
            this.label49.TabIndex = 10;
            this.label49.Text = "Fit Type";
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Location = new System.Drawing.Point(15, 48);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(123, 13);
            this.label48.TabIndex = 9;
            this.label48.Text = "Min Number of Ref Stars";
            // 
            // nudAstrMinimumStars
            // 
            this.nudAstrMinimumStars.Location = new System.Drawing.Point(151, 47);
            this.nudAstrMinimumStars.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.nudAstrMinimumStars.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nudAstrMinimumStars.Name = "nudAstrMinimumStars";
            this.nudAstrMinimumStars.Size = new System.Drawing.Size(50, 20);
            this.nudAstrMinimumStars.TabIndex = 8;
            this.nudAstrMinimumStars.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Location = new System.Drawing.Point(15, 98);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(71, 13);
            this.label51.TabIndex = 1;
            this.label51.Text = "Max Residual";
            // 
            // nudAstrMaxResidual
            // 
            this.nudAstrMaxResidual.DecimalPlaces = 1;
            this.nudAstrMaxResidual.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudAstrMaxResidual.Location = new System.Drawing.Point(151, 96);
            this.nudAstrMaxResidual.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudAstrMaxResidual.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            65536});
            this.nudAstrMaxResidual.Name = "nudAstrMaxResidual";
            this.nudAstrMaxResidual.Size = new System.Drawing.Size(50, 20);
            this.nudAstrMaxResidual.TabIndex = 0;
            this.nudAstrMaxResidual.ThousandsSeparator = true;
            this.nudAstrMaxResidual.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(201, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(18, 13);
            this.label3.TabIndex = 43;
            this.label3.Text = "px";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(131, 13);
            this.label4.TabIndex = 42;
            this.label4.Text = "Assumed Best Uncertainty";
            // 
            // nudAstrAssumedUncertainty
            // 
            this.nudAstrAssumedUncertainty.DecimalPlaces = 2;
            this.nudAstrAssumedUncertainty.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudAstrAssumedUncertainty.Location = new System.Drawing.Point(149, 21);
            this.nudAstrAssumedUncertainty.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudAstrAssumedUncertainty.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudAstrAssumedUncertainty.Name = "nudAstrAssumedUncertainty";
            this.nudAstrAssumedUncertainty.Size = new System.Drawing.Size(50, 20);
            this.nudAstrAssumedUncertainty.TabIndex = 41;
            this.nudAstrAssumedUncertainty.ThousandsSeparator = true;
            this.nudAstrAssumedUncertainty.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.nudNumberOfPivots);
            this.groupBox5.Controls.Add(this.label61);
            this.groupBox5.Controls.Add(this.nudZoneStarIndex);
            this.groupBox5.Controls.Add(this.label55);
            this.groupBox5.Controls.Add(this.label56);
            this.groupBox5.Controls.Add(this.nudAstrAttemptTimeout);
            this.groupBox5.Controls.Add(this.label46);
            this.groupBox5.Controls.Add(this.label45);
            this.groupBox5.Controls.Add(this.label44);
            this.groupBox5.Controls.Add(this.nudAstrFocLenVariation);
            this.groupBox5.Controls.Add(this.label36);
            this.groupBox5.Controls.Add(this.nudAstrOptimumStars);
            this.groupBox5.Controls.Add(this.label35);
            this.groupBox5.Controls.Add(this.nudAstrDistTolerance);
            this.groupBox5.Location = new System.Drawing.Point(3, 143);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(226, 180);
            this.groupBox5.TabIndex = 31;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Alignment Settings";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 33;
            this.label2.Text = "Number of Pivots";
            // 
            // nudNumberOfPivots
            // 
            this.nudNumberOfPivots.Location = new System.Drawing.Point(152, 94);
            this.nudNumberOfPivots.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nudNumberOfPivots.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudNumberOfPivots.Name = "nudNumberOfPivots";
            this.nudNumberOfPivots.Size = new System.Drawing.Size(47, 20);
            this.nudNumberOfPivots.TabIndex = 32;
            this.nudNumberOfPivots.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // label61
            // 
            this.label61.AutoSize = true;
            this.label61.Location = new System.Drawing.Point(15, 72);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(88, 13);
            this.label61.TabIndex = 12;
            this.label61.Text = "Distribution Index";
            // 
            // nudZoneStarIndex
            // 
            this.nudZoneStarIndex.Location = new System.Drawing.Point(151, 68);
            this.nudZoneStarIndex.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudZoneStarIndex.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudZoneStarIndex.Name = "nudZoneStarIndex";
            this.nudZoneStarIndex.Size = new System.Drawing.Size(47, 20);
            this.nudZoneStarIndex.TabIndex = 11;
            this.nudZoneStarIndex.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Location = new System.Drawing.Point(201, 152);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(24, 13);
            this.label55.TabIndex = 10;
            this.label55.Text = "sec";
            // 
            // label56
            // 
            this.label56.AutoSize = true;
            this.label56.Location = new System.Drawing.Point(16, 152);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(84, 13);
            this.label56.TabIndex = 9;
            this.label56.Text = "Attempt Timeout";
            // 
            // nudAstrAttemptTimeout
            // 
            this.nudAstrAttemptTimeout.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudAstrAttemptTimeout.Location = new System.Drawing.Point(152, 148);
            this.nudAstrAttemptTimeout.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudAstrAttemptTimeout.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudAstrAttemptTimeout.Name = "nudAstrAttemptTimeout";
            this.nudAstrAttemptTimeout.Size = new System.Drawing.Size(47, 20);
            this.nudAstrAttemptTimeout.TabIndex = 8;
            this.nudAstrAttemptTimeout.ThousandsSeparator = true;
            this.nudAstrAttemptTimeout.Value = new decimal(new int[] {
            90,
            0,
            0,
            0});
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Location = new System.Drawing.Point(198, 25);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(18, 13);
            this.label46.TabIndex = 7;
            this.label46.Text = "px";
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(201, 125);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(15, 13);
            this.label45.TabIndex = 6;
            this.label45.Text = "%";
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(16, 126);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(113, 13);
            this.label44.TabIndex = 5;
            this.label44.Text = "Focal Length Variation";
            // 
            // nudAstrFocLenVariation
            // 
            this.nudAstrFocLenVariation.DecimalPlaces = 1;
            this.nudAstrFocLenVariation.Increment = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.nudAstrFocLenVariation.Location = new System.Drawing.Point(152, 122);
            this.nudAstrFocLenVariation.Maximum = new decimal(new int[] {
            98,
            0,
            0,
            65536});
            this.nudAstrFocLenVariation.Name = "nudAstrFocLenVariation";
            this.nudAstrFocLenVariation.Size = new System.Drawing.Size(47, 20);
            this.nudAstrFocLenVariation.TabIndex = 4;
            this.nudAstrFocLenVariation.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(15, 47);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(120, 13);
            this.label36.TabIndex = 3;
            this.label36.Text = "Optimum Match Objects";
            // 
            // nudAstrOptimumStars
            // 
            this.nudAstrOptimumStars.Location = new System.Drawing.Point(151, 43);
            this.nudAstrOptimumStars.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudAstrOptimumStars.Name = "nudAstrOptimumStars";
            this.nudAstrOptimumStars.Size = new System.Drawing.Size(47, 20);
            this.nudAstrOptimumStars.TabIndex = 2;
            this.nudAstrOptimumStars.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(15, 23);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(100, 13);
            this.label35.TabIndex = 1;
            this.label35.Text = "Distance Tolerance";
            // 
            // nudAstrDistTolerance
            // 
            this.nudAstrDistTolerance.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.nudAstrDistTolerance.Location = new System.Drawing.Point(151, 19);
            this.nudAstrDistTolerance.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.nudAstrDistTolerance.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudAstrDistTolerance.Name = "nudAstrDistTolerance";
            this.nudAstrDistTolerance.Size = new System.Drawing.Size(47, 20);
            this.nudAstrDistTolerance.TabIndex = 0;
            this.nudAstrDistTolerance.ThousandsSeparator = true;
            this.nudAstrDistTolerance.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.label60);
            this.groupBox11.Controls.Add(this.nudTargetVRColour);
            this.groupBox11.Controls.Add(this.label59);
            this.groupBox11.Controls.Add(this.cbxMagBand);
            this.groupBox11.Controls.Add(this.label54);
            this.groupBox11.Controls.Add(this.label57);
            this.groupBox11.Controls.Add(this.nudMagResidual);
            this.groupBox11.Controls.Add(this.label53);
            this.groupBox11.Location = new System.Drawing.Point(234, 3);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(231, 134);
            this.groupBox11.TabIndex = 34;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Calibrated Photometry";
            // 
            // label60
            // 
            this.label60.AutoSize = true;
            this.label60.Location = new System.Drawing.Point(189, 92);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(15, 13);
            this.label60.TabIndex = 42;
            this.label60.Text = "m";
            // 
            // nudTargetVRColour
            // 
            this.nudTargetVRColour.DecimalPlaces = 1;
            this.nudTargetVRColour.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudTargetVRColour.Location = new System.Drawing.Point(146, 87);
            this.nudTargetVRColour.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudTargetVRColour.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            -2147483648});
            this.nudTargetVRColour.Name = "nudTargetVRColour";
            this.nudTargetVRColour.Size = new System.Drawing.Size(41, 20);
            this.nudTargetVRColour.TabIndex = 41;
            this.nudTargetVRColour.ThousandsSeparator = true;
            this.nudTargetVRColour.Value = new decimal(new int[] {
            4,
            0,
            0,
            65536});
            // 
            // label59
            // 
            this.label59.AutoSize = true;
            this.label59.Location = new System.Drawing.Point(15, 92);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(98, 13);
            this.label59.TabIndex = 40;
            this.label59.Text = "Target (V-R) Colour";
            // 
            // cbxMagBand
            // 
            this.cbxMagBand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxMagBand.FormattingEnabled = true;
            this.cbxMagBand.Items.AddRange(new object[] {
            "Johnson V",
            "Cousins R"});
            this.cbxMagBand.Location = new System.Drawing.Point(93, 55);
            this.cbxMagBand.Name = "cbxMagBand";
            this.cbxMagBand.Size = new System.Drawing.Size(95, 21);
            this.cbxMagBand.TabIndex = 39;
            // 
            // label54
            // 
            this.label54.AutoSize = true;
            this.label54.Location = new System.Drawing.Point(189, 29);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(15, 13);
            this.label54.TabIndex = 38;
            this.label54.Text = "m";
            // 
            // label57
            // 
            this.label57.AutoSize = true;
            this.label57.Location = new System.Drawing.Point(15, 26);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(71, 13);
            this.label57.TabIndex = 37;
            this.label57.Text = "Max Residual";
            // 
            // nudMagResidual
            // 
            this.nudMagResidual.DecimalPlaces = 1;
            this.nudMagResidual.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudMagResidual.Location = new System.Drawing.Point(146, 24);
            this.nudMagResidual.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudMagResidual.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudMagResidual.Name = "nudMagResidual";
            this.nudMagResidual.Size = new System.Drawing.Size(41, 20);
            this.nudMagResidual.TabIndex = 36;
            this.nudMagResidual.ThousandsSeparator = true;
            this.nudMagResidual.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Location = new System.Drawing.Point(15, 58);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(67, 13);
            this.label53.TabIndex = 34;
            this.label53.Text = "Output Band";
            // 
            // cbxDebugOutput
            // 
            this.cbxDebugOutput.AutoSize = true;
            this.cbxDebugOutput.Location = new System.Drawing.Point(16, 68);
            this.cbxDebugOutput.Name = "cbxDebugOutput";
            this.cbxDebugOutput.Size = new System.Drawing.Size(125, 17);
            this.cbxDebugOutput.TabIndex = 35;
            this.cbxDebugOutput.Text = "Enable debug output";
            this.cbxDebugOutput.UseVisualStyleBackColor = true;
            // 
            // cbxExportUncertainties
            // 
            this.cbxExportUncertainties.AutoSize = true;
            this.cbxExportUncertainties.Location = new System.Drawing.Point(16, 91);
            this.cbxExportUncertainties.Name = "cbxExportUncertainties";
            this.cbxExportUncertainties.Size = new System.Drawing.Size(119, 17);
            this.cbxExportUncertainties.TabIndex = 50;
            this.cbxExportUncertainties.Text = "Export uncertainties";
            this.cbxExportUncertainties.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbxExportHigherPositionAccuracy);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cbxExportUncertainties);
            this.groupBox1.Controls.Add(this.nudAstrAssumedUncertainty);
            this.groupBox1.Controls.Add(this.cbxDebugOutput);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(234, 143);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(231, 180);
            this.groupBox1.TabIndex = 51;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Advanced";
            // 
            // cbxExportHigherPositionAccuracy
            // 
            this.cbxExportHigherPositionAccuracy.AutoSize = true;
            this.cbxExportHigherPositionAccuracy.Location = new System.Drawing.Point(16, 114);
            this.cbxExportHigherPositionAccuracy.Name = "cbxExportHigherPositionAccuracy";
            this.cbxExportHigherPositionAccuracy.Size = new System.Drawing.Size(170, 17);
            this.cbxExportHigherPositionAccuracy.TabIndex = 51;
            this.cbxExportHigherPositionAccuracy.Text = "Export high accuracy positions";
            this.cbxExportHigherPositionAccuracy.UseVisualStyleBackColor = true;
            // 
            // ucAstrometry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox11);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBox5);
            this.Name = "ucAstrometry";
            this.Size = new System.Drawing.Size(850, 481);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAstrMaximumStars)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAstrMinimumStars)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAstrMaxResidual)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAstrAssumedUncertainty)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumberOfPivots)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudZoneStarIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAstrAttemptTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAstrFocLenVariation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAstrOptimumStars)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAstrDistTolerance)).EndInit();
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTargetVRColour)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMagResidual)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.GroupBox groupBox8;
		private System.Windows.Forms.Label label62;
		private System.Windows.Forms.ComboBox cbxAstrFitMethod;
		private System.Windows.Forms.NumericUpDown nudAstrMaximumStars;
		private System.Windows.Forms.Label label49;
		private System.Windows.Forms.Label label48;
        private System.Windows.Forms.NumericUpDown nudAstrMinimumStars;
		private System.Windows.Forms.Label label51;
		private System.Windows.Forms.NumericUpDown nudAstrMaxResidual;
        private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Label label61;
		private System.Windows.Forms.NumericUpDown nudZoneStarIndex;
		private System.Windows.Forms.Label label55;
		private System.Windows.Forms.Label label56;
		private System.Windows.Forms.NumericUpDown nudAstrAttemptTimeout;
		private System.Windows.Forms.Label label46;
		private System.Windows.Forms.Label label45;
		private System.Windows.Forms.Label label44;
		private System.Windows.Forms.NumericUpDown nudAstrFocLenVariation;
		private System.Windows.Forms.Label label36;
		private System.Windows.Forms.NumericUpDown nudAstrOptimumStars;
		private System.Windows.Forms.Label label35;
		private System.Windows.Forms.NumericUpDown nudAstrDistTolerance;
		private System.Windows.Forms.GroupBox groupBox11;
		private System.Windows.Forms.Label label60;
		private System.Windows.Forms.NumericUpDown nudTargetVRColour;
		private System.Windows.Forms.Label label59;
		private System.Windows.Forms.ComboBox cbxMagBand;
		private System.Windows.Forms.Label label54;
		private System.Windows.Forms.Label label57;
		private System.Windows.Forms.NumericUpDown nudMagResidual;
		private System.Windows.Forms.Label label53;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudNumberOfPivots;
        private System.Windows.Forms.CheckBox cbxDebugOutput;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudAstrAssumedUncertainty;
        private System.Windows.Forms.CheckBox cbxExportUncertainties;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbxExportHigherPositionAccuracy;
	}
}

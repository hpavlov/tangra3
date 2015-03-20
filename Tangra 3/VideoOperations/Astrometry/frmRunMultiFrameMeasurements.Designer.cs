using Tangra.Model.Controls;

namespace Tangra.VideoOperations.Astrometry
{
	partial class frmRunMultiFrameMeasurements
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRunMultiFrameMeasurements));
			this.btnNext = new System.Windows.Forms.Button();
			this.cbxExport = new System.Windows.Forms.CheckBox();
			this.pnlExportConfig = new System.Windows.Forms.Panel();
			this.btnConfigureAstrometryExport = new System.Windows.Forms.Button();
			this.pnlExportPhotometry = new System.Windows.Forms.Panel();
			this.btnConfigurePhotometryExport = new System.Windows.Forms.Button();
			this.cbxExportPhotometry = new System.Windows.Forms.CheckBox();
			this.cbxExportAstrometry = new System.Windows.Forms.CheckBox();
			this.btnPrevious = new System.Windows.Forms.Button();
			this.pnlPhotometry = new System.Windows.Forms.Panel();
			this.cbxFitMagnitudes = new System.Windows.Forms.CheckBox();
			this.pnlPhotometryMethods = new System.Windows.Forms.Panel();
			this.label17 = new System.Windows.Forms.Label();
			this.nudAperture = new System.Windows.Forms.NumericUpDown();
			this.label16 = new System.Windows.Forms.Label();
			this.cbxCatalogPhotometryBand = new System.Windows.Forms.ComboBox();
			this.lblCatBand = new System.Windows.Forms.Label();
			this.label53 = new System.Windows.Forms.Label();
			this.cbxOutputMagBand = new System.Windows.Forms.ComboBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.cbxReductionType = new System.Windows.Forms.ComboBox();
			this.cbxBackgroundMethod = new System.Windows.Forms.ComboBox();
			this.pnlAstrometry = new System.Windows.Forms.Panel();
			this.label5 = new System.Windows.Forms.Label();
			this.ucFrameInterval = new Tangra.VideoOperations.Astrometry.ucFrameInterval();
			this.label3 = new System.Windows.Forms.Label();
			this.nudNumberMeasurements = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.nudMaxStdDev = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.nudMinStars = new System.Windows.Forms.NumericUpDown();
			this.label6 = new System.Windows.Forms.Label();
			this.cbxStopOnNoFit = new System.Windows.Forms.CheckBox();
			this.label7 = new System.Windows.Forms.Label();
			this.pnlTime = new System.Windows.Forms.Panel();
			this.linkExamples = new System.Windows.Forms.LinkLabel();
			this.label15 = new System.Windows.Forms.Label();
			this.pnlInstrDelay = new System.Windows.Forms.Panel();
			this.label12 = new System.Windows.Forms.Label();
			this.nudInstrDelay = new System.Windows.Forms.NumericUpDown();
			this.cbxInstDelayUnit = new System.Windows.Forms.ComboBox();
			this.cbxSignalType = new System.Windows.Forms.ComboBox();
			this.label10 = new System.Windows.Forms.Label();
			this.pnlIntegration = new System.Windows.Forms.Panel();
			this.btnDetectIntegration = new System.Windows.Forms.Button();
			this.label14 = new System.Windows.Forms.Label();
			this.nudIntegratedFrames = new System.Windows.Forms.NumericUpDown();
			this.label13 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.ucUtcTimePicker = new Tangra.Model.Controls.ucUtcTimePicker();
			this.cbxExpectedMotion = new System.Windows.Forms.ComboBox();
			this.label11 = new System.Windows.Forms.Label();
			this.cbxFrameTimeType = new System.Windows.Forms.ComboBox();
			this.gbConfig = new System.Windows.Forms.GroupBox();
			this.pnlAddins = new System.Windows.Forms.Panel();
			this.clbAddinsToRun = new System.Windows.Forms.CheckedListBox();
			this.pnlExportConfig.SuspendLayout();
			this.pnlExportPhotometry.SuspendLayout();
			this.pnlPhotometry.SuspendLayout();
			this.pnlPhotometryMethods.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudAperture)).BeginInit();
			this.pnlAstrometry.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudNumberMeasurements)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxStdDev)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinStars)).BeginInit();
			this.pnlTime.SuspendLayout();
			this.pnlInstrDelay.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudInstrDelay)).BeginInit();
			this.pnlIntegration.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudIntegratedFrames)).BeginInit();
			this.gbConfig.SuspendLayout();
			this.pnlAddins.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnNext
			// 
			this.btnNext.Location = new System.Drawing.Point(379, 301);
			this.btnNext.Name = "btnNext";
			this.btnNext.Size = new System.Drawing.Size(75, 23);
			this.btnNext.TabIndex = 4;
			this.btnNext.Text = "Next >";
			this.btnNext.UseVisualStyleBackColor = true;
			this.btnNext.Click += new System.EventHandler(this.btnStartStop_Click);
			// 
			// cbxExport
			// 
			this.cbxExport.AutoSize = true;
			this.cbxExport.Location = new System.Drawing.Point(512, 48);
			this.cbxExport.Name = "cbxExport";
			this.cbxExport.Size = new System.Drawing.Size(165, 17);
			this.cbxExport.TabIndex = 3;
			this.cbxExport.Text = "Export Data in a Spreadsheet";
			this.cbxExport.UseVisualStyleBackColor = true;
			this.cbxExport.CheckedChanged += new System.EventHandler(this.cbxExport_CheckedChanged);
			// 
			// pnlExportConfig
			// 
			this.pnlExportConfig.Controls.Add(this.btnConfigureAstrometryExport);
			this.pnlExportConfig.Controls.Add(this.pnlExportPhotometry);
			this.pnlExportConfig.Controls.Add(this.cbxExportAstrometry);
			this.pnlExportConfig.Enabled = false;
			this.pnlExportConfig.Location = new System.Drawing.Point(525, 76);
			this.pnlExportConfig.Name = "pnlExportConfig";
			this.pnlExportConfig.Size = new System.Drawing.Size(287, 145);
			this.pnlExportConfig.TabIndex = 4;
			// 
			// btnConfigureAstrometryExport
			// 
			this.btnConfigureAstrometryExport.Location = new System.Drawing.Point(149, 8);
			this.btnConfigureAstrometryExport.Name = "btnConfigureAstrometryExport";
			this.btnConfigureAstrometryExport.Size = new System.Drawing.Size(75, 23);
			this.btnConfigureAstrometryExport.TabIndex = 3;
			this.btnConfigureAstrometryExport.Text = "Configure";
			this.btnConfigureAstrometryExport.UseVisualStyleBackColor = true;
			// 
			// pnlExportPhotometry
			// 
			this.pnlExportPhotometry.Controls.Add(this.btnConfigurePhotometryExport);
			this.pnlExportPhotometry.Controls.Add(this.cbxExportPhotometry);
			this.pnlExportPhotometry.Enabled = false;
			this.pnlExportPhotometry.Location = new System.Drawing.Point(4, 35);
			this.pnlExportPhotometry.Name = "pnlExportPhotometry";
			this.pnlExportPhotometry.Size = new System.Drawing.Size(280, 62);
			this.pnlExportPhotometry.TabIndex = 2;
			// 
			// btnConfigurePhotometryExport
			// 
			this.btnConfigurePhotometryExport.Location = new System.Drawing.Point(145, 17);
			this.btnConfigurePhotometryExport.Name = "btnConfigurePhotometryExport";
			this.btnConfigurePhotometryExport.Size = new System.Drawing.Size(75, 23);
			this.btnConfigurePhotometryExport.TabIndex = 4;
			this.btnConfigurePhotometryExport.Text = "Configure";
			this.btnConfigurePhotometryExport.UseVisualStyleBackColor = true;
			// 
			// cbxExportPhotometry
			// 
			this.cbxExportPhotometry.AutoSize = true;
			this.cbxExportPhotometry.Location = new System.Drawing.Point(9, 21);
			this.cbxExportPhotometry.Name = "cbxExportPhotometry";
			this.cbxExportPhotometry.Size = new System.Drawing.Size(117, 17);
			this.cbxExportPhotometry.TabIndex = 1;
			this.cbxExportPhotometry.Text = "Include Photometry";
			this.cbxExportPhotometry.UseVisualStyleBackColor = true;
			// 
			// cbxExportAstrometry
			// 
			this.cbxExportAstrometry.AutoSize = true;
			this.cbxExportAstrometry.Location = new System.Drawing.Point(13, 12);
			this.cbxExportAstrometry.Name = "cbxExportAstrometry";
			this.cbxExportAstrometry.Size = new System.Drawing.Size(113, 17);
			this.cbxExportAstrometry.TabIndex = 0;
			this.cbxExportAstrometry.Text = "Include Astrometry";
			this.cbxExportAstrometry.UseVisualStyleBackColor = true;
			// 
			// btnPrevious
			// 
			this.btnPrevious.Location = new System.Drawing.Point(298, 301);
			this.btnPrevious.Name = "btnPrevious";
			this.btnPrevious.Size = new System.Drawing.Size(75, 23);
			this.btnPrevious.TabIndex = 12;
			this.btnPrevious.Text = "Cancel";
			this.btnPrevious.UseVisualStyleBackColor = true;
			this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
			// 
			// pnlPhotometry
			// 
			this.pnlPhotometry.Controls.Add(this.cbxFitMagnitudes);
			this.pnlPhotometry.Controls.Add(this.pnlPhotometryMethods);
			this.pnlPhotometry.Location = new System.Drawing.Point(6, 21);
			this.pnlPhotometry.Name = "pnlPhotometry";
			this.pnlPhotometry.Size = new System.Drawing.Size(408, 255);
			this.pnlPhotometry.TabIndex = 15;
			// 
			// cbxFitMagnitudes
			// 
			this.cbxFitMagnitudes.AutoSize = true;
			this.cbxFitMagnitudes.Location = new System.Drawing.Point(3, 3);
			this.cbxFitMagnitudes.Name = "cbxFitMagnitudes";
			this.cbxFitMagnitudes.Size = new System.Drawing.Size(193, 17);
			this.cbxFitMagnitudes.TabIndex = 0;
			this.cbxFitMagnitudes.Text = "Fit and Compute Stellar Magnitudes";
			this.cbxFitMagnitudes.UseVisualStyleBackColor = true;
			this.cbxFitMagnitudes.CheckedChanged += new System.EventHandler(this.cbxFitMagnitudes_CheckedChanged);
			// 
			// pnlPhotometryMethods
			// 
			this.pnlPhotometryMethods.Controls.Add(this.label17);
			this.pnlPhotometryMethods.Controls.Add(this.nudAperture);
			this.pnlPhotometryMethods.Controls.Add(this.label16);
			this.pnlPhotometryMethods.Controls.Add(this.cbxCatalogPhotometryBand);
			this.pnlPhotometryMethods.Controls.Add(this.lblCatBand);
			this.pnlPhotometryMethods.Controls.Add(this.label53);
			this.pnlPhotometryMethods.Controls.Add(this.cbxOutputMagBand);
			this.pnlPhotometryMethods.Controls.Add(this.label8);
			this.pnlPhotometryMethods.Controls.Add(this.label9);
			this.pnlPhotometryMethods.Controls.Add(this.cbxReductionType);
			this.pnlPhotometryMethods.Controls.Add(this.cbxBackgroundMethod);
			this.pnlPhotometryMethods.Enabled = false;
			this.pnlPhotometryMethods.Location = new System.Drawing.Point(25, 24);
			this.pnlPhotometryMethods.Name = "pnlPhotometryMethods";
			this.pnlPhotometryMethods.Size = new System.Drawing.Size(343, 228);
			this.pnlPhotometryMethods.TabIndex = 2;
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Location = new System.Drawing.Point(128, 94);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(20, 13);
			this.label17.TabIndex = 43;
			this.label17.Text = "pix";
			// 
			// nudAperture
			// 
			this.nudAperture.DecimalPlaces = 2;
			this.nudAperture.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.nudAperture.Location = new System.Drawing.Point(76, 91);
			this.nudAperture.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
			this.nudAperture.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.nudAperture.Name = "nudAperture";
			this.nudAperture.Size = new System.Drawing.Size(49, 20);
			this.nudAperture.TabIndex = 42;
			this.nudAperture.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(3, 94);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(71, 13);
			this.label16.TabIndex = 41;
			this.label16.Text = "Aperture size:";
			// 
			// cbxCatalogPhotometryBand
			// 
			this.cbxCatalogPhotometryBand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxCatalogPhotometryBand.FormattingEnabled = true;
			this.cbxCatalogPhotometryBand.Items.AddRange(new object[] {
            "Model Fit Magnitude (fMag)",
            "Johnson V - Computed from fMag",
            "Cousins R - Computed from fMag"});
			this.cbxCatalogPhotometryBand.Location = new System.Drawing.Point(6, 154);
			this.cbxCatalogPhotometryBand.Name = "cbxCatalogPhotometryBand";
			this.cbxCatalogPhotometryBand.Size = new System.Drawing.Size(247, 21);
			this.cbxCatalogPhotometryBand.TabIndex = 40;
			// 
			// lblCatBand
			// 
			this.lblCatBand.AutoSize = true;
			this.lblCatBand.Location = new System.Drawing.Point(3, 138);
			this.lblCatBand.Name = "lblCatBand";
			this.lblCatBand.Size = new System.Drawing.Size(209, 13);
			this.lblCatBand.TabIndex = 39;
			this.lblCatBand.Text = "Magnitude Band for Photometry (from XXX)";
			// 
			// label53
			// 
			this.label53.AutoSize = true;
			this.label53.Location = new System.Drawing.Point(3, 183);
			this.label53.Name = "label53";
			this.label53.Size = new System.Drawing.Size(120, 13);
			this.label53.TabIndex = 38;
			this.label53.Text = "Output Magnitude Band";
			// 
			// cbxOutputMagBand
			// 
			this.cbxOutputMagBand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxOutputMagBand.FormattingEnabled = true;
			this.cbxOutputMagBand.Items.AddRange(new object[] {
            "Johnson V",
            "Cousins R"});
			this.cbxOutputMagBand.Location = new System.Drawing.Point(6, 199);
			this.cbxOutputMagBand.Name = "cbxOutputMagBand";
			this.cbxOutputMagBand.Size = new System.Drawing.Size(140, 21);
			this.cbxOutputMagBand.TabIndex = 37;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(3, 47);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(91, 13);
			this.label8.TabIndex = 6;
			this.label8.Text = "Background from:";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(3, 4);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(97, 13);
			this.label9.TabIndex = 8;
			this.label9.Text = "Reduction method:";
			// 
			// cbxReductionType
			// 
			this.cbxReductionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxReductionType.FormattingEnabled = true;
			this.cbxReductionType.Items.AddRange(new object[] {
            "Aperture Photometry",
            "PSF Photometry",
            "Optimal Extraction Photometry"});
			this.cbxReductionType.Location = new System.Drawing.Point(6, 20);
			this.cbxReductionType.Name = "cbxReductionType";
			this.cbxReductionType.Size = new System.Drawing.Size(205, 21);
			this.cbxReductionType.TabIndex = 9;
			// 
			// cbxBackgroundMethod
			// 
			this.cbxBackgroundMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxBackgroundMethod.FormattingEnabled = true;
			this.cbxBackgroundMethod.Items.AddRange(new object[] {
            "Average Background",
            "Background Mode",
            "Background Gradient Fit",
            "PSF-Fitting Background",
            "Median Background"});
			this.cbxBackgroundMethod.Location = new System.Drawing.Point(6, 63);
			this.cbxBackgroundMethod.Name = "cbxBackgroundMethod";
			this.cbxBackgroundMethod.Size = new System.Drawing.Size(205, 21);
			this.cbxBackgroundMethod.TabIndex = 7;
			// 
			// pnlAstrometry
			// 
			this.pnlAstrometry.Controls.Add(this.label5);
			this.pnlAstrometry.Controls.Add(this.ucFrameInterval);
			this.pnlAstrometry.Controls.Add(this.label3);
			this.pnlAstrometry.Controls.Add(this.nudNumberMeasurements);
			this.pnlAstrometry.Controls.Add(this.label2);
			this.pnlAstrometry.Controls.Add(this.nudMaxStdDev);
			this.pnlAstrometry.Controls.Add(this.label1);
			this.pnlAstrometry.Controls.Add(this.nudMinStars);
			this.pnlAstrometry.Controls.Add(this.label6);
			this.pnlAstrometry.Controls.Add(this.cbxStopOnNoFit);
			this.pnlAstrometry.Controls.Add(this.label7);
			this.pnlAstrometry.Location = new System.Drawing.Point(5, 20);
			this.pnlAstrometry.Name = "pnlAstrometry";
			this.pnlAstrometry.Size = new System.Drawing.Size(430, 233);
			this.pnlAstrometry.TabIndex = 13;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(19, 59);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(59, 13);
			this.label5.TabIndex = 8;
			this.label5.Text = "Take up to";
			// 
			// ucFrameInterval
			// 
			this.ucFrameInterval.Location = new System.Drawing.Point(17, 15);
			this.ucFrameInterval.Name = "ucFrameInterval";
			this.ucFrameInterval.Size = new System.Drawing.Size(179, 27);
			this.ucFrameInterval.TabIndex = 0;
			this.ucFrameInterval.Value = 1;
			this.ucFrameInterval.FrameIntervalChanged += new System.EventHandler<Tangra.VideoOperations.Astrometry.FrameIntervalChangedEventArgs>(this.ucFrameInterval_FrameIntervalChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(143, 61);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(75, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "measurements";
			// 
			// nudNumberMeasurements
			// 
			this.nudNumberMeasurements.Location = new System.Drawing.Point(84, 57);
			this.nudNumberMeasurements.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
			this.nudNumberMeasurements.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudNumberMeasurements.Name = "nudNumberMeasurements";
			this.nudNumberMeasurements.Size = new System.Drawing.Size(54, 20);
			this.nudNumberMeasurements.TabIndex = 6;
			this.nudNumberMeasurements.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(292, 133);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(12, 13);
			this.label2.TabIndex = 18;
			this.label2.Text = "\"";
			// 
			// nudMaxStdDev
			// 
			this.nudMaxStdDev.DecimalPlaces = 2;
			this.nudMaxStdDev.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
			this.nudMaxStdDev.Location = new System.Drawing.Point(242, 132);
			this.nudMaxStdDev.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            65536});
			this.nudMaxStdDev.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.nudMaxStdDev.Name = "nudMaxStdDev";
			this.nudMaxStdDev.Size = new System.Drawing.Size(47, 20);
			this.nudMaxStdDev.TabIndex = 17;
			this.nudMaxStdDev.Value = new decimal(new int[] {
            50,
            0,
            0,
            131072});
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(19, 134);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(217, 13);
			this.label1.TabIndex = 16;
			this.label1.Text = "Exclude frame fits with Std.Dev. greater than";
			// 
			// nudMinStars
			// 
			this.nudMinStars.Location = new System.Drawing.Point(166, 168);
			this.nudMinStars.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
			this.nudMinStars.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
			this.nudMinStars.Name = "nudMinStars";
			this.nudMinStars.Size = new System.Drawing.Size(37, 20);
			this.nudMinStars.TabIndex = 20;
			this.nudMinStars.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(19, 170);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(146, 13);
			this.label6.TabIndex = 19;
			this.label6.Text = "Exclude frames with less than";
			// 
			// cbxStopOnNoFit
			// 
			this.cbxStopOnNoFit.AutoSize = true;
			this.cbxStopOnNoFit.Location = new System.Drawing.Point(22, 205);
			this.cbxStopOnNoFit.Name = "cbxStopOnNoFit";
			this.cbxStopOnNoFit.Size = new System.Drawing.Size(98, 17);
			this.cbxStopOnNoFit.TabIndex = 22;
			this.cbxStopOnNoFit.Text = "Stop on \'No Fit\'";
			this.cbxStopOnNoFit.UseVisualStyleBackColor = true;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(209, 170);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(77, 13);
			this.label7.TabIndex = 21;
			this.label7.Text = "reference stars";
			// 
			// pnlTime
			// 
			this.pnlTime.Controls.Add(this.linkExamples);
			this.pnlTime.Controls.Add(this.label15);
			this.pnlTime.Controls.Add(this.pnlInstrDelay);
			this.pnlTime.Controls.Add(this.cbxSignalType);
			this.pnlTime.Controls.Add(this.label10);
			this.pnlTime.Controls.Add(this.pnlIntegration);
			this.pnlTime.Controls.Add(this.label4);
			this.pnlTime.Controls.Add(this.ucUtcTimePicker);
			this.pnlTime.Controls.Add(this.cbxExpectedMotion);
			this.pnlTime.Controls.Add(this.label11);
			this.pnlTime.Controls.Add(this.cbxFrameTimeType);
			this.pnlTime.Location = new System.Drawing.Point(7, 21);
			this.pnlTime.Name = "pnlTime";
			this.pnlTime.Size = new System.Drawing.Size(421, 245);
			this.pnlTime.TabIndex = 14;
			// 
			// linkExamples
			// 
			this.linkExamples.AutoSize = true;
			this.linkExamples.Location = new System.Drawing.Point(15, 228);
			this.linkExamples.Name = "linkExamples";
			this.linkExamples.Size = new System.Drawing.Size(278, 13);
			this.linkExamples.TabIndex = 35;
			this.linkExamples.TabStop = true;
			this.linkExamples.Text = "http://www.hristopavlov.net/Tangra/AstrometryExamples";
			this.linkExamples.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkExamples_LinkClicked);
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(14, 213);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(325, 13);
			this.label15.TabIndex = 34;
			this.label15.Text = "For observation guidelines and step-by-step reduction examples see";
			// 
			// pnlInstrDelay
			// 
			this.pnlInstrDelay.Controls.Add(this.label12);
			this.pnlInstrDelay.Controls.Add(this.nudInstrDelay);
			this.pnlInstrDelay.Controls.Add(this.cbxInstDelayUnit);
			this.pnlInstrDelay.Location = new System.Drawing.Point(7, 166);
			this.pnlInstrDelay.Name = "pnlInstrDelay";
			this.pnlInstrDelay.Size = new System.Drawing.Size(330, 36);
			this.pnlInstrDelay.TabIndex = 33;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(4, 10);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(95, 13);
			this.label12.TabIndex = 28;
			this.label12.Text = "Instrumental delay:";
			// 
			// nudInstrDelay
			// 
			this.nudInstrDelay.DecimalPlaces = 2;
			this.nudInstrDelay.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
			this.nudInstrDelay.Location = new System.Drawing.Point(100, 8);
			this.nudInstrDelay.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
			this.nudInstrDelay.Name = "nudInstrDelay";
			this.nudInstrDelay.Size = new System.Drawing.Size(56, 20);
			this.nudInstrDelay.TabIndex = 29;
			// 
			// cbxInstDelayUnit
			// 
			this.cbxInstDelayUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxInstDelayUnit.FormattingEnabled = true;
			this.cbxInstDelayUnit.Items.AddRange(new object[] {
            "frames",
            "seconds"});
			this.cbxInstDelayUnit.Location = new System.Drawing.Point(162, 7);
			this.cbxInstDelayUnit.Name = "cbxInstDelayUnit";
			this.cbxInstDelayUnit.Size = new System.Drawing.Size(70, 21);
			this.cbxInstDelayUnit.TabIndex = 30;
			this.cbxInstDelayUnit.SelectedIndexChanged += new System.EventHandler(this.cbxInstDelayUnit_SelectedIndexChanged);
			// 
			// cbxSignalType
			// 
			this.cbxSignalType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxSignalType.FormattingEnabled = true;
			this.cbxSignalType.Items.AddRange(new object[] {
            "Good Signal",
            "Underexposed",
            "Trailed"});
			this.cbxSignalType.Location = new System.Drawing.Point(322, 12);
			this.cbxSignalType.Name = "cbxSignalType";
			this.cbxSignalType.Size = new System.Drawing.Size(98, 21);
			this.cbxSignalType.TabIndex = 32;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(14, 15);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(86, 13);
			this.label10.TabIndex = 23;
			this.label10.Text = "Expected motion";
			// 
			// pnlIntegration
			// 
			this.pnlIntegration.Controls.Add(this.btnDetectIntegration);
			this.pnlIntegration.Controls.Add(this.label14);
			this.pnlIntegration.Controls.Add(this.nudIntegratedFrames);
			this.pnlIntegration.Controls.Add(this.label13);
			this.pnlIntegration.Location = new System.Drawing.Point(37, 93);
			this.pnlIntegration.Name = "pnlIntegration";
			this.pnlIntegration.Size = new System.Drawing.Size(374, 29);
			this.pnlIntegration.TabIndex = 31;
			// 
			// btnDetectIntegration
			// 
			this.btnDetectIntegration.Location = new System.Drawing.Point(174, 2);
			this.btnDetectIntegration.Name = "btnDetectIntegration";
			this.btnDetectIntegration.Size = new System.Drawing.Size(143, 23);
			this.btnDetectIntegration.TabIndex = 33;
			this.btnDetectIntegration.Text = "Detect Used Integration";
			this.btnDetectIntegration.UseVisualStyleBackColor = true;
			this.btnDetectIntegration.Click += new System.EventHandler(this.btnDetectIntegration_Click);
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(130, 7);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(38, 13);
			this.label14.TabIndex = 32;
			this.label14.Text = "frames";
			// 
			// nudIntegratedFrames
			// 
			this.nudIntegratedFrames.Location = new System.Drawing.Point(68, 4);
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
			this.nudIntegratedFrames.Size = new System.Drawing.Size(56, 20);
			this.nudIntegratedFrames.TabIndex = 31;
			this.nudIntegratedFrames.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudIntegratedFrames.ValueChanged += new System.EventHandler(this.nudIntegratedFrames_ValueChanged);
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(7, 7);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(60, 13);
			this.label13.TabIndex = 30;
			this.label13.Text = "Integration:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(13, 139);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(95, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "Current frame time:";
			// 
			// ucUtcTimePicker
			// 
			this.ucUtcTimePicker.DateTimeUtc = new System.DateTime(2009, 8, 4, 12, 21, 39, 406);
			this.ucUtcTimePicker.Location = new System.Drawing.Point(105, 132);
			this.ucUtcTimePicker.Name = "ucUtcTimePicker";
			this.ucUtcTimePicker.Size = new System.Drawing.Size(232, 26);
			this.ucUtcTimePicker.TabIndex = 9;
			// 
			// cbxExpectedMotion
			// 
			this.cbxExpectedMotion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxExpectedMotion.FormattingEnabled = true;
			this.cbxExpectedMotion.Items.AddRange(new object[] {
            "Slow < 2.0 \"/min",
            "Slow Flyby (2.0\"/min -> 200.0 \"/min)",
            "Fast Flyby > 200.0\"/min"});
			this.cbxExpectedMotion.Location = new System.Drawing.Point(105, 12);
			this.cbxExpectedMotion.Name = "cbxExpectedMotion";
			this.cbxExpectedMotion.Size = new System.Drawing.Size(211, 21);
			this.cbxExpectedMotion.TabIndex = 24;
			this.cbxExpectedMotion.SelectedIndexChanged += new System.EventHandler(this.cbxExpectedMotion_SelectedIndexChanged);
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(22, 65);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(83, 13);
			this.label11.TabIndex = 26;
			this.label11.Text = "Current frame is:";
			// 
			// cbxFrameTimeType
			// 
			this.cbxFrameTimeType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxFrameTimeType.FormattingEnabled = true;
			this.cbxFrameTimeType.Items.AddRange(new object[] {
            "Non integrated frame",
            "First frame of integrated interval"});
			this.cbxFrameTimeType.Location = new System.Drawing.Point(105, 62);
			this.cbxFrameTimeType.Name = "cbxFrameTimeType";
			this.cbxFrameTimeType.Size = new System.Drawing.Size(184, 21);
			this.cbxFrameTimeType.TabIndex = 27;
			this.cbxFrameTimeType.SelectedIndexChanged += new System.EventHandler(this.cbxFrameTimeType_SelectedIndexChanged);
			// 
			// gbConfig
			// 
			this.gbConfig.Controls.Add(this.pnlPhotometry);
			this.gbConfig.Controls.Add(this.pnlAddins);
			this.gbConfig.Controls.Add(this.pnlTime);
			this.gbConfig.Controls.Add(this.pnlAstrometry);
			this.gbConfig.Location = new System.Drawing.Point(13, 13);
			this.gbConfig.Name = "gbConfig";
			this.gbConfig.Size = new System.Drawing.Size(441, 282);
			this.gbConfig.TabIndex = 16;
			this.gbConfig.TabStop = false;
			this.gbConfig.Text = "Object and Time";
			// 
			// pnlAddins
			// 
			this.pnlAddins.Controls.Add(this.clbAddinsToRun);
			this.pnlAddins.Location = new System.Drawing.Point(6, 14);
			this.pnlAddins.Name = "pnlAddins";
			this.pnlAddins.Size = new System.Drawing.Size(410, 243);
			this.pnlAddins.TabIndex = 36;
			// 
			// clbAddinsToRun
			// 
			this.clbAddinsToRun.CheckOnClick = true;
			this.clbAddinsToRun.FormattingEnabled = true;
			this.clbAddinsToRun.Location = new System.Drawing.Point(10, 11);
			this.clbAddinsToRun.Name = "clbAddinsToRun";
			this.clbAddinsToRun.Size = new System.Drawing.Size(372, 214);
			this.clbAddinsToRun.TabIndex = 1;
			// 
			// frmRunMultiFrameMeasurements
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(466, 332);
			this.Controls.Add(this.gbConfig);
			this.Controls.Add(this.btnPrevious);
			this.Controls.Add(this.cbxExport);
			this.Controls.Add(this.pnlExportConfig);
			this.Controls.Add(this.btnNext);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmRunMultiFrameMeasurements";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Multi-Frame Measurements";
			this.Load += new System.EventHandler(this.frmRunMultiFrameMeasurements_Load);
			this.pnlExportConfig.ResumeLayout(false);
			this.pnlExportConfig.PerformLayout();
			this.pnlExportPhotometry.ResumeLayout(false);
			this.pnlExportPhotometry.PerformLayout();
			this.pnlPhotometry.ResumeLayout(false);
			this.pnlPhotometry.PerformLayout();
			this.pnlPhotometryMethods.ResumeLayout(false);
			this.pnlPhotometryMethods.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudAperture)).EndInit();
			this.pnlAstrometry.ResumeLayout(false);
			this.pnlAstrometry.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudNumberMeasurements)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxStdDev)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMinStars)).EndInit();
			this.pnlTime.ResumeLayout(false);
			this.pnlTime.PerformLayout();
			this.pnlInstrDelay.ResumeLayout(false);
			this.pnlInstrDelay.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudInstrDelay)).EndInit();
			this.pnlIntegration.ResumeLayout(false);
			this.pnlIntegration.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudIntegratedFrames)).EndInit();
			this.gbConfig.ResumeLayout(false);
			this.pnlAddins.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnNext;
		private System.Windows.Forms.CheckBox cbxExport;
		private System.Windows.Forms.Panel pnlExportConfig;
		private System.Windows.Forms.CheckBox cbxExportPhotometry;
		private System.Windows.Forms.CheckBox cbxExportAstrometry;
		private System.Windows.Forms.Panel pnlExportPhotometry;
		private System.Windows.Forms.Button btnConfigureAstrometryExport;
		private System.Windows.Forms.Button btnConfigurePhotometryExport;
		private System.Windows.Forms.Button btnPrevious;
		private System.Windows.Forms.Panel pnlPhotometry;
		private System.Windows.Forms.CheckBox cbxFitMagnitudes;
		private System.Windows.Forms.Panel pnlPhotometryMethods;
		private System.Windows.Forms.ComboBox cbxCatalogPhotometryBand;
		private System.Windows.Forms.Label lblCatBand;
		private System.Windows.Forms.Label label53;
		private System.Windows.Forms.ComboBox cbxOutputMagBand;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.ComboBox cbxReductionType;
		private System.Windows.Forms.ComboBox cbxBackgroundMethod;
		private System.Windows.Forms.Panel pnlAstrometry;
		private System.Windows.Forms.Label label5;
		private ucFrameInterval ucFrameInterval;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown nudNumberMeasurements;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown nudMaxStdDev;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown nudMinStars;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox cbxStopOnNoFit;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Panel pnlTime;
		private System.Windows.Forms.Panel pnlInstrDelay;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.NumericUpDown nudInstrDelay;
		private System.Windows.Forms.ComboBox cbxInstDelayUnit;
		private System.Windows.Forms.ComboBox cbxSignalType;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Panel pnlIntegration;
		private System.Windows.Forms.Button btnDetectIntegration;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.NumericUpDown nudIntegratedFrames;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label4;
		private ucUtcTimePicker ucUtcTimePicker;
		private System.Windows.Forms.ComboBox cbxExpectedMotion;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.ComboBox cbxFrameTimeType;
		private System.Windows.Forms.GroupBox gbConfig;
		private System.Windows.Forms.LinkLabel linkExamples;
		private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Panel pnlAddins;
        private System.Windows.Forms.CheckedListBox clbAddinsToRun;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.NumericUpDown nudAperture;
		private System.Windows.Forms.Label label16;

	}
}
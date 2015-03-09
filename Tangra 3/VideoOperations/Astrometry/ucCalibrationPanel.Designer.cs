namespace Tangra.VideoOperations.Astrometry
{
	partial class ucCalibrationPanel
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbRotation = new System.Windows.Forms.TrackBar();
            this.tbFocalLength = new System.Windows.Forms.TrackBar();
            this.lblFocalLengthValue = new System.Windows.Forms.Label();
            this.lblRotationValue = new System.Windows.Forms.Label();
            this.lblAspectValue = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbAspect = new System.Windows.Forms.TrackBar();
            this.btnSolve = new System.Windows.Forms.Button();
            this.lblLimitMagnitude = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbLimitMagnitude = new System.Windows.Forms.TrackBar();
            this.pnlSolve = new System.Windows.Forms.Panel();
            this.btnSendProblemFit = new System.Windows.Forms.Button();
            this.rbIdentify3Stars = new System.Windows.Forms.RadioButton();
            this.rbDoManualFit = new System.Windows.Forms.RadioButton();
            this.cbxShowGrid = new System.Windows.Forms.CheckBox();
            this.pnlDebugFits = new System.Windows.Forms.Panel();
            this.lblPixSize = new System.Windows.Forms.Label();
            this.rbPrelim = new System.Windows.Forms.RadioButton();
            this.rbDistImpr = new System.Windows.Forms.RadioButton();
            this.rbFirstFit = new System.Windows.Forms.RadioButton();
            this.rbDist = new System.Windows.Forms.RadioButton();
            this.rbSecondFit = new System.Windows.Forms.RadioButton();
            this.pnlPrepare = new System.Windows.Forms.Panel();
            this.rbAuto = new System.Windows.Forms.RadioButton();
            this.rbManual = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.trbarDepth = new System.Windows.Forms.TrackBar();
            this.btnSolveConfiguration = new System.Windows.Forms.Button();
            this.pnlFinished = new System.Windows.Forms.Panel();
            this.pnlSelectedStar = new System.Windows.Forms.Panel();
            this.lblStarNoDesrc = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lblStarNo = new System.Windows.Forms.Label();
            this.lblResDE = new System.Windows.Forms.Label();
            this.lblRA = new System.Windows.Forms.Label();
            this.lblResRADesc = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblResRA = new System.Windows.Forms.Label();
            this.lblDE = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblY = new System.Windows.Forms.Label();
            this.btnShowPSF = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.lblX = new System.Windows.Forms.Label();
            this.lblStdDevDe = new System.Windows.Forms.Label();
            this.lblStdDevRa = new System.Windows.Forms.Label();
            this.cbxShowMagnitudes2 = new System.Windows.Forms.CheckBox();
            this.cbxShowGrid2 = new System.Windows.Forms.CheckBox();
            this.cbxShowLabels2 = new System.Windows.Forms.CheckBox();
            this.lblLimitMagnitude2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tbLimitMagnitude2 = new System.Windows.Forms.TrackBar();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbOSDExclusion = new System.Windows.Forms.RadioButton();
            this.rbInclusion = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.tbRotation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbFocalLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAspect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbLimitMagnitude)).BeginInit();
            this.pnlSolve.SuspendLayout();
            this.pnlDebugFits.SuspendLayout();
            this.pnlPrepare.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trbarDepth)).BeginInit();
            this.pnlFinished.SuspendLayout();
            this.pnlSelectedStar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbLimitMagnitude2)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(4, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Rotation";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(4, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Focal Length";
            // 
            // tbRotation
            // 
            this.tbRotation.Location = new System.Drawing.Point(-2, 74);
            this.tbRotation.Maximum = 180;
            this.tbRotation.Minimum = -180;
            this.tbRotation.Name = "tbRotation";
            this.tbRotation.Size = new System.Drawing.Size(236, 42);
            this.tbRotation.TabIndex = 10;
            this.tbRotation.TickFrequency = 10;
            this.tbRotation.ValueChanged += new System.EventHandler(this.tbRotation_ValueChanged);
            // 
            // tbFocalLength
            // 
            this.tbFocalLength.Location = new System.Drawing.Point(-2, 18);
            this.tbFocalLength.Maximum = 20000;
            this.tbFocalLength.Name = "tbFocalLength";
            this.tbFocalLength.Size = new System.Drawing.Size(236, 42);
            this.tbFocalLength.TabIndex = 9;
            this.tbFocalLength.TickFrequency = 10;
            this.tbFocalLength.Value = 10;
            this.tbFocalLength.ValueChanged += new System.EventHandler(this.tbFocalLength_ValueChanged);
            // 
            // lblFocalLengthValue
            // 
            this.lblFocalLengthValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
            this.lblFocalLengthValue.Location = new System.Drawing.Point(145, 4);
            this.lblFocalLengthValue.Name = "lblFocalLengthValue";
            this.lblFocalLengthValue.Size = new System.Drawing.Size(81, 13);
            this.lblFocalLengthValue.TabIndex = 13;
            this.lblFocalLengthValue.Text = "600 mm";
            this.lblFocalLengthValue.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblRotationValue
            // 
            this.lblRotationValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
            this.lblRotationValue.Location = new System.Drawing.Point(145, 61);
            this.lblRotationValue.Name = "lblRotationValue";
            this.lblRotationValue.Size = new System.Drawing.Size(81, 13);
            this.lblRotationValue.TabIndex = 14;
            this.lblRotationValue.Text = "0 deg";
            this.lblRotationValue.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblAspectValue
            // 
            this.lblAspectValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
            this.lblAspectValue.Location = new System.Drawing.Point(145, 117);
            this.lblAspectValue.Name = "lblAspectValue";
            this.lblAspectValue.Size = new System.Drawing.Size(81, 13);
            this.lblAspectValue.TabIndex = 17;
            this.lblAspectValue.Text = "0.754";
            this.lblAspectValue.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(4, 117);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "X/Y Aspect";
            // 
            // tbAspect
            // 
            this.tbAspect.Location = new System.Drawing.Point(-2, 130);
            this.tbAspect.Maximum = 200;
            this.tbAspect.Minimum = -200;
            this.tbAspect.Name = "tbAspect";
            this.tbAspect.Size = new System.Drawing.Size(236, 42);
            this.tbAspect.TabIndex = 15;
            this.tbAspect.TickFrequency = 10;
            this.tbAspect.ValueChanged += new System.EventHandler(this.tbAspect_ValueChanged);
            // 
            // btnSolve
            // 
            this.btnSolve.Location = new System.Drawing.Point(123, 275);
            this.btnSolve.Name = "btnSolve";
            this.btnSolve.Size = new System.Drawing.Size(103, 23);
            this.btnSolve.TabIndex = 18;
            this.btnSolve.Text = "Solve Plate";
            this.btnSolve.UseVisualStyleBackColor = true;
            this.btnSolve.Visible = false;
            this.btnSolve.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnSolve_MouseClick);
            // 
            // lblLimitMagnitude
            // 
            this.lblLimitMagnitude.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
            this.lblLimitMagnitude.Location = new System.Drawing.Point(145, 172);
            this.lblLimitMagnitude.Name = "lblLimitMagnitude";
            this.lblLimitMagnitude.Size = new System.Drawing.Size(81, 13);
            this.lblLimitMagnitude.TabIndex = 21;
            this.lblLimitMagnitude.Text = "0.754";
            this.lblLimitMagnitude.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(4, 172);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(113, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Limiting Magnitude";
            // 
            // tbLimitMagnitude
            // 
            this.tbLimitMagnitude.Location = new System.Drawing.Point(-2, 185);
            this.tbLimitMagnitude.Maximum = 35;
            this.tbLimitMagnitude.Minimum = 8;
            this.tbLimitMagnitude.Name = "tbLimitMagnitude";
            this.tbLimitMagnitude.Size = new System.Drawing.Size(236, 42);
            this.tbLimitMagnitude.TabIndex = 19;
            this.tbLimitMagnitude.Value = 8;
            this.tbLimitMagnitude.ValueChanged += new System.EventHandler(this.tbLimitMagnitude_ValueChanged);
            // 
            // pnlSolve
            // 
            this.pnlSolve.Controls.Add(this.btnSendProblemFit);
            this.pnlSolve.Controls.Add(this.rbIdentify3Stars);
            this.pnlSolve.Controls.Add(this.rbDoManualFit);
            this.pnlSolve.Controls.Add(this.cbxShowGrid);
            this.pnlSolve.Controls.Add(this.label2);
            this.pnlSolve.Controls.Add(this.tbFocalLength);
            this.pnlSolve.Controls.Add(this.lblLimitMagnitude);
            this.pnlSolve.Controls.Add(this.tbRotation);
            this.pnlSolve.Controls.Add(this.label5);
            this.pnlSolve.Controls.Add(this.label3);
            this.pnlSolve.Controls.Add(this.tbLimitMagnitude);
            this.pnlSolve.Controls.Add(this.lblFocalLengthValue);
            this.pnlSolve.Controls.Add(this.btnSolve);
            this.pnlSolve.Controls.Add(this.lblRotationValue);
            this.pnlSolve.Controls.Add(this.lblAspectValue);
            this.pnlSolve.Controls.Add(this.tbAspect);
            this.pnlSolve.Controls.Add(this.label4);
            this.pnlSolve.Location = new System.Drawing.Point(220, 4);
            this.pnlSolve.Name = "pnlSolve";
            this.pnlSolve.Size = new System.Drawing.Size(257, 354);
            this.pnlSolve.TabIndex = 23;
            // 
            // btnSendProblemFit
            // 
            this.btnSendProblemFit.BackColor = System.Drawing.Color.MistyRose;
            this.btnSendProblemFit.Location = new System.Drawing.Point(7, 316);
            this.btnSendProblemFit.Name = "btnSendProblemFit";
            this.btnSendProblemFit.Size = new System.Drawing.Size(219, 21);
            this.btnSendProblemFit.TabIndex = 42;
            this.btnSendProblemFit.Text = "Report Unsolved Plate";
            this.btnSendProblemFit.UseVisualStyleBackColor = false;
            this.btnSendProblemFit.Visible = false;
            this.btnSendProblemFit.Click += new System.EventHandler(this.btnSendProblemFit_Click);
            // 
            // rbIdentify3Stars
            // 
            this.rbIdentify3Stars.AutoSize = true;
            this.rbIdentify3Stars.Checked = true;
            this.rbIdentify3Stars.Location = new System.Drawing.Point(19, 227);
            this.rbIdentify3Stars.Name = "rbIdentify3Stars";
            this.rbIdentify3Stars.Size = new System.Drawing.Size(153, 17);
            this.rbIdentify3Stars.TabIndex = 29;
            this.rbIdentify3Stars.TabStop = true;
            this.rbIdentify3Stars.Text = "Solve by Identifying 3 Stars";
            this.rbIdentify3Stars.UseVisualStyleBackColor = true;
            this.rbIdentify3Stars.CheckedChanged += new System.EventHandler(this.rbIdentify3Stars_CheckedChanged);
            // 
            // rbDoManualFit
            // 
            this.rbDoManualFit.AutoSize = true;
            this.rbDoManualFit.Location = new System.Drawing.Point(19, 252);
            this.rbDoManualFit.Name = "rbDoManualFit";
            this.rbDoManualFit.Size = new System.Drawing.Size(96, 17);
            this.rbDoManualFit.TabIndex = 28;
            this.rbDoManualFit.Text = "Do a manual fit";
            this.rbDoManualFit.UseVisualStyleBackColor = true;
            this.rbDoManualFit.CheckedChanged += new System.EventHandler(this.rbDoManualFit_CheckedChanged);
            // 
            // cbxShowGrid
            // 
            this.cbxShowGrid.AutoSize = true;
            this.cbxShowGrid.Location = new System.Drawing.Point(40, 279);
            this.cbxShowGrid.Name = "cbxShowGrid";
            this.cbxShowGrid.Size = new System.Drawing.Size(75, 17);
            this.cbxShowGrid.TabIndex = 26;
            this.cbxShowGrid.Text = "Show Grid";
            this.cbxShowGrid.UseVisualStyleBackColor = true;
            this.cbxShowGrid.Visible = false;
            this.cbxShowGrid.CheckedChanged += new System.EventHandler(this.cbxShowGrid_CheckedChanged);
            // 
            // pnlDebugFits
            // 
            this.pnlDebugFits.Controls.Add(this.lblPixSize);
            this.pnlDebugFits.Controls.Add(this.rbPrelim);
            this.pnlDebugFits.Controls.Add(this.rbDistImpr);
            this.pnlDebugFits.Controls.Add(this.rbFirstFit);
            this.pnlDebugFits.Controls.Add(this.rbDist);
            this.pnlDebugFits.Controls.Add(this.rbSecondFit);
            this.pnlDebugFits.Location = new System.Drawing.Point(3, 263);
            this.pnlDebugFits.Name = "pnlDebugFits";
            this.pnlDebugFits.Size = new System.Drawing.Size(210, 44);
            this.pnlDebugFits.TabIndex = 26;
            // 
            // lblPixSize
            // 
            this.lblPixSize.AutoSize = true;
            this.lblPixSize.Location = new System.Drawing.Point(3, 24);
            this.lblPixSize.Name = "lblPixSize";
            this.lblPixSize.Size = new System.Drawing.Size(0, 13);
            this.lblPixSize.TabIndex = 28;
            // 
            // rbPrelim
            // 
            this.rbPrelim.AutoSize = true;
            this.rbPrelim.Location = new System.Drawing.Point(3, 3);
            this.rbPrelim.Name = "rbPrelim";
            this.rbPrelim.Size = new System.Drawing.Size(32, 17);
            this.rbPrelim.TabIndex = 33;
            this.rbPrelim.TabStop = true;
            this.rbPrelim.Text = "P";
            this.rbPrelim.UseVisualStyleBackColor = true;
            this.rbPrelim.CheckedChanged += new System.EventHandler(this.ChangePlottedFit);
            // 
            // rbDistImpr
            // 
            this.rbDistImpr.AutoSize = true;
            this.rbDistImpr.Checked = true;
            this.rbDistImpr.Location = new System.Drawing.Point(154, 4);
            this.rbDistImpr.Name = "rbDistImpr";
            this.rbDistImpr.Size = new System.Drawing.Size(36, 17);
            this.rbDistImpr.TabIndex = 33;
            this.rbDistImpr.TabStop = true;
            this.rbDistImpr.Text = "DI";
            this.rbDistImpr.UseVisualStyleBackColor = true;
            this.rbDistImpr.CheckedChanged += new System.EventHandler(this.ChangePlottedFit);
            // 
            // rbFirstFit
            // 
            this.rbFirstFit.AutoSize = true;
            this.rbFirstFit.Location = new System.Drawing.Point(37, 4);
            this.rbFirstFit.Name = "rbFirstFit";
            this.rbFirstFit.Size = new System.Drawing.Size(37, 17);
            this.rbFirstFit.TabIndex = 28;
            this.rbFirstFit.TabStop = true;
            this.rbFirstFit.Text = "F1";
            this.rbFirstFit.UseVisualStyleBackColor = true;
            this.rbFirstFit.CheckedChanged += new System.EventHandler(this.ChangePlottedFit);
            // 
            // rbDist
            // 
            this.rbDist.AutoSize = true;
            this.rbDist.Location = new System.Drawing.Point(121, 4);
            this.rbDist.Name = "rbDist";
            this.rbDist.Size = new System.Drawing.Size(33, 17);
            this.rbDist.TabIndex = 32;
            this.rbDist.TabStop = true;
            this.rbDist.Text = "D";
            this.rbDist.UseVisualStyleBackColor = true;
            this.rbDist.CheckedChanged += new System.EventHandler(this.ChangePlottedFit);
            // 
            // rbSecondFit
            // 
            this.rbSecondFit.AutoSize = true;
            this.rbSecondFit.Location = new System.Drawing.Point(78, 4);
            this.rbSecondFit.Name = "rbSecondFit";
            this.rbSecondFit.Size = new System.Drawing.Size(37, 17);
            this.rbSecondFit.TabIndex = 29;
            this.rbSecondFit.TabStop = true;
            this.rbSecondFit.Text = "F2";
            this.rbSecondFit.UseVisualStyleBackColor = true;
            this.rbSecondFit.CheckedChanged += new System.EventHandler(this.ChangePlottedFit);
            // 
            // pnlPrepare
            // 
            this.pnlPrepare.Controls.Add(this.groupBox1);
            this.pnlPrepare.Controls.Add(this.rbAuto);
            this.pnlPrepare.Controls.Add(this.rbManual);
            this.pnlPrepare.Controls.Add(this.label1);
            this.pnlPrepare.Controls.Add(this.trbarDepth);
            this.pnlPrepare.Controls.Add(this.btnSolveConfiguration);
            this.pnlPrepare.Location = new System.Drawing.Point(3, 4);
            this.pnlPrepare.Name = "pnlPrepare";
            this.pnlPrepare.Size = new System.Drawing.Size(199, 292);
            this.pnlPrepare.TabIndex = 24;
            // 
            // rbAuto
            // 
            this.rbAuto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.rbAuto.AutoSize = true;
            this.rbAuto.Location = new System.Drawing.Point(18, 118);
            this.rbAuto.Name = "rbAuto";
            this.rbAuto.Size = new System.Drawing.Size(46, 17);
            this.rbAuto.TabIndex = 85;
            this.rbAuto.Text = "auto";
            this.rbAuto.UseVisualStyleBackColor = true;
            // 
            // rbManual
            // 
            this.rbManual.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.rbManual.AutoSize = true;
            this.rbManual.Checked = true;
            this.rbManual.Location = new System.Drawing.Point(70, 118);
            this.rbManual.Name = "rbManual";
            this.rbManual.Size = new System.Drawing.Size(59, 17);
            this.rbManual.TabIndex = 84;
            this.rbManual.TabStop = true;
            this.rbManual.Text = "manual";
            this.rbManual.UseVisualStyleBackColor = true;
            this.rbManual.CheckedChanged += new System.EventHandler(this.rbManual_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(15, 100);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 75;
            this.label1.Text = "Star Finder Depth";
            // 
            // trbarDepth
            // 
            this.trbarDepth.Location = new System.Drawing.Point(11, 141);
            this.trbarDepth.Minimum = 2;
            this.trbarDepth.Name = "trbarDepth";
            this.trbarDepth.Size = new System.Drawing.Size(163, 42);
            this.trbarDepth.TabIndex = 74;
            this.trbarDepth.Value = 2;
            this.trbarDepth.ValueChanged += new System.EventHandler(this.trbarDepth_ValueChanged);
            // 
            // btnSolveConfiguration
            // 
            this.btnSolveConfiguration.Location = new System.Drawing.Point(18, 195);
            this.btnSolveConfiguration.Name = "btnSolveConfiguration";
            this.btnSolveConfiguration.Size = new System.Drawing.Size(146, 23);
            this.btnSolveConfiguration.TabIndex = 0;
            this.btnSolveConfiguration.Text = "Calibrate Configuration";
            this.btnSolveConfiguration.UseVisualStyleBackColor = true;
            this.btnSolveConfiguration.Click += new System.EventHandler(this.btnSolveConfiguration_Click);
            // 
            // pnlFinished
            // 
            this.pnlFinished.Controls.Add(this.pnlSelectedStar);
            this.pnlFinished.Controls.Add(this.lblStdDevDe);
            this.pnlFinished.Controls.Add(this.lblStdDevRa);
            this.pnlFinished.Controls.Add(this.cbxShowMagnitudes2);
            this.pnlFinished.Controls.Add(this.cbxShowGrid2);
            this.pnlFinished.Controls.Add(this.pnlDebugFits);
            this.pnlFinished.Controls.Add(this.cbxShowLabels2);
            this.pnlFinished.Controls.Add(this.lblLimitMagnitude2);
            this.pnlFinished.Controls.Add(this.label7);
            this.pnlFinished.Controls.Add(this.tbLimitMagnitude2);
            this.pnlFinished.Location = new System.Drawing.Point(493, 4);
            this.pnlFinished.Name = "pnlFinished";
            this.pnlFinished.Size = new System.Drawing.Size(257, 354);
            this.pnlFinished.TabIndex = 25;
            // 
            // pnlSelectedStar
            // 
            this.pnlSelectedStar.Controls.Add(this.lblStarNoDesrc);
            this.pnlSelectedStar.Controls.Add(this.label13);
            this.pnlSelectedStar.Controls.Add(this.lblStarNo);
            this.pnlSelectedStar.Controls.Add(this.lblResDE);
            this.pnlSelectedStar.Controls.Add(this.lblRA);
            this.pnlSelectedStar.Controls.Add(this.lblResRADesc);
            this.pnlSelectedStar.Controls.Add(this.label6);
            this.pnlSelectedStar.Controls.Add(this.lblResRA);
            this.pnlSelectedStar.Controls.Add(this.lblDE);
            this.pnlSelectedStar.Controls.Add(this.label9);
            this.pnlSelectedStar.Controls.Add(this.label8);
            this.pnlSelectedStar.Controls.Add(this.lblY);
            this.pnlSelectedStar.Controls.Add(this.btnShowPSF);
            this.pnlSelectedStar.Controls.Add(this.label11);
            this.pnlSelectedStar.Controls.Add(this.lblX);
            this.pnlSelectedStar.Location = new System.Drawing.Point(7, 7);
            this.pnlSelectedStar.Name = "pnlSelectedStar";
            this.pnlSelectedStar.Size = new System.Drawing.Size(237, 157);
            this.pnlSelectedStar.TabIndex = 50;
            this.pnlSelectedStar.Visible = false;
            // 
            // lblStarNoDesrc
            // 
            this.lblStarNoDesrc.AutoSize = true;
            this.lblStarNoDesrc.Location = new System.Drawing.Point(3, 4);
            this.lblStarNoDesrc.Name = "lblStarNoDesrc";
            this.lblStarNoDesrc.Size = new System.Drawing.Size(46, 13);
            this.lblStarNoDesrc.TabIndex = 35;
            this.lblStarNoDesrc.Text = "Star No:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 82);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(47, 13);
            this.label13.TabIndex = 47;
            this.label13.Text = "Res.DE:";
            // 
            // lblStarNo
            // 
            this.lblStarNo.AutoSize = true;
            this.lblStarNo.Location = new System.Drawing.Point(55, 4);
            this.lblStarNo.Name = "lblStarNo";
            this.lblStarNo.Size = new System.Drawing.Size(0, 13);
            this.lblStarNo.TabIndex = 36;
            // 
            // lblResDE
            // 
            this.lblResDE.AutoSize = true;
            this.lblResDE.Location = new System.Drawing.Point(49, 82);
            this.lblResDE.Name = "lblResDE";
            this.lblResDE.Size = new System.Drawing.Size(39, 13);
            this.lblResDE.TabIndex = 49;
            this.lblResDE.Text = "+1.04\"";
            // 
            // lblRA
            // 
            this.lblRA.AutoSize = true;
            this.lblRA.Location = new System.Drawing.Point(139, 25);
            this.lblRA.Name = "lblRA";
            this.lblRA.Size = new System.Drawing.Size(83, 13);
            this.lblRA.TabIndex = 39;
            this.lblRA.Text = "12h 34m 23.23s";
            // 
            // lblResRADesc
            // 
            this.lblResRADesc.AutoSize = true;
            this.lblResRADesc.Location = new System.Drawing.Point(3, 67);
            this.lblResRADesc.Name = "lblResRADesc";
            this.lblResRADesc.Size = new System.Drawing.Size(47, 13);
            this.lblResRADesc.TabIndex = 46;
            this.lblResRADesc.Text = "Res.RA:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(118, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(25, 13);
            this.label6.TabIndex = 37;
            this.label6.Text = "RA:";
            // 
            // lblResRA
            // 
            this.lblResRA.AutoSize = true;
            this.lblResRA.Location = new System.Drawing.Point(51, 67);
            this.lblResRA.Name = "lblResRA";
            this.lblResRA.Size = new System.Drawing.Size(36, 13);
            this.lblResRA.TabIndex = 48;
            this.lblResRA.Text = "-0.23\"";
            // 
            // lblDE
            // 
            this.lblDE.AutoSize = true;
            this.lblDE.Location = new System.Drawing.Point(141, 43);
            this.lblDE.Name = "lblDE";
            this.lblDE.Size = new System.Drawing.Size(74, 13);
            this.lblDE.TabIndex = 40;
            this.lblDE.Text = "-23o 34\' 12.5\"";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 43);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(17, 13);
            this.label9.TabIndex = 43;
            this.label9.Text = "Y:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(118, 43);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(25, 13);
            this.label8.TabIndex = 38;
            this.label8.Text = "DE:";
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.Location = new System.Drawing.Point(26, 43);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(34, 13);
            this.lblY.TabIndex = 45;
            this.lblY.Text = "12.45";
            // 
            // btnShowPSF
            // 
            this.btnShowPSF.Location = new System.Drawing.Point(6, 117);
            this.btnShowPSF.Name = "btnShowPSF";
            this.btnShowPSF.Size = new System.Drawing.Size(72, 21);
            this.btnShowPSF.TabIndex = 41;
            this.btnShowPSF.Text = "Show PSF";
            this.btnShowPSF.UseVisualStyleBackColor = true;
            this.btnShowPSF.Click += new System.EventHandler(this.btnShowPSF_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(5, 25);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 13);
            this.label11.TabIndex = 42;
            this.label11.Text = "X:";
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Location = new System.Drawing.Point(26, 25);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(40, 13);
            this.lblX.TabIndex = 44;
            this.lblX.Text = "234.57";
            // 
            // lblStdDevDe
            // 
            this.lblStdDevDe.AutoSize = true;
            this.lblStdDevDe.Location = new System.Drawing.Point(134, 240);
            this.lblStdDevDe.Name = "lblStdDevDe";
            this.lblStdDevDe.Size = new System.Drawing.Size(0, 13);
            this.lblStdDevDe.TabIndex = 34;
            // 
            // lblStdDevRa
            // 
            this.lblStdDevRa.AutoSize = true;
            this.lblStdDevRa.Location = new System.Drawing.Point(134, 223);
            this.lblStdDevRa.Name = "lblStdDevRa";
            this.lblStdDevRa.Size = new System.Drawing.Size(0, 13);
            this.lblStdDevRa.TabIndex = 33;
            // 
            // cbxShowMagnitudes2
            // 
            this.cbxShowMagnitudes2.AutoSize = true;
            this.cbxShowMagnitudes2.Location = new System.Drawing.Point(3, 240);
            this.cbxShowMagnitudes2.Name = "cbxShowMagnitudes2";
            this.cbxShowMagnitudes2.Size = new System.Drawing.Size(111, 17);
            this.cbxShowMagnitudes2.TabIndex = 29;
            this.cbxShowMagnitudes2.Text = "Show Magnitudes";
            this.cbxShowMagnitudes2.UseVisualStyleBackColor = true;
            this.cbxShowMagnitudes2.Visible = false;
            this.cbxShowMagnitudes2.CheckedChanged += new System.EventHandler(this.cbxShowMagnitudes2_CheckedChanged);
            // 
            // cbxShowGrid2
            // 
            this.cbxShowGrid2.AutoSize = true;
            this.cbxShowGrid2.Location = new System.Drawing.Point(96, 223);
            this.cbxShowGrid2.Name = "cbxShowGrid2";
            this.cbxShowGrid2.Size = new System.Drawing.Size(75, 17);
            this.cbxShowGrid2.TabIndex = 28;
            this.cbxShowGrid2.Text = "Show Grid";
            this.cbxShowGrid2.UseVisualStyleBackColor = true;
            this.cbxShowGrid2.Visible = false;
            this.cbxShowGrid2.CheckedChanged += new System.EventHandler(this.cbxShowGrid2_CheckedChanged);
            // 
            // cbxShowLabels2
            // 
            this.cbxShowLabels2.AutoSize = true;
            this.cbxShowLabels2.Location = new System.Drawing.Point(3, 223);
            this.cbxShowLabels2.Name = "cbxShowLabels2";
            this.cbxShowLabels2.Size = new System.Drawing.Size(87, 17);
            this.cbxShowLabels2.TabIndex = 24;
            this.cbxShowLabels2.Text = "Show Labels";
            this.cbxShowLabels2.UseVisualStyleBackColor = true;
            this.cbxShowLabels2.Visible = false;
            this.cbxShowLabels2.CheckedChanged += new System.EventHandler(this.cbxShowLabels2_CheckedChanged);
            // 
            // lblLimitMagnitude2
            // 
            this.lblLimitMagnitude2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
            this.lblLimitMagnitude2.Location = new System.Drawing.Point(148, 171);
            this.lblLimitMagnitude2.Name = "lblLimitMagnitude2";
            this.lblLimitMagnitude2.Size = new System.Drawing.Size(81, 13);
            this.lblLimitMagnitude2.TabIndex = 32;
            this.lblLimitMagnitude2.Text = "0.754";
            this.lblLimitMagnitude2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(7, 171);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(113, 13);
            this.label7.TabIndex = 31;
            this.label7.Text = "Limiting Magnitude";
            // 
            // tbLimitMagnitude2
            // 
            this.tbLimitMagnitude2.Location = new System.Drawing.Point(1, 184);
            this.tbLimitMagnitude2.Maximum = 35;
            this.tbLimitMagnitude2.Minimum = 8;
            this.tbLimitMagnitude2.Name = "tbLimitMagnitude2";
            this.tbLimitMagnitude2.Size = new System.Drawing.Size(236, 42);
            this.tbLimitMagnitude2.TabIndex = 30;
            this.tbLimitMagnitude2.Value = 8;
            this.tbLimitMagnitude2.ValueChanged += new System.EventHandler(this.tbLimitMagnitude2_ValueChanged);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "plate";
            this.saveFileDialog.Filter = "Tangra Plate (*.plate)|*.plate";
            // 
            // timer1
            // 
            this.timer1.Interval = 250;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbInclusion);
            this.groupBox1.Controls.Add(this.rbOSDExclusion);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(18, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(156, 73);
            this.groupBox1.TabIndex = 86;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Area Definition";
            // 
            // rbOSDExclusion
            // 
            this.rbOSDExclusion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.rbOSDExclusion.AutoSize = true;
            this.rbOSDExclusion.Checked = true;
            this.rbOSDExclusion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbOSDExclusion.Location = new System.Drawing.Point(15, 23);
            this.rbOSDExclusion.Name = "rbOSDExclusion";
            this.rbOSDExclusion.Size = new System.Drawing.Size(96, 17);
            this.rbOSDExclusion.TabIndex = 86;
            this.rbOSDExclusion.TabStop = true;
            this.rbOSDExclusion.Text = "OSD Exclusion";
            this.rbOSDExclusion.UseVisualStyleBackColor = true;
            // 
            // rbInclusion
            // 
            this.rbInclusion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.rbInclusion.AutoSize = true;
            this.rbInclusion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbInclusion.Location = new System.Drawing.Point(15, 47);
            this.rbInclusion.Name = "rbInclusion";
            this.rbInclusion.Size = new System.Drawing.Size(67, 17);
            this.rbInclusion.TabIndex = 87;
            this.rbInclusion.Text = "Inclusion";
            this.rbInclusion.UseVisualStyleBackColor = true;
            // 
            // ucCalibrationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlFinished);
            this.Controls.Add(this.pnlPrepare);
            this.Controls.Add(this.pnlSolve);
            this.Name = "ucCalibrationPanel";
            this.Size = new System.Drawing.Size(753, 361);
            ((System.ComponentModel.ISupportInitialize)(this.tbRotation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbFocalLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAspect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbLimitMagnitude)).EndInit();
            this.pnlSolve.ResumeLayout(false);
            this.pnlSolve.PerformLayout();
            this.pnlDebugFits.ResumeLayout(false);
            this.pnlDebugFits.PerformLayout();
            this.pnlPrepare.ResumeLayout(false);
            this.pnlPrepare.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trbarDepth)).EndInit();
            this.pnlFinished.ResumeLayout(false);
            this.pnlFinished.PerformLayout();
            this.pnlSelectedStar.ResumeLayout(false);
            this.pnlSelectedStar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbLimitMagnitude2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel pnlFinished;
		private System.Windows.Forms.CheckBox cbxShowLabels2;
		private System.Windows.Forms.Panel pnlPrepare;
		private System.Windows.Forms.Button btnSolveConfiguration;
		private System.Windows.Forms.Panel pnlSolve;
		private System.Windows.Forms.CheckBox cbxShowGrid;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TrackBar tbFocalLength;
		private System.Windows.Forms.Label lblLimitMagnitude;
		private System.Windows.Forms.TrackBar tbRotation;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TrackBar tbLimitMagnitude;
		private System.Windows.Forms.Label lblFocalLengthValue;
		private System.Windows.Forms.Button btnSolve;
		private System.Windows.Forms.Label lblRotationValue;
		private System.Windows.Forms.Label lblAspectValue;
		private System.Windows.Forms.TrackBar tbAspect;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.RadioButton rbDistImpr;
		private System.Windows.Forms.RadioButton rbDist;
		private System.Windows.Forms.RadioButton rbSecondFit;
		private System.Windows.Forms.RadioButton rbFirstFit;
		private System.Windows.Forms.RadioButton rbPrelim;
		private System.Windows.Forms.Panel pnlDebugFits;
		private System.Windows.Forms.Label lblPixSize;
		private System.Windows.Forms.CheckBox cbxShowMagnitudes2;
		private System.Windows.Forms.CheckBox cbxShowGrid2;
		private System.Windows.Forms.Label lblLimitMagnitude2;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TrackBar tbLimitMagnitude2;
		private System.Windows.Forms.Label lblStdDevDe;
		private System.Windows.Forms.Label lblStdDevRa;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label lblDE;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label lblRA;
		private System.Windows.Forms.Label lblStarNo;
		private System.Windows.Forms.Label lblStarNoDesrc;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label lblResDE;
		private System.Windows.Forms.Label lblResRADesc;
		private System.Windows.Forms.Label lblResRA;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label lblY;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label lblX;
		private System.Windows.Forms.Button btnShowPSF;
		private System.Windows.Forms.Panel pnlSelectedStar;
		private System.Windows.Forms.RadioButton rbIdentify3Stars;
		private System.Windows.Forms.RadioButton rbDoManualFit;
        private System.Windows.Forms.Button btnSendProblemFit;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.TrackBar trbarDepth;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton rbAuto;
		private System.Windows.Forms.RadioButton rbManual;
		private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbInclusion;
        private System.Windows.Forms.RadioButton rbOSDExclusion;
	}
}

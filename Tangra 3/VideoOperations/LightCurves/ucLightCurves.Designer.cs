using Tangra.Model.Controls;

namespace Tangra.VideoOperations.LightCurves
{
    partial class ucLightCurves
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
			this.pnlProcessing = new System.Windows.Forms.Panel();
			this.lblUsedTracker = new System.Windows.Forms.Label();
			this.pnlEnterTimes = new System.Windows.Forms.Panel();
			this.btn1FrMinus = new System.Windows.Forms.Button();
			this.btn1FrPlus = new System.Windows.Forms.Button();
			this.btnShowFields = new System.Windows.Forms.Button();
			this.btnContinueWithNoTimes = new System.Windows.Forms.Button();
			this.btnNextTime = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.lblTimesHeader = new System.Windows.Forms.Label();
			this.ucUtcTime = new Tangra.Model.Controls.ucUtcTimePicker();
			this.btnLightCurve = new System.Windows.Forms.Button();
			this.btnStop = new System.Windows.Forms.Button();
			this.pnlMeasureZoomOptions = new System.Windows.Forms.Panel();
			this.rbDisplayPSFs = new System.Windows.Forms.RadioButton();
			this.rbDisplayPixels = new System.Windows.Forms.RadioButton();
			this.rbDisplayBrightness = new System.Windows.Forms.RadioButton();
			this.label3 = new System.Windows.Forms.Label();
			this.lblElapsedTime = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.lblSkippedFrames = new System.Windows.Forms.Label();
			this.lblProcessedFrames = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.lblUsedTrackerLabel = new System.Windows.Forms.Label();
			this.pnlUserAction = new System.Windows.Forms.Panel();
			this.lblMeasurementType = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.btnAddObject = new System.Windows.Forms.Button();
			this.btnEditObject = new System.Windows.Forms.Button();
			this.btnStartOver = new System.Windows.Forms.Button();
			this.btnStart = new System.Windows.Forms.Button();
			this.pnlSelectedObject = new System.Windows.Forms.Panel();
			this.btnAdjustApertures = new System.Windows.Forms.Button();
			this.rbM1 = new System.Windows.Forms.RadioButton();
			this.rbM2 = new System.Windows.Forms.RadioButton();
			this.rbM3 = new System.Windows.Forms.RadioButton();
			this.rbM4 = new System.Windows.Forms.RadioButton();
			this.lblM4 = new System.Windows.Forms.Label();
			this.lblM3 = new System.Windows.Forms.Label();
			this.lblM2 = new System.Windows.Forms.Label();
			this.lblM1 = new System.Windows.Forms.Label();
			this.lblObj = new System.Windows.Forms.Label();
			this.lblSel = new System.Windows.Forms.Label();
			this.lblBackgroundMethod = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.lblSignalMethod = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.pnlMeasuringSelection = new System.Windows.Forms.Panel();
			this.label8 = new System.Windows.Forms.Label();
			this.lblMeasuringStars = new System.Windows.Forms.Label();
			this.pnlTrackingSelection = new System.Windows.Forms.Panel();
			this.lblUserAction = new System.Windows.Forms.Label();
			this.lblTrackingStars = new System.Windows.Forms.Label();
			this.lblInfo = new System.Windows.Forms.Label();
			this.pnlViewLightCurve = new System.Windows.Forms.Panel();
			this.timerMoveToFirstFrame = new System.Windows.Forms.Timer(this.components);
			this.lblPartiallySuccessfulFrames = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.pnlProcessing.SuspendLayout();
			this.pnlEnterTimes.SuspendLayout();
			this.pnlMeasureZoomOptions.SuspendLayout();
			this.pnlUserAction.SuspendLayout();
			this.pnlSelectedObject.SuspendLayout();
			this.pnlMeasuringSelection.SuspendLayout();
			this.pnlTrackingSelection.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlProcessing
			// 
			this.pnlProcessing.Controls.Add(this.lblPartiallySuccessfulFrames);
			this.pnlProcessing.Controls.Add(this.label11);
			this.pnlProcessing.Controls.Add(this.lblUsedTracker);
			this.pnlProcessing.Controls.Add(this.pnlEnterTimes);
			this.pnlProcessing.Controls.Add(this.btnLightCurve);
			this.pnlProcessing.Controls.Add(this.btnStop);
			this.pnlProcessing.Controls.Add(this.pnlMeasureZoomOptions);
			this.pnlProcessing.Controls.Add(this.lblElapsedTime);
			this.pnlProcessing.Controls.Add(this.label7);
			this.pnlProcessing.Controls.Add(this.lblSkippedFrames);
			this.pnlProcessing.Controls.Add(this.lblProcessedFrames);
			this.pnlProcessing.Controls.Add(this.label2);
			this.pnlProcessing.Controls.Add(this.label1);
			this.pnlProcessing.Controls.Add(this.lblUsedTrackerLabel);
			this.pnlProcessing.Location = new System.Drawing.Point(3, 3);
			this.pnlProcessing.Name = "pnlProcessing";
			this.pnlProcessing.Size = new System.Drawing.Size(261, 304);
			this.pnlProcessing.TabIndex = 6;
			this.pnlProcessing.Tag = "";
			this.pnlProcessing.Visible = false;
			// 
			// lblUsedTracker
			// 
			this.lblUsedTracker.AutoSize = true;
			this.lblUsedTracker.Location = new System.Drawing.Point(116, 38);
			this.lblUsedTracker.Name = "lblUsedTracker";
			this.lblUsedTracker.Size = new System.Drawing.Size(0, 13);
			this.lblUsedTracker.TabIndex = 44;
			this.lblUsedTracker.Visible = false;
			// 
			// pnlEnterTimes
			// 
			this.pnlEnterTimes.Controls.Add(this.btn1FrMinus);
			this.pnlEnterTimes.Controls.Add(this.btn1FrPlus);
			this.pnlEnterTimes.Controls.Add(this.btnShowFields);
			this.pnlEnterTimes.Controls.Add(this.btnContinueWithNoTimes);
			this.pnlEnterTimes.Controls.Add(this.btnNextTime);
			this.pnlEnterTimes.Controls.Add(this.label4);
			this.pnlEnterTimes.Controls.Add(this.lblTimesHeader);
			this.pnlEnterTimes.Controls.Add(this.ucUtcTime);
			this.pnlEnterTimes.Location = new System.Drawing.Point(1, 127);
			this.pnlEnterTimes.Name = "pnlEnterTimes";
			this.pnlEnterTimes.Size = new System.Drawing.Size(257, 149);
			this.pnlEnterTimes.TabIndex = 32;
			this.pnlEnterTimes.Tag = "";
			this.pnlEnterTimes.Visible = false;
			// 
			// btn1FrMinus
			// 
			this.btn1FrMinus.Location = new System.Drawing.Point(7, 123);
			this.btn1FrMinus.Name = "btn1FrMinus";
			this.btn1FrMinus.Size = new System.Drawing.Size(37, 23);
			this.btn1FrMinus.TabIndex = 45;
			this.btn1FrMinus.Text = "-1Fr";
			this.btn1FrMinus.Click += new System.EventHandler(this.btn1FrMinus_Click);
			// 
			// btn1FrPlus
			// 
			this.btn1FrPlus.Location = new System.Drawing.Point(50, 123);
			this.btn1FrPlus.Name = "btn1FrPlus";
			this.btn1FrPlus.Size = new System.Drawing.Size(37, 23);
			this.btn1FrPlus.TabIndex = 44;
			this.btn1FrPlus.Text = "1Fr+";
			this.btn1FrPlus.Click += new System.EventHandler(this.btn1FrPlus_Click);
			// 
			// btnShowFields
			// 
			this.btnShowFields.Location = new System.Drawing.Point(93, 123);
			this.btnShowFields.Name = "btnShowFields";
			this.btnShowFields.Size = new System.Drawing.Size(79, 23);
			this.btnShowFields.TabIndex = 43;
			this.btnShowFields.Text = "Show Fields";
			this.btnShowFields.Click += new System.EventHandler(this.btnShowFields_Click);
			// 
			// btnContinueWithNoTimes
			// 
			this.btnContinueWithNoTimes.Location = new System.Drawing.Point(5, 73);
			this.btnContinueWithNoTimes.Name = "btnContinueWithNoTimes";
			this.btnContinueWithNoTimes.Size = new System.Drawing.Size(145, 23);
			this.btnContinueWithNoTimes.TabIndex = 42;
			this.btnContinueWithNoTimes.Text = "Continue with no times";
			this.btnContinueWithNoTimes.Click += new System.EventHandler(this.btnContinueWithNoTimes_Click);
			// 
			// btnNextTime
			// 
			this.btnNextTime.Location = new System.Drawing.Point(156, 73);
			this.btnNextTime.Name = "btnNextTime";
			this.btnNextTime.Size = new System.Drawing.Size(79, 23);
			this.btnNextTime.TabIndex = 41;
			this.btnNextTime.Text = "Next >>";
			this.btnNextTime.Click += new System.EventHandler(this.btnNextTime_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(4, 26);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(205, 13);
			this.label4.TabIndex = 32;
			this.label4.Text = "The frame is currently displayed on the left";
			// 
			// lblTimesHeader
			// 
			this.lblTimesHeader.AutoSize = true;
			this.lblTimesHeader.Location = new System.Drawing.Point(4, 9);
			this.lblTimesHeader.Name = "lblTimesHeader";
			this.lblTimesHeader.Size = new System.Drawing.Size(224, 13);
			this.lblTimesHeader.TabIndex = 31;
			this.lblTimesHeader.Text = "Enter the UTC time of the first measured frame";
			// 
			// ucUtcTime
			// 
			this.ucUtcTime.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ucUtcTime.BackColor = System.Drawing.SystemColors.Control;
			this.ucUtcTime.DateTimeUtc = new System.DateTime(2012, 9, 26, 11, 56, 14, 265);
			this.ucUtcTime.Location = new System.Drawing.Point(5, 41);
			this.ucUtcTime.Name = "ucUtcTime";
			this.ucUtcTime.Size = new System.Drawing.Size(239, 26);
			this.ucUtcTime.TabIndex = 30;
			// 
			// btnLightCurve
			// 
			this.btnLightCurve.Location = new System.Drawing.Point(109, 157);
			this.btnLightCurve.Name = "btnLightCurve";
			this.btnLightCurve.Size = new System.Drawing.Size(126, 23);
			this.btnLightCurve.TabIndex = 42;
			this.btnLightCurve.Text = "Finished Measurements";
			this.btnLightCurve.Click += new System.EventHandler(this.btnLightCurve_Click);
			// 
			// btnStop
			// 
			this.btnStop.Location = new System.Drawing.Point(109, 128);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(126, 23);
			this.btnStop.TabIndex = 41;
			this.btnStop.Text = "Cancel Measurements";
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// pnlMeasureZoomOptions
			// 
			this.pnlMeasureZoomOptions.Controls.Add(this.rbDisplayPSFs);
			this.pnlMeasureZoomOptions.Controls.Add(this.rbDisplayPixels);
			this.pnlMeasureZoomOptions.Controls.Add(this.rbDisplayBrightness);
			this.pnlMeasureZoomOptions.Controls.Add(this.label3);
			this.pnlMeasureZoomOptions.Location = new System.Drawing.Point(3, 2);
			this.pnlMeasureZoomOptions.Name = "pnlMeasureZoomOptions";
			this.pnlMeasureZoomOptions.Size = new System.Drawing.Size(255, 26);
			this.pnlMeasureZoomOptions.TabIndex = 39;
			this.pnlMeasureZoomOptions.Visible = false;
			// 
			// rbDisplayPSFs
			// 
			this.rbDisplayPSFs.Location = new System.Drawing.Point(187, 2);
			this.rbDisplayPSFs.Name = "rbDisplayPSFs";
			this.rbDisplayPSFs.Size = new System.Drawing.Size(68, 19);
			this.rbDisplayPSFs.TabIndex = 45;
			this.rbDisplayPSFs.Text = "PSF";
			this.rbDisplayPSFs.CheckedChanged += new System.EventHandler(this.TargetDisplayModeChanged);
			// 
			// rbDisplayPixels
			// 
			this.rbDisplayPixels.Location = new System.Drawing.Point(129, 2);
			this.rbDisplayPixels.Name = "rbDisplayPixels";
			this.rbDisplayPixels.Size = new System.Drawing.Size(59, 19);
			this.rbDisplayPixels.TabIndex = 41;
			this.rbDisplayPixels.Text = "Pixels";
			this.rbDisplayPixels.CheckedChanged += new System.EventHandler(this.TargetDisplayModeChanged);
			// 
			// rbDisplayBrightness
			// 
			this.rbDisplayBrightness.Location = new System.Drawing.Point(49, 2);
			this.rbDisplayBrightness.Name = "rbDisplayBrightness";
			this.rbDisplayBrightness.Size = new System.Drawing.Size(75, 19);
			this.rbDisplayBrightness.TabIndex = 40;
			this.rbDisplayBrightness.Text = "Brightness";
			this.rbDisplayBrightness.CheckedChanged += new System.EventHandler(this.TargetDisplayModeChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(1, 4);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(44, 13);
			this.label3.TabIndex = 39;
			this.label3.Text = "Display:";
			// 
			// lblElapsedTime
			// 
			this.lblElapsedTime.AutoSize = true;
			this.lblElapsedTime.Location = new System.Drawing.Point(116, 107);
			this.lblElapsedTime.Name = "lblElapsedTime";
			this.lblElapsedTime.Size = new System.Drawing.Size(13, 13);
			this.lblElapsedTime.TabIndex = 35;
			this.lblElapsedTime.Text = "0";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(33, 107);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(74, 13);
			this.label7.TabIndex = 34;
			this.label7.Text = "Elapsed Time:";
			// 
			// lblSkippedFrames
			// 
			this.lblSkippedFrames.AutoSize = true;
			this.lblSkippedFrames.Location = new System.Drawing.Point(116, 70);
			this.lblSkippedFrames.Name = "lblSkippedFrames";
			this.lblSkippedFrames.Size = new System.Drawing.Size(13, 13);
			this.lblSkippedFrames.TabIndex = 4;
			this.lblSkippedFrames.Text = "0";
			// 
			// lblProcessedFrames
			// 
			this.lblProcessedFrames.AutoSize = true;
			this.lblProcessedFrames.Location = new System.Drawing.Point(116, 54);
			this.lblProcessedFrames.Name = "lblProcessedFrames";
			this.lblProcessedFrames.Size = new System.Drawing.Size(13, 13);
			this.lblProcessedFrames.TabIndex = 3;
			this.lblProcessedFrames.Text = "0";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(32, 70);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(74, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Unsuccessful:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(10, 54);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(97, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Processed Frames:";
			// 
			// lblUsedTrackerLabel
			// 
			this.lblUsedTrackerLabel.AutoSize = true;
			this.lblUsedTrackerLabel.Location = new System.Drawing.Point(55, 38);
			this.lblUsedTrackerLabel.Name = "lblUsedTrackerLabel";
			this.lblUsedTrackerLabel.Size = new System.Drawing.Size(52, 13);
			this.lblUsedTrackerLabel.TabIndex = 43;
			this.lblUsedTrackerLabel.Text = "Tracking:";
			this.lblUsedTrackerLabel.Visible = false;
			// 
			// pnlUserAction
			// 
			this.pnlUserAction.Controls.Add(this.lblMeasurementType);
			this.pnlUserAction.Controls.Add(this.label10);
			this.pnlUserAction.Controls.Add(this.btnAddObject);
			this.pnlUserAction.Controls.Add(this.btnEditObject);
			this.pnlUserAction.Controls.Add(this.btnStartOver);
			this.pnlUserAction.Controls.Add(this.btnStart);
			this.pnlUserAction.Controls.Add(this.pnlSelectedObject);
			this.pnlUserAction.Controls.Add(this.lblBackgroundMethod);
			this.pnlUserAction.Controls.Add(this.label9);
			this.pnlUserAction.Controls.Add(this.lblSignalMethod);
			this.pnlUserAction.Controls.Add(this.label5);
			this.pnlUserAction.Controls.Add(this.pnlMeasuringSelection);
			this.pnlUserAction.Controls.Add(this.pnlTrackingSelection);
			this.pnlUserAction.Controls.Add(this.lblInfo);
			this.pnlUserAction.Location = new System.Drawing.Point(300, 3);
			this.pnlUserAction.Name = "pnlUserAction";
			this.pnlUserAction.Size = new System.Drawing.Size(249, 289);
			this.pnlUserAction.TabIndex = 5;
			this.pnlUserAction.Tag = "4,3";
			// 
			// lblMeasurementType
			// 
			this.lblMeasurementType.AutoSize = true;
			this.lblMeasurementType.Location = new System.Drawing.Point(110, 237);
			this.lblMeasurementType.Name = "lblMeasurementType";
			this.lblMeasurementType.Size = new System.Drawing.Size(0, 13);
			this.lblMeasurementType.TabIndex = 44;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(74, 237);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(34, 13);
			this.label10.TabIndex = 43;
			this.label10.Text = "Type:";
			// 
			// btnAddObject
			// 
			this.btnAddObject.Location = new System.Drawing.Point(5, 4);
			this.btnAddObject.Name = "btnAddObject";
			this.btnAddObject.Size = new System.Drawing.Size(171, 23);
			this.btnAddObject.TabIndex = 41;
			this.btnAddObject.Text = "Add Object";
			this.btnAddObject.Click += new System.EventHandler(this.btnAddObject_Click);
			// 
			// btnEditObject
			// 
			this.btnEditObject.Location = new System.Drawing.Point(5, 4);
			this.btnEditObject.Name = "btnEditObject";
			this.btnEditObject.Size = new System.Drawing.Size(171, 23);
			this.btnEditObject.TabIndex = 42;
			this.btnEditObject.Text = "Edit Object";
			this.btnEditObject.Click += new System.EventHandler(this.btnEditObject_Click);
			// 
			// btnStartOver
			// 
			this.btnStartOver.Location = new System.Drawing.Point(4, 196);
			this.btnStartOver.Name = "btnStartOver";
			this.btnStartOver.Size = new System.Drawing.Size(101, 23);
			this.btnStartOver.TabIndex = 40;
			this.btnStartOver.Text = "Reset";
			this.btnStartOver.Click += new System.EventHandler(this.btnStartOver_Click);
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(118, 196);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(101, 23);
			this.btnStart.TabIndex = 39;
			this.btnStart.Text = "Start";
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// pnlSelectedObject
			// 
			this.pnlSelectedObject.BackColor = System.Drawing.SystemColors.Control;
			this.pnlSelectedObject.Controls.Add(this.btnAdjustApertures);
			this.pnlSelectedObject.Controls.Add(this.rbM1);
			this.pnlSelectedObject.Controls.Add(this.rbM2);
			this.pnlSelectedObject.Controls.Add(this.rbM3);
			this.pnlSelectedObject.Controls.Add(this.rbM4);
			this.pnlSelectedObject.Controls.Add(this.lblM4);
			this.pnlSelectedObject.Controls.Add(this.lblM3);
			this.pnlSelectedObject.Controls.Add(this.lblM2);
			this.pnlSelectedObject.Controls.Add(this.lblM1);
			this.pnlSelectedObject.Controls.Add(this.lblObj);
			this.pnlSelectedObject.Controls.Add(this.lblSel);
			this.pnlSelectedObject.Location = new System.Drawing.Point(6, 32);
			this.pnlSelectedObject.Name = "pnlSelectedObject";
			this.pnlSelectedObject.Size = new System.Drawing.Size(170, 37);
			this.pnlSelectedObject.TabIndex = 38;
			// 
			// btnAdjustApertures
			// 
			this.btnAdjustApertures.Location = new System.Drawing.Point(121, 0);
			this.btnAdjustApertures.Name = "btnAdjustApertures";
			this.btnAdjustApertures.Size = new System.Drawing.Size(49, 23);
			this.btnAdjustApertures.TabIndex = 45;
			this.btnAdjustApertures.TabStop = false;
			this.btnAdjustApertures.Text = "Adjust Apertures";
			this.btnAdjustApertures.Click += new System.EventHandler(this.btnAdjustApertures_Click);
			// 
			// rbM1
			// 
			this.rbM1.Location = new System.Drawing.Point(32, 1);
			this.rbM1.Name = "rbM1";
			this.rbM1.Size = new System.Drawing.Size(18, 19);
			this.rbM1.TabIndex = 46;
			this.rbM1.CheckedChanged += new System.EventHandler(this.SelectedTargetChanged);
			// 
			// rbM2
			// 
			this.rbM2.Location = new System.Drawing.Point(51, 1);
			this.rbM2.Name = "rbM2";
			this.rbM2.Size = new System.Drawing.Size(18, 19);
			this.rbM2.TabIndex = 45;
			this.rbM2.CheckedChanged += new System.EventHandler(this.SelectedTargetChanged);
			// 
			// rbM3
			// 
			this.rbM3.Location = new System.Drawing.Point(72, 1);
			this.rbM3.Name = "rbM3";
			this.rbM3.Size = new System.Drawing.Size(18, 19);
			this.rbM3.TabIndex = 44;
			this.rbM3.CheckedChanged += new System.EventHandler(this.SelectedTargetChanged);
			// 
			// rbM4
			// 
			this.rbM4.Location = new System.Drawing.Point(92, 1);
			this.rbM4.Name = "rbM4";
			this.rbM4.Size = new System.Drawing.Size(18, 19);
			this.rbM4.TabIndex = 43;
			this.rbM4.CheckedChanged += new System.EventHandler(this.SelectedTargetChanged);
			// 
			// lblM4
			// 
			this.lblM4.AutoSize = true;
			this.lblM4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblM4.Location = new System.Drawing.Point(94, 21);
			this.lblM4.Name = "lblM4";
			this.lblM4.Size = new System.Drawing.Size(14, 13);
			this.lblM4.TabIndex = 37;
			this.lblM4.Text = "4";
			// 
			// lblM3
			// 
			this.lblM3.AutoSize = true;
			this.lblM3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblM3.Location = new System.Drawing.Point(74, 21);
			this.lblM3.Name = "lblM3";
			this.lblM3.Size = new System.Drawing.Size(14, 13);
			this.lblM3.TabIndex = 36;
			this.lblM3.Text = "3";
			// 
			// lblM2
			// 
			this.lblM2.AutoSize = true;
			this.lblM2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblM2.Location = new System.Drawing.Point(53, 21);
			this.lblM2.Name = "lblM2";
			this.lblM2.Size = new System.Drawing.Size(14, 13);
			this.lblM2.TabIndex = 35;
			this.lblM2.Text = "2";
			// 
			// lblM1
			// 
			this.lblM1.AutoSize = true;
			this.lblM1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblM1.Location = new System.Drawing.Point(34, 21);
			this.lblM1.Name = "lblM1";
			this.lblM1.Size = new System.Drawing.Size(14, 13);
			this.lblM1.TabIndex = 34;
			this.lblM1.Text = "1";
			// 
			// lblObj
			// 
			this.lblObj.AutoSize = true;
			this.lblObj.Location = new System.Drawing.Point(6, 21);
			this.lblObj.Name = "lblObj";
			this.lblObj.Size = new System.Drawing.Size(26, 13);
			this.lblObj.TabIndex = 33;
			this.lblObj.Text = "Obj:";
			// 
			// lblSel
			// 
			this.lblSel.AutoSize = true;
			this.lblSel.Location = new System.Drawing.Point(6, 4);
			this.lblSel.Name = "lblSel";
			this.lblSel.Size = new System.Drawing.Size(25, 13);
			this.lblSel.TabIndex = 32;
			this.lblSel.Text = "Sel:";
			// 
			// lblBackgroundMethod
			// 
			this.lblBackgroundMethod.AutoSize = true;
			this.lblBackgroundMethod.Location = new System.Drawing.Point(110, 263);
			this.lblBackgroundMethod.Name = "lblBackgroundMethod";
			this.lblBackgroundMethod.Size = new System.Drawing.Size(0, 13);
			this.lblBackgroundMethod.TabIndex = 31;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(2, 263);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(107, 13);
			this.label9.TabIndex = 30;
			this.label9.Text = "Background Method:";
			// 
			// lblSignalMethod
			// 
			this.lblSignalMethod.AutoSize = true;
			this.lblSignalMethod.Location = new System.Drawing.Point(110, 250);
			this.lblSignalMethod.Name = "lblSignalMethod";
			this.lblSignalMethod.Size = new System.Drawing.Size(0, 13);
			this.lblSignalMethod.TabIndex = 29;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(31, 250);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(78, 13);
			this.label5.TabIndex = 28;
			this.label5.Text = "Signal Method:";
			// 
			// pnlMeasuringSelection
			// 
			this.pnlMeasuringSelection.BackColor = System.Drawing.Color.WhiteSmoke;
			this.pnlMeasuringSelection.Controls.Add(this.label8);
			this.pnlMeasuringSelection.Controls.Add(this.lblMeasuringStars);
			this.pnlMeasuringSelection.Location = new System.Drawing.Point(6, 73);
			this.pnlMeasuringSelection.Name = "pnlMeasuringSelection";
			this.pnlMeasuringSelection.Size = new System.Drawing.Size(170, 23);
			this.pnlMeasuringSelection.TabIndex = 6;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(3, 5);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(59, 13);
			this.label8.TabIndex = 1;
			this.label8.Text = "Measuring:";
			// 
			// lblMeasuringStars
			// 
			this.lblMeasuringStars.AutoSize = true;
			this.lblMeasuringStars.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblMeasuringStars.Location = new System.Drawing.Point(66, 5);
			this.lblMeasuringStars.Name = "lblMeasuringStars";
			this.lblMeasuringStars.Size = new System.Drawing.Size(66, 13);
			this.lblMeasuringStars.TabIndex = 2;
			this.lblMeasuringStars.Text = "4 selected";
			// 
			// pnlTrackingSelection
			// 
			this.pnlTrackingSelection.BackColor = System.Drawing.Color.WhiteSmoke;
			this.pnlTrackingSelection.Controls.Add(this.lblUserAction);
			this.pnlTrackingSelection.Controls.Add(this.lblTrackingStars);
			this.pnlTrackingSelection.Location = new System.Drawing.Point(6, 101);
			this.pnlTrackingSelection.Name = "pnlTrackingSelection";
			this.pnlTrackingSelection.Size = new System.Drawing.Size(170, 23);
			this.pnlTrackingSelection.TabIndex = 5;
			// 
			// lblUserAction
			// 
			this.lblUserAction.AutoSize = true;
			this.lblUserAction.Location = new System.Drawing.Point(3, 4);
			this.lblUserAction.Name = "lblUserAction";
			this.lblUserAction.Size = new System.Drawing.Size(61, 13);
			this.lblUserAction.TabIndex = 1;
			this.lblUserAction.Text = "Guiding on:";
			// 
			// lblTrackingStars
			// 
			this.lblTrackingStars.AutoSize = true;
			this.lblTrackingStars.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblTrackingStars.Location = new System.Drawing.Point(68, 4);
			this.lblTrackingStars.Name = "lblTrackingStars";
			this.lblTrackingStars.Size = new System.Drawing.Size(66, 13);
			this.lblTrackingStars.TabIndex = 2;
			this.lblTrackingStars.Text = "4 selected";
			// 
			// lblInfo
			// 
			this.lblInfo.ForeColor = System.Drawing.Color.Black;
			this.lblInfo.Location = new System.Drawing.Point(3, 130);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Size = new System.Drawing.Size(226, 49);
			this.lblInfo.TabIndex = 3;
			this.lblInfo.Text = "Click on an object to select it\r\nControl-Click to overwrite last selection\r\nShift" +
    "-Click to delete it";
			// 
			// pnlViewLightCurve
			// 
			this.pnlViewLightCurve.Location = new System.Drawing.Point(580, 3);
			this.pnlViewLightCurve.Name = "pnlViewLightCurve";
			this.pnlViewLightCurve.Size = new System.Drawing.Size(281, 304);
			this.pnlViewLightCurve.TabIndex = 7;
			// 
			// timerMoveToFirstFrame
			// 
			this.timerMoveToFirstFrame.Interval = 500;
			this.timerMoveToFirstFrame.Tick += new System.EventHandler(this.timerMoveToFirstFrame_Tick);
			// 
			// lblPartiallySuccessfulFrames
			// 
			this.lblPartiallySuccessfulFrames.AutoSize = true;
			this.lblPartiallySuccessfulFrames.Location = new System.Drawing.Point(116, 86);
			this.lblPartiallySuccessfulFrames.Name = "lblPartiallySuccessfulFrames";
			this.lblPartiallySuccessfulFrames.Size = new System.Drawing.Size(13, 13);
			this.lblPartiallySuccessfulFrames.TabIndex = 46;
			this.lblPartiallySuccessfulFrames.Text = "0";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(5, 86);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(101, 13);
			this.label11.TabIndex = 45;
			this.label11.Text = "Partially Successful:";
			// 
			// ucLightCurves
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.pnlUserAction);
			this.Controls.Add(this.pnlProcessing);
			this.Controls.Add(this.pnlViewLightCurve);
			this.Name = "ucLightCurves";
			this.Size = new System.Drawing.Size(891, 312);
			this.pnlProcessing.ResumeLayout(false);
			this.pnlProcessing.PerformLayout();
			this.pnlEnterTimes.ResumeLayout(false);
			this.pnlEnterTimes.PerformLayout();
			this.pnlMeasureZoomOptions.ResumeLayout(false);
			this.pnlMeasureZoomOptions.PerformLayout();
			this.pnlUserAction.ResumeLayout(false);
			this.pnlUserAction.PerformLayout();
			this.pnlSelectedObject.ResumeLayout(false);
			this.pnlSelectedObject.PerformLayout();
			this.pnlMeasuringSelection.ResumeLayout(false);
			this.pnlMeasuringSelection.PerformLayout();
			this.pnlTrackingSelection.ResumeLayout(false);
			this.pnlTrackingSelection.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlProcessing;
		private System.Windows.Forms.Panel pnlEnterTimes;
        private System.Windows.Forms.Label lblTimesHeader;
        private ucUtcTimePicker ucUtcTime;
        private System.Windows.Forms.Label lblElapsedTime;
		private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblSkippedFrames;
        private System.Windows.Forms.Label lblProcessedFrames;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel pnlUserAction;
        private System.Windows.Forms.Panel pnlMeasuringSelection;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblMeasuringStars;
        private System.Windows.Forms.Panel pnlTrackingSelection;
        private System.Windows.Forms.Label lblUserAction;
		private System.Windows.Forms.Label lblTrackingStars;
		private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Panel pnlMeasureZoomOptions;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Panel pnlViewLightCurve;
        private System.Windows.Forms.Timer timerMoveToFirstFrame;
        private System.Windows.Forms.Label lblBackgroundMethod;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblSignalMethod;
        private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label lblObj;
		private System.Windows.Forms.Label lblSel;
		private System.Windows.Forms.Label lblM4;
		private System.Windows.Forms.Label lblM3;
		private System.Windows.Forms.Label lblM2;
		private System.Windows.Forms.Label lblM1;
		private System.Windows.Forms.Panel pnlSelectedObject;
        private System.Windows.Forms.Button btnAddObject;
        private System.Windows.Forms.Button btnEditObject;
        private System.Windows.Forms.Button btnStartOver;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.RadioButton rbM3;
        private System.Windows.Forms.RadioButton rbM4;
        private System.Windows.Forms.RadioButton rbM1;
        private System.Windows.Forms.RadioButton rbM2;
        private System.Windows.Forms.Button btnNextTime;
        private System.Windows.Forms.Button btn1FrMinus;
        private System.Windows.Forms.Button btn1FrPlus;
        private System.Windows.Forms.Button btnShowFields;
        private System.Windows.Forms.Button btnContinueWithNoTimes;
        private System.Windows.Forms.Button btnLightCurve;
        private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.RadioButton rbDisplayPixels;
		private System.Windows.Forms.RadioButton rbDisplayBrightness;
		private System.Windows.Forms.Label lblUsedTracker;
		private System.Windows.Forms.Label lblUsedTrackerLabel;
        private System.Windows.Forms.RadioButton rbDisplayPSFs;
		private System.Windows.Forms.Label lblMeasurementType;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Button btnAdjustApertures;
		private System.Windows.Forms.Label lblPartiallySuccessfulFrames;
		private System.Windows.Forms.Label label11;

    }
}

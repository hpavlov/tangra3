namespace Tangra.VideoOperations.Astrometry
{
	partial class ucChooseCalibratedConfiguration
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucChooseCalibratedConfiguration));
			this.cbxSolveConstantsNow = new System.Windows.Forms.CheckBox();
			this.btnDelConfig = new System.Windows.Forms.Button();
			this.btnNewConfig = new System.Windows.Forms.Button();
			this.label11 = new System.Windows.Forms.Label();
			this.btnDelete = new System.Windows.Forms.Button();
			this.btnNew = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.cbxSavedConfigurations = new System.Windows.Forms.ComboBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.tbxMatrixHeight = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.tbxMatrixWidth = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tbxPixelY = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tbxPixelX = new System.Windows.Forms.TextBox();
			this.cbxSavedCameras = new System.Windows.Forms.ComboBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.tbxSolvedFocalLength = new System.Windows.Forms.TextBox();
			this.label18 = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.pnlNotSolved = new System.Windows.Forms.Panel();
			this.linkLblAboutCalibration = new System.Windows.Forms.LinkLabel();
			this.label21 = new System.Windows.Forms.Label();
			this.label20 = new System.Windows.Forms.Label();
			this.gbxSolved = new System.Windows.Forms.GroupBox();
			this.label17 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.btnRecalibrate = new System.Windows.Forms.Button();
			this.label12 = new System.Windows.Forms.Label();
			this.tbxSolvedCellX = new System.Windows.Forms.TextBox();
			this.tbxSolvedCellY = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.btnEdit = new System.Windows.Forms.Button();
			this.btnEditCamera = new System.Windows.Forms.Button();
			this.cbxFlipVertically = new System.Windows.Forms.CheckBox();
			this.cbxFlipHorizontally = new System.Windows.Forms.CheckBox();
			this.nudLimitMagnitude = new System.Windows.Forms.NumericUpDown();
			this.lblLimitingMagnitude = new System.Windows.Forms.Label();
			this.pnlEditableConfigSettings = new System.Windows.Forms.Panel();
			this.cbxRotate180 = new System.Windows.Forms.CheckBox();
			this.btnPreProcessingFilter = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.contextMenuFilter = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miNoFilter = new System.Windows.Forms.ToolStripMenuItem();
			this.miLowPass = new System.Windows.Forms.ToolStripMenuItem();
			this.groupBox1.SuspendLayout();
			this.pnlNotSolved.SuspendLayout();
			this.gbxSolved.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudLimitMagnitude)).BeginInit();
			this.pnlEditableConfigSettings.SuspendLayout();
			this.contextMenuFilter.SuspendLayout();
			this.SuspendLayout();
			// 
			// cbxSolveConstantsNow
			// 
			this.cbxSolveConstantsNow.AutoSize = true;
			this.cbxSolveConstantsNow.Location = new System.Drawing.Point(177, 1);
			this.cbxSolveConstantsNow.Name = "cbxSolveConstantsNow";
			this.cbxSolveConstantsNow.Size = new System.Drawing.Size(92, 17);
			this.cbxSolveConstantsNow.TabIndex = 53;
			this.cbxSolveConstantsNow.Text = "Calibrate Now";
			this.cbxSolveConstantsNow.UseVisualStyleBackColor = true;
			// 
			// btnDelConfig
			// 
			this.btnDelConfig.Location = new System.Drawing.Point(357, 146);
			this.btnDelConfig.Name = "btnDelConfig";
			this.btnDelConfig.Size = new System.Drawing.Size(41, 23);
			this.btnDelConfig.TabIndex = 50;
			this.btnDelConfig.Text = "Del";
			this.btnDelConfig.UseVisualStyleBackColor = true;
			this.btnDelConfig.Click += new System.EventHandler(this.btnDelConfig_Click);
			// 
			// btnNewConfig
			// 
			this.btnNewConfig.Location = new System.Drawing.Point(273, 146);
			this.btnNewConfig.Name = "btnNewConfig";
			this.btnNewConfig.Size = new System.Drawing.Size(41, 23);
			this.btnNewConfig.TabIndex = 49;
			this.btnNewConfig.Text = "New";
			this.btnNewConfig.UseVisualStyleBackColor = true;
			this.btnNewConfig.Click += new System.EventHandler(this.btnNewConfig_Click);
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(12, 130);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(260, 13);
			this.label11.TabIndex = 48;
			this.label11.Text = "Telescope + Focal Reducer + Recorder Configuration";
			// 
			// btnDelete
			// 
			this.btnDelete.Location = new System.Drawing.Point(301, 10);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(41, 23);
			this.btnDelete.TabIndex = 47;
			this.btnDelete.Text = "Del";
			this.btnDelete.UseVisualStyleBackColor = true;
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// btnNew
			// 
			this.btnNew.Location = new System.Drawing.Point(217, 10);
			this.btnNew.Name = "btnNew";
			this.btnNew.Size = new System.Drawing.Size(41, 23);
			this.btnNew.TabIndex = 45;
			this.btnNew.Text = "New";
			this.btnNew.UseVisualStyleBackColor = true;
			this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(306, 320);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(92, 32);
			this.btnCancel.TabIndex = 44;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(207, 320);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(93, 32);
			this.btnOK.TabIndex = 43;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// cbxSavedConfigurations
			// 
			this.cbxSavedConfigurations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxSavedConfigurations.FormattingEnabled = true;
			this.cbxSavedConfigurations.Items.AddRange(new object[] {
            "<No Configs Available - Press \'New\'>"});
			this.cbxSavedConfigurations.Location = new System.Drawing.Point(15, 148);
			this.cbxSavedConfigurations.Name = "cbxSavedConfigurations";
			this.cbxSavedConfigurations.Size = new System.Drawing.Size(257, 21);
			this.cbxSavedConfigurations.TabIndex = 42;
			this.cbxSavedConfigurations.SelectedIndexChanged += new System.EventHandler(this.cbxSavedConfigurations_SelectedIndexChanged);
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(318, 53);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(28, 13);
			this.label10.TabIndex = 41;
			this.label10.Text = "cells";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(318, 27);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(28, 13);
			this.label9.TabIndex = 40;
			this.label9.Text = "cells";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(186, 53);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(69, 13);
			this.label7.TabIndex = 39;
			this.label7.Text = "Matrix Height";
			// 
			// tbxMatrixHeight
			// 
			this.tbxMatrixHeight.Location = new System.Drawing.Point(258, 50);
			this.tbxMatrixHeight.Name = "tbxMatrixHeight";
			this.tbxMatrixHeight.ReadOnly = true;
			this.tbxMatrixHeight.Size = new System.Drawing.Size(54, 20);
			this.tbxMatrixHeight.TabIndex = 38;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(186, 27);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(66, 13);
			this.label8.TabIndex = 37;
			this.label8.Text = "Matrix Width";
			// 
			// tbxMatrixWidth
			// 
			this.tbxMatrixWidth.Location = new System.Drawing.Point(258, 24);
			this.tbxMatrixWidth.Name = "tbxMatrixWidth";
			this.tbxMatrixWidth.ReadOnly = true;
			this.tbxMatrixWidth.Size = new System.Drawing.Size(54, 20);
			this.tbxMatrixWidth.TabIndex = 36;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(140, 53);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(15, 13);
			this.label5.TabIndex = 35;
			this.label5.Text = "m";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
			this.label6.Location = new System.Drawing.Point(131, 53);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(13, 13);
			this.label6.TabIndex = 34;
			this.label6.Text = "m";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(140, 27);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(15, 13);
			this.label4.TabIndex = 33;
			this.label4.Text = "m";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
			this.label3.Location = new System.Drawing.Point(131, 27);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(13, 13);
			this.label3.TabIndex = 32;
			this.label3.Text = "m";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 53);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(58, 13);
			this.label2.TabIndex = 31;
			this.label2.Text = "Cell Height";
			// 
			// tbxPixelY
			// 
			this.tbxPixelY.Location = new System.Drawing.Point(71, 50);
			this.tbxPixelY.Name = "tbxPixelY";
			this.tbxPixelY.ReadOnly = true;
			this.tbxPixelY.Size = new System.Drawing.Size(54, 20);
			this.tbxPixelY.TabIndex = 30;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 27);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(55, 13);
			this.label1.TabIndex = 29;
			this.label1.Text = "Cell Width";
			// 
			// tbxPixelX
			// 
			this.tbxPixelX.Location = new System.Drawing.Point(71, 24);
			this.tbxPixelX.Name = "tbxPixelX";
			this.tbxPixelX.ReadOnly = true;
			this.tbxPixelX.Size = new System.Drawing.Size(54, 20);
			this.tbxPixelX.TabIndex = 28;
			// 
			// cbxSavedCameras
			// 
			this.cbxSavedCameras.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxSavedCameras.FormattingEnabled = true;
			this.cbxSavedCameras.Items.AddRange(new object[] {
            "WAT 902H, PAL",
            "WAT 120N+, PAL",
            "WAT 902H, NTSC",
            "WAT 120N+, NTSC"});
			this.cbxSavedCameras.Location = new System.Drawing.Point(15, 11);
			this.cbxSavedCameras.Name = "cbxSavedCameras";
			this.cbxSavedCameras.Size = new System.Drawing.Size(197, 21);
			this.cbxSavedCameras.TabIndex = 27;
			this.cbxSavedCameras.SelectedIndexChanged += new System.EventHandler(this.cbxSavedCameras_SelectedIndexChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.tbxPixelX);
			this.groupBox1.Controls.Add(this.tbxPixelY);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.tbxMatrixWidth);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.tbxMatrixHeight);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.label10);
			this.groupBox1.Controls.Add(this.label9);
			this.groupBox1.Location = new System.Drawing.Point(15, 39);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(383, 81);
			this.groupBox1.TabIndex = 54;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Physical Dimentions";
			// 
			// tbxSolvedFocalLength
			// 
			this.tbxSolvedFocalLength.Location = new System.Drawing.Point(258, 21);
			this.tbxSolvedFocalLength.Name = "tbxSolvedFocalLength";
			this.tbxSolvedFocalLength.ReadOnly = true;
			this.tbxSolvedFocalLength.Size = new System.Drawing.Size(54, 20);
			this.tbxSolvedFocalLength.TabIndex = 63;
			// 
			// label18
			// 
			this.label18.AutoSize = true;
			this.label18.Location = new System.Drawing.Point(186, 24);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(69, 13);
			this.label18.TabIndex = 64;
			this.label18.Text = "Focal Length";
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.Location = new System.Drawing.Point(318, 24);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(23, 13);
			this.label19.TabIndex = 65;
			this.label19.Text = "mm";
			// 
			// pnlNotSolved
			// 
			this.pnlNotSolved.Controls.Add(this.linkLblAboutCalibration);
			this.pnlNotSolved.Controls.Add(this.cbxSolveConstantsNow);
			this.pnlNotSolved.Controls.Add(this.label21);
			this.pnlNotSolved.Controls.Add(this.label20);
			this.pnlNotSolved.Location = new System.Drawing.Point(13, 227);
			this.pnlNotSolved.Name = "pnlNotSolved";
			this.pnlNotSolved.Size = new System.Drawing.Size(385, 87);
			this.pnlNotSolved.TabIndex = 66;
			// 
			// linkLblAboutCalibration
			// 
			this.linkLblAboutCalibration.AutoSize = true;
			this.linkLblAboutCalibration.Location = new System.Drawing.Point(68, 65);
			this.linkLblAboutCalibration.Name = "linkLblAboutCalibration";
			this.linkLblAboutCalibration.Size = new System.Drawing.Size(207, 13);
			this.linkLblAboutCalibration.TabIndex = 69;
			this.linkLblAboutCalibration.TabStop = true;
			this.linkLblAboutCalibration.Text = "Read the instructions before you continue.";
			this.linkLblAboutCalibration.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLblAboutCalibration_LinkClicked);
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(3, 27);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(379, 52);
			this.label21.TabIndex = 68;
			this.label21.Text = resources.GetString("label21.Text");
			// 
			// label20
			// 
			this.label20.AutoSize = true;
			this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label20.ForeColor = System.Drawing.Color.Red;
			this.label20.Location = new System.Drawing.Point(3, 2);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(167, 13);
			this.label20.TabIndex = 53;
			this.label20.Text = "Configuration Not Calibrated";
			// 
			// gbxSolved
			// 
			this.gbxSolved.Controls.Add(this.label17);
			this.gbxSolved.Controls.Add(this.label15);
			this.gbxSolved.Controls.Add(this.btnRecalibrate);
			this.gbxSolved.Controls.Add(this.label12);
			this.gbxSolved.Controls.Add(this.tbxSolvedCellX);
			this.gbxSolved.Controls.Add(this.tbxSolvedFocalLength);
			this.gbxSolved.Controls.Add(this.tbxSolvedCellY);
			this.gbxSolved.Controls.Add(this.label18);
			this.gbxSolved.Controls.Add(this.label19);
			this.gbxSolved.Controls.Add(this.label13);
			this.gbxSolved.Controls.Add(this.label14);
			this.gbxSolved.Controls.Add(this.label16);
			this.gbxSolved.Location = new System.Drawing.Point(15, 229);
			this.gbxSolved.Name = "gbxSolved";
			this.gbxSolved.Size = new System.Drawing.Size(383, 74);
			this.gbxSolved.TabIndex = 67;
			this.gbxSolved.TabStop = false;
			this.gbxSolved.Text = "Solved Configuration";
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Location = new System.Drawing.Point(140, 50);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(15, 13);
			this.label17.TabIndex = 43;
			this.label17.Text = "m";
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(140, 24);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(15, 13);
			this.label15.TabIndex = 41;
			this.label15.Text = "m";
			// 
			// btnRecalibrate
			// 
			this.btnRecalibrate.Location = new System.Drawing.Point(258, 46);
			this.btnRecalibrate.Name = "btnRecalibrate";
			this.btnRecalibrate.Size = new System.Drawing.Size(69, 23);
			this.btnRecalibrate.TabIndex = 66;
			this.btnRecalibrate.Text = "Recalibrate";
			this.btnRecalibrate.UseVisualStyleBackColor = true;
			this.btnRecalibrate.Click += new System.EventHandler(this.btnRecalibrate_Click);
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(6, 24);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(60, 13);
			this.label12.TabIndex = 37;
			this.label12.Text = "Pixel Width";
			// 
			// tbxSolvedCellX
			// 
			this.tbxSolvedCellX.Location = new System.Drawing.Point(71, 21);
			this.tbxSolvedCellX.Name = "tbxSolvedCellX";
			this.tbxSolvedCellX.ReadOnly = true;
			this.tbxSolvedCellX.Size = new System.Drawing.Size(54, 20);
			this.tbxSolvedCellX.TabIndex = 36;
			// 
			// tbxSolvedCellY
			// 
			this.tbxSolvedCellY.Location = new System.Drawing.Point(71, 47);
			this.tbxSolvedCellY.Name = "tbxSolvedCellY";
			this.tbxSolvedCellY.ReadOnly = true;
			this.tbxSolvedCellY.Size = new System.Drawing.Size(54, 20);
			this.tbxSolvedCellY.TabIndex = 38;
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(6, 50);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(63, 13);
			this.label13.TabIndex = 39;
			this.label13.Text = "Pixel Height";
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Font = new System.Drawing.Font("Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
			this.label14.Location = new System.Drawing.Point(131, 24);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(13, 13);
			this.label14.TabIndex = 40;
			this.label14.Text = "m";
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Font = new System.Drawing.Font("Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
			this.label16.Location = new System.Drawing.Point(131, 50);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(13, 13);
			this.label16.TabIndex = 42;
			this.label16.Text = "m";
			// 
			// btnEdit
			// 
			this.btnEdit.Location = new System.Drawing.Point(315, 146);
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.Size = new System.Drawing.Size(41, 23);
			this.btnEdit.TabIndex = 68;
			this.btnEdit.Text = "Edit";
			this.btnEdit.UseVisualStyleBackColor = true;
			this.btnEdit.Click += new System.EventHandler(this.btnEditConfiguration_Click);
			// 
			// btnEditCamera
			// 
			this.btnEditCamera.Location = new System.Drawing.Point(259, 10);
			this.btnEditCamera.Name = "btnEditCamera";
			this.btnEditCamera.Size = new System.Drawing.Size(41, 23);
			this.btnEditCamera.TabIndex = 69;
			this.btnEditCamera.Text = "Edit";
			this.btnEditCamera.UseVisualStyleBackColor = true;
			this.btnEditCamera.Click += new System.EventHandler(this.btnEditCamera_Click);
			// 
			// cbxFlipVertically
			// 
			this.cbxFlipVertically.AutoSize = true;
			this.cbxFlipVertically.Location = new System.Drawing.Point(0, 7);
			this.cbxFlipVertically.Name = "cbxFlipVertically";
			this.cbxFlipVertically.Size = new System.Drawing.Size(87, 17);
			this.cbxFlipVertically.TabIndex = 73;
			this.cbxFlipVertically.Text = "Flip Vertically";
			this.cbxFlipVertically.UseVisualStyleBackColor = true;
			this.cbxFlipVertically.CheckedChanged += new System.EventHandler(this.cbxFlipVertically_CheckedChanged);
			// 
			// cbxFlipHorizontally
			// 
			this.cbxFlipHorizontally.AutoSize = true;
			this.cbxFlipHorizontally.Location = new System.Drawing.Point(0, 28);
			this.cbxFlipHorizontally.Name = "cbxFlipHorizontally";
			this.cbxFlipHorizontally.Size = new System.Drawing.Size(99, 17);
			this.cbxFlipHorizontally.TabIndex = 72;
			this.cbxFlipHorizontally.Text = "Flip Horizontally";
			this.cbxFlipHorizontally.UseVisualStyleBackColor = true;
			this.cbxFlipHorizontally.CheckedChanged += new System.EventHandler(this.cbxFlipHorizontally_CheckedChanged);
			// 
			// nudLimitMagnitude
			// 
			this.nudLimitMagnitude.DecimalPlaces = 1;
			this.nudLimitMagnitude.Location = new System.Drawing.Point(333, 5);
			this.nudLimitMagnitude.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
			this.nudLimitMagnitude.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.nudLimitMagnitude.Name = "nudLimitMagnitude";
			this.nudLimitMagnitude.Size = new System.Drawing.Size(46, 20);
			this.nudLimitMagnitude.TabIndex = 71;
			this.nudLimitMagnitude.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
			this.nudLimitMagnitude.ValueChanged += new System.EventHandler(this.nudLimitMagnitude_ValueChanged);
			// 
			// lblLimitingMagnitude
			// 
			this.lblLimitingMagnitude.AutoSize = true;
			this.lblLimitingMagnitude.Location = new System.Drawing.Point(231, 8);
			this.lblLimitingMagnitude.Name = "lblLimitingMagnitude";
			this.lblLimitingMagnitude.Size = new System.Drawing.Size(98, 13);
			this.lblLimitingMagnitude.TabIndex = 70;
			this.lblLimitingMagnitude.Text = "Limiting Magnitude:";
			// 
			// pnlEditableConfigSettings
			// 
			this.pnlEditableConfigSettings.Controls.Add(this.cbxRotate180);
			this.pnlEditableConfigSettings.Controls.Add(this.cbxFlipHorizontally);
			this.pnlEditableConfigSettings.Controls.Add(this.cbxFlipVertically);
			this.pnlEditableConfigSettings.Controls.Add(this.lblLimitingMagnitude);
			this.pnlEditableConfigSettings.Controls.Add(this.nudLimitMagnitude);
			this.pnlEditableConfigSettings.Location = new System.Drawing.Point(15, 172);
			this.pnlEditableConfigSettings.Name = "pnlEditableConfigSettings";
			this.pnlEditableConfigSettings.Size = new System.Drawing.Size(383, 51);
			this.pnlEditableConfigSettings.TabIndex = 74;
			// 
			// cbxRotate180
			// 
			this.cbxRotate180.AutoSize = true;
			this.cbxRotate180.Location = new System.Drawing.Point(125, 7);
			this.cbxRotate180.Name = "cbxRotate180";
			this.cbxRotate180.Size = new System.Drawing.Size(100, 17);
			this.cbxRotate180.TabIndex = 74;
			this.cbxRotate180.Text = "Rotate 180 deg";
			this.cbxRotate180.UseVisualStyleBackColor = true;
			// 
			// btnPreProcessingFilter
			// 
			this.btnPreProcessingFilter.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnPreProcessingFilter.Location = new System.Drawing.Point(13, 328);
			this.btnPreProcessingFilter.Name = "btnPreProcessingFilter";
			this.btnPreProcessingFilter.Size = new System.Drawing.Size(132, 21);
			this.btnPreProcessingFilter.TabIndex = 75;
			this.btnPreProcessingFilter.Text = "Low Pass Diff Filter";
			this.btnPreProcessingFilter.UseVisualStyleBackColor = true;
			this.btnPreProcessingFilter.Click += new System.EventHandler(this.btnPreProcessingFilter_Click);
			// 
			// contextMenuFilter
			// 
			this.contextMenuFilter.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miNoFilter,
            this.miLowPass});
			this.contextMenuFilter.Name = "contextMenuFilter";
			this.contextMenuFilter.Size = new System.Drawing.Size(154, 48);
			this.contextMenuFilter.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuFilter_Opening);
			// 
			// miNoFilter
			// 
			this.miNoFilter.Checked = true;
			this.miNoFilter.CheckOnClick = true;
			this.miNoFilter.CheckState = System.Windows.Forms.CheckState.Checked;
			this.miNoFilter.Name = "miNoFilter";
			this.miNoFilter.Size = new System.Drawing.Size(153, 22);
			this.miNoFilter.Text = "&No Filter";
			this.miNoFilter.CheckedChanged += new System.EventHandler(this.miFilter_CheckedChanged);
			// 
			// miLowPass
			// 
			this.miLowPass.CheckOnClick = true;
			this.miLowPass.Name = "miLowPass";
			this.miLowPass.Size = new System.Drawing.Size(153, 22);
			this.miLowPass.Text = "&Low-Pass Filter";
			this.miLowPass.CheckedChanged += new System.EventHandler(this.miFilter_CheckedChanged);
			// 
			// ucChooseCalibratedConfiguration
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pnlNotSolved);
			this.Controls.Add(this.pnlEditableConfigSettings);
			this.Controls.Add(this.btnPreProcessingFilter);
			this.Controls.Add(this.btnEditCamera);
			this.Controls.Add(this.btnEdit);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.btnDelConfig);
			this.Controls.Add(this.btnNewConfig);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.btnNew);
			this.Controls.Add(this.cbxSavedConfigurations);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.cbxSavedCameras);
			this.Controls.Add(this.gbxSolved);
			this.Name = "ucChooseCalibratedConfiguration";
			this.Size = new System.Drawing.Size(410, 366);
			this.Load += new System.EventHandler(this.ucCameraSettings_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.pnlNotSolved.ResumeLayout(false);
			this.pnlNotSolved.PerformLayout();
			this.gbxSolved.ResumeLayout(false);
			this.gbxSolved.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudLimitMagnitude)).EndInit();
			this.pnlEditableConfigSettings.ResumeLayout(false);
			this.pnlEditableConfigSettings.PerformLayout();
			this.contextMenuFilter.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion


		private System.Windows.Forms.CheckBox cbxSolveConstantsNow;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.Button btnRecalibrate;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox tbxSolvedCellX;
		private System.Windows.Forms.Panel pnlEditableConfigSettings;
		private System.Windows.Forms.CheckBox cbxRotate180;
		private System.Windows.Forms.CheckBox cbxFlipHorizontally;
		private System.Windows.Forms.CheckBox cbxFlipVertically;
		private System.Windows.Forms.Label lblLimitingMagnitude;
		private System.Windows.Forms.NumericUpDown nudLimitMagnitude;
		private System.Windows.Forms.Button btnEditCamera;
		private System.Windows.Forms.TextBox tbxSolvedCellY;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.LinkLabel linkLblAboutCalibration;
		private System.Windows.Forms.Button btnPreProcessingFilter;
		private System.Windows.Forms.Button btnEdit;
		private System.Windows.Forms.Panel pnlNotSolved;
		private System.Windows.Forms.ContextMenuStrip contextMenuFilter;
		private System.Windows.Forms.ToolStripMenuItem miNoFilter;
		private System.Windows.Forms.ToolStripMenuItem miLowPass;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.TextBox tbxSolvedFocalLength;
		private System.Windows.Forms.GroupBox gbxSolved;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbxPixelX;
		private System.Windows.Forms.TextBox tbxPixelY;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox tbxMatrixWidth;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox tbxMatrixHeight;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Button btnDelConfig;
		private System.Windows.Forms.Button btnNewConfig;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.Button btnNew;
		private System.Windows.Forms.ComboBox cbxSavedConfigurations;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.ComboBox cbxSavedCameras;
	}
}

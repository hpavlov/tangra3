namespace Tangra.VideoOperations.Astrometry.MotionFitting
{
    partial class frmAstrometryMotionFitting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAstrometryMotionFitting));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbtnOpenDirectory = new System.Windows.Forms.ToolStripButton();
            this.tsbtnOpenFile = new System.Windows.Forms.ToolStripButton();
            this.pnlFiles = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.nudPixelsPerArcSec = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbWeightingAstr = new System.Windows.Forms.RadioButton();
            this.label12 = new System.Windows.Forms.Label();
            this.nudSigmaExclusion = new System.Windows.Forms.NumericUpDown();
            this.cbxFactorInPositionalUncertainty = new System.Windows.Forms.CheckBox();
            this.cbxContraintPattern = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cbxOutlierRemoval = new System.Windows.Forms.CheckBox();
            this.rbWeightingPosAstr = new System.Windows.Forms.RadioButton();
            this.rbWeightingNone = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.nudMeaIntervals = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.nudInstDelaySec = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.tbxObjectDesign = new System.Windows.Forms.TextBox();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.tbxObsCode = new System.Windows.Forms.TextBox();
            this.lbAvailableFiles = new System.Windows.Forms.ListBox();
            this.lblAvailableSpectraTitle = new System.Windows.Forms.Label();
            this.pnlClient = new System.Windows.Forms.Panel();
            this.pnlPlot = new System.Windows.Forms.Panel();
            this.pboxDECPlot = new System.Windows.Forms.PictureBox();
            this.pboxRAPlot = new System.Windows.Forms.PictureBox();
            this.pnlOutput = new System.Windows.Forms.Panel();
            this.pnlReportHolder = new System.Windows.Forms.Panel();
            this.tbxMeasurements = new System.Windows.Forms.TextBox();
            this.pnlExportOptions = new System.Windows.Forms.Panel();
            this.label14 = new System.Windows.Forms.Label();
            this.lblDE = new System.Windows.Forms.Label();
            this.lblRA = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.nudSinglePosMea = new System.Windows.Forms.NumericUpDown();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.cbxErrorMethod = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            this.pnlFiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPixelsPerArcSec)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSigmaExclusion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMeaIntervals)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudInstDelaySec)).BeginInit();
            this.pnlClient.SuspendLayout();
            this.pnlPlot.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pboxDECPlot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pboxRAPlot)).BeginInit();
            this.pnlOutput.SuspendLayout();
            this.pnlReportHolder.SuspendLayout();
            this.pnlExportOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSinglePosMea)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbtnOpenDirectory,
            this.tsbtnOpenFile});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(872, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbtnOpenDirectory
            // 
            this.tsbtnOpenDirectory.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbtnOpenDirectory.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnOpenDirectory.Image")));
            this.tsbtnOpenDirectory.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnOpenDirectory.Name = "tsbtnOpenDirectory";
            this.tsbtnOpenDirectory.Size = new System.Drawing.Size(84, 22);
            this.tsbtnOpenDirectory.Text = "Add Directory";
            this.tsbtnOpenDirectory.Click += new System.EventHandler(this.tsbtnOpenDirectory_Click);
            // 
            // tsbtnOpenFile
            // 
            this.tsbtnOpenFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbtnOpenFile.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnOpenFile.Image")));
            this.tsbtnOpenFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnOpenFile.Name = "tsbtnOpenFile";
            this.tsbtnOpenFile.Size = new System.Drawing.Size(59, 22);
            this.tsbtnOpenFile.Text = "Add Files";
            this.tsbtnOpenFile.Click += new System.EventHandler(this.tsbtnOpenFile_Click);
            // 
            // pnlFiles
            // 
            this.pnlFiles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFiles.Controls.Add(this.label10);
            this.pnlFiles.Controls.Add(this.nudPixelsPerArcSec);
            this.pnlFiles.Controls.Add(this.label9);
            this.pnlFiles.Controls.Add(this.groupBox1);
            this.pnlFiles.Controls.Add(this.label3);
            this.pnlFiles.Controls.Add(this.label2);
            this.pnlFiles.Controls.Add(this.label1);
            this.pnlFiles.Controls.Add(this.label5);
            this.pnlFiles.Controls.Add(this.nudInstDelaySec);
            this.pnlFiles.Controls.Add(this.label4);
            this.pnlFiles.Controls.Add(this.tbxObjectDesign);
            this.pnlFiles.Controls.Add(this.dtpDate);
            this.pnlFiles.Controls.Add(this.tbxObsCode);
            this.pnlFiles.Controls.Add(this.lbAvailableFiles);
            this.pnlFiles.Controls.Add(this.lblAvailableSpectraTitle);
            this.pnlFiles.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlFiles.Location = new System.Drawing.Point(0, 25);
            this.pnlFiles.Name = "pnlFiles";
            this.pnlFiles.Size = new System.Drawing.Size(207, 531);
            this.pnlFiles.TabIndex = 3;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(165, 288);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(39, 13);
            this.label10.TabIndex = 43;
            this.label10.Text = "arcsec";
            // 
            // nudPixelsPerArcSec
            // 
            this.nudPixelsPerArcSec.DecimalPlaces = 2;
            this.nudPixelsPerArcSec.Location = new System.Drawing.Point(109, 286);
            this.nudPixelsPerArcSec.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.nudPixelsPerArcSec.Name = "nudPixelsPerArcSec";
            this.nudPixelsPerArcSec.Size = new System.Drawing.Size(49, 20);
            this.nudPixelsPerArcSec.TabIndex = 42;
            this.nudPixelsPerArcSec.ValueChanged += new System.EventHandler(this.nudPixelsPerArcSec_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(25, 288);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(83, 13);
            this.label9.TabIndex = 41;
            this.label9.Text = "Scale, 1 Pixel = ";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.cbxErrorMethod);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.rbWeightingAstr);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.nudSigmaExclusion);
            this.groupBox1.Controls.Add(this.cbxFactorInPositionalUncertainty);
            this.groupBox1.Controls.Add(this.cbxContraintPattern);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.cbxOutlierRemoval);
            this.groupBox1.Controls.Add(this.rbWeightingPosAstr);
            this.groupBox1.Controls.Add(this.rbWeightingNone);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.nudMeaIntervals);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Location = new System.Drawing.Point(6, 312);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(194, 214);
            this.groupBox1.TabIndex = 40;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Reduction Settings";
            // 
            // rbWeightingAstr
            // 
            this.rbWeightingAstr.AutoSize = true;
            this.rbWeightingAstr.Location = new System.Drawing.Point(19, 172);
            this.rbWeightingAstr.Name = "rbWeightingAstr";
            this.rbWeightingAstr.Size = new System.Drawing.Size(124, 17);
            this.rbWeightingAstr.TabIndex = 46;
            this.rbWeightingAstr.Text = "StdDev (Plate Solve)";
            this.rbWeightingAstr.UseVisualStyleBackColor = true;
            this.rbWeightingAstr.CheckedChanged += new System.EventHandler(this.OnWeightingChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.label12.Location = new System.Drawing.Point(156, 99);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(13, 13);
            this.label12.TabIndex = 45;
            this.label12.Text = "s";
            // 
            // nudSigmaExclusion
            // 
            this.nudSigmaExclusion.DecimalPlaces = 1;
            this.nudSigmaExclusion.Location = new System.Drawing.Point(115, 95);
            this.nudSigmaExclusion.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.nudSigmaExclusion.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudSigmaExclusion.Name = "nudSigmaExclusion";
            this.nudSigmaExclusion.Size = new System.Drawing.Size(39, 20);
            this.nudSigmaExclusion.TabIndex = 44;
            this.nudSigmaExclusion.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudSigmaExclusion.ValueChanged += new System.EventHandler(this.nudSigmaExclusion_ValueChanged);
            // 
            // cbxFactorInPositionalUncertainty
            // 
            this.cbxFactorInPositionalUncertainty.AutoSize = true;
            this.cbxFactorInPositionalUncertainty.Checked = true;
            this.cbxFactorInPositionalUncertainty.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxFactorInPositionalUncertainty.Location = new System.Drawing.Point(12, 116);
            this.cbxFactorInPositionalUncertainty.Name = "cbxFactorInPositionalUncertainty";
            this.cbxFactorInPositionalUncertainty.Size = new System.Drawing.Size(164, 17);
            this.cbxFactorInPositionalUncertainty.TabIndex = 43;
            this.cbxFactorInPositionalUncertainty.Text = "Include Positional Uncertanty";
            this.cbxFactorInPositionalUncertainty.UseVisualStyleBackColor = true;
            this.cbxFactorInPositionalUncertainty.CheckedChanged += new System.EventHandler(this.cbxFactorInPositionalUncertainty_CheckedChanged);
            // 
            // cbxContraintPattern
            // 
            this.cbxContraintPattern.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxContraintPattern.FormattingEnabled = true;
            this.cbxContraintPattern.Items.AddRange(new object[] {
            "None",
            "Pattern 1",
            "Pattern 2",
            "Pattern 3"});
            this.cbxContraintPattern.Location = new System.Drawing.Point(103, 46);
            this.cbxContraintPattern.Name = "cbxContraintPattern";
            this.cbxContraintPattern.Size = new System.Drawing.Size(82, 21);
            this.cbxContraintPattern.TabIndex = 42;
            this.cbxContraintPattern.SelectedIndexChanged += new System.EventHandler(this.cbxContraintPattern_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 50);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 13);
            this.label8.TabIndex = 41;
            this.label8.Text = "Constraint Points:";
            // 
            // cbxOutlierRemoval
            // 
            this.cbxOutlierRemoval.AutoSize = true;
            this.cbxOutlierRemoval.Checked = true;
            this.cbxOutlierRemoval.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxOutlierRemoval.Location = new System.Drawing.Point(12, 98);
            this.cbxOutlierRemoval.Name = "cbxOutlierRemoval";
            this.cbxOutlierRemoval.Size = new System.Drawing.Size(104, 17);
            this.cbxOutlierRemoval.TabIndex = 40;
            this.cbxOutlierRemoval.Text = "Remove Outliers";
            this.cbxOutlierRemoval.UseVisualStyleBackColor = true;
            this.cbxOutlierRemoval.CheckedChanged += new System.EventHandler(this.cbxOutlierRemoval_CheckedChanged);
            // 
            // rbWeightingPosAstr
            // 
            this.rbWeightingPosAstr.AutoSize = true;
            this.rbWeightingPosAstr.Checked = true;
            this.rbWeightingPosAstr.Location = new System.Drawing.Point(19, 190);
            this.rbWeightingPosAstr.Name = "rbWeightingPosAstr";
            this.rbWeightingPosAstr.Size = new System.Drawing.Size(173, 17);
            this.rbWeightingPosAstr.TabIndex = 39;
            this.rbWeightingPosAstr.TabStop = true;
            this.rbWeightingPosAstr.Text = "StdDev (Plate Solve + Position)";
            this.rbWeightingPosAstr.UseVisualStyleBackColor = true;
            this.rbWeightingPosAstr.CheckedChanged += new System.EventHandler(this.OnWeightingChanged);
            // 
            // rbWeightingNone
            // 
            this.rbWeightingNone.AutoSize = true;
            this.rbWeightingNone.Location = new System.Drawing.Point(19, 154);
            this.rbWeightingNone.Name = "rbWeightingNone";
            this.rbWeightingNone.Size = new System.Drawing.Size(51, 17);
            this.rbWeightingNone.TabIndex = 38;
            this.rbWeightingNone.Text = "None";
            this.rbWeightingNone.UseVisualStyleBackColor = true;
            this.rbWeightingNone.CheckedChanged += new System.EventHandler(this.OnWeightingChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 136);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 37;
            this.label7.Text = "Weighting:";
            // 
            // nudMeaIntervals
            // 
            this.nudMeaIntervals.Location = new System.Drawing.Point(105, 23);
            this.nudMeaIntervals.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudMeaIntervals.Name = "nudMeaIntervals";
            this.nudMeaIntervals.Size = new System.Drawing.Size(49, 20);
            this.nudMeaIntervals.TabIndex = 36;
            this.nudMeaIntervals.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudMeaIntervals.ValueChanged += new System.EventHandler(this.OnChunkSizeChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(97, 13);
            this.label6.TabIndex = 35;
            this.label6.Text = "Intervals/Positions:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 237);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 13);
            this.label3.TabIndex = 39;
            this.label3.Text = "Observatory Code:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 210);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 38;
            this.label2.Text = "Observation Date:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 185);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 37;
            this.label1.Text = "MPC Designation:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(166, 263);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(24, 13);
            this.label5.TabIndex = 36;
            this.label5.Text = "sec";
            // 
            // nudInstDelaySec
            // 
            this.nudInstDelaySec.DecimalPlaces = 2;
            this.nudInstDelaySec.Location = new System.Drawing.Point(109, 260);
            this.nudInstDelaySec.Name = "nudInstDelaySec";
            this.nudInstDelaySec.Size = new System.Drawing.Size(49, 20);
            this.nudInstDelaySec.TabIndex = 35;
            this.nudInstDelaySec.ValueChanged += new System.EventHandler(this.nudInstDelaySec_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 263);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 13);
            this.label4.TabIndex = 34;
            this.label4.Text = "Instrumental Delay:";
            // 
            // tbxObjectDesign
            // 
            this.tbxObjectDesign.Location = new System.Drawing.Point(109, 182);
            this.tbxObjectDesign.Name = "tbxObjectDesign";
            this.tbxObjectDesign.Size = new System.Drawing.Size(85, 20);
            this.tbxObjectDesign.TabIndex = 33;
            this.tbxObjectDesign.TextChanged += new System.EventHandler(this.tbxObjectDesign_TextChanged);
            // 
            // dtpDate
            // 
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDate.Location = new System.Drawing.Point(109, 208);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(82, 20);
            this.dtpDate.TabIndex = 32;
            this.dtpDate.Value = new System.DateTime(2017, 2, 9, 17, 49, 22, 0);
            this.dtpDate.ValueChanged += new System.EventHandler(this.dtpDate_ValueChanged);
            // 
            // tbxObsCode
            // 
            this.tbxObsCode.Location = new System.Drawing.Point(109, 234);
            this.tbxObsCode.Name = "tbxObsCode";
            this.tbxObsCode.Size = new System.Drawing.Size(29, 20);
            this.tbxObsCode.TabIndex = 31;
            this.tbxObsCode.TextChanged += new System.EventHandler(this.tbxObsCode_TextChanged);
            // 
            // lbAvailableFiles
            // 
            this.lbAvailableFiles.FormattingEnabled = true;
            this.lbAvailableFiles.Location = new System.Drawing.Point(10, 18);
            this.lbAvailableFiles.Name = "lbAvailableFiles";
            this.lbAvailableFiles.Size = new System.Drawing.Size(184, 160);
            this.lbAvailableFiles.TabIndex = 3;
            this.lbAvailableFiles.SelectedIndexChanged += new System.EventHandler(this.lbAvailableFiles_SelectedIndexChanged);
            // 
            // lblAvailableSpectraTitle
            // 
            this.lblAvailableSpectraTitle.AutoSize = true;
            this.lblAvailableSpectraTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAvailableSpectraTitle.Location = new System.Drawing.Point(7, 2);
            this.lblAvailableSpectraTitle.Name = "lblAvailableSpectraTitle";
            this.lblAvailableSpectraTitle.Size = new System.Drawing.Size(163, 13);
            this.lblAvailableSpectraTitle.TabIndex = 2;
            this.lblAvailableSpectraTitle.Text = "Available Files (select to process)";
            // 
            // pnlClient
            // 
            this.pnlClient.Controls.Add(this.pnlPlot);
            this.pnlClient.Controls.Add(this.pnlOutput);
            this.pnlClient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlClient.Location = new System.Drawing.Point(207, 25);
            this.pnlClient.Name = "pnlClient";
            this.pnlClient.Size = new System.Drawing.Size(665, 531);
            this.pnlClient.TabIndex = 4;
            // 
            // pnlPlot
            // 
            this.pnlPlot.Controls.Add(this.pboxDECPlot);
            this.pnlPlot.Controls.Add(this.pboxRAPlot);
            this.pnlPlot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPlot.Location = new System.Drawing.Point(0, 0);
            this.pnlPlot.Name = "pnlPlot";
            this.pnlPlot.Size = new System.Drawing.Size(665, 452);
            this.pnlPlot.TabIndex = 2;
            // 
            // pboxDECPlot
            // 
            this.pboxDECPlot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pboxDECPlot.Location = new System.Drawing.Point(330, 0);
            this.pboxDECPlot.Name = "pboxDECPlot";
            this.pboxDECPlot.Size = new System.Drawing.Size(335, 452);
            this.pboxDECPlot.TabIndex = 1;
            this.pboxDECPlot.TabStop = false;
            // 
            // pboxRAPlot
            // 
            this.pboxRAPlot.Dock = System.Windows.Forms.DockStyle.Left;
            this.pboxRAPlot.Location = new System.Drawing.Point(0, 0);
            this.pboxRAPlot.Name = "pboxRAPlot";
            this.pboxRAPlot.Size = new System.Drawing.Size(330, 452);
            this.pboxRAPlot.TabIndex = 0;
            this.pboxRAPlot.TabStop = false;
            // 
            // pnlOutput
            // 
            this.pnlOutput.Controls.Add(this.pnlReportHolder);
            this.pnlOutput.Controls.Add(this.pnlExportOptions);
            this.pnlOutput.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlOutput.Location = new System.Drawing.Point(0, 452);
            this.pnlOutput.Name = "pnlOutput";
            this.pnlOutput.Size = new System.Drawing.Size(665, 79);
            this.pnlOutput.TabIndex = 1;
            // 
            // pnlReportHolder
            // 
            this.pnlReportHolder.Controls.Add(this.tbxMeasurements);
            this.pnlReportHolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlReportHolder.Location = new System.Drawing.Point(0, 0);
            this.pnlReportHolder.Name = "pnlReportHolder";
            this.pnlReportHolder.Size = new System.Drawing.Size(505, 79);
            this.pnlReportHolder.TabIndex = 2;
            // 
            // tbxMeasurements
            // 
            this.tbxMeasurements.BackColor = System.Drawing.SystemColors.Info;
            this.tbxMeasurements.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbxMeasurements.Location = new System.Drawing.Point(0, 0);
            this.tbxMeasurements.Multiline = true;
            this.tbxMeasurements.Name = "tbxMeasurements";
            this.tbxMeasurements.ReadOnly = true;
            this.tbxMeasurements.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbxMeasurements.Size = new System.Drawing.Size(505, 79);
            this.tbxMeasurements.TabIndex = 0;
            // 
            // pnlExportOptions
            // 
            this.pnlExportOptions.Controls.Add(this.label14);
            this.pnlExportOptions.Controls.Add(this.lblDE);
            this.pnlExportOptions.Controls.Add(this.lblRA);
            this.pnlExportOptions.Controls.Add(this.label11);
            this.pnlExportOptions.Controls.Add(this.nudSinglePosMea);
            this.pnlExportOptions.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlExportOptions.Location = new System.Drawing.Point(505, 0);
            this.pnlExportOptions.Name = "pnlExportOptions";
            this.pnlExportOptions.Size = new System.Drawing.Size(160, 79);
            this.pnlExportOptions.TabIndex = 1;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 4);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(103, 13);
            this.label14.TabIndex = 40;
            this.label14.Text = "Single Measurement";
            // 
            // lblDE
            // 
            this.lblDE.AutoSize = true;
            this.lblDE.Location = new System.Drawing.Point(12, 61);
            this.lblDE.Name = "lblDE";
            this.lblDE.Size = new System.Drawing.Size(0, 13);
            this.lblDE.TabIndex = 39;
            // 
            // lblRA
            // 
            this.lblRA.AutoSize = true;
            this.lblRA.Location = new System.Drawing.Point(12, 44);
            this.lblRA.Name = "lblRA";
            this.lblRA.Size = new System.Drawing.Size(0, 13);
            this.lblRA.TabIndex = 38;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 23);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(67, 13);
            this.label11.TabIndex = 37;
            this.label11.Text = "Time of Day:";
            // 
            // nudSinglePosMea
            // 
            this.nudSinglePosMea.DecimalPlaces = 6;
            this.nudSinglePosMea.Increment = new decimal(new int[] {
            1,
            0,
            0,
            393216});
            this.nudSinglePosMea.Location = new System.Drawing.Point(79, 21);
            this.nudSinglePosMea.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSinglePosMea.Name = "nudSinglePosMea";
            this.nudSinglePosMea.Size = new System.Drawing.Size(69, 20);
            this.nudSinglePosMea.TabIndex = 36;
            this.nudSinglePosMea.KeyDown += new System.Windows.Forms.KeyEventHandler(this.nudSinglePosMea_KeyDown);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "*.csv";
            this.openFileDialog1.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            this.openFileDialog1.Multiselect = true;
            // 
            // cbxErrorMethod
            // 
            this.cbxErrorMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxErrorMethod.FormattingEnabled = true;
            this.cbxErrorMethod.Items.AddRange(new object[] {
            "None",
            "Pattern 1",
            "Pattern 2",
            "Pattern 3"});
            this.cbxErrorMethod.Location = new System.Drawing.Point(84, 71);
            this.cbxErrorMethod.Name = "cbxErrorMethod";
            this.cbxErrorMethod.Size = new System.Drawing.Size(101, 21);
            this.cbxErrorMethod.TabIndex = 48;
            this.cbxErrorMethod.SelectedIndexChanged += new System.EventHandler(this.cbxErrorMethod_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 75);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(73, 13);
            this.label13.TabIndex = 47;
            this.label13.Text = "Solution Error:";
            // 
            // frmAstrometryMotionFitting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(872, 556);
            this.Controls.Add(this.pnlClient);
            this.Controls.Add(this.pnlFiles);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(888, 581);
            this.Name = "frmAstrometryMotionFitting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tangra - Fast Motion Astrometry";
            this.Resize += new System.EventHandler(this.frmAstrometryMotionFitting_Resize);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.pnlFiles.ResumeLayout(false);
            this.pnlFiles.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPixelsPerArcSec)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSigmaExclusion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMeaIntervals)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudInstDelaySec)).EndInit();
            this.pnlClient.ResumeLayout(false);
            this.pnlPlot.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pboxDECPlot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pboxRAPlot)).EndInit();
            this.pnlOutput.ResumeLayout(false);
            this.pnlReportHolder.ResumeLayout(false);
            this.pnlReportHolder.PerformLayout();
            this.pnlExportOptions.ResumeLayout(false);
            this.pnlExportOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSinglePosMea)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbtnOpenDirectory;
        private System.Windows.Forms.ToolStripButton tsbtnOpenFile;
        private System.Windows.Forms.Panel pnlFiles;
        private System.Windows.Forms.ListBox lbAvailableFiles;
        private System.Windows.Forms.Label lblAvailableSpectraTitle;
        private System.Windows.Forms.Panel pnlClient;
        private System.Windows.Forms.Panel pnlPlot;
        private System.Windows.Forms.Panel pnlOutput;
        private System.Windows.Forms.TextBox tbxMeasurements;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudInstDelaySec;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbxObjectDesign;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.TextBox tbxObsCode;
        private System.Windows.Forms.PictureBox pboxDECPlot;
        private System.Windows.Forms.PictureBox pboxRAPlot;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown nudMeaIntervals;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RadioButton rbWeightingPosAstr;
        private System.Windows.Forms.RadioButton rbWeightingNone;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.CheckBox cbxOutlierRemoval;
        private System.Windows.Forms.Panel pnlReportHolder;
        private System.Windows.Forms.Panel pnlExportOptions;
        private System.Windows.Forms.ComboBox cbxContraintPattern;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown nudPixelsPerArcSec;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox cbxFactorInPositionalUncertainty;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown nudSinglePosMea;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lblDE;
        private System.Windows.Forms.Label lblRA;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown nudSigmaExclusion;
        private System.Windows.Forms.RadioButton rbWeightingAstr;
        private System.Windows.Forms.ComboBox cbxErrorMethod;
        private System.Windows.Forms.Label label13;
    }
}
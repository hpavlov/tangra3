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
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.nudInstDelaySec = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.tbxObjectDesign = new System.Windows.Forms.TextBox();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.tbxObsCode = new System.Windows.Forms.TextBox();
            this.btnBrowseFiles = new System.Windows.Forms.Button();
            this.lbAvailableFiles = new System.Windows.Forms.ListBox();
            this.lblAvailableSpectraTitle = new System.Windows.Forms.Label();
            this.pnlClient = new System.Windows.Forms.Panel();
            this.pnlPlot = new System.Windows.Forms.Panel();
            this.pboxDECPlot = new System.Windows.Forms.PictureBox();
            this.pboxRAPlot = new System.Windows.Forms.PictureBox();
            this.pnlOutput = new System.Windows.Forms.Panel();
            this.tbxMeasurements = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.nudMeaIntervals = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.rbWeightingNone = new System.Windows.Forms.RadioButton();
            this.rbWeightingPosAstr = new System.Windows.Forms.RadioButton();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.toolStrip1.SuspendLayout();
            this.pnlFiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudInstDelaySec)).BeginInit();
            this.pnlClient.SuspendLayout();
            this.pnlPlot.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pboxDECPlot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pboxRAPlot)).BeginInit();
            this.pnlOutput.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMeaIntervals)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbtnOpenDirectory,
            this.tsbtnOpenFile});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(874, 25);
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
            this.tsbtnOpenFile.Size = new System.Drawing.Size(54, 22);
            this.tsbtnOpenFile.Text = "Add File";
            this.tsbtnOpenFile.Click += new System.EventHandler(this.tsbtnOpenFile_Click);
            // 
            // pnlFiles
            // 
            this.pnlFiles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
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
            this.pnlFiles.Controls.Add(this.btnBrowseFiles);
            this.pnlFiles.Controls.Add(this.lbAvailableFiles);
            this.pnlFiles.Controls.Add(this.lblAvailableSpectraTitle);
            this.pnlFiles.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlFiles.Location = new System.Drawing.Point(0, 25);
            this.pnlFiles.Name = "pnlFiles";
            this.pnlFiles.Size = new System.Drawing.Size(207, 436);
            this.pnlFiles.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 253);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 13);
            this.label3.TabIndex = 39;
            this.label3.Text = "Observatory Code:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 226);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 38;
            this.label2.Text = "Observation Date:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 201);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 37;
            this.label1.Text = "MPC Designation:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(166, 279);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(24, 13);
            this.label5.TabIndex = 36;
            this.label5.Text = "sec";
            // 
            // nudInstDelaySec
            // 
            this.nudInstDelaySec.DecimalPlaces = 2;
            this.nudInstDelaySec.Location = new System.Drawing.Point(109, 276);
            this.nudInstDelaySec.Name = "nudInstDelaySec";
            this.nudInstDelaySec.Size = new System.Drawing.Size(49, 20);
            this.nudInstDelaySec.TabIndex = 35;
            this.nudInstDelaySec.Value = new decimal(new int[] {
            131,
            0,
            0,
            131072});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 279);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 13);
            this.label4.TabIndex = 34;
            this.label4.Text = "Instrumental Delay:";
            // 
            // tbxObjectDesign
            // 
            this.tbxObjectDesign.Location = new System.Drawing.Point(109, 198);
            this.tbxObjectDesign.Name = "tbxObjectDesign";
            this.tbxObjectDesign.Size = new System.Drawing.Size(85, 20);
            this.tbxObjectDesign.TabIndex = 33;
            this.tbxObjectDesign.Text = "K10R12F";
            // 
            // dtpDate
            // 
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDate.Location = new System.Drawing.Point(109, 224);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(82, 20);
            this.dtpDate.TabIndex = 32;
            this.dtpDate.Value = new System.DateTime(2010, 9, 8, 0, 0, 0, 0);
            // 
            // tbxObsCode
            // 
            this.tbxObsCode.Location = new System.Drawing.Point(109, 250);
            this.tbxObsCode.Name = "tbxObsCode";
            this.tbxObsCode.Size = new System.Drawing.Size(29, 20);
            this.tbxObsCode.TabIndex = 31;
            this.tbxObsCode.Text = "E28";
            // 
            // btnBrowseFiles
            // 
            this.btnBrowseFiles.Location = new System.Drawing.Point(13, 460);
            this.btnBrowseFiles.Name = "btnBrowseFiles";
            this.btnBrowseFiles.Size = new System.Drawing.Size(184, 27);
            this.btnBrowseFiles.TabIndex = 4;
            this.btnBrowseFiles.Text = "Change Spectra Files Location";
            this.btnBrowseFiles.UseVisualStyleBackColor = true;
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
            this.pnlClient.Size = new System.Drawing.Size(667, 436);
            this.pnlClient.TabIndex = 4;
            // 
            // pnlPlot
            // 
            this.pnlPlot.Controls.Add(this.pboxDECPlot);
            this.pnlPlot.Controls.Add(this.pboxRAPlot);
            this.pnlPlot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPlot.Location = new System.Drawing.Point(0, 0);
            this.pnlPlot.Name = "pnlPlot";
            this.pnlPlot.Size = new System.Drawing.Size(667, 357);
            this.pnlPlot.TabIndex = 2;
            // 
            // pboxDECPlot
            // 
            this.pboxDECPlot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pboxDECPlot.Location = new System.Drawing.Point(334, 3);
            this.pboxDECPlot.Name = "pboxDECPlot";
            this.pboxDECPlot.Size = new System.Drawing.Size(330, 351);
            this.pboxDECPlot.TabIndex = 1;
            this.pboxDECPlot.TabStop = false;
            // 
            // pboxRAPlot
            // 
            this.pboxRAPlot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pboxRAPlot.Location = new System.Drawing.Point(3, 3);
            this.pboxRAPlot.Name = "pboxRAPlot";
            this.pboxRAPlot.Size = new System.Drawing.Size(330, 351);
            this.pboxRAPlot.TabIndex = 0;
            this.pboxRAPlot.TabStop = false;
            // 
            // pnlOutput
            // 
            this.pnlOutput.Controls.Add(this.tbxMeasurements);
            this.pnlOutput.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlOutput.Location = new System.Drawing.Point(0, 357);
            this.pnlOutput.Name = "pnlOutput";
            this.pnlOutput.Size = new System.Drawing.Size(667, 79);
            this.pnlOutput.TabIndex = 1;
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
            this.tbxMeasurements.Size = new System.Drawing.Size(667, 79);
            this.tbxMeasurements.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.rbWeightingPosAstr);
            this.groupBox1.Controls.Add(this.rbWeightingNone);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.nudMeaIntervals);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Location = new System.Drawing.Point(6, 310);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(194, 121);
            this.groupBox1.TabIndex = 40;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Reduction Settings";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(97, 13);
            this.label6.TabIndex = 35;
            this.label6.Text = "Intervals/Positions:";
            // 
            // nudMeaIntervals
            // 
            this.nudMeaIntervals.Location = new System.Drawing.Point(109, 23);
            this.nudMeaIntervals.Name = "nudMeaIntervals";
            this.nudMeaIntervals.Size = new System.Drawing.Size(49, 20);
            this.nudMeaIntervals.TabIndex = 36;
            this.nudMeaIntervals.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 49);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 37;
            this.label7.Text = "Weighting:";
            // 
            // rbWeightingNone
            // 
            this.rbWeightingNone.AutoSize = true;
            this.rbWeightingNone.Location = new System.Drawing.Point(22, 69);
            this.rbWeightingNone.Name = "rbWeightingNone";
            this.rbWeightingNone.Size = new System.Drawing.Size(51, 17);
            this.rbWeightingNone.TabIndex = 38;
            this.rbWeightingNone.Text = "None";
            this.rbWeightingNone.UseVisualStyleBackColor = true;
            // 
            // rbWeightingPosAstr
            // 
            this.rbWeightingPosAstr.AutoSize = true;
            this.rbWeightingPosAstr.Checked = true;
            this.rbWeightingPosAstr.Location = new System.Drawing.Point(22, 92);
            this.rbWeightingPosAstr.Name = "rbWeightingPosAstr";
            this.rbWeightingPosAstr.Size = new System.Drawing.Size(170, 17);
            this.rbWeightingPosAstr.TabIndex = 39;
            this.rbWeightingPosAstr.TabStop = true;
            this.rbWeightingPosAstr.Text = "StdDev(Plate Solve + Position)";
            this.rbWeightingPosAstr.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "*.csv";
            this.openFileDialog1.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            // 
            // frmAstrometryMotionFitting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(874, 461);
            this.Controls.Add(this.pnlClient);
            this.Controls.Add(this.pnlFiles);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "frmAstrometryMotionFitting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tangra - Fast Motion Astrometry";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.pnlFiles.ResumeLayout(false);
            this.pnlFiles.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudInstDelaySec)).EndInit();
            this.pnlClient.ResumeLayout(false);
            this.pnlPlot.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pboxDECPlot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pboxRAPlot)).EndInit();
            this.pnlOutput.ResumeLayout(false);
            this.pnlOutput.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMeaIntervals)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbtnOpenDirectory;
        private System.Windows.Forms.ToolStripButton tsbtnOpenFile;
        private System.Windows.Forms.Panel pnlFiles;
        private System.Windows.Forms.Button btnBrowseFiles;
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
    }
}
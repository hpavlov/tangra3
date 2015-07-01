namespace Tangra.VideoOperations.Spectroscopy
{
    partial class frmViewSpectra
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmViewSpectra));
			this.pnlBottom = new System.Windows.Forms.Panel();
			this.cbxSlidingWindow = new System.Windows.Forms.CheckBox();
			this.hsbSlidingWindow = new System.Windows.Forms.HScrollBar();
			this.gbSelection = new System.Windows.Forms.GroupBox();
			this.lblWavelength = new System.Windows.Forms.Label();
			this.lblWavelengthCaption = new System.Windows.Forms.Label();
			this.lblPixelValue = new System.Windows.Forms.Label();
			this.lblPixelNo = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.gbxCalibration = new System.Windows.Forms.GroupBox();
			this.lblOrder = new System.Windows.Forms.Label();
			this.lblRMS = new System.Windows.Forms.Label();
			this.lblDispersion = new System.Windows.Forms.Label();
			this.pnlClient = new System.Windows.Forms.Panel();
			this.picSpectraGraph = new System.Windows.Forms.PictureBox();
			this.picSpectra = new System.Windows.Forms.PictureBox();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.miFile = new System.Windows.Forms.ToolStripMenuItem();
			this.miLoadSpectra = new System.Windows.Forms.ToolStripMenuItem();
			this.miSaveSpectra = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.miExport = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.miCloseForm = new System.Windows.Forms.ToolStripMenuItem();
			this.miData = new System.Windows.Forms.ToolStripMenuItem();
			this.miSpectralCalibration = new System.Windows.Forms.ToolStripMenuItem();
			this.miProcessing = new System.Windows.Forms.ToolStripMenuItem();
			this.miLowPass = new System.Windows.Forms.ToolStripMenuItem();
			this.miLPNone = new System.Windows.Forms.ToolStripMenuItem();
			this.miLP1_0 = new System.Windows.Forms.ToolStripMenuItem();
			this.miLP1_5 = new System.Windows.Forms.ToolStripMenuItem();
			this.miLP2_0 = new System.Windows.Forms.ToolStripMenuItem();
			this.miLP2_5 = new System.Windows.Forms.ToolStripMenuItem();
			this.miLP3_0 = new System.Windows.Forms.ToolStripMenuItem();
			this.miLP3_5 = new System.Windows.Forms.ToolStripMenuItem();
			this.miLP4_0 = new System.Windows.Forms.ToolStripMenuItem();
			this.miSpline = new System.Windows.Forms.ToolStripMenuItem();
			this.miView = new System.Windows.Forms.ToolStripMenuItem();
			this.miShowCommonLines = new System.Windows.Forms.ToolStripMenuItem();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.exportFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.miCustomize = new System.Windows.Forms.ToolStripMenuItem();
			this.miDisplaySettings = new System.Windows.Forms.ToolStripMenuItem();
			this.pnlBottom.SuspendLayout();
			this.gbSelection.SuspendLayout();
			this.gbxCalibration.SuspendLayout();
			this.pnlClient.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picSpectraGraph)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picSpectra)).BeginInit();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlBottom
			// 
			this.pnlBottom.Controls.Add(this.cbxSlidingWindow);
			this.pnlBottom.Controls.Add(this.hsbSlidingWindow);
			this.pnlBottom.Controls.Add(this.gbSelection);
			this.pnlBottom.Controls.Add(this.gbxCalibration);
			this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlBottom.Location = new System.Drawing.Point(0, 482);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Size = new System.Drawing.Size(807, 74);
			this.pnlBottom.TabIndex = 1;
			// 
			// cbxSlidingWindow
			// 
			this.cbxSlidingWindow.AutoSize = true;
			this.cbxSlidingWindow.Location = new System.Drawing.Point(277, 30);
			this.cbxSlidingWindow.Name = "cbxSlidingWindow";
			this.cbxSlidingWindow.Size = new System.Drawing.Size(141, 17);
			this.cbxSlidingWindow.TabIndex = 6;
			this.cbxSlidingWindow.Text = "Stack in Sliding Window";
			this.cbxSlidingWindow.UseVisualStyleBackColor = true;
			this.cbxSlidingWindow.Visible = false;
			this.cbxSlidingWindow.CheckedChanged += new System.EventHandler(this.cbxSlidingWindow_CheckedChanged);
			// 
			// hsbSlidingWindow
			// 
			this.hsbSlidingWindow.Enabled = false;
			this.hsbSlidingWindow.Location = new System.Drawing.Point(277, 10);
			this.hsbSlidingWindow.Name = "hsbSlidingWindow";
			this.hsbSlidingWindow.Size = new System.Drawing.Size(308, 16);
			this.hsbSlidingWindow.TabIndex = 5;
			this.hsbSlidingWindow.Visible = false;
			this.hsbSlidingWindow.ValueChanged += new System.EventHandler(this.hsbSlidingWindow_ValueChanged);
			// 
			// gbSelection
			// 
			this.gbSelection.Controls.Add(this.lblWavelength);
			this.gbSelection.Controls.Add(this.lblWavelengthCaption);
			this.gbSelection.Controls.Add(this.lblPixelValue);
			this.gbSelection.Controls.Add(this.lblPixelNo);
			this.gbSelection.Controls.Add(this.label2);
			this.gbSelection.Controls.Add(this.label1);
			this.gbSelection.Location = new System.Drawing.Point(12, 6);
			this.gbSelection.Name = "gbSelection";
			this.gbSelection.Size = new System.Drawing.Size(246, 61);
			this.gbSelection.TabIndex = 4;
			this.gbSelection.TabStop = false;
			this.gbSelection.Text = "Selected Line";
			this.gbSelection.Visible = false;
			// 
			// lblWavelength
			// 
			this.lblWavelength.AutoSize = true;
			this.lblWavelength.ForeColor = System.Drawing.Color.Navy;
			this.lblWavelength.Location = new System.Drawing.Point(176, 19);
			this.lblWavelength.Name = "lblWavelength";
			this.lblWavelength.Size = new System.Drawing.Size(0, 13);
			this.lblWavelength.TabIndex = 8;
			// 
			// lblWavelengthCaption
			// 
			this.lblWavelengthCaption.AutoSize = true;
			this.lblWavelengthCaption.Location = new System.Drawing.Point(107, 19);
			this.lblWavelengthCaption.Name = "lblWavelengthCaption";
			this.lblWavelengthCaption.Size = new System.Drawing.Size(68, 13);
			this.lblWavelengthCaption.TabIndex = 7;
			this.lblWavelengthCaption.Text = "Wavelength:";
			// 
			// lblPixelValue
			// 
			this.lblPixelValue.AutoSize = true;
			this.lblPixelValue.ForeColor = System.Drawing.Color.Navy;
			this.lblPixelValue.Location = new System.Drawing.Point(61, 40);
			this.lblPixelValue.Name = "lblPixelValue";
			this.lblPixelValue.Size = new System.Drawing.Size(0, 13);
			this.lblPixelValue.TabIndex = 6;
			// 
			// lblPixelNo
			// 
			this.lblPixelNo.AutoSize = true;
			this.lblPixelNo.ForeColor = System.Drawing.Color.Navy;
			this.lblPixelNo.Location = new System.Drawing.Point(61, 20);
			this.lblPixelNo.Name = "lblPixelNo";
			this.lblPixelNo.Size = new System.Drawing.Size(0, 13);
			this.lblPixelNo.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(22, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(37, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Value:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(27, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(32, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Pixel:";
			// 
			// gbxCalibration
			// 
			this.gbxCalibration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.gbxCalibration.Controls.Add(this.lblOrder);
			this.gbxCalibration.Controls.Add(this.lblRMS);
			this.gbxCalibration.Controls.Add(this.lblDispersion);
			this.gbxCalibration.Location = new System.Drawing.Point(649, 6);
			this.gbxCalibration.Name = "gbxCalibration";
			this.gbxCalibration.Size = new System.Drawing.Size(146, 61);
			this.gbxCalibration.TabIndex = 2;
			this.gbxCalibration.TabStop = false;
			this.gbxCalibration.Text = "Calibration";
			this.gbxCalibration.Visible = false;
			// 
			// lblOrder
			// 
			this.lblOrder.AutoSize = true;
			this.lblOrder.ForeColor = System.Drawing.Color.Navy;
			this.lblOrder.Location = new System.Drawing.Point(6, 19);
			this.lblOrder.Name = "lblOrder";
			this.lblOrder.Size = new System.Drawing.Size(51, 13);
			this.lblOrder.TabIndex = 4;
			this.lblOrder.Text = "Order = 1";
			// 
			// lblRMS
			// 
			this.lblRMS.AutoSize = true;
			this.lblRMS.ForeColor = System.Drawing.Color.Navy;
			this.lblRMS.Location = new System.Drawing.Point(62, 19);
			this.lblRMS.Name = "lblRMS";
			this.lblRMS.Size = new System.Drawing.Size(77, 13);
			this.lblRMS.TabIndex = 3;
			this.lblRMS.Text = "RMS= 0.92 pix";
			// 
			// lblDispersion
			// 
			this.lblDispersion.AutoSize = true;
			this.lblDispersion.ForeColor = System.Drawing.Color.Navy;
			this.lblDispersion.Location = new System.Drawing.Point(6, 40);
			this.lblDispersion.Name = "lblDispersion";
			this.lblDispersion.Size = new System.Drawing.Size(66, 13);
			this.lblDispersion.TabIndex = 2;
			this.lblDispersion.Text = "lblDispersion";
			// 
			// pnlClient
			// 
			this.pnlClient.Controls.Add(this.picSpectraGraph);
			this.pnlClient.Controls.Add(this.picSpectra);
			this.pnlClient.Controls.Add(this.menuStrip1);
			this.pnlClient.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlClient.Location = new System.Drawing.Point(0, 0);
			this.pnlClient.Name = "pnlClient";
			this.pnlClient.Size = new System.Drawing.Size(807, 482);
			this.pnlClient.TabIndex = 2;
			// 
			// picSpectraGraph
			// 
			this.picSpectraGraph.Dock = System.Windows.Forms.DockStyle.Fill;
			this.picSpectraGraph.Location = new System.Drawing.Point(0, 24);
			this.picSpectraGraph.Name = "picSpectraGraph";
			this.picSpectraGraph.Size = new System.Drawing.Size(807, 408);
			this.picSpectraGraph.TabIndex = 1;
			this.picSpectraGraph.TabStop = false;
			this.picSpectraGraph.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picSpectraGraph_MouseClick);
			this.picSpectraGraph.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picSpectraGraph_MouseDown);
			this.picSpectraGraph.MouseEnter += new System.EventHandler(this.picSpectraGraph_MouseEnter);
			this.picSpectraGraph.MouseLeave += new System.EventHandler(this.picSpectraGraph_MouseLeave);
			this.picSpectraGraph.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picSpectraGraph_MouseMove);
			this.picSpectraGraph.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picSpectraGraph_MouseUp);
			// 
			// picSpectra
			// 
			this.picSpectra.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.picSpectra.Location = new System.Drawing.Point(0, 432);
			this.picSpectra.Name = "picSpectra";
			this.picSpectra.Size = new System.Drawing.Size(807, 50);
			this.picSpectra.TabIndex = 0;
			this.picSpectra.TabStop = false;
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFile,
            this.miData,
            this.miCustomize,
            this.miView});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(807, 24);
			this.menuStrip1.TabIndex = 2;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// miFile
			// 
			this.miFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miLoadSpectra,
            this.miSaveSpectra,
            this.toolStripMenuItem1,
            this.miExport,
            this.toolStripSeparator1,
            this.miCloseForm});
			this.miFile.Name = "miFile";
			this.miFile.Size = new System.Drawing.Size(35, 20);
			this.miFile.Text = "&File";
			// 
			// miLoadSpectra
			// 
			this.miLoadSpectra.Name = "miLoadSpectra";
			this.miLoadSpectra.Size = new System.Drawing.Size(140, 22);
			this.miLoadSpectra.Text = "&Load Spectra ";
			// 
			// miSaveSpectra
			// 
			this.miSaveSpectra.Name = "miSaveSpectra";
			this.miSaveSpectra.Size = new System.Drawing.Size(140, 22);
			this.miSaveSpectra.Text = "&Save Spectra";
			this.miSaveSpectra.Click += new System.EventHandler(this.miSaveSpectra_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(137, 6);
			// 
			// miExport
			// 
			this.miExport.Name = "miExport";
			this.miExport.Size = new System.Drawing.Size(140, 22);
			this.miExport.Text = "Export";
			this.miExport.Click += new System.EventHandler(this.miExport_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(137, 6);
			// 
			// miCloseForm
			// 
			this.miCloseForm.Name = "miCloseForm";
			this.miCloseForm.Size = new System.Drawing.Size(140, 22);
			this.miCloseForm.Text = "&Close";
			// 
			// miData
			// 
			this.miData.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSpectralCalibration,
            this.miProcessing});
			this.miData.Name = "miData";
			this.miData.Size = new System.Drawing.Size(42, 20);
			this.miData.Text = "&Data";
			// 
			// miSpectralCalibration
			// 
			this.miSpectralCalibration.Name = "miSpectralCalibration";
			this.miSpectralCalibration.Size = new System.Drawing.Size(167, 22);
			this.miSpectralCalibration.Text = "Spectral &Calibration";
			this.miSpectralCalibration.Click += new System.EventHandler(this.miSpectralCalibration_Click);
			// 
			// miProcessing
			// 
			this.miProcessing.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miLowPass,
            this.miSpline});
			this.miProcessing.Name = "miProcessing";
			this.miProcessing.Size = new System.Drawing.Size(167, 22);
			this.miProcessing.Text = "&Processing";
			// 
			// miLowPass
			// 
			this.miLowPass.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miLPNone,
            this.miLP1_0,
            this.miLP1_5,
            this.miLP2_0,
            this.miLP2_5,
            this.miLP3_0,
            this.miLP3_5,
            this.miLP4_0});
			this.miLowPass.Name = "miLowPass";
			this.miLowPass.Size = new System.Drawing.Size(167, 22);
			this.miLowPass.Text = "&Low Pass Filter";
			this.miLowPass.DropDownOpening += new System.EventHandler(this.miLowPass_DropDownOpening);
			// 
			// miLPNone
			// 
			this.miLPNone.Name = "miLPNone";
			this.miLPNone.Size = new System.Drawing.Size(135, 22);
			this.miLPNone.Tag = "0";
			this.miLPNone.Text = "None";
			this.miLPNone.Click += new System.EventHandler(this.ApplyLowPassFilterMenuItemClicked);
			// 
			// miLP1_0
			// 
			this.miLP1_0.Name = "miLP1_0";
			this.miLP1_0.Size = new System.Drawing.Size(135, 22);
			this.miLP1_0.Tag = "1";
			this.miLP1_0.Text = "FWHM = 1";
			this.miLP1_0.Click += new System.EventHandler(this.ApplyLowPassFilterMenuItemClicked);
			// 
			// miLP1_5
			// 
			this.miLP1_5.Name = "miLP1_5";
			this.miLP1_5.Size = new System.Drawing.Size(135, 22);
			this.miLP1_5.Tag = "1.5";
			this.miLP1_5.Text = "FWHM = 1.5";
			this.miLP1_5.Click += new System.EventHandler(this.ApplyLowPassFilterMenuItemClicked);
			// 
			// miLP2_0
			// 
			this.miLP2_0.Name = "miLP2_0";
			this.miLP2_0.Size = new System.Drawing.Size(135, 22);
			this.miLP2_0.Tag = "2";
			this.miLP2_0.Text = "FWHM = 2";
			this.miLP2_0.Click += new System.EventHandler(this.ApplyLowPassFilterMenuItemClicked);
			// 
			// miLP2_5
			// 
			this.miLP2_5.Name = "miLP2_5";
			this.miLP2_5.Size = new System.Drawing.Size(135, 22);
			this.miLP2_5.Tag = "2.5";
			this.miLP2_5.Text = "FWHM = 2.5";
			this.miLP2_5.Click += new System.EventHandler(this.ApplyLowPassFilterMenuItemClicked);
			// 
			// miLP3_0
			// 
			this.miLP3_0.Name = "miLP3_0";
			this.miLP3_0.Size = new System.Drawing.Size(135, 22);
			this.miLP3_0.Tag = "3";
			this.miLP3_0.Text = "FWHM = 3";
			this.miLP3_0.Click += new System.EventHandler(this.ApplyLowPassFilterMenuItemClicked);
			// 
			// miLP3_5
			// 
			this.miLP3_5.Name = "miLP3_5";
			this.miLP3_5.Size = new System.Drawing.Size(135, 22);
			this.miLP3_5.Tag = "3.5";
			this.miLP3_5.Text = "FWHM = 3.5";
			this.miLP3_5.Click += new System.EventHandler(this.ApplyLowPassFilterMenuItemClicked);
			// 
			// miLP4_0
			// 
			this.miLP4_0.Name = "miLP4_0";
			this.miLP4_0.Size = new System.Drawing.Size(135, 22);
			this.miLP4_0.Tag = "4";
			this.miLP4_0.Text = "FWHM = 4.0";
			this.miLP4_0.Click += new System.EventHandler(this.ApplyLowPassFilterMenuItemClicked);
			// 
			// miSpline
			// 
			this.miSpline.Name = "miSpline";
			this.miSpline.Size = new System.Drawing.Size(167, 22);
			this.miSpline.Text = "&Spline Interpolation";
			// 
			// miView
			// 
			this.miView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miShowCommonLines});
			this.miView.Name = "miView";
			this.miView.Size = new System.Drawing.Size(41, 20);
			this.miView.Text = "&View";
			// 
			// miShowCommonLines
			// 
			this.miShowCommonLines.CheckOnClick = true;
			this.miShowCommonLines.Name = "miShowCommonLines";
			this.miShowCommonLines.Size = new System.Drawing.Size(171, 22);
			this.miShowCommonLines.Text = "Show &Common Lines";
			this.miShowCommonLines.CheckedChanged += new System.EventHandler(this.miShowCommonLines_CheckedChanged);
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.Filter = "Tangra Spectra (*.spectra)|*.spectra";
			// 
			// exportFileDialog
			// 
			this.exportFileDialog.Filter = "Tabular Spectra Format (*.dat)|*.dat";
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "Tangra Spectra (*.spectra)|*.spectra";
			// 
			// miCustomize
			// 
			this.miCustomize.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miDisplaySettings});
			this.miCustomize.Name = "miCustomize";
			this.miCustomize.Size = new System.Drawing.Size(68, 20);
			this.miCustomize.Text = "&Customize";
			// 
			// miDisplaySettings
			// 
			this.miDisplaySettings.Name = "miDisplaySettings";
			this.miDisplaySettings.Size = new System.Drawing.Size(152, 22);
			this.miDisplaySettings.Text = "&Display Settings";
			this.miDisplaySettings.Click += new System.EventHandler(this.miDisplaySettings_Click);
			// 
			// frmViewSpectra
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(807, 556);
			this.Controls.Add(this.pnlClient);
			this.Controls.Add(this.pnlBottom);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "frmViewSpectra";
			this.Text = "Spectra Viewer";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmViewSpectra_FormClosing);
			this.Load += new System.EventHandler(this.frmViewSpectra_Load);
			this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.frmViewSpectra_PreviewKeyDown);
			this.Resize += new System.EventHandler(this.frmViewSpectra_Resize);
			this.pnlBottom.ResumeLayout(false);
			this.pnlBottom.PerformLayout();
			this.gbSelection.ResumeLayout(false);
			this.gbSelection.PerformLayout();
			this.gbxCalibration.ResumeLayout(false);
			this.gbxCalibration.PerformLayout();
			this.pnlClient.ResumeLayout(false);
			this.pnlClient.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.picSpectraGraph)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picSpectra)).EndInit();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Panel pnlClient;
        private System.Windows.Forms.PictureBox picSpectra;
        private System.Windows.Forms.PictureBox picSpectraGraph;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem miFile;
		private System.Windows.Forms.ToolStripMenuItem miLoadSpectra;
		private System.Windows.Forms.ToolStripMenuItem miSaveSpectra;
		private System.Windows.Forms.ToolStripMenuItem miView;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.ToolStripMenuItem miData;
		private System.Windows.Forms.ToolStripMenuItem miSpectralCalibration;
        private System.Windows.Forms.GroupBox gbxCalibration;
        private System.Windows.Forms.Label lblDispersion;
		private System.Windows.Forms.ToolStripMenuItem miShowCommonLines;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem miCloseForm;
		private System.Windows.Forms.ToolStripMenuItem miExport;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.GroupBox gbSelection;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblPixelValue;
		private System.Windows.Forms.Label lblPixelNo;
		private System.Windows.Forms.Label lblWavelength;
        private System.Windows.Forms.Label lblWavelengthCaption;
        private System.Windows.Forms.ToolStripMenuItem miProcessing;
        private System.Windows.Forms.ToolStripMenuItem miLowPass;
        private System.Windows.Forms.ToolStripMenuItem miLP1_0;
        private System.Windows.Forms.ToolStripMenuItem miLP1_5;
        private System.Windows.Forms.ToolStripMenuItem miLP2_0;
        private System.Windows.Forms.ToolStripMenuItem miLP2_5;
        private System.Windows.Forms.ToolStripMenuItem miLP3_0;
        private System.Windows.Forms.ToolStripMenuItem miLP3_5;
        private System.Windows.Forms.ToolStripMenuItem miLP4_0;
        private System.Windows.Forms.ToolStripMenuItem miSpline;
        private System.Windows.Forms.ToolStripMenuItem miLPNone;
        private System.Windows.Forms.HScrollBar hsbSlidingWindow;
        private System.Windows.Forms.CheckBox cbxSlidingWindow;
        private System.Windows.Forms.SaveFileDialog exportFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.Label lblOrder;
		private System.Windows.Forms.Label lblRMS;
		private System.Windows.Forms.ToolStripMenuItem miCustomize;
		private System.Windows.Forms.ToolStripMenuItem miDisplaySettings;
    }
}
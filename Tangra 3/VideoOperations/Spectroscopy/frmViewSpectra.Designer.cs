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
			this.gbxDispersion = new System.Windows.Forms.GroupBox();
			this.lblDispersion = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.pnlClient = new System.Windows.Forms.Panel();
			this.picSpectraGraph = new System.Windows.Forms.PictureBox();
			this.picSpectra = new System.Windows.Forms.PictureBox();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.miFile = new System.Windows.Forms.ToolStripMenuItem();
			this.miLoadSpectra = new System.Windows.Forms.ToolStripMenuItem();
			this.miSaveSpectra = new System.Windows.Forms.ToolStripMenuItem();
			this.miData = new System.Windows.Forms.ToolStripMenuItem();
			this.miSpectralCalibration = new System.Windows.Forms.ToolStripMenuItem();
			this.miView = new System.Windows.Forms.ToolStripMenuItem();
			this.miShowCommonLines = new System.Windows.Forms.ToolStripMenuItem();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.label3 = new System.Windows.Forms.Label();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.miCloseForm = new System.Windows.Forms.ToolStripMenuItem();
			this.miExport = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.pnlBottom.SuspendLayout();
			this.gbxDispersion.SuspendLayout();
			this.pnlClient.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picSpectraGraph)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picSpectra)).BeginInit();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlBottom
			// 
			this.pnlBottom.Controls.Add(this.label3);
			this.pnlBottom.Controls.Add(this.gbxDispersion);
			this.pnlBottom.Controls.Add(this.label2);
			this.pnlBottom.Controls.Add(this.label1);
			this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlBottom.Location = new System.Drawing.Point(0, 482);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Size = new System.Drawing.Size(807, 74);
			this.pnlBottom.TabIndex = 1;
			// 
			// gbxDispersion
			// 
			this.gbxDispersion.Controls.Add(this.lblDispersion);
			this.gbxDispersion.Location = new System.Drawing.Point(678, 10);
			this.gbxDispersion.Name = "gbxDispersion";
			this.gbxDispersion.Size = new System.Drawing.Size(117, 49);
			this.gbxDispersion.TabIndex = 2;
			this.gbxDispersion.TabStop = false;
			this.gbxDispersion.Text = "Dispersion";
			this.gbxDispersion.Visible = false;
			// 
			// lblDispersion
			// 
			this.lblDispersion.AutoSize = true;
			this.lblDispersion.Location = new System.Drawing.Point(10, 20);
			this.lblDispersion.Name = "lblDispersion";
			this.lblDispersion.Size = new System.Drawing.Size(0, 13);
			this.lblDispersion.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(193, 18);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(35, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "label2";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(22, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "label1";
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
			this.picSpectraGraph.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.picSpectraGraph_PreviewKeyDown);
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
			this.miFile.Size = new System.Drawing.Size(37, 20);
			this.miFile.Text = "&File";
			// 
			// miLoadSpectra
			// 
			this.miLoadSpectra.Name = "miLoadSpectra";
			this.miLoadSpectra.Size = new System.Drawing.Size(152, 22);
			this.miLoadSpectra.Text = "&Load Spectra ";
			// 
			// miSaveSpectra
			// 
			this.miSaveSpectra.Name = "miSaveSpectra";
			this.miSaveSpectra.Size = new System.Drawing.Size(152, 22);
			this.miSaveSpectra.Text = "&Save Spectra";
			this.miSaveSpectra.Click += new System.EventHandler(this.miSaveSpectra_Click);
			// 
			// miData
			// 
			this.miData.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSpectralCalibration});
			this.miData.Name = "miData";
			this.miData.Size = new System.Drawing.Size(43, 20);
			this.miData.Text = "&Data";
			// 
			// miSpectralCalibration
			// 
			this.miSpectralCalibration.Name = "miSpectralCalibration";
			this.miSpectralCalibration.Size = new System.Drawing.Size(177, 22);
			this.miSpectralCalibration.Text = "Spectral &Calibration";
			this.miSpectralCalibration.Click += new System.EventHandler(this.miSpectralCalibration_Click);
			// 
			// miView
			// 
			this.miView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miShowCommonLines});
			this.miView.Name = "miView";
			this.miView.Size = new System.Drawing.Size(44, 20);
			this.miView.Text = "&View";
			// 
			// miShowCommonLines
			// 
			this.miShowCommonLines.CheckOnClick = true;
			this.miShowCommonLines.Name = "miShowCommonLines";
			this.miShowCommonLines.Size = new System.Drawing.Size(187, 22);
			this.miShowCommonLines.Text = "Show &Common Lines";
			this.miShowCommonLines.CheckedChanged += new System.EventHandler(this.miShowCommonLines_CheckedChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(340, 18);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(35, 13);
			this.label3.TabIndex = 3;
			this.label3.Text = "label3";
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
			// 
			// miCloseForm
			// 
			this.miCloseForm.Name = "miCloseForm";
			this.miCloseForm.Size = new System.Drawing.Size(152, 22);
			this.miCloseForm.Text = "&Close";
			// 
			// miExport
			// 
			this.miExport.Name = "miExport";
			this.miExport.Size = new System.Drawing.Size(152, 22);
			this.miExport.Text = "Export";
			this.miExport.Click += new System.EventHandler(this.miExport_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
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
			this.Load += new System.EventHandler(this.frmViewSpectra_Load);
			this.Resize += new System.EventHandler(this.frmViewSpectra_Resize);
			this.pnlBottom.ResumeLayout(false);
			this.pnlBottom.PerformLayout();
			this.gbxDispersion.ResumeLayout(false);
			this.gbxDispersion.PerformLayout();
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gbxDispersion;
        private System.Windows.Forms.Label lblDispersion;
        private System.Windows.Forms.ToolStripMenuItem miShowCommonLines;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem miCloseForm;
		private System.Windows.Forms.ToolStripMenuItem miExport;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}
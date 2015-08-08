namespace Tangra.VideoOperations.Spectroscopy.AbsFluxCalibration
{
	partial class frmAbsFlux
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAbsFlux));
			this.pnlFiles = new System.Windows.Forms.Panel();
			this.btnBrowseFiles = new System.Windows.Forms.Button();
			this.lbAvailableFiles = new System.Windows.Forms.ListBox();
			this.lblAvailableSpectraTitle = new System.Windows.Forms.Label();
			this.lblUsedSpectraTitle = new System.Windows.Forms.Label();
			this.lbIncludedSpecta = new System.Windows.Forms.CheckedListBox();
			this.ctxMenuIncludedSpectra = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miExcludeSpectra = new System.Windows.Forms.ToolStripMenuItem();
			this.pnlDetail = new System.Windows.Forms.Panel();
			this.lblObj9 = new System.Windows.Forms.Label();
			this.lbl9 = new System.Windows.Forms.Label();
			this.pbox9 = new System.Windows.Forms.PictureBox();
			this.lblObj8 = new System.Windows.Forms.Label();
			this.lbl8 = new System.Windows.Forms.Label();
			this.pbox8 = new System.Windows.Forms.PictureBox();
			this.lblObj7 = new System.Windows.Forms.Label();
			this.lbl7 = new System.Windows.Forms.Label();
			this.pbox7 = new System.Windows.Forms.PictureBox();
			this.lblObj6 = new System.Windows.Forms.Label();
			this.lbl6 = new System.Windows.Forms.Label();
			this.pbox6 = new System.Windows.Forms.PictureBox();
			this.lblObj5 = new System.Windows.Forms.Label();
			this.lbl5 = new System.Windows.Forms.Label();
			this.pbox5 = new System.Windows.Forms.PictureBox();
			this.lblObj4 = new System.Windows.Forms.Label();
			this.lbl4 = new System.Windows.Forms.Label();
			this.pbox4 = new System.Windows.Forms.PictureBox();
			this.lblObj3 = new System.Windows.Forms.Label();
			this.lbl3 = new System.Windows.Forms.Label();
			this.pbox3 = new System.Windows.Forms.PictureBox();
			this.lblObj2 = new System.Windows.Forms.Label();
			this.lbl2 = new System.Windows.Forms.Label();
			this.pbox2 = new System.Windows.Forms.PictureBox();
			this.lblObj1 = new System.Windows.Forms.Label();
			this.lbl1 = new System.Windows.Forms.Label();
			this.pbox1 = new System.Windows.Forms.PictureBox();
			this.pnlClient = new System.Windows.Forms.Panel();
			this.picPlot = new System.Windows.Forms.PictureBox();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.miMaintenance = new System.Windows.Forms.ToolStripMenuItem();
			this.miBuildCalSpecDB = new System.Windows.Forms.ToolStripMenuItem();
			this.miTestCalSpecDB = new System.Windows.Forms.ToolStripMenuItem();
			this.miExportAbsFluxFiles = new System.Windows.Forms.ToolStripMenuItem();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.resizeTimer = new System.Windows.Forms.Timer(this.components);
			this.miView = new System.Windows.Forms.ToolStripMenuItem();
			this.miSeries = new System.Windows.Forms.ToolStripMenuItem();
			this.miAbsoluteFlux = new System.Windows.Forms.ToolStripMenuItem();
			this.miObservedFlux = new System.Windows.Forms.ToolStripMenuItem();
			this.pnlFiles.SuspendLayout();
			this.ctxMenuIncludedSpectra.SuspendLayout();
			this.pnlDetail.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pbox9)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbox8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbox7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbox6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbox5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbox4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbox3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbox1)).BeginInit();
			this.pnlClient.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picPlot)).BeginInit();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlFiles
			// 
			this.pnlFiles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlFiles.Controls.Add(this.btnBrowseFiles);
			this.pnlFiles.Controls.Add(this.lbAvailableFiles);
			this.pnlFiles.Controls.Add(this.lblAvailableSpectraTitle);
			this.pnlFiles.Controls.Add(this.lblUsedSpectraTitle);
			this.pnlFiles.Controls.Add(this.lbIncludedSpecta);
			this.pnlFiles.Dock = System.Windows.Forms.DockStyle.Right;
			this.pnlFiles.Location = new System.Drawing.Point(598, 0);
			this.pnlFiles.Name = "pnlFiles";
			this.pnlFiles.Size = new System.Drawing.Size(207, 510);
			this.pnlFiles.TabIndex = 0;
			// 
			// btnBrowseFiles
			// 
			this.btnBrowseFiles.Location = new System.Drawing.Point(13, 460);
			this.btnBrowseFiles.Name = "btnBrowseFiles";
			this.btnBrowseFiles.Size = new System.Drawing.Size(184, 27);
			this.btnBrowseFiles.TabIndex = 4;
			this.btnBrowseFiles.Text = "Change Spectra Files Location";
			this.btnBrowseFiles.UseVisualStyleBackColor = true;
			this.btnBrowseFiles.Click += new System.EventHandler(this.btnBrowseFiles_Click);
			// 
			// lbAvailableFiles
			// 
			this.lbAvailableFiles.FormattingEnabled = true;
			this.lbAvailableFiles.Location = new System.Drawing.Point(13, 224);
			this.lbAvailableFiles.Name = "lbAvailableFiles";
			this.lbAvailableFiles.Size = new System.Drawing.Size(184, 225);
			this.lbAvailableFiles.TabIndex = 3;
			this.lbAvailableFiles.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbAvailableFiles_MouseDoubleClick);
			// 
			// lblAvailableSpectraTitle
			// 
			this.lblAvailableSpectraTitle.AutoSize = true;
			this.lblAvailableSpectraTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblAvailableSpectraTitle.Location = new System.Drawing.Point(10, 208);
			this.lblAvailableSpectraTitle.Name = "lblAvailableSpectraTitle";
			this.lblAvailableSpectraTitle.Size = new System.Drawing.Size(186, 13);
			this.lblAvailableSpectraTitle.TabIndex = 2;
			this.lblAvailableSpectraTitle.Text = "Available Files (doubleclick to include)";
			// 
			// lblUsedSpectraTitle
			// 
			this.lblUsedSpectraTitle.AutoSize = true;
			this.lblUsedSpectraTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblUsedSpectraTitle.Location = new System.Drawing.Point(10, 10);
			this.lblUsedSpectraTitle.Name = "lblUsedSpectraTitle";
			this.lblUsedSpectraTitle.Size = new System.Drawing.Size(188, 13);
			this.lblUsedSpectraTitle.TabIndex = 1;
			this.lblUsedSpectraTitle.Text = "Included Spectra (dbl-click to exclude)";
			// 
			// lbIncludedSpecta
			// 
			this.lbIncludedSpecta.ContextMenuStrip = this.ctxMenuIncludedSpectra;
			this.lbIncludedSpecta.FormattingEnabled = true;
			this.lbIncludedSpecta.Location = new System.Drawing.Point(10, 31);
			this.lbIncludedSpecta.Name = "lbIncludedSpecta";
			this.lbIncludedSpecta.Size = new System.Drawing.Size(187, 154);
			this.lbIncludedSpecta.TabIndex = 0;
			this.lbIncludedSpecta.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lbIncludedSpecta_ItemCheck);
			// 
			// ctxMenuIncludedSpectra
			// 
			this.ctxMenuIncludedSpectra.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miExcludeSpectra});
			this.ctxMenuIncludedSpectra.Name = "ctxMenuIncludedSpectra";
			this.ctxMenuIncludedSpectra.Size = new System.Drawing.Size(152, 26);
			this.ctxMenuIncludedSpectra.Opening += new System.ComponentModel.CancelEventHandler(this.ctxMenuIncludedSpectra_Opening);
			// 
			// miExcludeSpectra
			// 
			this.miExcludeSpectra.Name = "miExcludeSpectra";
			this.miExcludeSpectra.Size = new System.Drawing.Size(151, 22);
			this.miExcludeSpectra.Text = "&Exclude Spectra";
			this.miExcludeSpectra.Click += new System.EventHandler(this.miExcludeSpectra_Click);
			// 
			// pnlDetail
			// 
			this.pnlDetail.Controls.Add(this.lblObj9);
			this.pnlDetail.Controls.Add(this.lbl9);
			this.pnlDetail.Controls.Add(this.pbox9);
			this.pnlDetail.Controls.Add(this.lblObj8);
			this.pnlDetail.Controls.Add(this.lbl8);
			this.pnlDetail.Controls.Add(this.pbox8);
			this.pnlDetail.Controls.Add(this.lblObj7);
			this.pnlDetail.Controls.Add(this.lbl7);
			this.pnlDetail.Controls.Add(this.pbox7);
			this.pnlDetail.Controls.Add(this.lblObj6);
			this.pnlDetail.Controls.Add(this.lbl6);
			this.pnlDetail.Controls.Add(this.pbox6);
			this.pnlDetail.Controls.Add(this.lblObj5);
			this.pnlDetail.Controls.Add(this.lbl5);
			this.pnlDetail.Controls.Add(this.pbox5);
			this.pnlDetail.Controls.Add(this.lblObj4);
			this.pnlDetail.Controls.Add(this.lbl4);
			this.pnlDetail.Controls.Add(this.pbox4);
			this.pnlDetail.Controls.Add(this.lblObj3);
			this.pnlDetail.Controls.Add(this.lbl3);
			this.pnlDetail.Controls.Add(this.pbox3);
			this.pnlDetail.Controls.Add(this.lblObj2);
			this.pnlDetail.Controls.Add(this.lbl2);
			this.pnlDetail.Controls.Add(this.pbox2);
			this.pnlDetail.Controls.Add(this.lblObj1);
			this.pnlDetail.Controls.Add(this.lbl1);
			this.pnlDetail.Controls.Add(this.pbox1);
			this.pnlDetail.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlDetail.Location = new System.Drawing.Point(0, 439);
			this.pnlDetail.Name = "pnlDetail";
			this.pnlDetail.Size = new System.Drawing.Size(598, 71);
			this.pnlDetail.TabIndex = 1;
			// 
			// lblObj9
			// 
			this.lblObj9.AutoSize = true;
			this.lblObj9.Location = new System.Drawing.Point(221, 50);
			this.lblObj9.Name = "lblObj9";
			this.lblObj9.Size = new System.Drawing.Size(36, 13);
			this.lblObj9.TabIndex = 26;
			this.lblObj9.Tag = "109";
			this.lblObj9.Text = "-6.5 %";
			this.lblObj9.Visible = false;
			// 
			// lbl9
			// 
			this.lbl9.Location = new System.Drawing.Point(182, 50);
			this.lbl9.Name = "lbl9";
			this.lbl9.Size = new System.Drawing.Size(22, 13);
			this.lbl9.TabIndex = 25;
			this.lbl9.Tag = "109";
			this.lbl9.Text = "9.";
			this.lbl9.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.lbl9.Visible = false;
			// 
			// pbox9
			// 
			this.pbox9.BackColor = System.Drawing.Color.Red;
			this.pbox9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pbox9.Location = new System.Drawing.Point(206, 50);
			this.pbox9.Name = "pbox9";
			this.pbox9.Size = new System.Drawing.Size(14, 13);
			this.pbox9.TabIndex = 24;
			this.pbox9.TabStop = false;
			this.pbox9.Tag = "109";
			this.pbox9.Visible = false;
			// 
			// lblObj8
			// 
			this.lblObj8.AutoSize = true;
			this.lblObj8.Location = new System.Drawing.Point(221, 31);
			this.lblObj8.Name = "lblObj8";
			this.lblObj8.Size = new System.Drawing.Size(36, 13);
			this.lblObj8.TabIndex = 23;
			this.lblObj8.Tag = "108";
			this.lblObj8.Text = "-6.5 %";
			this.lblObj8.Visible = false;
			// 
			// lbl8
			// 
			this.lbl8.Location = new System.Drawing.Point(182, 31);
			this.lbl8.Name = "lbl8";
			this.lbl8.Size = new System.Drawing.Size(22, 13);
			this.lbl8.TabIndex = 22;
			this.lbl8.Tag = "108";
			this.lbl8.Text = "8.";
			this.lbl8.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.lbl8.Visible = false;
			// 
			// pbox8
			// 
			this.pbox8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.pbox8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pbox8.Location = new System.Drawing.Point(206, 31);
			this.pbox8.Name = "pbox8";
			this.pbox8.Size = new System.Drawing.Size(14, 13);
			this.pbox8.TabIndex = 21;
			this.pbox8.TabStop = false;
			this.pbox8.Tag = "108";
			this.pbox8.Visible = false;
			// 
			// lblObj7
			// 
			this.lblObj7.AutoSize = true;
			this.lblObj7.Location = new System.Drawing.Point(221, 12);
			this.lblObj7.Name = "lblObj7";
			this.lblObj7.Size = new System.Drawing.Size(36, 13);
			this.lblObj7.TabIndex = 20;
			this.lblObj7.Tag = "107";
			this.lblObj7.Text = "-6.5 %";
			this.lblObj7.Visible = false;
			// 
			// lbl7
			// 
			this.lbl7.Location = new System.Drawing.Point(182, 12);
			this.lbl7.Name = "lbl7";
			this.lbl7.Size = new System.Drawing.Size(22, 13);
			this.lbl7.TabIndex = 19;
			this.lbl7.Tag = "107";
			this.lbl7.Text = "7.";
			this.lbl7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.lbl7.Visible = false;
			// 
			// pbox7
			// 
			this.pbox7.BackColor = System.Drawing.Color.Green;
			this.pbox7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pbox7.Location = new System.Drawing.Point(206, 12);
			this.pbox7.Name = "pbox7";
			this.pbox7.Size = new System.Drawing.Size(14, 13);
			this.pbox7.TabIndex = 18;
			this.pbox7.TabStop = false;
			this.pbox7.Tag = "107";
			this.pbox7.Visible = false;
			// 
			// lblObj6
			// 
			this.lblObj6.AutoSize = true;
			this.lblObj6.Location = new System.Drawing.Point(131, 50);
			this.lblObj6.Name = "lblObj6";
			this.lblObj6.Size = new System.Drawing.Size(36, 13);
			this.lblObj6.TabIndex = 17;
			this.lblObj6.Tag = "106";
			this.lblObj6.Text = "-6.5 %";
			this.lblObj6.Visible = false;
			// 
			// lbl6
			// 
			this.lbl6.Location = new System.Drawing.Point(92, 50);
			this.lbl6.Name = "lbl6";
			this.lbl6.Size = new System.Drawing.Size(22, 13);
			this.lbl6.TabIndex = 16;
			this.lbl6.Tag = "106";
			this.lbl6.Text = "6.";
			this.lbl6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.lbl6.Visible = false;
			// 
			// pbox6
			// 
			this.pbox6.BackColor = System.Drawing.Color.Blue;
			this.pbox6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pbox6.Location = new System.Drawing.Point(116, 50);
			this.pbox6.Name = "pbox6";
			this.pbox6.Size = new System.Drawing.Size(14, 13);
			this.pbox6.TabIndex = 15;
			this.pbox6.TabStop = false;
			this.pbox6.Tag = "106";
			this.pbox6.Visible = false;
			// 
			// lblObj5
			// 
			this.lblObj5.AutoSize = true;
			this.lblObj5.Location = new System.Drawing.Point(131, 31);
			this.lblObj5.Name = "lblObj5";
			this.lblObj5.Size = new System.Drawing.Size(36, 13);
			this.lblObj5.TabIndex = 14;
			this.lblObj5.Tag = "105";
			this.lblObj5.Text = "-6.5 %";
			this.lblObj5.Visible = false;
			// 
			// lbl5
			// 
			this.lbl5.Location = new System.Drawing.Point(92, 31);
			this.lbl5.Name = "lbl5";
			this.lbl5.Size = new System.Drawing.Size(22, 13);
			this.lbl5.TabIndex = 13;
			this.lbl5.Tag = "105";
			this.lbl5.Text = "5.";
			this.lbl5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.lbl5.Visible = false;
			// 
			// pbox5
			// 
			this.pbox5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
			this.pbox5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pbox5.Location = new System.Drawing.Point(116, 31);
			this.pbox5.Name = "pbox5";
			this.pbox5.Size = new System.Drawing.Size(14, 13);
			this.pbox5.TabIndex = 12;
			this.pbox5.TabStop = false;
			this.pbox5.Tag = "105";
			this.pbox5.Visible = false;
			// 
			// lblObj4
			// 
			this.lblObj4.AutoSize = true;
			this.lblObj4.Location = new System.Drawing.Point(131, 12);
			this.lblObj4.Name = "lblObj4";
			this.lblObj4.Size = new System.Drawing.Size(36, 13);
			this.lblObj4.TabIndex = 11;
			this.lblObj4.Tag = "104";
			this.lblObj4.Text = "-6.5 %";
			this.lblObj4.Visible = false;
			// 
			// lbl4
			// 
			this.lbl4.Location = new System.Drawing.Point(92, 12);
			this.lbl4.Name = "lbl4";
			this.lbl4.Size = new System.Drawing.Size(22, 13);
			this.lbl4.TabIndex = 10;
			this.lbl4.Tag = "104";
			this.lbl4.Text = "4.";
			this.lbl4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.lbl4.Visible = false;
			// 
			// pbox4
			// 
			this.pbox4.BackColor = System.Drawing.Color.Fuchsia;
			this.pbox4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pbox4.Location = new System.Drawing.Point(116, 12);
			this.pbox4.Name = "pbox4";
			this.pbox4.Size = new System.Drawing.Size(14, 13);
			this.pbox4.TabIndex = 9;
			this.pbox4.TabStop = false;
			this.pbox4.Tag = "104";
			this.pbox4.Visible = false;
			// 
			// lblObj3
			// 
			this.lblObj3.AutoSize = true;
			this.lblObj3.Location = new System.Drawing.Point(43, 50);
			this.lblObj3.Name = "lblObj3";
			this.lblObj3.Size = new System.Drawing.Size(36, 13);
			this.lblObj3.TabIndex = 8;
			this.lblObj3.Tag = "103";
			this.lblObj3.Text = "-6.5 %";
			this.lblObj3.Visible = false;
			// 
			// lbl3
			// 
			this.lbl3.Location = new System.Drawing.Point(4, 50);
			this.lbl3.Name = "lbl3";
			this.lbl3.Size = new System.Drawing.Size(22, 13);
			this.lbl3.TabIndex = 7;
			this.lbl3.Tag = "103";
			this.lbl3.Text = "3.";
			this.lbl3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.lbl3.Visible = false;
			// 
			// pbox3
			// 
			this.pbox3.BackColor = System.Drawing.Color.Yellow;
			this.pbox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pbox3.Location = new System.Drawing.Point(28, 50);
			this.pbox3.Name = "pbox3";
			this.pbox3.Size = new System.Drawing.Size(14, 13);
			this.pbox3.TabIndex = 6;
			this.pbox3.TabStop = false;
			this.pbox3.Tag = "103";
			this.pbox3.Visible = false;
			// 
			// lblObj2
			// 
			this.lblObj2.AutoSize = true;
			this.lblObj2.Location = new System.Drawing.Point(43, 31);
			this.lblObj2.Name = "lblObj2";
			this.lblObj2.Size = new System.Drawing.Size(36, 13);
			this.lblObj2.TabIndex = 5;
			this.lblObj2.Tag = "102";
			this.lblObj2.Text = "-6.5 %";
			this.lblObj2.Visible = false;
			// 
			// lbl2
			// 
			this.lbl2.Location = new System.Drawing.Point(4, 31);
			this.lbl2.Name = "lbl2";
			this.lbl2.Size = new System.Drawing.Size(22, 13);
			this.lbl2.TabIndex = 4;
			this.lbl2.Tag = "102";
			this.lbl2.Text = "2.";
			this.lbl2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.lbl2.Visible = false;
			// 
			// pbox2
			// 
			this.pbox2.BackColor = System.Drawing.Color.Aqua;
			this.pbox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pbox2.Location = new System.Drawing.Point(28, 31);
			this.pbox2.Name = "pbox2";
			this.pbox2.Size = new System.Drawing.Size(14, 13);
			this.pbox2.TabIndex = 3;
			this.pbox2.TabStop = false;
			this.pbox2.Tag = "102";
			this.pbox2.Visible = false;
			// 
			// lblObj1
			// 
			this.lblObj1.AutoSize = true;
			this.lblObj1.Location = new System.Drawing.Point(43, 12);
			this.lblObj1.Name = "lblObj1";
			this.lblObj1.Size = new System.Drawing.Size(36, 13);
			this.lblObj1.TabIndex = 2;
			this.lblObj1.Tag = "101";
			this.lblObj1.Text = "-6.5 %";
			this.lblObj1.Visible = false;
			// 
			// lbl1
			// 
			this.lbl1.Location = new System.Drawing.Point(4, 12);
			this.lbl1.Name = "lbl1";
			this.lbl1.Size = new System.Drawing.Size(22, 13);
			this.lbl1.TabIndex = 1;
			this.lbl1.Tag = "101";
			this.lbl1.Text = "1.";
			this.lbl1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.lbl1.Visible = false;
			this.lbl1.Click += new System.EventHandler(this.label1_Click);
			// 
			// pbox1
			// 
			this.pbox1.BackColor = System.Drawing.Color.Lime;
			this.pbox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pbox1.Location = new System.Drawing.Point(28, 12);
			this.pbox1.Name = "pbox1";
			this.pbox1.Size = new System.Drawing.Size(14, 13);
			this.pbox1.TabIndex = 0;
			this.pbox1.TabStop = false;
			this.pbox1.Tag = "101";
			this.pbox1.Visible = false;
			// 
			// pnlClient
			// 
			this.pnlClient.Controls.Add(this.picPlot);
			this.pnlClient.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlClient.Location = new System.Drawing.Point(0, 0);
			this.pnlClient.Name = "pnlClient";
			this.pnlClient.Size = new System.Drawing.Size(598, 439);
			this.pnlClient.TabIndex = 2;
			// 
			// picPlot
			// 
			this.picPlot.BackColor = System.Drawing.SystemColors.ControlDark;
			this.picPlot.Dock = System.Windows.Forms.DockStyle.Fill;
			this.picPlot.Location = new System.Drawing.Point(0, 0);
			this.picPlot.Name = "picPlot";
			this.picPlot.Size = new System.Drawing.Size(598, 439);
			this.picPlot.TabIndex = 0;
			this.picPlot.TabStop = false;
			this.picPlot.Resize += new System.EventHandler(this.picPlot_Resize);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miView,
            this.miMaintenance});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(598, 24);
			this.menuStrip1.TabIndex = 3;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// miMaintenance
			// 
			this.miMaintenance.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miBuildCalSpecDB,
            this.miTestCalSpecDB,
            this.miExportAbsFluxFiles});
			this.miMaintenance.Name = "miMaintenance";
			this.miMaintenance.Size = new System.Drawing.Size(80, 20);
			this.miMaintenance.Text = "&Maintenance";
			this.miMaintenance.Visible = false;
			// 
			// miBuildCalSpecDB
			// 
			this.miBuildCalSpecDB.Name = "miBuildCalSpecDB";
			this.miBuildCalSpecDB.Size = new System.Drawing.Size(180, 22);
			this.miBuildCalSpecDB.Text = "Build CalSpec DB";
			this.miBuildCalSpecDB.Click += new System.EventHandler(this.miBuildCalSpecDB_Click);
			// 
			// miTestCalSpecDB
			// 
			this.miTestCalSpecDB.Name = "miTestCalSpecDB";
			this.miTestCalSpecDB.Size = new System.Drawing.Size(180, 22);
			this.miTestCalSpecDB.Text = "Test CalSpec DB";
			this.miTestCalSpecDB.Click += new System.EventHandler(this.miTestCalSpecDB_Click);
			// 
			// miExportAbsFluxFiles
			// 
			this.miExportAbsFluxFiles.Name = "miExportAbsFluxFiles";
			this.miExportAbsFluxFiles.Size = new System.Drawing.Size(180, 22);
			this.miExportAbsFluxFiles.Text = "Export AbsFlux Fomat";
			this.miExportAbsFluxFiles.Click += new System.EventHandler(this.miExportAbsFluxFiles_Click);
			// 
			// openFileDialog
			// 
			this.openFileDialog.DefaultExt = "spectra";
			this.openFileDialog.Filter = "Tangra Exports (*.dat)|*.dat|All Files (*.*)|*.*";
			this.openFileDialog.Title = "Select Spectra File to Add All Files from Same Location";
			// 
			// resizeTimer
			// 
			this.resizeTimer.Interval = 50;
			this.resizeTimer.Tick += new System.EventHandler(this.resizeTimer_Tick);
			// 
			// miView
			// 
			this.miView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSeries});
			this.miView.Name = "miView";
			this.miView.Size = new System.Drawing.Size(37, 20);
			this.miView.Text = "&Plot";
			// 
			// miSeries
			// 
			this.miSeries.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miAbsoluteFlux,
            this.miObservedFlux});
			this.miSeries.Name = "miSeries";
			this.miSeries.Size = new System.Drawing.Size(152, 22);
			this.miSeries.Text = "&Series";
			// 
			// miAbsoluteFlux
			// 
			this.miAbsoluteFlux.Checked = true;
			this.miAbsoluteFlux.CheckState = System.Windows.Forms.CheckState.Checked;
			this.miAbsoluteFlux.Name = "miAbsoluteFlux";
			this.miAbsoluteFlux.Size = new System.Drawing.Size(152, 22);
			this.miAbsoluteFlux.Text = "&Absolute Flux";
			this.miAbsoluteFlux.Click += new System.EventHandler(this.miAbsoluteFlux_Click);
			// 
			// miObservedFlux
			// 
			this.miObservedFlux.Name = "miObservedFlux";
			this.miObservedFlux.Size = new System.Drawing.Size(152, 22);
			this.miObservedFlux.Text = "&Observed Fulx";
			this.miObservedFlux.Click += new System.EventHandler(this.miObservedFlux_Click);
			// 
			// frmAbsFlux
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(805, 510);
			this.Controls.Add(this.menuStrip1);
			this.Controls.Add(this.pnlClient);
			this.Controls.Add(this.pnlDetail);
			this.Controls.Add(this.pnlFiles);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmAbsFlux";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Spectroscopy - Absolute Flux Calibration (Based on HST/STIS CALSPEC Spectra)";
			this.Load += new System.EventHandler(this.frmAbsFlux_Load);
			this.pnlFiles.ResumeLayout(false);
			this.pnlFiles.PerformLayout();
			this.ctxMenuIncludedSpectra.ResumeLayout(false);
			this.pnlDetail.ResumeLayout(false);
			this.pnlDetail.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pbox9)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbox8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbox7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbox6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbox5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbox4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbox3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbox2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbox1)).EndInit();
			this.pnlClient.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.picPlot)).EndInit();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel pnlFiles;
		private System.Windows.Forms.Label lblUsedSpectraTitle;
		private System.Windows.Forms.CheckedListBox lbIncludedSpecta;
		private System.Windows.Forms.Panel pnlDetail;
		private System.Windows.Forms.Panel pnlClient;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.Button btnBrowseFiles;
		private System.Windows.Forms.ListBox lbAvailableFiles;
		private System.Windows.Forms.Label lblAvailableSpectraTitle;
		private System.Windows.Forms.PictureBox picPlot;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.ContextMenuStrip ctxMenuIncludedSpectra;
		private System.Windows.Forms.ToolStripMenuItem miExcludeSpectra;
        private System.Windows.Forms.ToolStripMenuItem miMaintenance;
        private System.Windows.Forms.ToolStripMenuItem miBuildCalSpecDB;
        private System.Windows.Forms.ToolStripMenuItem miTestCalSpecDB;
        private System.Windows.Forms.ToolStripMenuItem miExportAbsFluxFiles;
		private System.Windows.Forms.Timer resizeTimer;
		private System.Windows.Forms.Label lblObj1;
		private System.Windows.Forms.Label lbl1;
		private System.Windows.Forms.PictureBox pbox1;
		private System.Windows.Forms.Label lblObj9;
		private System.Windows.Forms.Label lbl9;
		private System.Windows.Forms.PictureBox pbox9;
		private System.Windows.Forms.Label lblObj8;
		private System.Windows.Forms.Label lbl8;
		private System.Windows.Forms.PictureBox pbox8;
		private System.Windows.Forms.Label lblObj7;
		private System.Windows.Forms.Label lbl7;
		private System.Windows.Forms.PictureBox pbox7;
		private System.Windows.Forms.Label lblObj6;
		private System.Windows.Forms.Label lbl6;
		private System.Windows.Forms.PictureBox pbox6;
		private System.Windows.Forms.Label lblObj5;
		private System.Windows.Forms.Label lbl5;
		private System.Windows.Forms.PictureBox pbox5;
		private System.Windows.Forms.Label lblObj4;
		private System.Windows.Forms.Label lbl4;
		private System.Windows.Forms.PictureBox pbox4;
		private System.Windows.Forms.Label lblObj3;
		private System.Windows.Forms.Label lbl3;
		private System.Windows.Forms.PictureBox pbox3;
		private System.Windows.Forms.Label lblObj2;
		private System.Windows.Forms.Label lbl2;
		private System.Windows.Forms.PictureBox pbox2;
		private System.Windows.Forms.ToolStripMenuItem miView;
		private System.Windows.Forms.ToolStripMenuItem miSeries;
		private System.Windows.Forms.ToolStripMenuItem miAbsoluteFlux;
		private System.Windows.Forms.ToolStripMenuItem miObservedFlux;
	}
}
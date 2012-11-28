﻿namespace Tangra
{
	partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.miFile = new System.Windows.Forms.ToolStripMenuItem();
            this.miOpenVideo = new System.Windows.Forms.ToolStripMenuItem();
            this.miRecentVideos = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.miOpenLightCurve = new System.Windows.Forms.ToolStripMenuItem();
            this.miRecentLightCurves = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.miExit = new System.Windows.Forms.ToolStripMenuItem();
            this.miFrameActions = new System.Windows.Forms.ToolStripMenuItem();
            this.miExportToFits = new System.Windows.Forms.ToolStripMenuItem();
            this.miExportToBMP = new System.Windows.Forms.ToolStripMenuItem();
            this.miTasks = new System.Windows.Forms.ToolStripMenuItem();
            this.miReduceLightCurve = new System.Windows.Forms.ToolStripMenuItem();
            this.miTools = new System.Windows.Forms.ToolStripMenuItem();
            this.miSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.miHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.miOnlineHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.miCheckForUpdates = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.miAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.tsProgessBar = new System.Windows.Forms.ToolStripProgressBar();
            this.ssStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.ssToolInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.ssGamma = new System.Windows.Forms.ToolStripStatusLabel();
            this.ssPixelFormat = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslblIntegration = new System.Windows.Forms.ToolStripStatusLabel();
            this.ssFrameNo = new System.Windows.Forms.ToolStripStatusLabel();
            this.ssFPS = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslblRecDbg = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslblUsingAviSynth = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslblDarkFrameLoaded = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslblFlatFrameLoaded = new System.Windows.Forms.ToolStripStatusLabel();
            this.ssMoreInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.pnlNewVersionAvailable = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelRight = new System.Windows.Forms.Panel();
            this.pnlControlerPanel = new System.Windows.Forms.Panel();
            this.panelVideo = new System.Windows.Forms.Panel();
            this.pnlPlayControls = new System.Windows.Forms.Panel();
            this.pnlPlayButtons = new System.Windows.Forms.Panel();
            this.btnJumpTo = new System.Windows.Forms.Button();
            this.btn1SecMinus = new System.Windows.Forms.Button();
            this.btn10SecMinus = new System.Windows.Forms.Button();
            this.btn10SecPlus = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btn1FrPlus = new System.Windows.Forms.Button();
            this.btn1SecPlus = new System.Windows.Forms.Button();
            this.btn1FrMinus = new System.Windows.Forms.Button();
            this.scrollBarFrames = new System.Windows.Forms.HScrollBar();
            this.openVideoFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.btnPlay = new System.Windows.Forms.Button();
            this.zoomedImage = new System.Windows.Forms.PictureBox();
            this.mainMenu.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.panelVideo.SuspendLayout();
            this.pnlPlayControls.SuspendLayout();
            this.pnlPlayButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomedImage)).BeginInit();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFile,
            this.miFrameActions,
            this.miTasks,
            this.miTools,
            this.miSettings,
            this.miHelp});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(879, 24);
            this.mainMenu.TabIndex = 0;
            // 
            // miFile
            // 
            this.miFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miOpenVideo,
            this.miRecentVideos,
            this.toolStripSeparator1,
            this.miOpenLightCurve,
            this.miRecentLightCurves,
            this.toolStripSeparator2,
            this.miExit});
            this.miFile.Name = "miFile";
            this.miFile.Size = new System.Drawing.Size(35, 20);
            this.miFile.Text = "&File";
            // 
            // miOpenVideo
            // 
            this.miOpenVideo.Name = "miOpenVideo";
            this.miOpenVideo.Size = new System.Drawing.Size(182, 22);
            this.miOpenVideo.Text = "&Open Video";
            this.miOpenVideo.Click += new System.EventHandler(this.miOpenVideo_Click);
            // 
            // miRecentVideos
            // 
            this.miRecentVideos.Name = "miRecentVideos";
            this.miRecentVideos.Size = new System.Drawing.Size(182, 22);
            this.miRecentVideos.Text = "&Recent Videos";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(179, 6);
            // 
            // miOpenLightCurve
            // 
            this.miOpenLightCurve.Name = "miOpenLightCurve";
            this.miOpenLightCurve.Size = new System.Drawing.Size(182, 22);
            this.miOpenLightCurve.Text = "Open &Light Curve";
            // 
            // miRecentLightCurves
            // 
            this.miRecentLightCurves.Name = "miRecentLightCurves";
            this.miRecentLightCurves.Size = new System.Drawing.Size(182, 22);
            this.miRecentLightCurves.Text = "Recent Light Curves";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(179, 6);
            // 
            // miExit
            // 
            this.miExit.Name = "miExit";
            this.miExit.Size = new System.Drawing.Size(182, 22);
            this.miExit.Text = "&Exit";
            // 
            // miFrameActions
            // 
            this.miFrameActions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miExportToFits,
            this.miExportToBMP});
            this.miFrameActions.Name = "miFrameActions";
            this.miFrameActions.Size = new System.Drawing.Size(87, 20);
            this.miFrameActions.Text = "Frame Actions";
            // 
            // miExportToFits
            // 
            this.miExportToFits.Name = "miExportToFits";
            this.miExportToFits.Size = new System.Drawing.Size(155, 22);
            this.miExportToFits.Text = "Export to &FITS";
            // 
            // miExportToBMP
            // 
            this.miExportToBMP.Name = "miExportToBMP";
            this.miExportToBMP.Size = new System.Drawing.Size(155, 22);
            this.miExportToBMP.Text = "Export to &BMP";
            // 
            // miTasks
            // 
            this.miTasks.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miReduceLightCurve});
            this.miTasks.Name = "miTasks";
            this.miTasks.Size = new System.Drawing.Size(46, 20);
            this.miTasks.Text = "&Tasks";
            // 
            // miReduceLightCurve
            // 
            this.miReduceLightCurve.Name = "miReduceLightCurve";
            this.miReduceLightCurve.Size = new System.Drawing.Size(179, 22);
            this.miReduceLightCurve.Text = "Reduce &Light Curve";
            // 
            // miTools
            // 
            this.miTools.Name = "miTools";
            this.miTools.Size = new System.Drawing.Size(44, 20);
            this.miTools.Text = "&Tools";
            // 
            // miSettings
            // 
            this.miSettings.Name = "miSettings";
            this.miSettings.Size = new System.Drawing.Size(58, 20);
            this.miSettings.Text = "&Settings";
            // 
            // miHelp
            // 
            this.miHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miOnlineHelp,
            this.miCheckForUpdates,
            this.toolStripMenuItem1,
            this.miAbout});
            this.miHelp.Name = "miHelp";
            this.miHelp.Size = new System.Drawing.Size(40, 20);
            this.miHelp.Text = "&Help";
            // 
            // miOnlineHelp
            // 
            this.miOnlineHelp.Name = "miOnlineHelp";
            this.miOnlineHelp.Size = new System.Drawing.Size(174, 22);
            this.miOnlineHelp.Text = "&Online Help";
            // 
            // miCheckForUpdates
            // 
            this.miCheckForUpdates.Name = "miCheckForUpdates";
            this.miCheckForUpdates.Size = new System.Drawing.Size(174, 22);
            this.miCheckForUpdates.Text = "Check for &Updates";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(171, 6);
            // 
            // miAbout
            // 
            this.miAbout.Name = "miAbout";
            this.miAbout.Size = new System.Drawing.Size(174, 22);
            this.miAbout.Text = "&About";
            this.miAbout.Click += new System.EventHandler(this.miAbout_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsProgessBar,
            this.ssStatus,
            this.ssToolInfo,
            this.ssGamma,
            this.ssPixelFormat,
            this.tslblIntegration,
            this.ssFrameNo,
            this.ssFPS,
            this.tslblRecDbg,
            this.tslblUsingAviSynth,
            this.tslblDarkFrameLoaded,
            this.tslblFlatFrameLoaded,
            this.ssMoreInfo,
            this.pnlNewVersionAvailable});
            this.statusStrip.Location = new System.Drawing.Point(0, 595);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(879, 24);
            this.statusStrip.TabIndex = 1;
            // 
            // tsProgessBar
            // 
            this.tsProgessBar.Name = "tsProgessBar";
            this.tsProgessBar.Size = new System.Drawing.Size(100, 18);
            this.tsProgessBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.tsProgessBar.Visible = false;
            // 
            // ssStatus
            // 
            this.ssStatus.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.ssStatus.Name = "ssStatus";
            this.ssStatus.Size = new System.Drawing.Size(42, 19);
            this.ssStatus.Text = "Ready";
            // 
            // ssToolInfo
            // 
            this.ssToolInfo.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.ssToolInfo.Name = "ssToolInfo";
            this.ssToolInfo.Size = new System.Drawing.Size(4, 19);
            this.ssToolInfo.Visible = false;
            // 
            // ssGamma
            // 
            this.ssGamma.AutoSize = false;
            this.ssGamma.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(193)))), ((int)(((byte)(188)))));
            this.ssGamma.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.ssGamma.Name = "ssGamma";
            this.ssGamma.Size = new System.Drawing.Size(89, 19);
            this.ssGamma.Text = "Gamma = 1.00";
            // 
            // ssPixelFormat
            // 
            this.ssPixelFormat.Name = "ssPixelFormat";
            this.ssPixelFormat.Size = new System.Drawing.Size(0, 19);
            // 
            // tslblIntegration
            // 
            this.tslblIntegration.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.tslblIntegration.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tslblIntegration.Name = "tslblIntegration";
            this.tslblIntegration.Size = new System.Drawing.Size(116, 19);
            this.tslblIntegration.Text = "Integrating 16 frames";
            this.tslblIntegration.Visible = false;
            // 
            // ssFrameNo
            // 
            this.ssFrameNo.AutoSize = false;
            this.ssFrameNo.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.ssFrameNo.Name = "ssFrameNo";
            this.ssFrameNo.Size = new System.Drawing.Size(79, 19);
            // 
            // ssFPS
            // 
            this.ssFPS.AutoSize = false;
            this.ssFPS.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.ssFPS.Name = "ssFPS";
            this.ssFPS.Size = new System.Drawing.Size(60, 19);
            this.ssFPS.Visible = false;
            // 
            // tslblRecDbg
            // 
            this.tslblRecDbg.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.tslblRecDbg.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tslblRecDbg.ForeColor = System.Drawing.Color.Red;
            this.tslblRecDbg.Name = "tslblRecDbg";
            this.tslblRecDbg.Size = new System.Drawing.Size(51, 19);
            this.tslblRecDbg.Text = "Rec Dbg";
            this.tslblRecDbg.Visible = false;
            // 
            // tslblUsingAviSynth
            // 
            this.tslblUsingAviSynth.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.tslblUsingAviSynth.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tslblUsingAviSynth.Name = "tslblUsingAviSynth";
            this.tslblUsingAviSynth.Size = new System.Drawing.Size(54, 19);
            this.tslblUsingAviSynth.Text = "AviSynth";
            this.tslblUsingAviSynth.Visible = false;
            // 
            // tslblDarkFrameLoaded
            // 
            this.tslblDarkFrameLoaded.BackColor = System.Drawing.Color.MediumAquamarine;
            this.tslblDarkFrameLoaded.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tslblDarkFrameLoaded.Name = "tslblDarkFrameLoaded";
            this.tslblDarkFrameLoaded.Size = new System.Drawing.Size(66, 19);
            this.tslblDarkFrameLoaded.Text = "Dark Frame";
            this.tslblDarkFrameLoaded.Visible = false;
            // 
            // tslblFlatFrameLoaded
            // 
            this.tslblFlatFrameLoaded.BackColor = System.Drawing.Color.MediumAquamarine;
            this.tslblFlatFrameLoaded.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tslblFlatFrameLoaded.Name = "tslblFlatFrameLoaded";
            this.tslblFlatFrameLoaded.Size = new System.Drawing.Size(62, 19);
            this.tslblFlatFrameLoaded.Text = "Flat Frame";
            this.tslblFlatFrameLoaded.Visible = false;
            // 
            // ssMoreInfo
            // 
            this.ssMoreInfo.Name = "ssMoreInfo";
            this.ssMoreInfo.Size = new System.Drawing.Size(0, 19);
            // 
            // pnlNewVersionAvailable
            // 
            this.pnlNewVersionAvailable.AutoSize = false;
            this.pnlNewVersionAvailable.BackColor = System.Drawing.Color.Black;
            this.pnlNewVersionAvailable.IsLink = true;
            this.pnlNewVersionAvailable.LinkColor = System.Drawing.Color.Lime;
            this.pnlNewVersionAvailable.Name = "pnlNewVersionAvailable";
            this.pnlNewVersionAvailable.Size = new System.Drawing.Size(304, 19);
            this.pnlNewVersionAvailable.Text = "New version of Tangra is available. Click here to upgrade.";
            this.pnlNewVersionAvailable.Visible = false;
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.pnlControlerPanel);
            this.panelRight.Controls.Add(this.zoomedImage);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(625, 24);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(254, 571);
            this.panelRight.TabIndex = 3;
            // 
            // pnlControlerPanel
            // 
            this.pnlControlerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlControlerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(193)))), ((int)(((byte)(188)))));
            this.pnlControlerPanel.Cursor = System.Windows.Forms.Cursors.Default;
            this.pnlControlerPanel.Location = new System.Drawing.Point(3, 255);
            this.pnlControlerPanel.Name = "pnlControlerPanel";
            this.pnlControlerPanel.Size = new System.Drawing.Size(249, 311);
            this.pnlControlerPanel.TabIndex = 10;
            // 
            // panelVideo
            // 
            this.panelVideo.Controls.Add(this.pictureBox);
            this.panelVideo.Controls.Add(this.pnlPlayControls);
            this.panelVideo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelVideo.Location = new System.Drawing.Point(0, 24);
            this.panelVideo.Name = "panelVideo";
            this.panelVideo.Size = new System.Drawing.Size(625, 571);
            this.panelVideo.TabIndex = 4;
            // 
            // pnlPlayControls
            // 
            this.pnlPlayControls.Controls.Add(this.pnlPlayButtons);
            this.pnlPlayControls.Controls.Add(this.scrollBarFrames);
            this.pnlPlayControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlPlayControls.Location = new System.Drawing.Point(0, 504);
            this.pnlPlayControls.Name = "pnlPlayControls";
            this.pnlPlayControls.Size = new System.Drawing.Size(625, 67);
            this.pnlPlayControls.TabIndex = 0;
            // 
            // pnlPlayButtons
            // 
            this.pnlPlayButtons.Controls.Add(this.btnJumpTo);
            this.pnlPlayButtons.Controls.Add(this.btn1SecMinus);
            this.pnlPlayButtons.Controls.Add(this.btn10SecMinus);
            this.pnlPlayButtons.Controls.Add(this.btnPlay);
            this.pnlPlayButtons.Controls.Add(this.btn10SecPlus);
            this.pnlPlayButtons.Controls.Add(this.btnStop);
            this.pnlPlayButtons.Controls.Add(this.btn1FrPlus);
            this.pnlPlayButtons.Controls.Add(this.btn1SecPlus);
            this.pnlPlayButtons.Controls.Add(this.btn1FrMinus);
            this.pnlPlayButtons.Location = new System.Drawing.Point(69, 30);
            this.pnlPlayButtons.Name = "pnlPlayButtons";
            this.pnlPlayButtons.Size = new System.Drawing.Size(473, 36);
            this.pnlPlayButtons.TabIndex = 12;
            // 
            // btnJumpTo
            // 
            this.btnJumpTo.BackColor = System.Drawing.SystemColors.Control;
            this.btnJumpTo.Location = new System.Drawing.Point(413, 4);
            this.btnJumpTo.Name = "btnJumpTo";
            this.btnJumpTo.Size = new System.Drawing.Size(57, 29);
            this.btnJumpTo.TabIndex = 10;
            this.btnJumpTo.Text = "Jump To";
            this.btnJumpTo.UseVisualStyleBackColor = false;
            // 
            // btn1SecMinus
            // 
            this.btn1SecMinus.BackColor = System.Drawing.SystemColors.Control;
            this.btn1SecMinus.Location = new System.Drawing.Point(63, 3);
            this.btn1SecMinus.Name = "btn1SecMinus";
            this.btn1SecMinus.Size = new System.Drawing.Size(44, 29);
            this.btn1SecMinus.TabIndex = 7;
            this.btn1SecMinus.Text = "-1sec";
            this.btn1SecMinus.UseVisualStyleBackColor = false;
            // 
            // btn10SecMinus
            // 
            this.btn10SecMinus.BackColor = System.Drawing.SystemColors.Control;
            this.btn10SecMinus.Location = new System.Drawing.Point(7, 3);
            this.btn10SecMinus.Name = "btn10SecMinus";
            this.btn10SecMinus.Size = new System.Drawing.Size(50, 29);
            this.btn10SecMinus.TabIndex = 9;
            this.btn10SecMinus.Text = "-10sec";
            this.btn10SecMinus.UseVisualStyleBackColor = false;
            // 
            // btn10SecPlus
            // 
            this.btn10SecPlus.BackColor = System.Drawing.SystemColors.Control;
            this.btn10SecPlus.Location = new System.Drawing.Point(325, 3);
            this.btn10SecPlus.Name = "btn10SecPlus";
            this.btn10SecPlus.Size = new System.Drawing.Size(50, 29);
            this.btn10SecPlus.TabIndex = 8;
            this.btn10SecPlus.Text = "10sec+";
            this.btn10SecPlus.UseVisualStyleBackColor = false;
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.SystemColors.Control;
            this.btnStop.Image = global::Tangra.Properties.Resources.stop24;
            this.btnStop.Location = new System.Drawing.Point(194, 3);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(32, 29);
            this.btnStop.TabIndex = 2;
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btn1FrPlus
            // 
            this.btn1FrPlus.BackColor = System.Drawing.SystemColors.Control;
            this.btn1FrPlus.Location = new System.Drawing.Point(232, 3);
            this.btn1FrPlus.Name = "btn1FrPlus";
            this.btn1FrPlus.Size = new System.Drawing.Size(37, 29);
            this.btn1FrPlus.TabIndex = 4;
            this.btn1FrPlus.Text = "1Fr+";
            this.btn1FrPlus.UseVisualStyleBackColor = false;
            // 
            // btn1SecPlus
            // 
            this.btn1SecPlus.BackColor = System.Drawing.SystemColors.Control;
            this.btn1SecPlus.Location = new System.Drawing.Point(275, 3);
            this.btn1SecPlus.Name = "btn1SecPlus";
            this.btn1SecPlus.Size = new System.Drawing.Size(44, 29);
            this.btn1SecPlus.TabIndex = 6;
            this.btn1SecPlus.Text = "1sec+";
            this.btn1SecPlus.UseVisualStyleBackColor = false;
            // 
            // btn1FrMinus
            // 
            this.btn1FrMinus.BackColor = System.Drawing.SystemColors.Control;
            this.btn1FrMinus.Location = new System.Drawing.Point(113, 3);
            this.btn1FrMinus.Name = "btn1FrMinus";
            this.btn1FrMinus.Size = new System.Drawing.Size(37, 29);
            this.btn1FrMinus.TabIndex = 5;
            this.btn1FrMinus.Text = "-1Fr";
            this.btn1FrMinus.UseVisualStyleBackColor = false;
            // 
            // scrollBarFrames
            // 
            this.scrollBarFrames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.scrollBarFrames.Location = new System.Drawing.Point(11, 7);
            this.scrollBarFrames.Maximum = 1000;
            this.scrollBarFrames.Name = "scrollBarFrames";
            this.scrollBarFrames.Size = new System.Drawing.Size(601, 16);
            this.scrollBarFrames.TabIndex = 11;
            // 
            // openVideoFileDialog
            // 
            this.openVideoFileDialog.DefaultExt = "avi";
            this.openVideoFileDialog.Filter = "All Supported Files (*.avi;*.avs;*.adv)|*.avi;*.avs;*.adv";
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(625, 504);
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            // 
            // btnPlay
            // 
            this.btnPlay.BackColor = System.Drawing.SystemColors.Control;
            this.btnPlay.Image = global::Tangra.Properties.Resources.play24;
            this.btnPlay.Location = new System.Drawing.Point(156, 3);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(32, 29);
            this.btnPlay.TabIndex = 1;
            this.btnPlay.UseVisualStyleBackColor = false;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // zoomedImage
            // 
            this.zoomedImage.BackColor = System.Drawing.SystemColors.ControlDark;
            this.zoomedImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.zoomedImage.Location = new System.Drawing.Point(3, 3);
            this.zoomedImage.Name = "zoomedImage";
            this.zoomedImage.Size = new System.Drawing.Size(248, 248);
            this.zoomedImage.TabIndex = 1;
            this.zoomedImage.TabStop = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(879, 619);
            this.Controls.Add(this.panelVideo);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.mainMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenu;
            this.MinimumSize = new System.Drawing.Size(800, 645);
            this.Name = "frmMain";
            this.Text = "Tangra v3.0";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.panelRight.ResumeLayout(false);
            this.panelVideo.ResumeLayout(false);
            this.pnlPlayControls.ResumeLayout(false);
            this.pnlPlayButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomedImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip mainMenu;
		private System.Windows.Forms.Panel panelRight;
		private System.Windows.Forms.Panel panelVideo;
		protected internal System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.ToolStripMenuItem miFile;
		private System.Windows.Forms.ToolStripMenuItem miExit;
		private System.Windows.Forms.ToolStripMenuItem miOpenVideo;
		private System.Windows.Forms.ToolStripMenuItem miRecentVideos;
		private System.Windows.Forms.ToolStripMenuItem miOpenLightCurve;
		private System.Windows.Forms.ToolStripMenuItem miRecentLightCurves;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem miFrameActions;
		private System.Windows.Forms.ToolStripMenuItem miHelp;
		private System.Windows.Forms.ToolStripMenuItem miOnlineHelp;
		private System.Windows.Forms.ToolStripMenuItem miCheckForUpdates;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem miAbout;
		private System.Windows.Forms.ToolStripMenuItem miSettings;
		private System.Windows.Forms.ToolStripMenuItem miTools;
		private System.Windows.Forms.ToolStripMenuItem miTasks;
		private System.Windows.Forms.Button btn1SecMinus;
		private System.Windows.Forms.Button btn10SecMinus;
		private System.Windows.Forms.Button btn10SecPlus;
		private System.Windows.Forms.Button btn1FrPlus;
		private System.Windows.Forms.Button btn1SecPlus;
		private System.Windows.Forms.Button btn1FrMinus;
		private System.Windows.Forms.Panel pnlControlerPanel;
		private System.Windows.Forms.PictureBox zoomedImage;
		private System.Windows.Forms.OpenFileDialog openVideoFileDialog;
		protected internal System.Windows.Forms.StatusStrip statusStrip;
		protected internal System.Windows.Forms.Panel pnlPlayButtons;
		protected internal System.Windows.Forms.HScrollBar scrollBarFrames;
		protected internal System.Windows.Forms.Panel pnlPlayControls;
		protected internal System.Windows.Forms.Button btnJumpTo;
		protected internal System.Windows.Forms.Button btnPlay;
		protected internal System.Windows.Forms.Button btnStop;
		protected internal System.Windows.Forms.ToolStripMenuItem miExportToBMP;
		protected internal System.Windows.Forms.ToolStripMenuItem miExportToFits;
		protected internal System.Windows.Forms.ToolStripMenuItem miReduceLightCurve;
		protected internal System.Windows.Forms.ToolStripProgressBar tsProgessBar;
		protected internal System.Windows.Forms.ToolStripStatusLabel ssStatus;
		protected internal System.Windows.Forms.ToolStripStatusLabel ssToolInfo;
		protected internal System.Windows.Forms.ToolStripStatusLabel ssGamma;
		protected internal System.Windows.Forms.ToolStripStatusLabel ssPixelFormat;
		protected internal System.Windows.Forms.ToolStripStatusLabel tslblIntegration;
		protected internal System.Windows.Forms.ToolStripStatusLabel ssFrameNo;
		protected internal System.Windows.Forms.ToolStripStatusLabel ssFPS;
		protected internal System.Windows.Forms.ToolStripStatusLabel tslblRecDbg;
		protected internal System.Windows.Forms.ToolStripStatusLabel tslblUsingAviSynth;
		protected internal System.Windows.Forms.ToolStripStatusLabel tslblDarkFrameLoaded;
		protected internal System.Windows.Forms.ToolStripStatusLabel tslblFlatFrameLoaded;
		protected internal System.Windows.Forms.ToolStripStatusLabel ssMoreInfo;
		protected internal System.Windows.Forms.ToolStripStatusLabel pnlNewVersionAvailable;
	}
}

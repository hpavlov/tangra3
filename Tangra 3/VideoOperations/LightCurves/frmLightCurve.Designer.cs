namespace Tangra.VideoOperations.LightCurves
{
    partial class frmLightCurve
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLightCurve));
			this.pnlLegend = new System.Windows.Forms.Panel();
			this.pnlSmallGraph = new Tangra.Controls.SmoothPanel();
			this.pnlMeasurementDetails = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.pnlGeoLocation = new System.Windows.Forms.Panel();
			this.tbxGeoLocation = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.lblInstDelayWarning = new System.Windows.Forms.Label();
			this.lblSN4 = new System.Windows.Forms.Label();
			this.lblSN3 = new System.Windows.Forms.Label();
			this.lblSN2 = new System.Windows.Forms.Label();
			this.lblSN1 = new System.Windows.Forms.Label();
			this.lblSNLBL4 = new System.Windows.Forms.Label();
			this.lblSNLBL3 = new System.Windows.Forms.Label();
			this.lblSNLBL2 = new System.Windows.Forms.Label();
			this.lblSNLBL1 = new System.Windows.Forms.Label();
			this.lblFrameNo = new System.Windows.Forms.Label();
			this.lblMeasurement1 = new System.Windows.Forms.Label();
			this.picTarget4PSF = new System.Windows.Forms.PictureBox();
			this.lblMeasurement2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.lblMeasurement3 = new System.Windows.Forms.Label();
			this.pnlBinInfo = new System.Windows.Forms.Panel();
			this.label3 = new System.Windows.Forms.Label();
			this.lblBinNo = new System.Windows.Forms.Label();
			this.lblFrameTime = new System.Windows.Forms.Label();
			this.pb4 = new System.Windows.Forms.PictureBox();
			this.lblMeasurement4 = new System.Windows.Forms.Label();
			this.picTarget4Pixels = new System.Windows.Forms.PictureBox();
			this.pb3 = new System.Windows.Forms.PictureBox();
			this.picTarget3PSF = new System.Windows.Forms.PictureBox();
			this.picTarget1Pixels = new System.Windows.Forms.PictureBox();
			this.pb2 = new System.Windows.Forms.PictureBox();
			this.picTarget3Pixels = new System.Windows.Forms.PictureBox();
			this.picTarget1PSF = new System.Windows.Forms.PictureBox();
			this.picTarget2Pixels = new System.Windows.Forms.PictureBox();
			this.picTarget2PSF = new System.Windows.Forms.PictureBox();
			this.pb1 = new System.Windows.Forms.PictureBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.sbZoomStartFrame = new System.Windows.Forms.HScrollBar();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.lightCurveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.miLoad = new System.Windows.Forms.ToolStripMenuItem();
			this.miSave = new System.Windows.Forms.ToolStripMenuItem();
			this.exportToCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.miExportTangraCSV = new System.Windows.Forms.ToolStripMenuItem();
			this.miSaveAsImage = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.miCopyToClipboard = new System.Windows.Forms.ToolStripMenuItem();
			this.miData = new System.Windows.Forms.ToolStripMenuItem();
			this.miShowZoomedAreas = new System.Windows.Forms.ToolStripMenuItem();
			this.miShowPSFFits = new System.Windows.Forms.ToolStripMenuItem();
			this.miBackgroundHistograms = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.miOutlierRemoval = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.miReprocess = new System.Windows.Forms.ToolStripMenuItem();
			this.miFullReprocess = new System.Windows.Forms.ToolStripMenuItem();
			this.miAdjustMeasurements = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.miNoiseDistribution = new System.Windows.Forms.ToolStripMenuItem();
			this.miPixelDistribution = new System.Windows.Forms.ToolStripMenuItem();
			this.miExportNTPDebugData = new System.Windows.Forms.ToolStripMenuItem();
			this.processToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.miDisplaySettings = new System.Windows.Forms.ToolStripMenuItem();
			this.miAddTitle = new System.Windows.Forms.ToolStripMenuItem();
			this.miAddins = new System.Windows.Forms.ToolStripMenuItem();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.tslblSignalType = new System.Windows.Forms.ToolStripDropDownButton();
			this.miSignalDividedByNoise = new System.Windows.Forms.ToolStripMenuItem();
			this.miSignalDividedByBackground = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.miNoiseOnly = new System.Windows.Forms.ToolStripMenuItem();
			this.miSignalMinusNoise = new System.Windows.Forms.ToolStripMenuItem();
			this.miSignalOnly = new System.Windows.Forms.ToolStripMenuItem();
			this.tslblNormalisation = new System.Windows.Forms.ToolStripDropDownButton();
			this.normalisationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.miNormalisation1 = new System.Windows.Forms.ToolStripMenuItem();
			this.miNormalisation2 = new System.Windows.Forms.ToolStripMenuItem();
			this.miNormalisation3 = new System.Windows.Forms.ToolStripMenuItem();
			this.miNormalisation4 = new System.Windows.Forms.ToolStripMenuItem();
			this.miNormalizationSeparator = new System.Windows.Forms.ToolStripSeparator();
			this.miNormalisationMethod = new System.Windows.Forms.ToolStripMenuItem();
			this.miNormalisation16DataPoints = new System.Windows.Forms.ToolStripMenuItem();
			this.miNormalisation4DataPoints = new System.Windows.Forms.ToolStripMenuItem();
			this.miNormalisation1DataPoints = new System.Windows.Forms.ToolStripMenuItem();
			this.miNormalisationLinearFit = new System.Windows.Forms.ToolStripMenuItem();
			this.miNoNormalization = new System.Windows.Forms.ToolStripMenuItem();
			this.tslblBinning = new System.Windows.Forms.ToolStripDropDownButton();
			this.binningToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.miBinning4 = new System.Windows.Forms.ToolStripMenuItem();
			this.miBinning8 = new System.Windows.Forms.ToolStripMenuItem();
			this.miBinning16 = new System.Windows.Forms.ToolStripMenuItem();
			this.miBinning32 = new System.Windows.Forms.ToolStripMenuItem();
			this.miBinning64 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.miCustomBinning = new System.Windows.Forms.ToolStripMenuItem();
			this.miNoBinning = new System.Windows.Forms.ToolStripMenuItem();
			this.miIncludeObject = new System.Windows.Forms.ToolStripDropDownButton();
			this.miIncludeObj4 = new System.Windows.Forms.ToolStripMenuItem();
			this.miIncludeObj3 = new System.Windows.Forms.ToolStripMenuItem();
			this.miIncludeObj2 = new System.Windows.Forms.ToolStripMenuItem();
			this.miIncludeObj1 = new System.Windows.Forms.ToolStripMenuItem();
			this.pnlMain = new System.Windows.Forms.Panel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.pnlChart = new Tangra.Controls.SmoothPanel();
			this.btnSetMags = new System.Windows.Forms.Button();
			this.btnZoomIn = new System.Windows.Forms.Button();
			this.btnZoomOut = new System.Windows.Forms.Button();
			this.hintTimer = new System.Windows.Forms.Timer(this.components);
			this.saveImageDialog = new System.Windows.Forms.SaveFileDialog();
			this.saveCSVDialog = new System.Windows.Forms.SaveFileDialog();
			this.firstFrameTimer = new System.Windows.Forms.Timer(this.components);
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.lblMagnitude1 = new System.Windows.Forms.Label();
			this.lblMagnitude2 = new System.Windows.Forms.Label();
			this.lblMagnitude3 = new System.Windows.Forms.Label();
			this.lblMagnitude4 = new System.Windows.Forms.Label();
			this.pnlLegend.SuspendLayout();
			this.pnlSmallGraph.SuspendLayout();
			this.pnlMeasurementDetails.SuspendLayout();
			this.pnlGeoLocation.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picTarget4PSF)).BeginInit();
			this.pnlBinInfo.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pb4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget4Pixels)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pb3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget3PSF)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget1Pixels)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pb2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget3Pixels)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget1PSF)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget2Pixels)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget2PSF)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pb1)).BeginInit();
			this.panel2.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.pnlMain.SuspendLayout();
			this.panel1.SuspendLayout();
			this.pnlChart.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlLegend
			// 
			this.pnlLegend.Controls.Add(this.pnlSmallGraph);
			this.pnlLegend.Controls.Add(this.panel2);
			this.pnlLegend.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlLegend.Location = new System.Drawing.Point(0, 381);
			this.pnlLegend.Name = "pnlLegend";
			this.pnlLegend.Size = new System.Drawing.Size(704, 87);
			this.pnlLegend.TabIndex = 1;
			// 
			// pnlSmallGraph
			// 
			this.pnlSmallGraph.Controls.Add(this.pnlMeasurementDetails);
			this.pnlSmallGraph.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlSmallGraph.Location = new System.Drawing.Point(0, 19);
			this.pnlSmallGraph.Name = "pnlSmallGraph";
			this.pnlSmallGraph.Size = new System.Drawing.Size(704, 68);
			this.pnlSmallGraph.TabIndex = 4;
			this.pnlSmallGraph.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlSmallGraph_MouseDown);
			this.pnlSmallGraph.MouseLeave += new System.EventHandler(this.pnlSmallGraph_MouseLeave);
			this.pnlSmallGraph.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlSmallGraph_MouseMove);
			this.pnlSmallGraph.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlSmallGraph_MouseUp);
			this.pnlSmallGraph.Resize += new System.EventHandler(this.pnlSmallGraph_Resize);
			// 
			// pnlMeasurementDetails
			// 
			this.pnlMeasurementDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pnlMeasurementDetails.Controls.Add(this.lblMagnitude1);
			this.pnlMeasurementDetails.Controls.Add(this.lblMagnitude2);
			this.pnlMeasurementDetails.Controls.Add(this.lblMagnitude3);
			this.pnlMeasurementDetails.Controls.Add(this.lblMagnitude4);
			this.pnlMeasurementDetails.Controls.Add(this.label2);
			this.pnlMeasurementDetails.Controls.Add(this.pnlGeoLocation);
			this.pnlMeasurementDetails.Controls.Add(this.lblInstDelayWarning);
			this.pnlMeasurementDetails.Controls.Add(this.lblSN4);
			this.pnlMeasurementDetails.Controls.Add(this.lblSN3);
			this.pnlMeasurementDetails.Controls.Add(this.lblSN2);
			this.pnlMeasurementDetails.Controls.Add(this.lblSN1);
			this.pnlMeasurementDetails.Controls.Add(this.lblSNLBL4);
			this.pnlMeasurementDetails.Controls.Add(this.lblSNLBL3);
			this.pnlMeasurementDetails.Controls.Add(this.lblSNLBL2);
			this.pnlMeasurementDetails.Controls.Add(this.lblSNLBL1);
			this.pnlMeasurementDetails.Controls.Add(this.lblFrameNo);
			this.pnlMeasurementDetails.Controls.Add(this.picTarget4PSF);
			this.pnlMeasurementDetails.Controls.Add(this.label1);
			this.pnlMeasurementDetails.Controls.Add(this.pnlBinInfo);
			this.pnlMeasurementDetails.Controls.Add(this.lblFrameTime);
			this.pnlMeasurementDetails.Controls.Add(this.pb4);
			this.pnlMeasurementDetails.Controls.Add(this.picTarget4Pixels);
			this.pnlMeasurementDetails.Controls.Add(this.pb3);
			this.pnlMeasurementDetails.Controls.Add(this.picTarget3PSF);
			this.pnlMeasurementDetails.Controls.Add(this.picTarget1Pixels);
			this.pnlMeasurementDetails.Controls.Add(this.pb2);
			this.pnlMeasurementDetails.Controls.Add(this.picTarget3Pixels);
			this.pnlMeasurementDetails.Controls.Add(this.picTarget1PSF);
			this.pnlMeasurementDetails.Controls.Add(this.picTarget2Pixels);
			this.pnlMeasurementDetails.Controls.Add(this.picTarget2PSF);
			this.pnlMeasurementDetails.Controls.Add(this.pb1);
			this.pnlMeasurementDetails.Controls.Add(this.lblMeasurement1);
			this.pnlMeasurementDetails.Controls.Add(this.lblMeasurement2);
			this.pnlMeasurementDetails.Controls.Add(this.lblMeasurement3);
			this.pnlMeasurementDetails.Controls.Add(this.lblMeasurement4);
			this.pnlMeasurementDetails.Location = new System.Drawing.Point(3, 3);
			this.pnlMeasurementDetails.Name = "pnlMeasurementDetails";
			this.pnlMeasurementDetails.Size = new System.Drawing.Size(698, 64);
			this.pnlMeasurementDetails.TabIndex = 30;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(26, 27);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(33, 13);
			this.label2.TabIndex = 17;
			this.label2.Text = "Time:";
			// 
			// pnlGeoLocation
			// 
			this.pnlGeoLocation.Controls.Add(this.tbxGeoLocation);
			this.pnlGeoLocation.Controls.Add(this.label4);
			this.pnlGeoLocation.Controls.Add(this.label5);
			this.pnlGeoLocation.Location = new System.Drawing.Point(8, 38);
			this.pnlGeoLocation.Name = "pnlGeoLocation";
			this.pnlGeoLocation.Size = new System.Drawing.Size(352, 21);
			this.pnlGeoLocation.TabIndex = 38;
			// 
			// tbxGeoLocation
			// 
			this.tbxGeoLocation.BackColor = System.Drawing.SystemColors.Control;
			this.tbxGeoLocation.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbxGeoLocation.ForeColor = System.Drawing.SystemColors.ActiveCaption;
			this.tbxGeoLocation.Location = new System.Drawing.Point(60, 5);
			this.tbxGeoLocation.Name = "tbxGeoLocation";
			this.tbxGeoLocation.Size = new System.Drawing.Size(289, 13);
			this.tbxGeoLocation.TabIndex = 29;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(1, 5);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(51, 13);
			this.label4.TabIndex = 27;
			this.label4.Text = "Location:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(48, 5);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(0, 13);
			this.label5.TabIndex = 28;
			// 
			// lblInstDelayWarning
			// 
			this.lblInstDelayWarning.AutoSize = true;
			this.lblInstDelayWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblInstDelayWarning.ForeColor = System.Drawing.Color.Red;
			this.lblInstDelayWarning.Location = new System.Drawing.Point(12, 26);
			this.lblInstDelayWarning.Name = "lblInstDelayWarning";
			this.lblInstDelayWarning.Size = new System.Drawing.Size(20, 25);
			this.lblInstDelayWarning.TabIndex = 39;
			this.lblInstDelayWarning.Text = "*";
			this.toolTip1.SetToolTip(this.lblInstDelayWarning, "Instrumental delay has not been applied to the times");
			this.lblInstDelayWarning.Visible = false;
			// 
			// lblSN4
			// 
			this.lblSN4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblSN4.AutoSize = true;
			this.lblSN4.Location = new System.Drawing.Point(659, 50);
			this.lblSN4.Name = "lblSN4";
			this.lblSN4.Size = new System.Drawing.Size(34, 13);
			this.lblSN4.TabIndex = 37;
			this.lblSN4.Text = "12.56";
			// 
			// lblSN3
			// 
			this.lblSN3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblSN3.AutoSize = true;
			this.lblSN3.Location = new System.Drawing.Point(575, 50);
			this.lblSN3.Name = "lblSN3";
			this.lblSN3.Size = new System.Drawing.Size(34, 13);
			this.lblSN3.TabIndex = 36;
			this.lblSN3.Text = "12.56";
			// 
			// lblSN2
			// 
			this.lblSN2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblSN2.AutoSize = true;
			this.lblSN2.Location = new System.Drawing.Point(488, 50);
			this.lblSN2.Name = "lblSN2";
			this.lblSN2.Size = new System.Drawing.Size(34, 13);
			this.lblSN2.TabIndex = 35;
			this.lblSN2.Text = "12.56";
			// 
			// lblSN1
			// 
			this.lblSN1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblSN1.AutoSize = true;
			this.lblSN1.Location = new System.Drawing.Point(401, 50);
			this.lblSN1.Name = "lblSN1";
			this.lblSN1.Size = new System.Drawing.Size(34, 13);
			this.lblSN1.TabIndex = 34;
			this.lblSN1.Text = "12.56";
			// 
			// lblSNLBL4
			// 
			this.lblSNLBL4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblSNLBL4.AutoSize = true;
			this.lblSNLBL4.Location = new System.Drawing.Point(623, 50);
			this.lblSNLBL4.Name = "lblSNLBL4";
			this.lblSNLBL4.Size = new System.Drawing.Size(39, 13);
			this.lblSNLBL4.TabIndex = 33;
			this.lblSNLBL4.Text = "S/N = ";
			// 
			// lblSNLBL3
			// 
			this.lblSNLBL3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblSNLBL3.AutoSize = true;
			this.lblSNLBL3.Location = new System.Drawing.Point(539, 50);
			this.lblSNLBL3.Name = "lblSNLBL3";
			this.lblSNLBL3.Size = new System.Drawing.Size(39, 13);
			this.lblSNLBL3.TabIndex = 32;
			this.lblSNLBL3.Text = "S/N = ";
			// 
			// lblSNLBL2
			// 
			this.lblSNLBL2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblSNLBL2.AutoSize = true;
			this.lblSNLBL2.Location = new System.Drawing.Point(452, 50);
			this.lblSNLBL2.Name = "lblSNLBL2";
			this.lblSNLBL2.Size = new System.Drawing.Size(39, 13);
			this.lblSNLBL2.TabIndex = 31;
			this.lblSNLBL2.Text = "S/N = ";
			// 
			// lblSNLBL1
			// 
			this.lblSNLBL1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblSNLBL1.AutoSize = true;
			this.lblSNLBL1.Location = new System.Drawing.Point(366, 50);
			this.lblSNLBL1.Name = "lblSNLBL1";
			this.lblSNLBL1.Size = new System.Drawing.Size(39, 13);
			this.lblSNLBL1.TabIndex = 30;
			this.lblSNLBL1.Text = "S/N = ";
			// 
			// lblFrameNo
			// 
			this.lblFrameNo.AutoSize = true;
			this.lblFrameNo.Location = new System.Drawing.Point(65, 11);
			this.lblFrameNo.Name = "lblFrameNo";
			this.lblFrameNo.Size = new System.Drawing.Size(35, 13);
			this.lblFrameNo.TabIndex = 1;
			this.lblFrameNo.Text = "sdfsdf";
			// 
			// lblMeasurement1
			// 
			this.lblMeasurement1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblMeasurement1.AutoSize = true;
			this.lblMeasurement1.Location = new System.Drawing.Point(379, -1);
			this.lblMeasurement1.Name = "lblMeasurement1";
			this.lblMeasurement1.Size = new System.Drawing.Size(0, 13);
			this.lblMeasurement1.TabIndex = 3;
			// 
			// picTarget4PSF
			// 
			this.picTarget4PSF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.picTarget4PSF.BackColor = System.Drawing.Color.White;
			this.picTarget4PSF.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.picTarget4PSF.Location = new System.Drawing.Point(662, 15);
			this.picTarget4PSF.Name = "picTarget4PSF";
			this.picTarget4PSF.Size = new System.Drawing.Size(34, 34);
			this.picTarget4PSF.TabIndex = 26;
			this.picTarget4PSF.TabStop = false;
			this.picTarget4PSF.Visible = false;
			// 
			// lblMeasurement2
			// 
			this.lblMeasurement2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblMeasurement2.AutoSize = true;
			this.lblMeasurement2.Location = new System.Drawing.Point(464, -1);
			this.lblMeasurement2.Name = "lblMeasurement2";
			this.lblMeasurement2.Size = new System.Drawing.Size(0, 13);
			this.lblMeasurement2.TabIndex = 11;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 11);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Frame No:";
			// 
			// lblMeasurement3
			// 
			this.lblMeasurement3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblMeasurement3.AutoSize = true;
			this.lblMeasurement3.Location = new System.Drawing.Point(551, -1);
			this.lblMeasurement3.Name = "lblMeasurement3";
			this.lblMeasurement3.Size = new System.Drawing.Size(0, 13);
			this.lblMeasurement3.TabIndex = 13;
			// 
			// pnlBinInfo
			// 
			this.pnlBinInfo.Controls.Add(this.label3);
			this.pnlBinInfo.Controls.Add(this.lblBinNo);
			this.pnlBinInfo.Location = new System.Drawing.Point(141, 6);
			this.pnlBinInfo.Name = "pnlBinInfo";
			this.pnlBinInfo.Size = new System.Drawing.Size(206, 21);
			this.pnlBinInfo.TabIndex = 29;
			this.pnlBinInfo.Visible = false;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(1, 5);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(42, 13);
			this.label3.TabIndex = 27;
			this.label3.Text = "Bin No:";
			// 
			// lblBinNo
			// 
			this.lblBinNo.AutoSize = true;
			this.lblBinNo.Location = new System.Drawing.Point(48, 5);
			this.lblBinNo.Name = "lblBinNo";
			this.lblBinNo.Size = new System.Drawing.Size(0, 13);
			this.lblBinNo.TabIndex = 28;
			// 
			// lblFrameTime
			// 
			this.lblFrameTime.AutoSize = true;
			this.lblFrameTime.Location = new System.Drawing.Point(65, 27);
			this.lblFrameTime.Name = "lblFrameTime";
			this.lblFrameTime.Size = new System.Drawing.Size(0, 13);
			this.lblFrameTime.TabIndex = 18;
			// 
			// pb4
			// 
			this.pb4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pb4.BackColor = System.Drawing.Color.Maroon;
			this.pb4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pb4.Location = new System.Drawing.Point(626, 0);
			this.pb4.Name = "pb4";
			this.pb4.Size = new System.Drawing.Size(10, 11);
			this.pb4.TabIndex = 16;
			this.pb4.TabStop = false;
			// 
			// lblMeasurement4
			// 
			this.lblMeasurement4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblMeasurement4.AutoSize = true;
			this.lblMeasurement4.Location = new System.Drawing.Point(635, -1);
			this.lblMeasurement4.Name = "lblMeasurement4";
			this.lblMeasurement4.Size = new System.Drawing.Size(0, 13);
			this.lblMeasurement4.TabIndex = 15;
			// 
			// picTarget4Pixels
			// 
			this.picTarget4Pixels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.picTarget4Pixels.BackColor = System.Drawing.Color.White;
			this.picTarget4Pixels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.picTarget4Pixels.Location = new System.Drawing.Point(626, 15);
			this.picTarget4Pixels.Name = "picTarget4Pixels";
			this.picTarget4Pixels.Size = new System.Drawing.Size(34, 34);
			this.picTarget4Pixels.TabIndex = 25;
			this.picTarget4Pixels.TabStop = false;
			this.picTarget4Pixels.Visible = false;
			// 
			// pb3
			// 
			this.pb3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pb3.BackColor = System.Drawing.Color.Maroon;
			this.pb3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pb3.Location = new System.Drawing.Point(542, 0);
			this.pb3.Name = "pb3";
			this.pb3.Size = new System.Drawing.Size(10, 11);
			this.pb3.TabIndex = 14;
			this.pb3.TabStop = false;
			// 
			// picTarget3PSF
			// 
			this.picTarget3PSF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.picTarget3PSF.BackColor = System.Drawing.Color.White;
			this.picTarget3PSF.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.picTarget3PSF.Location = new System.Drawing.Point(578, 15);
			this.picTarget3PSF.Name = "picTarget3PSF";
			this.picTarget3PSF.Size = new System.Drawing.Size(34, 34);
			this.picTarget3PSF.TabIndex = 24;
			this.picTarget3PSF.TabStop = false;
			this.picTarget3PSF.Visible = false;
			// 
			// picTarget1Pixels
			// 
			this.picTarget1Pixels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.picTarget1Pixels.BackColor = System.Drawing.Color.White;
			this.picTarget1Pixels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.picTarget1Pixels.Location = new System.Drawing.Point(369, 15);
			this.picTarget1Pixels.Name = "picTarget1Pixels";
			this.picTarget1Pixels.Size = new System.Drawing.Size(34, 34);
			this.picTarget1Pixels.TabIndex = 19;
			this.picTarget1Pixels.TabStop = false;
			this.picTarget1Pixels.Visible = false;
			// 
			// pb2
			// 
			this.pb2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pb2.BackColor = System.Drawing.Color.Maroon;
			this.pb2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pb2.Location = new System.Drawing.Point(455, 0);
			this.pb2.Name = "pb2";
			this.pb2.Size = new System.Drawing.Size(10, 11);
			this.pb2.TabIndex = 12;
			this.pb2.TabStop = false;
			// 
			// picTarget3Pixels
			// 
			this.picTarget3Pixels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.picTarget3Pixels.BackColor = System.Drawing.Color.White;
			this.picTarget3Pixels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.picTarget3Pixels.Location = new System.Drawing.Point(542, 15);
			this.picTarget3Pixels.Name = "picTarget3Pixels";
			this.picTarget3Pixels.Size = new System.Drawing.Size(34, 34);
			this.picTarget3Pixels.TabIndex = 23;
			this.picTarget3Pixels.TabStop = false;
			this.picTarget3Pixels.Visible = false;
			// 
			// picTarget1PSF
			// 
			this.picTarget1PSF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.picTarget1PSF.BackColor = System.Drawing.Color.White;
			this.picTarget1PSF.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.picTarget1PSF.Location = new System.Drawing.Point(405, 15);
			this.picTarget1PSF.Name = "picTarget1PSF";
			this.picTarget1PSF.Size = new System.Drawing.Size(34, 34);
			this.picTarget1PSF.TabIndex = 20;
			this.picTarget1PSF.TabStop = false;
			this.picTarget1PSF.Visible = false;
			// 
			// picTarget2Pixels
			// 
			this.picTarget2Pixels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.picTarget2Pixels.BackColor = System.Drawing.Color.White;
			this.picTarget2Pixels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.picTarget2Pixels.Location = new System.Drawing.Point(455, 15);
			this.picTarget2Pixels.Name = "picTarget2Pixels";
			this.picTarget2Pixels.Size = new System.Drawing.Size(34, 34);
			this.picTarget2Pixels.TabIndex = 21;
			this.picTarget2Pixels.TabStop = false;
			this.picTarget2Pixels.Visible = false;
			// 
			// picTarget2PSF
			// 
			this.picTarget2PSF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.picTarget2PSF.BackColor = System.Drawing.Color.White;
			this.picTarget2PSF.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.picTarget2PSF.Location = new System.Drawing.Point(491, 15);
			this.picTarget2PSF.Name = "picTarget2PSF";
			this.picTarget2PSF.Size = new System.Drawing.Size(34, 34);
			this.picTarget2PSF.TabIndex = 22;
			this.picTarget2PSF.TabStop = false;
			this.picTarget2PSF.Visible = false;
			// 
			// pb1
			// 
			this.pb1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pb1.BackColor = System.Drawing.Color.Maroon;
			this.pb1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pb1.Location = new System.Drawing.Point(370, 0);
			this.pb1.Name = "pb1";
			this.pb1.Size = new System.Drawing.Size(10, 11);
			this.pb1.TabIndex = 10;
			this.pb1.TabStop = false;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.sbZoomStartFrame);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(704, 19);
			this.panel2.TabIndex = 3;
			// 
			// sbZoomStartFrame
			// 
			this.sbZoomStartFrame.Dock = System.Windows.Forms.DockStyle.Top;
			this.sbZoomStartFrame.Location = new System.Drawing.Point(0, 0);
			this.sbZoomStartFrame.Name = "sbZoomStartFrame";
			this.sbZoomStartFrame.Size = new System.Drawing.Size(704, 16);
			this.sbZoomStartFrame.TabIndex = 2;
			this.sbZoomStartFrame.ValueChanged += new System.EventHandler(this.hScrollBar1_ValueChanged);
			this.sbZoomStartFrame.MouseCaptureChanged += new System.EventHandler(this.sbZoomStartFrame_MouseCaptureChanged);
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.White;
			this.imageList1.Images.SetKeyName(0, "minus15x15.bmp");
			this.imageList1.Images.SetKeyName(1, "plus15x15.bmp");
			this.imageList1.Images.SetKeyName(2, "m15x15.bmp");
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lightCurveToolStripMenuItem,
            this.miData,
            this.processToolStripMenuItem,
            this.miAddins});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(704, 24);
			this.menuStrip1.TabIndex = 3;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// lightCurveToolStripMenuItem
			// 
			this.lightCurveToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miLoad,
            this.miSave,
            this.exportToCSVToolStripMenuItem});
			this.lightCurveToolStripMenuItem.Name = "lightCurveToolStripMenuItem";
			this.lightCurveToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.lightCurveToolStripMenuItem.Text = "&File";
			// 
			// miLoad
			// 
			this.miLoad.Name = "miLoad";
			this.miLoad.Size = new System.Drawing.Size(171, 22);
			this.miLoad.Text = "&Load Light Curve";
			this.miLoad.Click += new System.EventHandler(this.miLoad_Click);
			// 
			// miSave
			// 
			this.miSave.Name = "miSave";
			this.miSave.Size = new System.Drawing.Size(171, 22);
			this.miSave.Text = "&Save Light Curve";
			this.miSave.Click += new System.EventHandler(this.miSave_Click);
			// 
			// exportToCSVToolStripMenuItem
			// 
			this.exportToCSVToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miExportTangraCSV,
            this.miSaveAsImage,
            this.toolStripSeparator1,
            this.miCopyToClipboard});
			this.exportToCSVToolStripMenuItem.Name = "exportToCSVToolStripMenuItem";
			this.exportToCSVToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.exportToCSVToolStripMenuItem.Text = "&Export Light Curve";
			// 
			// miExportTangraCSV
			// 
			this.miExportTangraCSV.Name = "miExportTangraCSV";
			this.miExportTangraCSV.Size = new System.Drawing.Size(195, 22);
			this.miExportTangraCSV.Text = "Save as &CSV File";
			this.miExportTangraCSV.Click += new System.EventHandler(this.miExportTangraCSV_Click);
			// 
			// miSaveAsImage
			// 
			this.miSaveAsImage.Name = "miSaveAsImage";
			this.miSaveAsImage.Size = new System.Drawing.Size(195, 22);
			this.miSaveAsImage.Text = "Save as &Image File";
			this.miSaveAsImage.Click += new System.EventHandler(this.miSaveAsImage_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(192, 6);
			// 
			// miCopyToClipboard
			// 
			this.miCopyToClipboard.Name = "miCopyToClipboard";
			this.miCopyToClipboard.Size = new System.Drawing.Size(195, 22);
			this.miCopyToClipboard.Text = "Copy &Plot to Clipboard";
			this.miCopyToClipboard.Click += new System.EventHandler(this.miCopyToClipboard_Click);
			// 
			// miData
			// 
			this.miData.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miShowZoomedAreas,
            this.miShowPSFFits,
            this.miBackgroundHistograms,
            this.toolStripSeparator3,
            this.miOutlierRemoval,
            this.toolStripSeparator5,
            this.miReprocess,
            this.miFullReprocess,
            this.miAdjustMeasurements,
            this.toolStripSeparator2,
            this.miNoiseDistribution,
            this.miPixelDistribution,
            this.miExportNTPDebugData});
			this.miData.Name = "miData";
			this.miData.Size = new System.Drawing.Size(43, 20);
			this.miData.Text = "&Data";
			// 
			// miShowZoomedAreas
			// 
			this.miShowZoomedAreas.CheckOnClick = true;
			this.miShowZoomedAreas.Name = "miShowZoomedAreas";
			this.miShowZoomedAreas.Size = new System.Drawing.Size(234, 22);
			this.miShowZoomedAreas.Text = "Show &Measured Pixel Areas";
			this.miShowZoomedAreas.Click += new System.EventHandler(this.miShowZoomedAreas_Click);
			// 
			// miShowPSFFits
			// 
			this.miShowPSFFits.CheckOnClick = true;
			this.miShowPSFFits.Name = "miShowPSFFits";
			this.miShowPSFFits.Size = new System.Drawing.Size(234, 22);
			this.miShowPSFFits.Text = "Show PSF Fits";
			this.miShowPSFFits.Click += new System.EventHandler(this.miShowPSFFits_Click);
			// 
			// miBackgroundHistograms
			// 
			this.miBackgroundHistograms.CheckOnClick = true;
			this.miBackgroundHistograms.Name = "miBackgroundHistograms";
			this.miBackgroundHistograms.Size = new System.Drawing.Size(234, 22);
			this.miBackgroundHistograms.Text = "Show &Background Histograms";
			this.miBackgroundHistograms.Click += new System.EventHandler(this.miBackgroundHistograms_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(231, 6);
			// 
			// miOutlierRemoval
			// 
			this.miOutlierRemoval.Checked = true;
			this.miOutlierRemoval.CheckOnClick = true;
			this.miOutlierRemoval.CheckState = System.Windows.Forms.CheckState.Checked;
			this.miOutlierRemoval.Name = "miOutlierRemoval";
			this.miOutlierRemoval.Size = new System.Drawing.Size(234, 22);
			this.miOutlierRemoval.Text = "Automatic Outlier Removal";
			this.miOutlierRemoval.CheckedChanged += new System.EventHandler(this.miOutlierRemoval_CheckedChanged);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(231, 6);
			// 
			// miReprocess
			// 
			this.miReprocess.Name = "miReprocess";
			this.miReprocess.Size = new System.Drawing.Size(234, 22);
			this.miReprocess.Text = "Quick &Re-Process";
			this.miReprocess.Click += new System.EventHandler(this.miReprocess_Click);
			// 
			// miFullReprocess
			// 
			this.miFullReprocess.Name = "miFullReprocess";
			this.miFullReprocess.Size = new System.Drawing.Size(234, 22);
			this.miFullReprocess.Text = "&Full Re-Process";
			this.miFullReprocess.Click += new System.EventHandler(this.miFullReprocess_Click);
			// 
			// miAdjustMeasurements
			// 
			this.miAdjustMeasurements.Name = "miAdjustMeasurements";
			this.miAdjustMeasurements.Size = new System.Drawing.Size(234, 22);
			this.miAdjustMeasurements.Text = "&Edit Measurements";
			this.miAdjustMeasurements.Click += new System.EventHandler(this.miAdjustMeasurements_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(231, 6);
			// 
			// miNoiseDistribution
			// 
			this.miNoiseDistribution.Name = "miNoiseDistribution";
			this.miNoiseDistribution.Size = new System.Drawing.Size(234, 22);
			this.miNoiseDistribution.Text = "&Reduced Data Distribution";
			this.miNoiseDistribution.Click += new System.EventHandler(this.miNoiseDistribution_Click);
			// 
			// miPixelDistribution
			// 
			this.miPixelDistribution.Name = "miPixelDistribution";
			this.miPixelDistribution.Size = new System.Drawing.Size(234, 22);
			this.miPixelDistribution.Text = "Frame &Pixels Distribution";
			this.miPixelDistribution.Click += new System.EventHandler(this.miPixelDistribution_Click);
			// 
			// miExportNTPDebugData
			// 
			this.miExportNTPDebugData.Name = "miExportNTPDebugData";
			this.miExportNTPDebugData.Size = new System.Drawing.Size(234, 22);
			this.miExportNTPDebugData.Text = "Export NTP vs OCR-ed Times";
			this.miExportNTPDebugData.Visible = false;
			this.miExportNTPDebugData.Click += new System.EventHandler(this.miExportNTPDebugData_Click);
			// 
			// processToolStripMenuItem
			// 
			this.processToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miDisplaySettings,
            this.miAddTitle});
			this.processToolStripMenuItem.Name = "processToolStripMenuItem";
			this.processToolStripMenuItem.Size = new System.Drawing.Size(75, 20);
			this.processToolStripMenuItem.Text = "&Customize";
			// 
			// miDisplaySettings
			// 
			this.miDisplaySettings.Name = "miDisplaySettings";
			this.miDisplaySettings.Size = new System.Drawing.Size(163, 22);
			this.miDisplaySettings.Text = "&Display Settings";
			this.miDisplaySettings.Click += new System.EventHandler(this.miDisplaySettings_Click);
			// 
			// miAddTitle
			// 
			this.miAddTitle.Name = "miAddTitle";
			this.miAddTitle.Size = new System.Drawing.Size(163, 22);
			this.miAddTitle.Text = "Edit Object &Titles";
			this.miAddTitle.Visible = false;
			// 
			// miAddins
			// 
			this.miAddins.Name = "miAddins";
			this.miAddins.Size = new System.Drawing.Size(61, 20);
			this.miAddins.Text = "&Add-ins";
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.DefaultExt = "lc";
			this.saveFileDialog.Filter = "Tangra Light Curves (*.lc)|*.lc";
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslblSignalType,
            this.tslblNormalisation,
            this.tslblBinning,
            this.miIncludeObject});
			this.statusStrip1.Location = new System.Drawing.Point(0, 492);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(704, 22);
			this.statusStrip1.TabIndex = 4;
			this.statusStrip1.Text = "statusStrip";
			// 
			// tslblSignalType
			// 
			this.tslblSignalType.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSignalDividedByNoise,
            this.miSignalDividedByBackground,
            this.toolStripSeparator4,
            this.miNoiseOnly,
            this.miSignalMinusNoise,
            this.miSignalOnly});
			this.tslblSignalType.Name = "tslblSignalType";
			this.tslblSignalType.Size = new System.Drawing.Size(159, 20);
			this.tslblSignalType.Text = "Signal-minus-Background";
			// 
			// miSignalDividedByNoise
			// 
			this.miSignalDividedByNoise.Name = "miSignalDividedByNoise";
			this.miSignalDividedByNoise.Size = new System.Drawing.Size(262, 22);
			this.miSignalDividedByNoise.Text = "Signal-divided by-Noise ( % )";
			this.miSignalDividedByNoise.Click += new System.EventHandler(this.SignalMenuItemChecked);
			// 
			// miSignalDividedByBackground
			// 
			this.miSignalDividedByBackground.Name = "miSignalDividedByBackground";
			this.miSignalDividedByBackground.Size = new System.Drawing.Size(262, 22);
			this.miSignalDividedByBackground.Text = "Signal-divided by-Background ( % )";
			this.miSignalDividedByBackground.Click += new System.EventHandler(this.SignalMenuItemChecked);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(259, 6);
			// 
			// miNoiseOnly
			// 
			this.miNoiseOnly.Name = "miNoiseOnly";
			this.miNoiseOnly.Size = new System.Drawing.Size(262, 22);
			this.miNoiseOnly.Text = "Background-Only";
			this.miNoiseOnly.Click += new System.EventHandler(this.SignalMenuItemChecked);
			// 
			// miSignalMinusNoise
			// 
			this.miSignalMinusNoise.Checked = true;
			this.miSignalMinusNoise.CheckState = System.Windows.Forms.CheckState.Checked;
			this.miSignalMinusNoise.Name = "miSignalMinusNoise";
			this.miSignalMinusNoise.Size = new System.Drawing.Size(262, 22);
			this.miSignalMinusNoise.Text = "Signal-minus-Background";
			this.miSignalMinusNoise.Click += new System.EventHandler(this.SignalMenuItemChecked);
			// 
			// miSignalOnly
			// 
			this.miSignalOnly.Name = "miSignalOnly";
			this.miSignalOnly.Size = new System.Drawing.Size(262, 22);
			this.miSignalOnly.Text = "Signal-Only";
			this.miSignalOnly.Click += new System.EventHandler(this.SignalMenuItemChecked);
			// 
			// tslblNormalisation
			// 
			this.tslblNormalisation.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.normalisationToolStripMenuItem,
            this.miNoNormalization});
			this.tslblNormalisation.Name = "tslblNormalisation";
			this.tslblNormalisation.Size = new System.Drawing.Size(114, 20);
			this.tslblNormalisation.Text = "No Normalisation";
			// 
			// normalisationToolStripMenuItem
			// 
			this.normalisationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miNormalisation1,
            this.miNormalisation2,
            this.miNormalisation3,
            this.miNormalisation4,
            this.miNormalizationSeparator,
            this.miNormalisationMethod});
			this.normalisationToolStripMenuItem.Name = "normalisationToolStripMenuItem";
			this.normalisationToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
			this.normalisationToolStripMenuItem.Text = "      Normalisation";
			this.normalisationToolStripMenuItem.DropDownOpening += new System.EventHandler(this.normalisationToolStripMenuItem_DropDownOpening);
			// 
			// miNormalisation1
			// 
			this.miNormalisation1.Name = "miNormalisation1";
			this.miNormalisation1.Size = new System.Drawing.Size(194, 22);
			this.miNormalisation1.Text = "Use Object 1";
			this.miNormalisation1.Click += new System.EventHandler(this.NormalizationMenuItemChecked);
			// 
			// miNormalisation2
			// 
			this.miNormalisation2.Name = "miNormalisation2";
			this.miNormalisation2.Size = new System.Drawing.Size(194, 22);
			this.miNormalisation2.Text = "Use Object 2";
			this.miNormalisation2.Click += new System.EventHandler(this.NormalizationMenuItemChecked);
			// 
			// miNormalisation3
			// 
			this.miNormalisation3.Name = "miNormalisation3";
			this.miNormalisation3.Size = new System.Drawing.Size(194, 22);
			this.miNormalisation3.Text = "Use Object 3";
			this.miNormalisation3.Click += new System.EventHandler(this.NormalizationMenuItemChecked);
			// 
			// miNormalisation4
			// 
			this.miNormalisation4.Name = "miNormalisation4";
			this.miNormalisation4.Size = new System.Drawing.Size(194, 22);
			this.miNormalisation4.Text = "Use Object 4";
			this.miNormalisation4.Click += new System.EventHandler(this.NormalizationMenuItemChecked);
			// 
			// miNormalizationSeparator
			// 
			this.miNormalizationSeparator.Name = "miNormalizationSeparator";
			this.miNormalizationSeparator.Size = new System.Drawing.Size(191, 6);
			// 
			// miNormalisationMethod
			// 
			this.miNormalisationMethod.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miNormalisation16DataPoints,
            this.miNormalisation4DataPoints,
            this.miNormalisation1DataPoints,
            this.miNormalisationLinearFit});
			this.miNormalisationMethod.Name = "miNormalisationMethod";
			this.miNormalisationMethod.Size = new System.Drawing.Size(194, 22);
			this.miNormalisationMethod.Text = "Normalisation Method";
			// 
			// miNormalisation16DataPoints
			// 
			this.miNormalisation16DataPoints.Name = "miNormalisation16DataPoints";
			this.miNormalisation16DataPoints.Size = new System.Drawing.Size(207, 22);
			this.miNormalisation16DataPoints.Text = "16 Data Points Average";
			this.miNormalisation16DataPoints.Click += new System.EventHandler(this.OnNormalisationMethodChanged);
			// 
			// miNormalisation4DataPoints
			// 
			this.miNormalisation4DataPoints.Checked = true;
			this.miNormalisation4DataPoints.CheckState = System.Windows.Forms.CheckState.Checked;
			this.miNormalisation4DataPoints.Name = "miNormalisation4DataPoints";
			this.miNormalisation4DataPoints.Size = new System.Drawing.Size(207, 22);
			this.miNormalisation4DataPoints.Text = "4 Data Points Average";
			this.miNormalisation4DataPoints.Click += new System.EventHandler(this.OnNormalisationMethodChanged);
			// 
			// miNormalisation1DataPoints
			// 
			this.miNormalisation1DataPoints.Name = "miNormalisation1DataPoints";
			this.miNormalisation1DataPoints.Size = new System.Drawing.Size(207, 22);
			this.miNormalisation1DataPoints.Text = "Data Point per Data Point";
			this.miNormalisation1DataPoints.Click += new System.EventHandler(this.OnNormalisationMethodChanged);
			// 
			// miNormalisationLinearFit
			// 
			this.miNormalisationLinearFit.Name = "miNormalisationLinearFit";
			this.miNormalisationLinearFit.Size = new System.Drawing.Size(207, 22);
			this.miNormalisationLinearFit.Text = "Linear Fit";
			this.miNormalisationLinearFit.Click += new System.EventHandler(this.OnNormalisationMethodChanged);
			// 
			// miNoNormalization
			// 
			this.miNoNormalization.Checked = true;
			this.miNoNormalization.CheckState = System.Windows.Forms.CheckState.Checked;
			this.miNoNormalization.Name = "miNoNormalization";
			this.miNoNormalization.Size = new System.Drawing.Size(168, 22);
			this.miNoNormalization.Text = "No Normalisation";
			this.miNoNormalization.Click += new System.EventHandler(this.NormalizationMenuItemChecked);
			// 
			// tslblBinning
			// 
			this.tslblBinning.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.binningToolStripMenuItem,
            this.miNoBinning});
			this.tslblBinning.Name = "tslblBinning";
			this.tslblBinning.Size = new System.Drawing.Size(80, 20);
			this.tslblBinning.Text = "No Binning";
			// 
			// binningToolStripMenuItem
			// 
			this.binningToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miBinning4,
            this.miBinning8,
            this.miBinning16,
            this.miBinning32,
            this.miBinning64,
            this.toolStripMenuItem1,
            this.miCustomBinning});
			this.binningToolStripMenuItem.Name = "binningToolStripMenuItem";
			this.binningToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
			this.binningToolStripMenuItem.Text = "      Binning";
			// 
			// miBinning4
			// 
			this.miBinning4.Name = "miBinning4";
			this.miBinning4.Size = new System.Drawing.Size(127, 22);
			this.miBinning4.Text = "  4 Frames";
			this.miBinning4.Click += new System.EventHandler(this.BinningMenuItemChecked);
			// 
			// miBinning8
			// 
			this.miBinning8.Name = "miBinning8";
			this.miBinning8.Size = new System.Drawing.Size(127, 22);
			this.miBinning8.Text = "  8 Frames";
			this.miBinning8.Click += new System.EventHandler(this.BinningMenuItemChecked);
			// 
			// miBinning16
			// 
			this.miBinning16.Name = "miBinning16";
			this.miBinning16.Size = new System.Drawing.Size(127, 22);
			this.miBinning16.Text = "16 Frames";
			this.miBinning16.Click += new System.EventHandler(this.BinningMenuItemChecked);
			// 
			// miBinning32
			// 
			this.miBinning32.Name = "miBinning32";
			this.miBinning32.Size = new System.Drawing.Size(127, 22);
			this.miBinning32.Text = "32 Frames";
			this.miBinning32.Click += new System.EventHandler(this.BinningMenuItemChecked);
			// 
			// miBinning64
			// 
			this.miBinning64.Name = "miBinning64";
			this.miBinning64.Size = new System.Drawing.Size(127, 22);
			this.miBinning64.Text = "64 Frames";
			this.miBinning64.Click += new System.EventHandler(this.BinningMenuItemChecked);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(124, 6);
			// 
			// miCustomBinning
			// 
			this.miCustomBinning.Name = "miCustomBinning";
			this.miCustomBinning.Size = new System.Drawing.Size(127, 22);
			this.miCustomBinning.Text = "Custom";
			this.miCustomBinning.Click += new System.EventHandler(this.BinningMenuItemChecked);
			// 
			// miNoBinning
			// 
			this.miNoBinning.Checked = true;
			this.miNoBinning.CheckState = System.Windows.Forms.CheckState.Checked;
			this.miNoBinning.Name = "miNoBinning";
			this.miNoBinning.Size = new System.Drawing.Size(137, 22);
			this.miNoBinning.Text = " No Binning";
			this.miNoBinning.Click += new System.EventHandler(this.BinningMenuItemChecked);
			// 
			// miIncludeObject
			// 
			this.miIncludeObject.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miIncludeObj4,
            this.miIncludeObj3,
            this.miIncludeObj2,
            this.miIncludeObj1});
			this.miIncludeObject.Image = ((System.Drawing.Image)(resources.GetObject("miIncludeObject.Image")));
			this.miIncludeObject.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.miIncludeObject.Name = "miIncludeObject";
			this.miIncludeObject.Size = new System.Drawing.Size(118, 20);
			this.miIncludeObject.Text = "Include Objects";
			// 
			// miIncludeObj4
			// 
			this.miIncludeObj4.Name = "miIncludeObj4";
			this.miIncludeObj4.Size = new System.Drawing.Size(118, 22);
			this.miIncludeObj4.Text = "Object 4";
			this.miIncludeObj4.Click += new System.EventHandler(this.HandleIncludeExcludeObject);
			// 
			// miIncludeObj3
			// 
			this.miIncludeObj3.Name = "miIncludeObj3";
			this.miIncludeObj3.Size = new System.Drawing.Size(118, 22);
			this.miIncludeObj3.Text = "Object 3";
			this.miIncludeObj3.Click += new System.EventHandler(this.HandleIncludeExcludeObject);
			// 
			// miIncludeObj2
			// 
			this.miIncludeObj2.Name = "miIncludeObj2";
			this.miIncludeObj2.Size = new System.Drawing.Size(118, 22);
			this.miIncludeObj2.Text = "Object 2";
			this.miIncludeObj2.Click += new System.EventHandler(this.HandleIncludeExcludeObject);
			// 
			// miIncludeObj1
			// 
			this.miIncludeObj1.Name = "miIncludeObj1";
			this.miIncludeObj1.Size = new System.Drawing.Size(118, 22);
			this.miIncludeObj1.Text = "Object 1";
			this.miIncludeObj1.Click += new System.EventHandler(this.HandleIncludeExcludeObject);
			// 
			// pnlMain
			// 
			this.pnlMain.Controls.Add(this.panel1);
			this.pnlMain.Controls.Add(this.pnlLegend);
			this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlMain.Location = new System.Drawing.Point(0, 24);
			this.pnlMain.Name = "pnlMain";
			this.pnlMain.Size = new System.Drawing.Size(704, 468);
			this.pnlMain.TabIndex = 5;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.pnlChart);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(704, 381);
			this.panel1.TabIndex = 3;
			// 
			// pnlChart
			// 
			this.pnlChart.BackColor = System.Drawing.Color.White;
			this.pnlChart.Controls.Add(this.btnSetMags);
			this.pnlChart.Controls.Add(this.btnZoomIn);
			this.pnlChart.Controls.Add(this.btnZoomOut);
			this.pnlChart.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlChart.Location = new System.Drawing.Point(0, 0);
			this.pnlChart.Name = "pnlChart";
			this.pnlChart.Size = new System.Drawing.Size(704, 381);
			this.pnlChart.TabIndex = 1;
			this.pnlChart.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pnlChart_MouseClick);
			this.pnlChart.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.pnlChart_PreviewKeyDown);
			// 
			// btnSetMags
			// 
			this.btnSetMags.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSetMags.BackColor = System.Drawing.SystemColors.Control;
			this.btnSetMags.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnSetMags.ImageIndex = 2;
			this.btnSetMags.ImageList = this.imageList1;
			this.btnSetMags.Location = new System.Drawing.Point(683, 25);
			this.btnSetMags.Name = "btnSetMags";
			this.btnSetMags.Size = new System.Drawing.Size(16, 16);
			this.btnSetMags.TabIndex = 2;
			this.btnSetMags.TabStop = false;
			this.toolTip1.SetToolTip(this.btnSetMags, "Set reference magnitude");
			this.btnSetMags.UseVisualStyleBackColor = false;
			this.btnSetMags.Click += new System.EventHandler(this.btnSetMags_Click);
			// 
			// btnZoomIn
			// 
			this.btnZoomIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnZoomIn.BackColor = System.Drawing.SystemColors.Control;
			this.btnZoomIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnZoomIn.ImageIndex = 1;
			this.btnZoomIn.ImageList = this.imageList1;
			this.btnZoomIn.Location = new System.Drawing.Point(647, 5);
			this.btnZoomIn.Name = "btnZoomIn";
			this.btnZoomIn.Size = new System.Drawing.Size(16, 16);
			this.btnZoomIn.TabIndex = 1;
			this.btnZoomIn.TabStop = false;
			this.btnZoomIn.UseVisualStyleBackColor = false;
			this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
			// 
			// btnZoomOut
			// 
			this.btnZoomOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnZoomOut.BackColor = System.Drawing.SystemColors.Control;
			this.btnZoomOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnZoomOut.ImageKey = "minus15x15.bmp";
			this.btnZoomOut.ImageList = this.imageList1;
			this.btnZoomOut.Location = new System.Drawing.Point(665, 5);
			this.btnZoomOut.Name = "btnZoomOut";
			this.btnZoomOut.Size = new System.Drawing.Size(16, 16);
			this.btnZoomOut.TabIndex = 0;
			this.btnZoomOut.TabStop = false;
			this.btnZoomOut.UseVisualStyleBackColor = false;
			this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
			// 
			// hintTimer
			// 
			this.hintTimer.Interval = 3000;
			// 
			// saveImageDialog
			// 
			this.saveImageDialog.DefaultExt = "png";
			this.saveImageDialog.Filter = "Bitmap (*.bmp)|*.bmp|JPEG Image (*.jpg)|*.jpg|PNG Image (*.png)|*.png";
			// 
			// saveCSVDialog
			// 
			this.saveCSVDialog.DefaultExt = "csv";
			this.saveCSVDialog.Filter = "Comma Separated Values (*.csv)|*.csv";
			// 
			// firstFrameTimer
			// 
			this.firstFrameTimer.Interval = 250;
			this.firstFrameTimer.Tick += new System.EventHandler(this.firstFrameTimer_Tick);
			// 
			// lblMagnitude1
			// 
			this.lblMagnitude1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblMagnitude1.AutoSize = true;
			this.lblMagnitude1.ForeColor = System.Drawing.Color.Fuchsia;
			this.lblMagnitude1.Location = new System.Drawing.Point(416, -1);
			this.lblMagnitude1.Name = "lblMagnitude1";
			this.lblMagnitude1.Size = new System.Drawing.Size(0, 13);
			this.lblMagnitude1.TabIndex = 40;
			// 
			// lblMagnitude2
			// 
			this.lblMagnitude2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblMagnitude2.AutoSize = true;
			this.lblMagnitude2.ForeColor = System.Drawing.Color.Fuchsia;
			this.lblMagnitude2.Location = new System.Drawing.Point(501, -1);
			this.lblMagnitude2.Name = "lblMagnitude2";
			this.lblMagnitude2.Size = new System.Drawing.Size(0, 13);
			this.lblMagnitude2.TabIndex = 41;
			// 
			// lblMagnitude3
			// 
			this.lblMagnitude3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblMagnitude3.AutoSize = true;
			this.lblMagnitude3.ForeColor = System.Drawing.Color.Fuchsia;
			this.lblMagnitude3.Location = new System.Drawing.Point(588, -1);
			this.lblMagnitude3.Name = "lblMagnitude3";
			this.lblMagnitude3.Size = new System.Drawing.Size(0, 13);
			this.lblMagnitude3.TabIndex = 42;
			// 
			// lblMagnitude4
			// 
			this.lblMagnitude4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblMagnitude4.AutoSize = true;
			this.lblMagnitude4.ForeColor = System.Drawing.Color.Fuchsia;
			this.lblMagnitude4.Location = new System.Drawing.Point(672, -1);
			this.lblMagnitude4.Name = "lblMagnitude4";
			this.lblMagnitude4.Size = new System.Drawing.Size(0, 13);
			this.lblMagnitude4.TabIndex = 43;
			// 
			// frmLightCurve
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(704, 514);
			this.Controls.Add(this.pnlMain);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.menuStrip1);
			this.DoubleBuffered = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(534, 396);
			this.Name = "frmLightCurve";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Light Curves";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmLightCurve_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmLightCurve_FormClosed);
			this.Load += new System.EventHandler(this.frmLightCurve_Load);
			this.Move += new System.EventHandler(this.frmLightCurve_Move);
			this.Resize += new System.EventHandler(this.frmLightCurve_Resize);
			this.pnlLegend.ResumeLayout(false);
			this.pnlSmallGraph.ResumeLayout(false);
			this.pnlMeasurementDetails.ResumeLayout(false);
			this.pnlMeasurementDetails.PerformLayout();
			this.pnlGeoLocation.ResumeLayout(false);
			this.pnlGeoLocation.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.picTarget4PSF)).EndInit();
			this.pnlBinInfo.ResumeLayout(false);
			this.pnlBinInfo.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pb4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget4Pixels)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pb3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget3PSF)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget1Pixels)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pb2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget3Pixels)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget1PSF)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget2Pixels)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget2PSF)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pb1)).EndInit();
			this.panel2.ResumeLayout(false);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.pnlMain.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.pnlChart.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlLegend;
        private Tangra.Controls.SmoothPanel pnlChart;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem lightCurveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miSave;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripMenuItem processToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miLoad;
        private System.Windows.Forms.Label lblFrameNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblMeasurement1;
        private System.Windows.Forms.ToolStripMenuItem exportToCSVToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miExportTangraCSV;
        private System.Windows.Forms.PictureBox pb1;
        private System.Windows.Forms.PictureBox pb4;
        private System.Windows.Forms.Label lblMeasurement4;
        private System.Windows.Forms.PictureBox pb3;
        private System.Windows.Forms.Label lblMeasurement3;
        private System.Windows.Forms.PictureBox pb2;
        private System.Windows.Forms.Label lblMeasurement2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblFrameTime;
		private System.Windows.Forms.PictureBox picTarget1Pixels;
		private System.Windows.Forms.PictureBox picTarget1PSF;
		private System.Windows.Forms.PictureBox picTarget4PSF;
		private System.Windows.Forms.PictureBox picTarget4Pixels;
		private System.Windows.Forms.PictureBox picTarget3PSF;
		private System.Windows.Forms.PictureBox picTarget3Pixels;
		private System.Windows.Forms.PictureBox picTarget2PSF;
        private System.Windows.Forms.PictureBox picTarget2Pixels;
        private System.Windows.Forms.ToolStripMenuItem miSaveAsImage;
		private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel pnlMain;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ToolStripDropDownButton tslblSignalType;
		private System.Windows.Forms.ToolStripMenuItem miSignalMinusNoise;
		private System.Windows.Forms.ToolStripMenuItem miSignalOnly;
		private System.Windows.Forms.ToolStripDropDownButton tslblBinning;
		private System.Windows.Forms.ToolStripMenuItem binningToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem miBinning4;
		private System.Windows.Forms.ToolStripMenuItem miBinning8;
		private System.Windows.Forms.ToolStripMenuItem miBinning16;
		private System.Windows.Forms.ToolStripMenuItem miBinning32;
        private System.Windows.Forms.ToolStripMenuItem miNoBinning;
		private System.Windows.Forms.ToolStripDropDownButton tslblNormalisation;
		private System.Windows.Forms.ToolStripMenuItem normalisationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem miNormalisation1;
		private System.Windows.Forms.ToolStripMenuItem miNormalisation2;
		private System.Windows.Forms.ToolStripMenuItem miNormalisation3;
		private System.Windows.Forms.ToolStripMenuItem miNormalisation4;
		private System.Windows.Forms.ToolStripMenuItem miNoNormalization;
        private System.Windows.Forms.Panel pnlBinInfo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblBinNo;
        private System.Windows.Forms.ToolStripMenuItem miAddTitle;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem miCopyToClipboard;
        private System.Windows.Forms.ToolStripMenuItem miNoiseOnly;
        private System.Windows.Forms.ToolStripMenuItem miData;
        private System.Windows.Forms.Timer hintTimer;
		private System.Windows.Forms.ToolStripDropDownButton miIncludeObject;
		private System.Windows.Forms.ToolStripMenuItem miIncludeObj4;
		private System.Windows.Forms.ToolStripMenuItem miIncludeObj3;
		private System.Windows.Forms.ToolStripMenuItem miIncludeObj2;
		private System.Windows.Forms.ToolStripMenuItem miIncludeObj1;
        private System.Windows.Forms.SaveFileDialog saveImageDialog;
		private System.Windows.Forms.SaveFileDialog saveCSVDialog;
		private System.Windows.Forms.ToolStripSeparator miNormalizationSeparator;
		private System.Windows.Forms.Button btnZoomIn;
		private System.Windows.Forms.Button btnZoomOut;
		private System.Windows.Forms.ImageList imageList1;
        private Tangra.Controls.SmoothPanel pnlSmallGraph;
		private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Timer firstFrameTimer;
        private System.Windows.Forms.Panel pnlMeasurementDetails;
        private System.Windows.Forms.ToolStripMenuItem miBinning64;
        private System.Windows.Forms.ToolStripMenuItem miReprocess;
        private System.Windows.Forms.ToolStripMenuItem miNoiseDistribution;
        private System.Windows.Forms.ToolStripMenuItem miPixelDistribution;
        private System.Windows.Forms.ToolStripMenuItem miDisplaySettings;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem miShowZoomedAreas;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem miCustomBinning;
		private System.Windows.Forms.ToolStripMenuItem miFullReprocess;
		private System.Windows.Forms.ToolStripMenuItem miShowPSFFits;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem miSignalDividedByBackground;
		private System.Windows.Forms.ToolStripMenuItem miSignalDividedByNoise;
		private System.Windows.Forms.Label lblSN1;
		private System.Windows.Forms.Label lblSNLBL4;
		private System.Windows.Forms.Label lblSNLBL3;
		private System.Windows.Forms.Label lblSNLBL2;
		private System.Windows.Forms.Label lblSNLBL1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.Label lblSN4;
		private System.Windows.Forms.Label lblSN3;
		private System.Windows.Forms.Label lblSN2;
		private System.Windows.Forms.ToolStripMenuItem miBackgroundHistograms;
        private System.Windows.Forms.ToolStripMenuItem miNormalisationMethod;
        private System.Windows.Forms.ToolStripMenuItem miNormalisation16DataPoints;
        private System.Windows.Forms.ToolStripMenuItem miNormalisation4DataPoints;
        private System.Windows.Forms.ToolStripMenuItem miNormalisation1DataPoints;
        private System.Windows.Forms.ToolStripMenuItem miNormalisationLinearFit;
		private System.Windows.Forms.ToolStripMenuItem miAddins;
        private System.Windows.Forms.HScrollBar sbZoomStartFrame;
        private System.Windows.Forms.ToolStripMenuItem miAdjustMeasurements;
		private System.Windows.Forms.Panel pnlGeoLocation;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox tbxGeoLocation;
		private System.Windows.Forms.Label lblInstDelayWarning;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.ToolStripMenuItem miExportNTPDebugData;
		private System.Windows.Forms.ToolStripMenuItem miOutlierRemoval;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.Button btnSetMags;
		private System.Windows.Forms.Label lblMagnitude1;
		private System.Windows.Forms.Label lblMagnitude2;
		private System.Windows.Forms.Label lblMagnitude3;
		private System.Windows.Forms.Label lblMagnitude4;
    }
}
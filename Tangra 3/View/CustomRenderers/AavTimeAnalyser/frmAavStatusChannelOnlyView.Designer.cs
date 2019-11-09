namespace Tangra.View.CustomRenderers.AavTimeAnalyser
{
    partial class frmAavStatusChannelOnlyView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAavStatusChannelOnlyView));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabOverview = new System.Windows.Forms.TabPage();
            this.pbLoadData = new System.Windows.Forms.ProgressBar();
            this.tbxAnalysisDetails = new System.Windows.Forms.TextBox();
            this.tabGraphs = new System.Windows.Forms.TabPage();
            this.pbGraph = new System.Windows.Forms.PictureBox();
            this.pnlGraph = new System.Windows.Forms.Panel();
            this.pnlTimeDeltaConfig = new System.Windows.Forms.Panel();
            this.rbSystemTime = new System.Windows.Forms.RadioButton();
            this.rbSystemTimeAsFileTime = new System.Windows.Forms.RadioButton();
            this.cbxNtpTime = new System.Windows.Forms.CheckBox();
            this.cbxNtpError = new System.Windows.Forms.CheckBox();
            this.cbxGraphType = new System.Windows.Forms.ComboBox();
            this.pnlTimeMedianConfig = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.nudMedianInterval = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.tabOCRErrors = new System.Windows.Forms.TabPage();
            this.pbOcrErrorFrame = new System.Windows.Forms.PictureBox();
            this.pnlOcrErrorControl = new System.Windows.Forms.Panel();
            this.lblOcrText = new System.Windows.Forms.Label();
            this.nudOcrErrorFrame = new System.Windows.Forms.NumericUpDown();
            this.resizeUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.graphsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miSubset = new System.Windows.Forms.ToolStripMenuItem();
            this.gridlinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miCompleteGridlines = new System.Windows.Forms.ToolStripMenuItem();
            this.miTickGridlines = new System.Windows.Forms.ToolStripMenuItem();
            this.miExport = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.tabControl.SuspendLayout();
            this.tabOverview.SuspendLayout();
            this.tabGraphs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbGraph)).BeginInit();
            this.pnlGraph.SuspendLayout();
            this.pnlTimeDeltaConfig.SuspendLayout();
            this.pnlTimeMedianConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMedianInterval)).BeginInit();
            this.tabOCRErrors.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbOcrErrorFrame)).BeginInit();
            this.pnlOcrErrorControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudOcrErrorFrame)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabOverview);
            this.tabControl.Controls.Add(this.tabGraphs);
            this.tabControl.Controls.Add(this.tabOCRErrors);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 24);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(815, 550);
            this.tabControl.TabIndex = 0;
            // 
            // tabOverview
            // 
            this.tabOverview.Controls.Add(this.pbLoadData);
            this.tabOverview.Controls.Add(this.tbxAnalysisDetails);
            this.tabOverview.Location = new System.Drawing.Point(4, 22);
            this.tabOverview.Name = "tabOverview";
            this.tabOverview.Padding = new System.Windows.Forms.Padding(3);
            this.tabOverview.Size = new System.Drawing.Size(807, 524);
            this.tabOverview.TabIndex = 0;
            this.tabOverview.Text = "Overview";
            this.tabOverview.UseVisualStyleBackColor = true;
            // 
            // pbLoadData
            // 
            this.pbLoadData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbLoadData.Location = new System.Drawing.Point(17, 17);
            this.pbLoadData.Name = "pbLoadData";
            this.pbLoadData.Size = new System.Drawing.Size(773, 23);
            this.pbLoadData.TabIndex = 0;
            // 
            // tbxAnalysisDetails
            // 
            this.tbxAnalysisDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxAnalysisDetails.BackColor = System.Drawing.SystemColors.Info;
            this.tbxAnalysisDetails.Location = new System.Drawing.Point(17, 17);
            this.tbxAnalysisDetails.Multiline = true;
            this.tbxAnalysisDetails.Name = "tbxAnalysisDetails";
            this.tbxAnalysisDetails.ReadOnly = true;
            this.tbxAnalysisDetails.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbxAnalysisDetails.Size = new System.Drawing.Size(773, 485);
            this.tbxAnalysisDetails.TabIndex = 1;
            this.tbxAnalysisDetails.Visible = false;
            // 
            // tabGraphs
            // 
            this.tabGraphs.Controls.Add(this.pbGraph);
            this.tabGraphs.Controls.Add(this.pnlGraph);
            this.tabGraphs.Location = new System.Drawing.Point(4, 22);
            this.tabGraphs.Name = "tabGraphs";
            this.tabGraphs.Padding = new System.Windows.Forms.Padding(3);
            this.tabGraphs.Size = new System.Drawing.Size(807, 524);
            this.tabGraphs.TabIndex = 1;
            this.tabGraphs.Text = "Graphs";
            this.tabGraphs.UseVisualStyleBackColor = true;
            // 
            // pbGraph
            // 
            this.pbGraph.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbGraph.Location = new System.Drawing.Point(3, 37);
            this.pbGraph.Name = "pbGraph";
            this.pbGraph.Size = new System.Drawing.Size(801, 484);
            this.pbGraph.TabIndex = 0;
            this.pbGraph.TabStop = false;
            // 
            // pnlGraph
            // 
            this.pnlGraph.Controls.Add(this.pnlTimeDeltaConfig);
            this.pnlGraph.Controls.Add(this.cbxGraphType);
            this.pnlGraph.Controls.Add(this.pnlTimeMedianConfig);
            this.pnlGraph.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlGraph.Location = new System.Drawing.Point(3, 3);
            this.pnlGraph.Name = "pnlGraph";
            this.pnlGraph.Size = new System.Drawing.Size(801, 34);
            this.pnlGraph.TabIndex = 0;
            // 
            // pnlTimeDeltaConfig
            // 
            this.pnlTimeDeltaConfig.Controls.Add(this.rbSystemTime);
            this.pnlTimeDeltaConfig.Controls.Add(this.rbSystemTimeAsFileTime);
            this.pnlTimeDeltaConfig.Controls.Add(this.cbxNtpTime);
            this.pnlTimeDeltaConfig.Controls.Add(this.cbxNtpError);
            this.pnlTimeDeltaConfig.Location = new System.Drawing.Point(171, 0);
            this.pnlTimeDeltaConfig.Name = "pnlTimeDeltaConfig";
            this.pnlTimeDeltaConfig.Size = new System.Drawing.Size(606, 33);
            this.pnlTimeDeltaConfig.TabIndex = 1;
            // 
            // rbSystemTime
            // 
            this.rbSystemTime.AutoSize = true;
            this.rbSystemTime.Location = new System.Drawing.Point(177, 8);
            this.rbSystemTime.Name = "rbSystemTime";
            this.rbSystemTime.Size = new System.Drawing.Size(82, 17);
            this.rbSystemTime.TabIndex = 3;
            this.rbSystemTime.Text = "SystemTime";
            this.rbSystemTime.UseVisualStyleBackColor = true;
            // 
            // rbSystemTimeAsFileTime
            // 
            this.rbSystemTimeAsFileTime.AutoSize = true;
            this.rbSystemTimeAsFileTime.Checked = true;
            this.rbSystemTimeAsFileTime.Location = new System.Drawing.Point(3, 8);
            this.rbSystemTimeAsFileTime.Name = "rbSystemTimeAsFileTime";
            this.rbSystemTimeAsFileTime.Size = new System.Drawing.Size(168, 17);
            this.rbSystemTimeAsFileTime.TabIndex = 2;
            this.rbSystemTimeAsFileTime.TabStop = true;
            this.rbSystemTimeAsFileTime.Text = "SystemTimePreciseAsFileTime";
            this.rbSystemTimeAsFileTime.UseVisualStyleBackColor = true;
            this.rbSystemTimeAsFileTime.CheckedChanged += new System.EventHandler(this.TimeDeltasTimeSourceChanged);
            // 
            // cbxNtpTime
            // 
            this.cbxNtpTime.AutoSize = true;
            this.cbxNtpTime.Checked = true;
            this.cbxNtpTime.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxNtpTime.Location = new System.Drawing.Point(338, 9);
            this.cbxNtpTime.Name = "cbxNtpTime";
            this.cbxNtpTime.Size = new System.Drawing.Size(158, 17);
            this.cbxNtpTime.TabIndex = 1;
            this.cbxNtpTime.Text = "OccuRec\'s Reference Time";
            this.cbxNtpTime.UseVisualStyleBackColor = true;
            this.cbxNtpTime.CheckedChanged += new System.EventHandler(this.TimeDeltasTimeSourceChanged);
            // 
            // cbxNtpError
            // 
            this.cbxNtpError.AutoSize = true;
            this.cbxNtpError.Checked = true;
            this.cbxNtpError.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxNtpError.Location = new System.Drawing.Point(512, 9);
            this.cbxNtpError.Name = "cbxNtpError";
            this.cbxNtpError.Size = new System.Drawing.Size(73, 17);
            this.cbxNtpError.TabIndex = 0;
            this.cbxNtpError.Text = "NTP Error";
            this.cbxNtpError.UseVisualStyleBackColor = true;
            this.cbxNtpError.CheckedChanged += new System.EventHandler(this.TimeDeltasTimeSourceChanged);
            // 
            // cbxGraphType
            // 
            this.cbxGraphType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxGraphType.FormattingEnabled = true;
            this.cbxGraphType.Items.AddRange(new object[] {
            "Time Deltas - Lines",
            "Time Deltas - Dots",
            "System Utilisation",
            "NTP Updates",
            "NTP Updates & Unapplied",
            "Zoomed Time Deltas"});
            this.cbxGraphType.Location = new System.Drawing.Point(6, 7);
            this.cbxGraphType.Name = "cbxGraphType";
            this.cbxGraphType.Size = new System.Drawing.Size(159, 21);
            this.cbxGraphType.TabIndex = 0;
            this.cbxGraphType.SelectedIndexChanged += new System.EventHandler(this.cbxGraphType_SelectedIndexChanged);
            // 
            // pnlTimeMedianConfig
            // 
            this.pnlTimeMedianConfig.Controls.Add(this.label2);
            this.pnlTimeMedianConfig.Controls.Add(this.nudMedianInterval);
            this.pnlTimeMedianConfig.Controls.Add(this.label1);
            this.pnlTimeMedianConfig.Location = new System.Drawing.Point(171, 0);
            this.pnlTimeMedianConfig.Name = "pnlTimeMedianConfig";
            this.pnlTimeMedianConfig.Size = new System.Drawing.Size(625, 33);
            this.pnlTimeMedianConfig.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(150, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "min";
            // 
            // nudMedianInterval
            // 
            this.nudMedianInterval.Location = new System.Drawing.Point(103, 7);
            this.nudMedianInterval.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudMedianInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudMedianInterval.Name = "nudMedianInterval";
            this.nudMedianInterval.Size = new System.Drawing.Size(41, 20);
            this.nudMedianInterval.TabIndex = 1;
            this.nudMedianInterval.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudMedianInterval.ValueChanged += new System.EventHandler(this.TimeDeltasTimeSourceChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Median Interval:";
            // 
            // tabOCRErrors
            // 
            this.tabOCRErrors.Controls.Add(this.pbOcrErrorFrame);
            this.tabOCRErrors.Controls.Add(this.pnlOcrErrorControl);
            this.tabOCRErrors.Location = new System.Drawing.Point(4, 22);
            this.tabOCRErrors.Name = "tabOCRErrors";
            this.tabOCRErrors.Size = new System.Drawing.Size(807, 524);
            this.tabOCRErrors.TabIndex = 2;
            this.tabOCRErrors.Text = "OCR Errors";
            this.tabOCRErrors.UseVisualStyleBackColor = true;
            // 
            // pbOcrErrorFrame
            // 
            this.pbOcrErrorFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbOcrErrorFrame.Location = new System.Drawing.Point(0, 41);
            this.pbOcrErrorFrame.Name = "pbOcrErrorFrame";
            this.pbOcrErrorFrame.Size = new System.Drawing.Size(807, 483);
            this.pbOcrErrorFrame.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbOcrErrorFrame.TabIndex = 1;
            this.pbOcrErrorFrame.TabStop = false;
            // 
            // pnlOcrErrorControl
            // 
            this.pnlOcrErrorControl.Controls.Add(this.lblOcrText);
            this.pnlOcrErrorControl.Controls.Add(this.nudOcrErrorFrame);
            this.pnlOcrErrorControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlOcrErrorControl.Location = new System.Drawing.Point(0, 0);
            this.pnlOcrErrorControl.Name = "pnlOcrErrorControl";
            this.pnlOcrErrorControl.Size = new System.Drawing.Size(807, 41);
            this.pnlOcrErrorControl.TabIndex = 0;
            // 
            // lblOcrText
            // 
            this.lblOcrText.AutoSize = true;
            this.lblOcrText.Location = new System.Drawing.Point(114, 17);
            this.lblOcrText.Name = "lblOcrText";
            this.lblOcrText.Size = new System.Drawing.Size(0, 13);
            this.lblOcrText.TabIndex = 1;
            // 
            // nudOcrErrorFrame
            // 
            this.nudOcrErrorFrame.Location = new System.Drawing.Point(10, 11);
            this.nudOcrErrorFrame.Name = "nudOcrErrorFrame";
            this.nudOcrErrorFrame.Size = new System.Drawing.Size(70, 20);
            this.nudOcrErrorFrame.TabIndex = 0;
            this.nudOcrErrorFrame.ValueChanged += new System.EventHandler(this.nudOcrErrorFrame_ValueChanged);
            // 
            // resizeUpdateTimer
            // 
            this.resizeUpdateTimer.Interval = 250;
            this.resizeUpdateTimer.Tick += new System.EventHandler(this.resizeUpdateTimer_Tick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.graphsToolStripMenuItem,
            this.miExport});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(815, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // graphsToolStripMenuItem
            // 
            this.graphsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSubset,
            this.gridlinesToolStripMenuItem});
            this.graphsToolStripMenuItem.Name = "graphsToolStripMenuItem";
            this.graphsToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.graphsToolStripMenuItem.Text = "&Graphs";
            // 
            // miSubset
            // 
            this.miSubset.Name = "miSubset";
            this.miSubset.Size = new System.Drawing.Size(120, 22);
            this.miSubset.Text = "&Subset";
            this.miSubset.Click += new System.EventHandler(this.miSubset_Click);
            // 
            // gridlinesToolStripMenuItem
            // 
            this.gridlinesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miCompleteGridlines,
            this.miTickGridlines});
            this.gridlinesToolStripMenuItem.Name = "gridlinesToolStripMenuItem";
            this.gridlinesToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.gridlinesToolStripMenuItem.Text = "&Gridlines";
            // 
            // miCompleteGridlines
            // 
            this.miCompleteGridlines.Checked = true;
            this.miCompleteGridlines.CheckOnClick = true;
            this.miCompleteGridlines.CheckState = System.Windows.Forms.CheckState.Checked;
            this.miCompleteGridlines.Name = "miCompleteGridlines";
            this.miCompleteGridlines.Size = new System.Drawing.Size(126, 22);
            this.miCompleteGridlines.Text = "&Complete";
            this.miCompleteGridlines.Click += new System.EventHandler(this.GridlinesStyleChanged);
            // 
            // miTickGridlines
            // 
            this.miTickGridlines.CheckOnClick = true;
            this.miTickGridlines.Name = "miTickGridlines";
            this.miTickGridlines.Size = new System.Drawing.Size(126, 22);
            this.miTickGridlines.Text = "&Ticks";
            this.miTickGridlines.Click += new System.EventHandler(this.GridlinesStyleChanged);
            // 
            // miExport
            // 
            this.miExport.Name = "miExport";
            this.miExport.Size = new System.Drawing.Size(53, 20);
            this.miExport.Text = "&Export";
            this.miExport.Click += new System.EventHandler(this.miExport_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "cvs";
            // 
            // frmAavStatusChannelOnlyView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 574);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAavStatusChannelOnlyView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.frmAavStatusChannelOnlyView_Load);
            this.ResizeEnd += new System.EventHandler(this.frmAavStatusChannelOnlyView_ResizeEnd);
            this.tabControl.ResumeLayout(false);
            this.tabOverview.ResumeLayout(false);
            this.tabOverview.PerformLayout();
            this.tabGraphs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbGraph)).EndInit();
            this.pnlGraph.ResumeLayout(false);
            this.pnlTimeDeltaConfig.ResumeLayout(false);
            this.pnlTimeDeltaConfig.PerformLayout();
            this.pnlTimeMedianConfig.ResumeLayout(false);
            this.pnlTimeMedianConfig.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMedianInterval)).EndInit();
            this.tabOCRErrors.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbOcrErrorFrame)).EndInit();
            this.pnlOcrErrorControl.ResumeLayout(false);
            this.pnlOcrErrorControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudOcrErrorFrame)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabOverview;
        private System.Windows.Forms.TabPage tabGraphs;
        private System.Windows.Forms.TabPage tabOCRErrors;
        private System.Windows.Forms.ProgressBar pbLoadData;
        private System.Windows.Forms.Panel pnlGraph;
        private System.Windows.Forms.PictureBox pbGraph;
        private System.Windows.Forms.Timer resizeUpdateTimer;
        private System.Windows.Forms.ComboBox cbxGraphType;
        private System.Windows.Forms.Panel pnlTimeDeltaConfig;
        private System.Windows.Forms.RadioButton rbSystemTime;
        private System.Windows.Forms.RadioButton rbSystemTimeAsFileTime;
        private System.Windows.Forms.CheckBox cbxNtpTime;
        private System.Windows.Forms.CheckBox cbxNtpError;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem graphsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gridlinesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miCompleteGridlines;
        private System.Windows.Forms.ToolStripMenuItem miTickGridlines;
        private System.Windows.Forms.Panel pnlOcrErrorControl;
        private System.Windows.Forms.Label lblOcrText;
        private System.Windows.Forms.NumericUpDown nudOcrErrorFrame;
        private System.Windows.Forms.PictureBox pbOcrErrorFrame;
        private System.Windows.Forms.ToolStripMenuItem miExport;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripMenuItem miSubset;
        private System.Windows.Forms.TextBox tbxAnalysisDetails;
        private System.Windows.Forms.Panel pnlTimeMedianConfig;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudMedianInterval;
        private System.Windows.Forms.Label label1;
    }
}
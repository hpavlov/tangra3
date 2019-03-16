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
            this.tabGraph = new System.Windows.Forms.TabPage();
            this.pnlGraph = new System.Windows.Forms.Panel();
            this.pbGraph = new System.Windows.Forms.PictureBox();
            this.tabOCRErrors = new System.Windows.Forms.TabPage();
            this.tabExport = new System.Windows.Forms.TabPage();
            this.resizeUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.cbxGraphType = new System.Windows.Forms.ComboBox();
            this.pnlTimeDeltaConfig = new System.Windows.Forms.Panel();
            this.cbxNtpError = new System.Windows.Forms.CheckBox();
            this.cbxNtpTime = new System.Windows.Forms.CheckBox();
            this.rbSystemTimeAsFileTime = new System.Windows.Forms.RadioButton();
            this.rbSystemTime = new System.Windows.Forms.RadioButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.graphsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridlinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miCompleteGridlines = new System.Windows.Forms.ToolStripMenuItem();
            this.miTickGridlines = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl.SuspendLayout();
            this.tabOverview.SuspendLayout();
            this.tabGraph.SuspendLayout();
            this.pnlGraph.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbGraph)).BeginInit();
            this.pnlTimeDeltaConfig.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabOverview);
            this.tabControl.Controls.Add(this.tabGraph);
            this.tabControl.Controls.Add(this.tabOCRErrors);
            this.tabControl.Controls.Add(this.tabExport);
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
            this.tabOverview.Location = new System.Drawing.Point(4, 22);
            this.tabOverview.Name = "tabOverview";
            this.tabOverview.Padding = new System.Windows.Forms.Padding(3);
            this.tabOverview.Size = new System.Drawing.Size(749, 498);
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
            this.pbLoadData.Size = new System.Drawing.Size(715, 23);
            this.pbLoadData.TabIndex = 0;
            // 
            // tabGraph
            // 
            this.tabGraph.Controls.Add(this.pbGraph);
            this.tabGraph.Controls.Add(this.pnlGraph);
            this.tabGraph.Location = new System.Drawing.Point(4, 22);
            this.tabGraph.Name = "tabGraph";
            this.tabGraph.Padding = new System.Windows.Forms.Padding(3);
            this.tabGraph.Size = new System.Drawing.Size(807, 524);
            this.tabGraph.TabIndex = 1;
            this.tabGraph.Text = "Graph";
            this.tabGraph.UseVisualStyleBackColor = true;
            // 
            // pnlGraph
            // 
            this.pnlGraph.Controls.Add(this.pnlTimeDeltaConfig);
            this.pnlGraph.Controls.Add(this.cbxGraphType);
            this.pnlGraph.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlGraph.Location = new System.Drawing.Point(3, 3);
            this.pnlGraph.Name = "pnlGraph";
            this.pnlGraph.Size = new System.Drawing.Size(801, 34);
            this.pnlGraph.TabIndex = 0;
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
            // tabOCRErrors
            // 
            this.tabOCRErrors.Location = new System.Drawing.Point(4, 22);
            this.tabOCRErrors.Name = "tabOCRErrors";
            this.tabOCRErrors.Size = new System.Drawing.Size(749, 498);
            this.tabOCRErrors.TabIndex = 2;
            this.tabOCRErrors.Text = "OCR Errors";
            this.tabOCRErrors.UseVisualStyleBackColor = true;
            // 
            // tabExport
            // 
            this.tabExport.Location = new System.Drawing.Point(4, 22);
            this.tabExport.Name = "tabExport";
            this.tabExport.Size = new System.Drawing.Size(749, 498);
            this.tabExport.TabIndex = 3;
            this.tabExport.Text = "Export";
            this.tabExport.UseVisualStyleBackColor = true;
            // 
            // resizeUpdateTimer
            // 
            this.resizeUpdateTimer.Interval = 250;
            this.resizeUpdateTimer.Tick += new System.EventHandler(this.resizeUpdateTimer_Tick);
            // 
            // cbxGraphType
            // 
            this.cbxGraphType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxGraphType.FormattingEnabled = true;
            this.cbxGraphType.Items.AddRange(new object[] {
            "Time Deltas - Lines",
            "Time Deltas - Dots"});
            this.cbxGraphType.Location = new System.Drawing.Point(6, 7);
            this.cbxGraphType.Name = "cbxGraphType";
            this.cbxGraphType.Size = new System.Drawing.Size(159, 21);
            this.cbxGraphType.TabIndex = 0;
            this.cbxGraphType.SelectedIndexChanged += new System.EventHandler(this.cbxGraphType_SelectedIndexChanged);
            // 
            // pnlTimeDeltaConfig
            // 
            this.pnlTimeDeltaConfig.Controls.Add(this.rbSystemTime);
            this.pnlTimeDeltaConfig.Controls.Add(this.rbSystemTimeAsFileTime);
            this.pnlTimeDeltaConfig.Controls.Add(this.cbxNtpTime);
            this.pnlTimeDeltaConfig.Controls.Add(this.cbxNtpError);
            this.pnlTimeDeltaConfig.Location = new System.Drawing.Point(171, 0);
            this.pnlTimeDeltaConfig.Name = "pnlTimeDeltaConfig";
            this.pnlTimeDeltaConfig.Size = new System.Drawing.Size(625, 33);
            this.pnlTimeDeltaConfig.TabIndex = 1;
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
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.graphsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(815, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // graphsToolStripMenuItem
            // 
            this.graphsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gridlinesToolStripMenuItem});
            this.graphsToolStripMenuItem.Name = "graphsToolStripMenuItem";
            this.graphsToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.graphsToolStripMenuItem.Text = "&Graphs";
            // 
            // gridlinesToolStripMenuItem
            // 
            this.gridlinesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miCompleteGridlines,
            this.miTickGridlines});
            this.gridlinesToolStripMenuItem.Name = "gridlinesToolStripMenuItem";
            this.gridlinesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.gridlinesToolStripMenuItem.Text = "&Gridlines";
            // 
            // miCompleteGridlines
            // 
            this.miCompleteGridlines.Checked = true;
            this.miCompleteGridlines.CheckOnClick = true;
            this.miCompleteGridlines.CheckState = System.Windows.Forms.CheckState.Checked;
            this.miCompleteGridlines.Name = "miCompleteGridlines";
            this.miCompleteGridlines.Size = new System.Drawing.Size(152, 22);
            this.miCompleteGridlines.Text = "&Complete";
            this.miCompleteGridlines.Click += new System.EventHandler(this.GridlinesStyleChanged);
            // 
            // miTickGridlines
            // 
            this.miTickGridlines.CheckOnClick = true;
            this.miTickGridlines.Name = "miTickGridlines";
            this.miTickGridlines.Size = new System.Drawing.Size(152, 22);
            this.miTickGridlines.Text = "&Ticks";
            this.miTickGridlines.Click += new System.EventHandler(this.GridlinesStyleChanged);
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
            this.tabGraph.ResumeLayout(false);
            this.pnlGraph.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbGraph)).EndInit();
            this.pnlTimeDeltaConfig.ResumeLayout(false);
            this.pnlTimeDeltaConfig.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabOverview;
        private System.Windows.Forms.TabPage tabGraph;
        private System.Windows.Forms.TabPage tabOCRErrors;
        private System.Windows.Forms.TabPage tabExport;
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
    }
}
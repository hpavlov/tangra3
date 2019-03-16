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
            this.tabDebugFrames = new System.Windows.Forms.TabPage();
            this.tabExport = new System.Windows.Forms.TabPage();
            this.pnlGraph = new System.Windows.Forms.Panel();
            this.pbGraph = new System.Windows.Forms.PictureBox();
            this.resizeUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.tabControl.SuspendLayout();
            this.tabOverview.SuspendLayout();
            this.tabGraph.SuspendLayout();
            this.pnlGraph.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbGraph)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabOverview);
            this.tabControl.Controls.Add(this.tabGraph);
            this.tabControl.Controls.Add(this.tabDebugFrames);
            this.tabControl.Controls.Add(this.tabExport);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(757, 524);
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
            this.tabGraph.Controls.Add(this.pnlGraph);
            this.tabGraph.Location = new System.Drawing.Point(4, 22);
            this.tabGraph.Name = "tabGraph";
            this.tabGraph.Padding = new System.Windows.Forms.Padding(3);
            this.tabGraph.Size = new System.Drawing.Size(749, 498);
            this.tabGraph.TabIndex = 1;
            this.tabGraph.Text = "Graph";
            this.tabGraph.UseVisualStyleBackColor = true;
            // 
            // tabDebugFrames
            // 
            this.tabDebugFrames.Location = new System.Drawing.Point(4, 22);
            this.tabDebugFrames.Name = "tabDebugFrames";
            this.tabDebugFrames.Size = new System.Drawing.Size(749, 498);
            this.tabDebugFrames.TabIndex = 2;
            this.tabDebugFrames.Text = "Debug Frames";
            this.tabDebugFrames.UseVisualStyleBackColor = true;
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
            // pnlGraph
            // 
            this.pnlGraph.Controls.Add(this.pbGraph);
            this.pnlGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGraph.Location = new System.Drawing.Point(3, 3);
            this.pnlGraph.Name = "pnlGraph";
            this.pnlGraph.Size = new System.Drawing.Size(743, 492);
            this.pnlGraph.TabIndex = 0;
            // 
            // pbGraph
            // 
            this.pbGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbGraph.Location = new System.Drawing.Point(0, 0);
            this.pbGraph.Name = "pbGraph";
            this.pbGraph.Size = new System.Drawing.Size(743, 492);
            this.pbGraph.TabIndex = 0;
            this.pbGraph.TabStop = false;
            // 
            // resizeUpdateTimer
            // 
            this.resizeUpdateTimer.Interval = 250;
            this.resizeUpdateTimer.Tick += new System.EventHandler(this.resizeUpdateTimer_Tick);
            // 
            // frmAavStatusChannelOnlyView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(757, 524);
            this.Controls.Add(this.tabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabOverview;
        private System.Windows.Forms.TabPage tabGraph;
        private System.Windows.Forms.TabPage tabDebugFrames;
        private System.Windows.Forms.TabPage tabExport;
        private System.Windows.Forms.ProgressBar pbLoadData;
        private System.Windows.Forms.Panel pnlGraph;
        private System.Windows.Forms.PictureBox pbGraph;
        private System.Windows.Forms.Timer resizeUpdateTimer;
    }
}
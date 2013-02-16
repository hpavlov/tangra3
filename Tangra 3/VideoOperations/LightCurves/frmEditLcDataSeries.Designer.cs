namespace Tangra.VideoOperations.LightCurves
{
	partial class frmEditLcDataSeries
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEditLcDataSeries));
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.panelClient = new System.Windows.Forms.Panel();
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.lCFileSeriesEntryBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.signal1DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.background1DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.signal2DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.background2DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.signal3DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.background3DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.signal4DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.background4DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlBottom.SuspendLayout();
            this.panelClient.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lCFileSeriesEntryBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.btnSave);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 570);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(856, 50);
            this.pnlBottom.TabIndex = 1;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(12, 15);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(139, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Save and Close";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // panelClient
            // 
            this.panelClient.Controls.Add(this.dgvData);
            this.panelClient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelClient.Location = new System.Drawing.Point(0, 0);
            this.panelClient.Name = "panelClient";
            this.panelClient.Size = new System.Drawing.Size(856, 570);
            this.panelClient.TabIndex = 2;
            // 
            // dgvData
            // 
            this.dgvData.AllowUserToAddRows = false;
            this.dgvData.AllowUserToDeleteRows = false;
            this.dgvData.AutoGenerateColumns = false;
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.signal1DataGridViewTextBoxColumn,
            this.background1DataGridViewTextBoxColumn,
            this.signal2DataGridViewTextBoxColumn,
            this.background2DataGridViewTextBoxColumn,
            this.signal3DataGridViewTextBoxColumn,
            this.background3DataGridViewTextBoxColumn,
            this.signal4DataGridViewTextBoxColumn,
            this.background4DataGridViewTextBoxColumn});
            this.dgvData.DataSource = this.lCFileSeriesEntryBindingSource;
            this.dgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvData.Location = new System.Drawing.Point(0, 0);
            this.dgvData.Name = "dgvData";
            this.dgvData.Size = new System.Drawing.Size(856, 570);
            this.dgvData.TabIndex = 1;
            // 
            // lCFileSeriesEntryBindingSource
            // 
            this.lCFileSeriesEntryBindingSource.DataSource = typeof(Tangra.VideoOperations.LightCurves.LCFileSeriesEntry);
            // 
            // signal1DataGridViewTextBoxColumn
            // 
            this.signal1DataGridViewTextBoxColumn.DataPropertyName = "Signal1";
            this.signal1DataGridViewTextBoxColumn.HeaderText = "Signal1";
            this.signal1DataGridViewTextBoxColumn.Name = "signal1DataGridViewTextBoxColumn";
            // 
            // background1DataGridViewTextBoxColumn
            // 
            this.background1DataGridViewTextBoxColumn.DataPropertyName = "Background1";
            this.background1DataGridViewTextBoxColumn.HeaderText = "Background1";
            this.background1DataGridViewTextBoxColumn.Name = "background1DataGridViewTextBoxColumn";
            // 
            // signal2DataGridViewTextBoxColumn
            // 
            this.signal2DataGridViewTextBoxColumn.DataPropertyName = "Signal2";
            this.signal2DataGridViewTextBoxColumn.HeaderText = "Signal2";
            this.signal2DataGridViewTextBoxColumn.Name = "signal2DataGridViewTextBoxColumn";
            // 
            // background2DataGridViewTextBoxColumn
            // 
            this.background2DataGridViewTextBoxColumn.DataPropertyName = "Background2";
            this.background2DataGridViewTextBoxColumn.HeaderText = "Background2";
            this.background2DataGridViewTextBoxColumn.Name = "background2DataGridViewTextBoxColumn";
            // 
            // signal3DataGridViewTextBoxColumn
            // 
            this.signal3DataGridViewTextBoxColumn.DataPropertyName = "Signal3";
            this.signal3DataGridViewTextBoxColumn.HeaderText = "Signal3";
            this.signal3DataGridViewTextBoxColumn.Name = "signal3DataGridViewTextBoxColumn";
            // 
            // background3DataGridViewTextBoxColumn
            // 
            this.background3DataGridViewTextBoxColumn.DataPropertyName = "Background3";
            this.background3DataGridViewTextBoxColumn.HeaderText = "Background3";
            this.background3DataGridViewTextBoxColumn.Name = "background3DataGridViewTextBoxColumn";
            // 
            // signal4DataGridViewTextBoxColumn
            // 
            this.signal4DataGridViewTextBoxColumn.DataPropertyName = "Signal4";
            this.signal4DataGridViewTextBoxColumn.HeaderText = "Signal4";
            this.signal4DataGridViewTextBoxColumn.Name = "signal4DataGridViewTextBoxColumn";
            // 
            // background4DataGridViewTextBoxColumn
            // 
            this.background4DataGridViewTextBoxColumn.DataPropertyName = "Background4";
            this.background4DataGridViewTextBoxColumn.HeaderText = "Background4";
            this.background4DataGridViewTextBoxColumn.Name = "background4DataGridViewTextBoxColumn";
            // 
            // frmEditLcDataSeries
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(856, 620);
            this.Controls.Add(this.panelClient);
            this.Controls.Add(this.pnlBottom);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmEditLcDataSeries";
            this.Text = "Edit Light Curve Data Series";
            this.pnlBottom.ResumeLayout(false);
            this.panelClient.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lCFileSeriesEntryBindingSource)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel pnlBottom;
		private System.Windows.Forms.Panel panelClient;
		private System.Windows.Forms.DataGridView dgvData;
		private System.Windows.Forms.BindingSource lCFileSeriesEntryBindingSource;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DataGridViewTextBoxColumn signal1DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn background1DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn signal2DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn background2DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn signal3DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn background3DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn signal4DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn background4DataGridViewTextBoxColumn;
	}
}
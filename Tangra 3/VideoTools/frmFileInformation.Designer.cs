namespace Tangra.VideoTools
{
	partial class frmFileInformation
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
			this.dgvFileInfo = new System.Windows.Forms.DataGridView();
			this.tagValuePairBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.tagDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.valueDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.dgvFileInfo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tagValuePairBindingSource)).BeginInit();
			this.SuspendLayout();
			// 
			// dgvFileInfo
			// 
			this.dgvFileInfo.AllowUserToAddRows = false;
			this.dgvFileInfo.AllowUserToDeleteRows = false;
			this.dgvFileInfo.AutoGenerateColumns = false;
			this.dgvFileInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvFileInfo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.tagDataGridViewTextBoxColumn,
            this.valueDataGridViewTextBoxColumn});
			this.dgvFileInfo.DataSource = this.tagValuePairBindingSource;
			this.dgvFileInfo.Location = new System.Drawing.Point(12, 12);
			this.dgvFileInfo.Name = "dgvFileInfo";
			this.dgvFileInfo.ReadOnly = true;
			this.dgvFileInfo.Size = new System.Drawing.Size(320, 250);
			this.dgvFileInfo.TabIndex = 24;
			// 
			// tagValuePairBindingSource
			// 
			this.tagValuePairBindingSource.DataSource = typeof(Tangra.VideoTools.TagValuePair);
			// 
			// tagDataGridViewTextBoxColumn
			// 
			this.tagDataGridViewTextBoxColumn.DataPropertyName = "Tag";
			this.tagDataGridViewTextBoxColumn.FillWeight = 130F;
			this.tagDataGridViewTextBoxColumn.HeaderText = "Tag";
			this.tagDataGridViewTextBoxColumn.Name = "tagDataGridViewTextBoxColumn";
			this.tagDataGridViewTextBoxColumn.ReadOnly = true;
			this.tagDataGridViewTextBoxColumn.Width = 130;
			// 
			// valueDataGridViewTextBoxColumn
			// 
			this.valueDataGridViewTextBoxColumn.DataPropertyName = "Value";
			this.valueDataGridViewTextBoxColumn.FillWeight = 130F;
			this.valueDataGridViewTextBoxColumn.HeaderText = "Value";
			this.valueDataGridViewTextBoxColumn.Name = "valueDataGridViewTextBoxColumn";
			this.valueDataGridViewTextBoxColumn.ReadOnly = true;
			this.valueDataGridViewTextBoxColumn.Width = 130;
			// 
			// frmFileInformation
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(348, 274);
			this.Controls.Add(this.dgvFileInfo);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmFileInformation";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "File Information";
			((System.ComponentModel.ISupportInitialize)(this.dgvFileInfo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tagValuePairBindingSource)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView dgvFileInfo;
		private System.Windows.Forms.BindingSource tagValuePairBindingSource;
		private System.Windows.Forms.DataGridViewTextBoxColumn tagDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn valueDataGridViewTextBoxColumn;

	}
}
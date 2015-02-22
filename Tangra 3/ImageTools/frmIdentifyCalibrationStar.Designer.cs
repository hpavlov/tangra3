namespace Tangra.ImageTools
{
	partial class frmIdentifyCalibrationStar
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmIdentifyCalibrationStar));
			this.tbxStarNo = new System.Windows.Forms.TextBox();
			this.lvStars = new System.Windows.Forms.ListView();
			this.columnStarNo = new System.Windows.Forms.ColumnHeader();
			this.columnMag = new System.Windows.Forms.ColumnHeader();
			this.label1 = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.btnClear = new System.Windows.Forms.Button();
			this.lblIdentifiedStarNo = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// tbxStarNo
			// 
			this.tbxStarNo.Location = new System.Drawing.Point(113, 46);
			this.tbxStarNo.Name = "tbxStarNo";
			this.tbxStarNo.Size = new System.Drawing.Size(171, 20);
			this.tbxStarNo.TabIndex = 0;
			this.tbxStarNo.TextChanged += new System.EventHandler(this.tbxStarNo_TextChanged);
			// 
			// lvStars
			// 
			this.lvStars.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnStarNo,
            this.columnMag});
			this.lvStars.FullRowSelect = true;
			this.lvStars.GridLines = true;
			this.lvStars.Location = new System.Drawing.Point(12, 72);
			this.lvStars.Name = "lvStars";
			this.lvStars.Size = new System.Drawing.Size(273, 163);
			this.lvStars.TabIndex = 1;
			this.lvStars.UseCompatibleStateImageBehavior = false;
			this.lvStars.View = System.Windows.Forms.View.Details;
			this.lvStars.SelectedIndexChanged += new System.EventHandler(this.lvStars_SelectedIndexChanged);
			// 
			// columnStarNo
			// 
			this.columnStarNo.Text = "Star No";
			this.columnStarNo.Width = 182;
			// 
			// columnMag
			// 
			this.columnMag.Text = "Mag";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 49);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(97, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Search by Star No:";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(210, 253);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(129, 252);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 4;
			this.btnOK.Text = "Identify";
			this.btnOK.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 12);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(118, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Already Identified Stars:";
			// 
			// btnClear
			// 
			this.btnClear.DialogResult = System.Windows.Forms.DialogResult.Abort;
			this.btnClear.Location = new System.Drawing.Point(210, 7);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(75, 23);
			this.btnClear.TabIndex = 6;
			this.btnClear.Text = "Clear";
			this.btnClear.UseVisualStyleBackColor = true;
			// 
			// lblIdentifiedStarNo
			// 
			this.lblIdentifiedStarNo.AutoSize = true;
			this.lblIdentifiedStarNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblIdentifiedStarNo.Location = new System.Drawing.Point(128, 12);
			this.lblIdentifiedStarNo.Name = "lblIdentifiedStarNo";
			this.lblIdentifiedStarNo.Size = new System.Drawing.Size(14, 13);
			this.lblIdentifiedStarNo.TabIndex = 7;
			this.lblIdentifiedStarNo.Text = "2";
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.ControlText;
			this.panel1.Location = new System.Drawing.Point(12, 35);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(274, 1);
			this.panel1.TabIndex = 8;
			// 
			// frmIdentifyCalibrationStar
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(296, 288);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.lblIdentifiedStarNo);
			this.Controls.Add(this.btnClear);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lvStars);
			this.Controls.Add(this.tbxStarNo);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmIdentifyCalibrationStar";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Identify Calibration Star";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbxStarNo;
		private System.Windows.Forms.ListView lvStars;
		private System.Windows.Forms.ColumnHeader columnStarNo;
		private System.Windows.Forms.ColumnHeader columnMag;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.Label lblIdentifiedStarNo;
		private System.Windows.Forms.Panel panel1;
	}
}
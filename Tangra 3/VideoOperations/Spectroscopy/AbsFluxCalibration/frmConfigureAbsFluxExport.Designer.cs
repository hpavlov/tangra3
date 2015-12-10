namespace Tangra.VideoOperations.Spectroscopy.AbsFluxCalibration
{
	partial class frmConfigureAbsFluxExport
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
			this.cbxExpostSyntheticMags = new System.Windows.Forms.CheckBox();
			this.gbxMagConfig = new System.Windows.Forms.GroupBox();
			this.cb_Sloan_i = new System.Windows.Forms.CheckBox();
			this.cb_Sloan_g = new System.Windows.Forms.CheckBox();
			this.cb_Sloan_r = new System.Windows.Forms.CheckBox();
			this.cb_R = new System.Windows.Forms.CheckBox();
			this.cb_B = new System.Windows.Forms.CheckBox();
			this.cb_V = new System.Windows.Forms.CheckBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.gbxMagConfig.SuspendLayout();
			this.SuspendLayout();
			// 
			// cbxExpostSyntheticMags
			// 
			this.cbxExpostSyntheticMags.AutoSize = true;
			this.cbxExpostSyntheticMags.Location = new System.Drawing.Point(12, 12);
			this.cbxExpostSyntheticMags.Name = "cbxExpostSyntheticMags";
			this.cbxExpostSyntheticMags.Size = new System.Drawing.Size(161, 17);
			this.cbxExpostSyntheticMags.TabIndex = 0;
			this.cbxExpostSyntheticMags.Text = "Export Synthetic Magnitudes";
			this.cbxExpostSyntheticMags.UseVisualStyleBackColor = true;
			this.cbxExpostSyntheticMags.CheckedChanged += new System.EventHandler(this.cbxExpostSyntheticMags_CheckedChanged);
			// 
			// gbxMagConfig
			// 
			this.gbxMagConfig.Controls.Add(this.cb_Sloan_i);
			this.gbxMagConfig.Controls.Add(this.cb_Sloan_g);
			this.gbxMagConfig.Controls.Add(this.cb_Sloan_r);
			this.gbxMagConfig.Controls.Add(this.cb_R);
			this.gbxMagConfig.Controls.Add(this.cb_B);
			this.gbxMagConfig.Controls.Add(this.cb_V);
			this.gbxMagConfig.Enabled = false;
			this.gbxMagConfig.Location = new System.Drawing.Point(30, 35);
			this.gbxMagConfig.Name = "gbxMagConfig";
			this.gbxMagConfig.Size = new System.Drawing.Size(179, 85);
			this.gbxMagConfig.TabIndex = 1;
			this.gbxMagConfig.TabStop = false;
			// 
			// cb_Sloan_i
			// 
			this.cb_Sloan_i.AutoSize = true;
			this.cb_Sloan_i.Location = new System.Drawing.Point(120, 53);
			this.cb_Sloan_i.Name = "cb_Sloan_i";
			this.cb_Sloan_i.Size = new System.Drawing.Size(30, 17);
			this.cb_Sloan_i.TabIndex = 8;
			this.cb_Sloan_i.Text = "i\'";
			this.cb_Sloan_i.UseVisualStyleBackColor = true;
			// 
			// cb_Sloan_g
			// 
			this.cb_Sloan_g.AutoSize = true;
			this.cb_Sloan_g.Location = new System.Drawing.Point(21, 52);
			this.cb_Sloan_g.Name = "cb_Sloan_g";
			this.cb_Sloan_g.Size = new System.Drawing.Size(34, 17);
			this.cb_Sloan_g.TabIndex = 7;
			this.cb_Sloan_g.Text = "g\'";
			this.cb_Sloan_g.UseVisualStyleBackColor = true;
			// 
			// cb_Sloan_r
			// 
			this.cb_Sloan_r.AutoSize = true;
			this.cb_Sloan_r.Location = new System.Drawing.Point(72, 53);
			this.cb_Sloan_r.Name = "cb_Sloan_r";
			this.cb_Sloan_r.Size = new System.Drawing.Size(31, 17);
			this.cb_Sloan_r.TabIndex = 6;
			this.cb_Sloan_r.Text = "r\'";
			this.cb_Sloan_r.UseVisualStyleBackColor = true;
			// 
			// cb_R
			// 
			this.cb_R.AutoSize = true;
			this.cb_R.Location = new System.Drawing.Point(120, 20);
			this.cb_R.Name = "cb_R";
			this.cb_R.Size = new System.Drawing.Size(34, 17);
			this.cb_R.TabIndex = 3;
			this.cb_R.Text = "R";
			this.cb_R.UseVisualStyleBackColor = true;
			// 
			// cb_B
			// 
			this.cb_B.AutoSize = true;
			this.cb_B.Location = new System.Drawing.Point(21, 19);
			this.cb_B.Name = "cb_B";
			this.cb_B.Size = new System.Drawing.Size(33, 17);
			this.cb_B.TabIndex = 2;
			this.cb_B.Text = "B";
			this.cb_B.UseVisualStyleBackColor = true;
			// 
			// cb_V
			// 
			this.cb_V.AutoSize = true;
			this.cb_V.Location = new System.Drawing.Point(72, 20);
			this.cb_V.Name = "cb_V";
			this.cb_V.Size = new System.Drawing.Size(33, 17);
			this.cb_V.TabIndex = 1;
			this.cb_V.Text = "V";
			this.cb_V.UseVisualStyleBackColor = true;
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(38, 138);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(119, 138);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// frmConfigureAbsFluxExport
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(228, 173);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.gbxMagConfig);
			this.Controls.Add(this.cbxExpostSyntheticMags);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmConfigureAbsFluxExport";
			this.Text = "Configure Absolute Flux Export";
			this.gbxMagConfig.ResumeLayout(false);
			this.gbxMagConfig.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox cbxExpostSyntheticMags;
		private System.Windows.Forms.GroupBox gbxMagConfig;
		private System.Windows.Forms.CheckBox cb_Sloan_i;
		private System.Windows.Forms.CheckBox cb_Sloan_g;
		private System.Windows.Forms.CheckBox cb_Sloan_r;
		private System.Windows.Forms.CheckBox cb_R;
		private System.Windows.Forms.CheckBox cb_B;
		private System.Windows.Forms.CheckBox cb_V;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
	}
}
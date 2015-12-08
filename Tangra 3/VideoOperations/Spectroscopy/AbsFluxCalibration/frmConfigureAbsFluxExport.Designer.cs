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
			this.cb_U = new System.Windows.Forms.CheckBox();
			this.cb_V = new System.Windows.Forms.CheckBox();
			this.cb_B = new System.Windows.Forms.CheckBox();
			this.cb_R = new System.Windows.Forms.CheckBox();
			this.cb_I = new System.Windows.Forms.CheckBox();
			this.cb_Sloan_z = new System.Windows.Forms.CheckBox();
			this.cb_Sloan_i = new System.Windows.Forms.CheckBox();
			this.cb_Sloan_g = new System.Windows.Forms.CheckBox();
			this.cb_Sloan_r = new System.Windows.Forms.CheckBox();
			this.cb_Sloan_u = new System.Windows.Forms.CheckBox();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
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
			this.gbxMagConfig.Controls.Add(this.cb_Sloan_z);
			this.gbxMagConfig.Controls.Add(this.cb_Sloan_i);
			this.gbxMagConfig.Controls.Add(this.cb_Sloan_g);
			this.gbxMagConfig.Controls.Add(this.cb_Sloan_r);
			this.gbxMagConfig.Controls.Add(this.cb_Sloan_u);
			this.gbxMagConfig.Controls.Add(this.cb_I);
			this.gbxMagConfig.Controls.Add(this.cb_R);
			this.gbxMagConfig.Controls.Add(this.cb_B);
			this.gbxMagConfig.Controls.Add(this.cb_V);
			this.gbxMagConfig.Controls.Add(this.cb_U);
			this.gbxMagConfig.Enabled = false;
			this.gbxMagConfig.Location = new System.Drawing.Point(30, 35);
			this.gbxMagConfig.Name = "gbxMagConfig";
			this.gbxMagConfig.Size = new System.Drawing.Size(217, 91);
			this.gbxMagConfig.TabIndex = 1;
			this.gbxMagConfig.TabStop = false;
			// 
			// cb_U
			// 
			this.cb_U.AutoSize = true;
			this.cb_U.Location = new System.Drawing.Point(16, 20);
			this.cb_U.Name = "cb_U";
			this.cb_U.Size = new System.Drawing.Size(34, 17);
			this.cb_U.TabIndex = 0;
			this.cb_U.Text = "U";
			this.cb_U.UseVisualStyleBackColor = true;
			// 
			// cb_V
			// 
			this.cb_V.AutoSize = true;
			this.cb_V.Location = new System.Drawing.Point(95, 20);
			this.cb_V.Name = "cb_V";
			this.cb_V.Size = new System.Drawing.Size(33, 17);
			this.cb_V.TabIndex = 1;
			this.cb_V.Text = "V";
			this.cb_V.UseVisualStyleBackColor = true;
			// 
			// cb_B
			// 
			this.cb_B.AutoSize = true;
			this.cb_B.Location = new System.Drawing.Point(56, 20);
			this.cb_B.Name = "cb_B";
			this.cb_B.Size = new System.Drawing.Size(33, 17);
			this.cb_B.TabIndex = 2;
			this.cb_B.Text = "B";
			this.cb_B.UseVisualStyleBackColor = true;
			// 
			// cb_R
			// 
			this.cb_R.AutoSize = true;
			this.cb_R.Location = new System.Drawing.Point(134, 20);
			this.cb_R.Name = "cb_R";
			this.cb_R.Size = new System.Drawing.Size(34, 17);
			this.cb_R.TabIndex = 3;
			this.cb_R.Text = "R";
			this.cb_R.UseVisualStyleBackColor = true;
			// 
			// cb_I
			// 
			this.cb_I.AutoSize = true;
			this.cb_I.Location = new System.Drawing.Point(174, 20);
			this.cb_I.Name = "cb_I";
			this.cb_I.Size = new System.Drawing.Size(29, 17);
			this.cb_I.TabIndex = 4;
			this.cb_I.Text = "I";
			this.cb_I.UseVisualStyleBackColor = true;
			// 
			// cb_Sloan_z
			// 
			this.cb_Sloan_z.AutoSize = true;
			this.cb_Sloan_z.Location = new System.Drawing.Point(174, 53);
			this.cb_Sloan_z.Name = "cb_Sloan_z";
			this.cb_Sloan_z.Size = new System.Drawing.Size(33, 17);
			this.cb_Sloan_z.TabIndex = 9;
			this.cb_Sloan_z.Text = "z\'";
			this.cb_Sloan_z.UseVisualStyleBackColor = true;
			// 
			// cb_Sloan_i
			// 
			this.cb_Sloan_i.AutoSize = true;
			this.cb_Sloan_i.Location = new System.Drawing.Point(134, 53);
			this.cb_Sloan_i.Name = "cb_Sloan_i";
			this.cb_Sloan_i.Size = new System.Drawing.Size(30, 17);
			this.cb_Sloan_i.TabIndex = 8;
			this.cb_Sloan_i.Text = "i\'";
			this.cb_Sloan_i.UseVisualStyleBackColor = true;
			// 
			// cb_Sloan_g
			// 
			this.cb_Sloan_g.AutoSize = true;
			this.cb_Sloan_g.Location = new System.Drawing.Point(56, 53);
			this.cb_Sloan_g.Name = "cb_Sloan_g";
			this.cb_Sloan_g.Size = new System.Drawing.Size(34, 17);
			this.cb_Sloan_g.TabIndex = 7;
			this.cb_Sloan_g.Text = "g\'";
			this.cb_Sloan_g.UseVisualStyleBackColor = true;
			// 
			// cb_Sloan_r
			// 
			this.cb_Sloan_r.AutoSize = true;
			this.cb_Sloan_r.Location = new System.Drawing.Point(95, 53);
			this.cb_Sloan_r.Name = "cb_Sloan_r";
			this.cb_Sloan_r.Size = new System.Drawing.Size(31, 17);
			this.cb_Sloan_r.TabIndex = 6;
			this.cb_Sloan_r.Text = "r\'";
			this.cb_Sloan_r.UseVisualStyleBackColor = true;
			// 
			// cb_Sloan_u
			// 
			this.cb_Sloan_u.AutoSize = true;
			this.cb_Sloan_u.Location = new System.Drawing.Point(16, 53);
			this.cb_Sloan_u.Name = "cb_Sloan_u";
			this.cb_Sloan_u.Size = new System.Drawing.Size(34, 17);
			this.cb_Sloan_u.TabIndex = 5;
			this.cb_Sloan_u.Text = "u\'";
			this.cb_Sloan_u.UseVisualStyleBackColor = true;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(61, 152);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 2;
			this.button1.Text = "OK";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// button2
			// 
			this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button2.Location = new System.Drawing.Point(142, 152);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 3;
			this.button2.Text = "Cancel";
			this.button2.UseVisualStyleBackColor = true;
			// 
			// frmConfigureAbsFluxExport
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(266, 190);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
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
		private System.Windows.Forms.CheckBox cb_Sloan_z;
		private System.Windows.Forms.CheckBox cb_Sloan_i;
		private System.Windows.Forms.CheckBox cb_Sloan_g;
		private System.Windows.Forms.CheckBox cb_Sloan_r;
		private System.Windows.Forms.CheckBox cb_Sloan_u;
		private System.Windows.Forms.CheckBox cb_I;
		private System.Windows.Forms.CheckBox cb_R;
		private System.Windows.Forms.CheckBox cb_B;
		private System.Windows.Forms.CheckBox cb_V;
		private System.Windows.Forms.CheckBox cb_U;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
	}
}
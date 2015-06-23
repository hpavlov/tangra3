namespace Tangra.Config.SettingPannels
{
	partial class ucCustomizeSpectroscopy
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.label39 = new System.Windows.Forms.Label();
			this.ucColorPickerReferenceStar = new Tangra.Controls.ucColorPicker();
			this.groupBox6.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.label39);
			this.groupBox6.Controls.Add(this.ucColorPickerReferenceStar);
			this.groupBox6.Location = new System.Drawing.Point(3, 3);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(215, 178);
			this.groupBox6.TabIndex = 28;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Colours - Spectroscopy";
			// 
			// label39
			// 
			this.label39.AutoSize = true;
			this.label39.Location = new System.Drawing.Point(71, 39);
			this.label39.Name = "label39";
			this.label39.Size = new System.Drawing.Size(67, 13);
			this.label39.TabIndex = 1;
			this.label39.Text = "Spectra Line";
			// 
			// ucColorPickerReferenceStar
			// 
			this.ucColorPickerReferenceStar.AutoSize = true;
			this.ucColorPickerReferenceStar.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ucColorPickerReferenceStar.BackColor = System.Drawing.SystemColors.Control;
			this.ucColorPickerReferenceStar.Location = new System.Drawing.Point(9, 33);
			this.ucColorPickerReferenceStar.Name = "ucColorPickerReferenceStar";
			this.ucColorPickerReferenceStar.SelectedColor = System.Drawing.Color.Aqua;
			this.ucColorPickerReferenceStar.Size = new System.Drawing.Size(60, 26);
			this.ucColorPickerReferenceStar.TabIndex = 0;
			// 
			// ucCustomizeSpectroscopy
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox6);
			this.Name = "ucCustomizeSpectroscopy";
			this.Size = new System.Drawing.Size(362, 325);
			this.groupBox6.ResumeLayout(false);
			this.groupBox6.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.Label label39;
		private Controls.ucColorPicker ucColorPickerReferenceStar;
	}
}

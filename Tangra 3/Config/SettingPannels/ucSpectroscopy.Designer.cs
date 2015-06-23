namespace Tangra.Config.SettingPannels
{
	partial class ucSpectroscopy
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
			this.groupBox8 = new System.Windows.Forms.GroupBox();
			this.cbxInstrumentType = new System.Windows.Forms.ComboBox();
			this.label49 = new System.Windows.Forms.Label();
			this.label48 = new System.Windows.Forms.Label();
			this.rbOrder1 = new System.Windows.Forms.RadioButton();
			this.rbOrder2 = new System.Windows.Forms.RadioButton();
			this.rbOrder3 = new System.Windows.Forms.RadioButton();
			this.groupBox8.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox8
			// 
			this.groupBox8.Controls.Add(this.rbOrder3);
			this.groupBox8.Controls.Add(this.rbOrder2);
			this.groupBox8.Controls.Add(this.rbOrder1);
			this.groupBox8.Controls.Add(this.label48);
			this.groupBox8.Location = new System.Drawing.Point(3, 56);
			this.groupBox8.Name = "groupBox8";
			this.groupBox8.Size = new System.Drawing.Size(231, 136);
			this.groupBox8.TabIndex = 32;
			this.groupBox8.TabStop = false;
			this.groupBox8.Text = "Wavelength Calibration";
			// 
			// cbxInstrumentType
			// 
			this.cbxInstrumentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxInstrumentType.FormattingEnabled = true;
			this.cbxInstrumentType.Items.AddRange(new object[] {
            "Grating"});
			this.cbxInstrumentType.Location = new System.Drawing.Point(94, 14);
			this.cbxInstrumentType.Name = "cbxInstrumentType";
			this.cbxInstrumentType.Size = new System.Drawing.Size(101, 21);
			this.cbxInstrumentType.TabIndex = 32;
			// 
			// label49
			// 
			this.label49.AutoSize = true;
			this.label49.Location = new System.Drawing.Point(5, 17);
			this.label49.Name = "label49";
			this.label49.Size = new System.Drawing.Size(83, 13);
			this.label49.TabIndex = 10;
			this.label49.Text = "Instrument Type";
			// 
			// label48
			// 
			this.label48.AutoSize = true;
			this.label48.Location = new System.Drawing.Point(15, 35);
			this.label48.Name = "label48";
			this.label48.Size = new System.Drawing.Size(152, 13);
			this.label48.TabIndex = 9;
			this.label48.Text = "New Calibrations Default Order";
			// 
			// rbOrder1
			// 
			this.rbOrder1.AutoSize = true;
			this.rbOrder1.Checked = true;
			this.rbOrder1.Location = new System.Drawing.Point(32, 60);
			this.rbOrder1.Name = "rbOrder1";
			this.rbOrder1.Size = new System.Drawing.Size(54, 17);
			this.rbOrder1.TabIndex = 33;
			this.rbOrder1.TabStop = true;
			this.rbOrder1.Text = "Linear";
			this.rbOrder1.UseVisualStyleBackColor = true;
			// 
			// rbOrder2
			// 
			this.rbOrder2.AutoSize = true;
			this.rbOrder2.Location = new System.Drawing.Point(32, 83);
			this.rbOrder2.Name = "rbOrder2";
			this.rbOrder2.Size = new System.Drawing.Size(75, 17);
			this.rbOrder2.TabIndex = 34;
			this.rbOrder2.Text = "2-nd Order";
			this.rbOrder2.UseVisualStyleBackColor = true;
			// 
			// rbOrder3
			// 
			this.rbOrder3.AutoSize = true;
			this.rbOrder3.Location = new System.Drawing.Point(32, 106);
			this.rbOrder3.Name = "rbOrder3";
			this.rbOrder3.Size = new System.Drawing.Size(72, 17);
			this.rbOrder3.TabIndex = 35;
			this.rbOrder3.Text = "3-rd Order";
			this.rbOrder3.UseVisualStyleBackColor = true;
			// 
			// ucAstrometry
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox8);
			this.Controls.Add(this.label49);
			this.Controls.Add(this.cbxInstrumentType);
			this.Name = "ucAstrometry";
			this.Size = new System.Drawing.Size(520, 481);
			this.groupBox8.ResumeLayout(false);
			this.groupBox8.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox8;
		private System.Windows.Forms.ComboBox cbxInstrumentType;
		private System.Windows.Forms.Label label49;
		private System.Windows.Forms.Label label48;
		private System.Windows.Forms.RadioButton rbOrder3;
		private System.Windows.Forms.RadioButton rbOrder2;
		private System.Windows.Forms.RadioButton rbOrder1;
	}
}

namespace Tangra.VideoOperations.LightCurves.InfoForms
{
	partial class frmPSFFits
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.picTarget4PSF = new System.Windows.Forms.PictureBox();
			this.picTarget3PSF = new System.Windows.Forms.PictureBox();
			this.picTarget2PSF = new System.Windows.Forms.PictureBox();
			this.picTarget1PSF = new System.Windows.Forms.PictureBox();
			this.rbPreProcessedData = new System.Windows.Forms.RadioButton();
			this.rbRawData = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)(this.picTarget4PSF)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget3PSF)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget2PSF)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget1PSF)).BeginInit();
			this.SuspendLayout();
			// 
			// picTarget4PSF
			// 
			this.picTarget4PSF.BackColor = System.Drawing.SystemColors.ControlDark;
			this.picTarget4PSF.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.picTarget4PSF.Location = new System.Drawing.Point(151, 192);
			this.picTarget4PSF.Name = "picTarget4PSF";
			this.picTarget4PSF.Size = new System.Drawing.Size(140, 180);
			this.picTarget4PSF.TabIndex = 44;
			this.picTarget4PSF.TabStop = false;
			// 
			// picTarget3PSF
			// 
			this.picTarget3PSF.BackColor = System.Drawing.SystemColors.ControlDark;
			this.picTarget3PSF.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.picTarget3PSF.Location = new System.Drawing.Point(5, 192);
			this.picTarget3PSF.Name = "picTarget3PSF";
			this.picTarget3PSF.Size = new System.Drawing.Size(140, 180);
			this.picTarget3PSF.TabIndex = 43;
			this.picTarget3PSF.TabStop = false;
			// 
			// picTarget2PSF
			// 
			this.picTarget2PSF.BackColor = System.Drawing.SystemColors.ControlDark;
			this.picTarget2PSF.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.picTarget2PSF.Location = new System.Drawing.Point(151, 6);
			this.picTarget2PSF.Name = "picTarget2PSF";
			this.picTarget2PSF.Size = new System.Drawing.Size(140, 180);
			this.picTarget2PSF.TabIndex = 42;
			this.picTarget2PSF.TabStop = false;
			// 
			// picTarget1PSF
			// 
			this.picTarget1PSF.BackColor = System.Drawing.SystemColors.ControlDark;
			this.picTarget1PSF.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.picTarget1PSF.Location = new System.Drawing.Point(5, 6);
			this.picTarget1PSF.Name = "picTarget1PSF";
			this.picTarget1PSF.Size = new System.Drawing.Size(140, 180);
			this.picTarget1PSF.TabIndex = 41;
			this.picTarget1PSF.TabStop = false;
			// 
			// rbPreProcessedData
			// 
			this.rbPreProcessedData.Checked = true;
			this.rbPreProcessedData.Location = new System.Drawing.Point(3, 375);
			this.rbPreProcessedData.Name = "rbPreProcessedData";
			this.rbPreProcessedData.Size = new System.Drawing.Size(122, 19);
			this.rbPreProcessedData.TabIndex = 47;
			this.rbPreProcessedData.TabStop = true;
			this.rbPreProcessedData.Text = "Pre-Processed Data";
			this.rbPreProcessedData.CheckedChanged += new System.EventHandler(this.rbPreProcessedData_CheckedChanged);
			// 
			// rbRawData
			// 
			this.rbRawData.Location = new System.Drawing.Point(126, 376);
			this.rbRawData.Name = "rbRawData";
			this.rbRawData.Size = new System.Drawing.Size(122, 19);
			this.rbRawData.TabIndex = 48;
			this.rbRawData.Text = "Raw Data";
			this.rbRawData.CheckedChanged += new System.EventHandler(this.rbPreProcessedData_CheckedChanged);
			// 
			// frmPSFFits
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(296, 397);
			this.Controls.Add(this.rbRawData);
			this.Controls.Add(this.rbPreProcessedData);
			this.Controls.Add(this.picTarget4PSF);
			this.Controls.Add(this.picTarget3PSF);
			this.Controls.Add(this.picTarget2PSF);
			this.Controls.Add(this.picTarget1PSF);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmPSFFits";
			this.Text = "Target PSF Fits";
			((System.ComponentModel.ISupportInitialize)(this.picTarget4PSF)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget3PSF)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget2PSF)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget1PSF)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox picTarget4PSF;
		private System.Windows.Forms.PictureBox picTarget3PSF;
		private System.Windows.Forms.PictureBox picTarget2PSF;
		private System.Windows.Forms.PictureBox picTarget1PSF;
        private System.Windows.Forms.RadioButton rbPreProcessedData;
        private System.Windows.Forms.RadioButton rbRawData;
	}
}
namespace Tangra.Astrometry.Astrometry.Analysis
{
	partial class frmObjectResolver
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
			this.pbPlate = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pbPlate)).BeginInit();
			this.SuspendLayout();
			// 
			// pbPlate
			// 
			this.pbPlate.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pbPlate.Location = new System.Drawing.Point(0, 0);
			this.pbPlate.Name = "pbPlate";
			this.pbPlate.Size = new System.Drawing.Size(446, 368);
			this.pbPlate.TabIndex = 0;
			this.pbPlate.TabStop = false;
			// 
			// frmObjectResolver
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(446, 368);
			this.Controls.Add(this.pbPlate);
			this.Name = "frmObjectResolver";
			this.Text = "frmObjectResolver";
			this.Load += new System.EventHandler(this.frmObjectResolver_Load);
			((System.ComponentModel.ISupportInitialize)(this.pbPlate)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pbPlate;
	}
}
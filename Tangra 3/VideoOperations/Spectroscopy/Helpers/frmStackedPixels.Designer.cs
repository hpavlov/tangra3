namespace Tangra.VideoOperations.Spectroscopy.Helpers
{
	partial class frmStackedPixels
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
			this.picPixels = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.picPixels)).BeginInit();
			this.SuspendLayout();
			// 
			// picPixels
			// 
			this.picPixels.Dock = System.Windows.Forms.DockStyle.Fill;
			this.picPixels.Location = new System.Drawing.Point(0, 0);
			this.picPixels.Name = "picPixels";
			this.picPixels.Size = new System.Drawing.Size(672, 75);
			this.picPixels.TabIndex = 0;
			this.picPixels.TabStop = false;
			// 
			// frmStackedPixels
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(672, 75);
			this.Controls.Add(this.picPixels);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmStackedPixels";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Stacked Spectra Pixels";
			((System.ComponentModel.ISupportInitialize)(this.picPixels)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox picPixels;
	}
}
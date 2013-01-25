namespace Tangra.Model.Astro
{
	partial class frmPSFViewer
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
            this.picFIT = new System.Windows.Forms.PictureBox();
            this.picPixels = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picFIT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPixels)).BeginInit();
            this.SuspendLayout();
            // 
            // picFIT
            // 
            this.picFIT.Location = new System.Drawing.Point(0, 2);
            this.picFIT.Name = "picFIT";
            this.picFIT.Size = new System.Drawing.Size(213, 272);
            this.picFIT.TabIndex = 0;
            this.picFIT.TabStop = false;
            // 
            // picPixels
            // 
            this.picPixels.Location = new System.Drawing.Point(219, 2);
            this.picPixels.Name = "picPixels";
            this.picPixels.Size = new System.Drawing.Size(201, 184);
            this.picPixels.TabIndex = 1;
            this.picPixels.TabStop = false;
            // 
            // frmPSFViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 276);
            this.Controls.Add(this.picPixels);
            this.Controls.Add(this.picFIT);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmPSFViewer";
            this.Text = "PSF Fit Viewer";
            this.Load += new System.EventHandler(this.frmPSFViewer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picFIT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPixels)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox picFIT;
        private System.Windows.Forms.PictureBox picPixels;
	}
}
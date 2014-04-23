using Tangra.Video;

namespace Tangra.VideoOperations
{
	partial class frmFullSizePreview
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFullSizePreview));
			this.pictureBox = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox
			// 
			this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox.Location = new System.Drawing.Point(0, 0);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(627, 425);
			this.pictureBox.TabIndex = 0;
			this.pictureBox.TabStop = false;
			// 
			// frmFullSizePreview
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(627, 425);
			this.Controls.Add(this.pictureBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmFullSizePreview";
			this.Text = "Full Size Preview";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		protected internal System.Windows.Forms.PictureBox pictureBox;

	}
}
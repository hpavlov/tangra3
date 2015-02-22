namespace Tangra.Helpers
{
	partial class frmLongRunningOperation
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
			this.lblStatus = new System.Windows.Forms.Label();
			this.pbar = new System.Windows.Forms.ProgressBar();
			this.SuspendLayout();
			// 
			// lblStatus
			// 
			this.lblStatus.AutoSize = true;
			this.lblStatus.Location = new System.Drawing.Point(12, 39);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(35, 13);
			this.lblStatus.TabIndex = 0;
			this.lblStatus.Text = "label1";
			// 
			// pbar
			// 
			this.pbar.Location = new System.Drawing.Point(12, 13);
			this.pbar.Name = "pbar";
			this.pbar.Size = new System.Drawing.Size(365, 18);
			this.pbar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.pbar.TabIndex = 1;
			// 
			// frmLongRunningOperation
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(385, 56);
			this.ControlBox = false;
			this.Controls.Add(this.pbar);
			this.Controls.Add(this.lblStatus);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmLongRunningOperation";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Waiting ...";
			this.TopMost = true;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.ProgressBar pbar;
	}
}
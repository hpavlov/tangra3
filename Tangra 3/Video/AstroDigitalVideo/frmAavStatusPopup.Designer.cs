namespace Tangra.Video.AstroDigitalVideo
{
	partial class frmAavStatusPopup
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAavStatusPopup));
			this.lblStatusCombined = new System.Windows.Forms.Label();
			this.btnCopy = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblStatusCombined
			// 
			this.lblStatusCombined.AutoSize = true;
			this.lblStatusCombined.Location = new System.Drawing.Point(12, 9);
			this.lblStatusCombined.Name = "lblStatusCombined";
			this.lblStatusCombined.Size = new System.Drawing.Size(180, 65);
			this.lblStatusCombined.TabIndex = 0;
			this.lblStatusCombined.Text = "Tracked Satellites: 4\r\nAlmanac Status: Good\r\nGPS Fix: P-Fix\r\n\r\nSystem Time: 12 De" +
    "c 2011 10:45:00\r\n";
			// 
			// btnCopy
			// 
			this.btnCopy.Location = new System.Drawing.Point(65, 110);
			this.btnCopy.Name = "btnCopy";
			this.btnCopy.Size = new System.Drawing.Size(75, 23);
			this.btnCopy.TabIndex = 18;
			this.btnCopy.Text = "Copy";
			this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
			// 
			// frmAavStatusPopup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(207, 145);
			this.Controls.Add(this.btnCopy);
			this.Controls.Add(this.lblStatusCombined);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmAavStatusPopup";
			this.Text = "AAV State Channel";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblStatusCombined;
		private System.Windows.Forms.Button btnCopy;
	}
}
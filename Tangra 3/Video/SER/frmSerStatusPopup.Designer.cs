namespace Tangra.Video.SER
{
	partial class frmSerStatusPopup
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
            this.btnCopy = new System.Windows.Forms.Button();
            this.lblStatusCombined = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(64, 62);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(75, 23);
            this.btnCopy.TabIndex = 20;
            this.btnCopy.Text = "Copy";
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // lblStatusCombined
            // 
            this.lblStatusCombined.AutoSize = true;
            this.lblStatusCombined.Location = new System.Drawing.Point(12, 9);
            this.lblStatusCombined.Name = "lblStatusCombined";
            this.lblStatusCombined.Size = new System.Drawing.Size(176, 13);
            this.lblStatusCombined.TabIndex = 19;
            this.lblStatusCombined.Text = "Time Stamp: 12 Dec 2011 10:45:00\r\n";
            // 
            // frmSerStatusPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(211, 97);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.lblStatusCombined);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmSerStatusPopup";
            this.Text = "Ser File Timestamp";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnCopy;
		private System.Windows.Forms.Label lblStatusCombined;
	}
}
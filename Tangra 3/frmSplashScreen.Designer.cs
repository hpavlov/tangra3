namespace Tangra
{
	partial class frmSplashScreen
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
			this.lblTangraVersion = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblTangraVersion
			// 
			this.lblTangraVersion.AutoSize = true;
			this.lblTangraVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblTangraVersion.Location = new System.Drawing.Point(108, 55);
			this.lblTangraVersion.Name = "lblTangraVersion";
			this.lblTangraVersion.Size = new System.Drawing.Size(244, 24);
			this.lblTangraVersion.TabIndex = 0;
			this.lblTangraVersion.Text = "Tangra v3.0.24 (Linux Build)";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label2.Location = new System.Drawing.Point(108, 87);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(180, 24);
			this.label2.TabIndex = 1;
			this.label2.Text = "Tangra Core v3.0.12";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label1.Location = new System.Drawing.Point(108, 118);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(245, 24);
			this.label1.TabIndex = 2;
			this.label1.Text = "Tangra Video Engine v3.0.5";
			// 
			// frmSplashScreen
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(411, 193);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.lblTangraVersion);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "frmSplashScreen";
			this.Text = "frmSplashScreen";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblTangraVersion;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
	}
}
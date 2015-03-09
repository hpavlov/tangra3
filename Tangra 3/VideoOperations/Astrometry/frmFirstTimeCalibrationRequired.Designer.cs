namespace Tangra.VideoOperations.Astrometry
{
	partial class frmFirstTimeCalibrationRequired
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFirstTimeCalibrationRequired));
            this.btnContinue = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lnkCalinrationHelpLink = new System.Windows.Forms.LinkLabel();
            this.cbxNotShowAgain = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnContinue
            // 
            this.btnContinue.Location = new System.Drawing.Point(78, 145);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(137, 23);
            this.btnContinue.TabIndex = 3;
            this.btnContinue.Text = "Continue";
            this.btnContinue.UseVisualStyleBackColor = true;
            this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(292, 41);
            this.label1.TabIndex = 2;
            this.label1.Text = "   Before going any further please read the online help topic on how to complete " +
    "the calibration process in Tangra:";
            // 
            // lnkCalinrationHelpLink
            // 
            this.lnkCalinrationHelpLink.AutoSize = true;
            this.lnkCalinrationHelpLink.Location = new System.Drawing.Point(14, 59);
            this.lnkCalinrationHelpLink.Name = "lnkCalinrationHelpLink";
            this.lnkCalinrationHelpLink.Size = new System.Drawing.Size(238, 13);
            this.lnkCalinrationHelpLink.TabIndex = 4;
            this.lnkCalinrationHelpLink.TabStop = true;
            this.lnkCalinrationHelpLink.Text = "http://www.hristopavlov.net/Tangra/Calibration/";
            this.lnkCalinrationHelpLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkCalinrationHelpLink_LinkClicked);
            // 
            // cbxNotShowAgain
            // 
            this.cbxNotShowAgain.AutoSize = true;
            this.cbxNotShowAgain.Location = new System.Drawing.Point(17, 96);
            this.cbxNotShowAgain.Name = "cbxNotShowAgain";
            this.cbxNotShowAgain.Size = new System.Drawing.Size(172, 17);
            this.cbxNotShowAgain.TabIndex = 5;
            this.cbxNotShowAgain.Text = "Don\'t show this message again";
            this.cbxNotShowAgain.UseVisualStyleBackColor = true;
            // 
            // frmFirstTimeCalibrationRequired
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(306, 184);
            this.Controls.Add(this.cbxNotShowAgain);
            this.Controls.Add(this.lnkCalinrationHelpLink);
            this.Controls.Add(this.btnContinue);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmFirstTimeCalibrationRequired";
            this.Text = "Calibration Instructions";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnContinue;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.LinkLabel lnkCalinrationHelpLink;
		private System.Windows.Forms.CheckBox cbxNotShowAgain;

	}
}
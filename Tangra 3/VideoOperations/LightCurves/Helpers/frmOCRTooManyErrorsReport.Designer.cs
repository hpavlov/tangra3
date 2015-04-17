namespace Tangra.VideoOperations.LightCurves.Helpers
{
	partial class frmOCRTooManyErrorsReport
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
			this.tbxEmail = new System.Windows.Forms.TextBox();
			this.lblInfo = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.btnSend = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// tbxEmail
			// 
			this.tbxEmail.Location = new System.Drawing.Point(109, 92);
			this.tbxEmail.Name = "tbxEmail";
			this.tbxEmail.Size = new System.Drawing.Size(195, 20);
			this.tbxEmail.TabIndex = 0;
			this.tbxEmail.TextChanged += new System.EventHandler(this.tbxEmail_TextChanged);
			// 
			// lblInfo
			// 
			this.lblInfo.Location = new System.Drawing.Point(24, 22);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Size = new System.Drawing.Size(280, 53);
			this.lblInfo.TabIndex = 1;
			this.lblInfo.Text = "    There were {0} timestamps that failed OCR. To send an error report to the mai" +
    "ntainers of Tangra provide you email address and press \"Send\".";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(24, 95);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(79, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "E-mail Address:";
			// 
			// btnSend
			// 
			this.btnSend.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnSend.Enabled = false;
			this.btnSend.Location = new System.Drawing.Point(148, 147);
			this.btnSend.Name = "btnSend";
			this.btnSend.Size = new System.Drawing.Size(75, 23);
			this.btnSend.TabIndex = 3;
			this.btnSend.Text = "Send";
			this.btnSend.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(229, 147);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// frmOCRTooManyErrorsReport
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(332, 187);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnSend);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.lblInfo);
			this.Controls.Add(this.tbxEmail);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmOCRTooManyErrorsReport";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Too Many OCR Errors";
			this.Load += new System.EventHandler(this.frmOCRTooManyErrorsReport_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblInfo;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnSend;
		private System.Windows.Forms.Button btnCancel;
		protected internal System.Windows.Forms.TextBox tbxEmail;
	}
}
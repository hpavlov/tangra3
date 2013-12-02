namespace Tangra.VideoOperations
{
	partial class frmChooseOcrEngine
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChooseOcrEngine));
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.pnlOsdOcr = new System.Windows.Forms.Panel();
			this.cbxOcrAskEveryTime = new System.Windows.Forms.CheckBox();
			this.cbxOcrEngine = new System.Windows.Forms.ComboBox();
			this.cbxEnableOsdOcr = new System.Windows.Forms.CheckBox();
			this.cbxForceErrorReport = new System.Windows.Forms.CheckBox();
			this.pnlOsdOcr.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(29, 118);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 50;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(127, 118);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 51;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// pnlOsdOcr
			// 
			this.pnlOsdOcr.Controls.Add(this.cbxOcrAskEveryTime);
			this.pnlOsdOcr.Controls.Add(this.cbxOcrEngine);
			this.pnlOsdOcr.Location = new System.Drawing.Point(2, 35);
			this.pnlOsdOcr.Name = "pnlOsdOcr";
			this.pnlOsdOcr.Size = new System.Drawing.Size(229, 81);
			this.pnlOsdOcr.TabIndex = 54;
			// 
			// cbxOcrAskEveryTime
			// 
			this.cbxOcrAskEveryTime.AutoSize = true;
			this.cbxOcrAskEveryTime.Location = new System.Drawing.Point(27, 41);
			this.cbxOcrAskEveryTime.Name = "cbxOcrAskEveryTime";
			this.cbxOcrAskEveryTime.Size = new System.Drawing.Size(196, 17);
			this.cbxOcrAskEveryTime.TabIndex = 44;
			this.cbxOcrAskEveryTime.Text = "Ask me every time I process a video";
			this.cbxOcrAskEveryTime.UseVisualStyleBackColor = true;
			// 
			// cbxOcrEngine
			// 
			this.cbxOcrEngine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxOcrEngine.Items.AddRange(new object[] {
            "IOTA-VTI"});
			this.cbxOcrEngine.Location = new System.Drawing.Point(27, 8);
			this.cbxOcrEngine.Name = "cbxOcrEngine";
			this.cbxOcrEngine.Size = new System.Drawing.Size(173, 21);
			this.cbxOcrEngine.TabIndex = 51;
			// 
			// cbxEnableOsdOcr
			// 
			this.cbxEnableOsdOcr.AutoSize = true;
			this.cbxEnableOsdOcr.Location = new System.Drawing.Point(12, 12);
			this.cbxEnableOsdOcr.Name = "cbxEnableOsdOcr";
			this.cbxEnableOsdOcr.Size = new System.Drawing.Size(192, 17);
			this.cbxEnableOsdOcr.TabIndex = 53;
			this.cbxEnableOsdOcr.Text = "Read OSD timestamp automatically";
			this.cbxEnableOsdOcr.UseVisualStyleBackColor = true;
			this.cbxEnableOsdOcr.CheckedChanged += new System.EventHandler(this.cbxEnableOsdOcr_CheckedChanged);
			// 
			// cbxForceErrorReport
			// 
			this.cbxForceErrorReport.AutoSize = true;
			this.cbxForceErrorReport.Location = new System.Drawing.Point(29, 95);
			this.cbxForceErrorReport.Name = "cbxForceErrorReport";
			this.cbxForceErrorReport.Size = new System.Drawing.Size(175, 17);
			this.cbxForceErrorReport.TabIndex = 56;
			this.cbxForceErrorReport.Text = "Force produce OCR error report";
			this.cbxForceErrorReport.UseVisualStyleBackColor = true;
			// 
			// frmChooseOcrEngine
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(245, 154);
			this.Controls.Add(this.cbxForceErrorReport);
			this.Controls.Add(this.pnlOsdOcr);
			this.Controls.Add(this.cbxEnableOsdOcr);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmChooseOcrEngine";
			this.Text = "OSD Timestamp Reader";
			this.Load += new System.EventHandler(this.frmChooseOcrEngine_Load);
			this.pnlOsdOcr.ResumeLayout(false);
			this.pnlOsdOcr.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Panel pnlOsdOcr;
		private System.Windows.Forms.CheckBox cbxOcrAskEveryTime;
		private System.Windows.Forms.ComboBox cbxOcrEngine;
		private System.Windows.Forms.CheckBox cbxEnableOsdOcr;
		private System.Windows.Forms.CheckBox cbxForceErrorReport;
	}
}
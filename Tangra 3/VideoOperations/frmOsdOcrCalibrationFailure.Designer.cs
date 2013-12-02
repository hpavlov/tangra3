namespace Tangra.VideoOperations
{
	partial class frmOsdOcrCalibrationFailure
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOsdOcrCalibrationFailure));
			this.btnSendReport = new System.Windows.Forms.Button();
			this.btnIgnore = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label2 = new System.Windows.Forms.Label();
			this.lblOsdReaderType = new System.Windows.Forms.Label();
			this.pnlGenuineReport = new System.Windows.Forms.Panel();
			this.pnlForcedReport = new System.Windows.Forms.Panel();
			this.label3 = new System.Windows.Forms.Label();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.lblOsdReaderType2 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.pnlGenuineReport.SuspendLayout();
			this.pnlForcedReport.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			this.SuspendLayout();
			// 
			// btnSendReport
			// 
			this.btnSendReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSendReport.Location = new System.Drawing.Point(201, 219);
			this.btnSendReport.Name = "btnSendReport";
			this.btnSendReport.Size = new System.Drawing.Size(120, 23);
			this.btnSendReport.TabIndex = 0;
			this.btnSendReport.Text = "Send Error Report";
			this.btnSendReport.UseVisualStyleBackColor = true;
			this.btnSendReport.Click += new System.EventHandler(this.btnSendReport_Click);
			// 
			// btnIgnore
			// 
			this.btnIgnore.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnIgnore.Location = new System.Drawing.Point(92, 219);
			this.btnIgnore.Name = "btnIgnore";
			this.btnIgnore.Size = new System.Drawing.Size(75, 23);
			this.btnIgnore.TabIndex = 1;
			this.btnIgnore.Text = "Ignore";
			this.btnIgnore.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(40, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(373, 14);
			this.label1.TabIndex = 2;
			this.label1.Text = "Tangra couldn\'t read your timestamp based on the selected OSD reader type:";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(10, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(24, 23);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 3;
			this.pictureBox1.TabStop = false;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(13, 66);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(400, 135);
			this.label2.TabIndex = 4;
			this.label2.Text = resources.GetString("label2.Text");
			// 
			// lblOsdReaderType
			// 
			this.lblOsdReaderType.AutoSize = true;
			this.lblOsdReaderType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblOsdReaderType.Location = new System.Drawing.Point(44, 33);
			this.lblOsdReaderType.Name = "lblOsdReaderType";
			this.lblOsdReaderType.Size = new System.Drawing.Size(65, 13);
			this.lblOsdReaderType.TabIndex = 5;
			this.lblOsdReaderType.Text = "KIWI-OSD";
			// 
			// pnlGenuineReport
			// 
			this.pnlGenuineReport.Controls.Add(this.label1);
			this.pnlGenuineReport.Controls.Add(this.pictureBox1);
			this.pnlGenuineReport.Controls.Add(this.label2);
			this.pnlGenuineReport.Controls.Add(this.lblOsdReaderType);
			this.pnlGenuineReport.Location = new System.Drawing.Point(3, 3);
			this.pnlGenuineReport.Name = "pnlGenuineReport";
			this.pnlGenuineReport.Size = new System.Drawing.Size(414, 210);
			this.pnlGenuineReport.TabIndex = 6;
			// 
			// pnlForcedReport
			// 
			this.pnlForcedReport.Controls.Add(this.label5);
			this.pnlForcedReport.Controls.Add(this.label3);
			this.pnlForcedReport.Controls.Add(this.pictureBox2);
			this.pnlForcedReport.Controls.Add(this.lblOsdReaderType2);
			this.pnlForcedReport.Location = new System.Drawing.Point(422, 3);
			this.pnlForcedReport.Name = "pnlForcedReport";
			this.pnlForcedReport.Size = new System.Drawing.Size(428, 210);
			this.pnlForcedReport.TabIndex = 7;
			this.pnlForcedReport.Visible = false;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(39, 6);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(373, 14);
			this.label3.TabIndex = 8;
			this.label3.Text = "An OCR report has been requested using OSD reader type:";
			// 
			// pictureBox2
			// 
			this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
			this.pictureBox2.Location = new System.Drawing.Point(9, 0);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(24, 23);
			this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox2.TabIndex = 9;
			this.pictureBox2.TabStop = false;
			// 
			// lblOsdReaderType2
			// 
			this.lblOsdReaderType2.AutoSize = true;
			this.lblOsdReaderType2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblOsdReaderType2.Location = new System.Drawing.Point(39, 36);
			this.lblOsdReaderType2.Name = "lblOsdReaderType2";
			this.lblOsdReaderType2.Size = new System.Drawing.Size(65, 13);
			this.lblOsdReaderType2.TabIndex = 10;
			this.lblOsdReaderType2.Text = "KIWI-OSD";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(12, 66);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(400, 135);
			this.label5.TabIndex = 11;
			this.label5.Text = "Please submit the error report using the button below.";
			// 
			// frmOsdOcrCalibrationFailure
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(426, 260);
			this.Controls.Add(this.pnlForcedReport);
			this.Controls.Add(this.pnlGenuineReport);
			this.Controls.Add(this.btnIgnore);
			this.Controls.Add(this.btnSendReport);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmOsdOcrCalibrationFailure";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "OSD Reader Error";
			this.Load += new System.EventHandler(this.frmOsdOcrCalibrationFailure_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.pnlGenuineReport.ResumeLayout(false);
			this.pnlGenuineReport.PerformLayout();
			this.pnlForcedReport.ResumeLayout(false);
			this.pnlForcedReport.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnSendReport;
		private System.Windows.Forms.Button btnIgnore;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblOsdReaderType;
		private System.Windows.Forms.Panel pnlGenuineReport;
		private System.Windows.Forms.Panel pnlForcedReport;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.Label lblOsdReaderType2;
		private System.Windows.Forms.Label label5;
	}
}
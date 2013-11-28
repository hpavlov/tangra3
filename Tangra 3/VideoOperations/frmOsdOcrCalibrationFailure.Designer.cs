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
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
			this.label1.Location = new System.Drawing.Point(45, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(373, 14);
			this.label1.TabIndex = 2;
			this.label1.Text = "Tangra couldn\'t read your timestamp based on the selected OSD reader type:";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(15, 12);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(24, 23);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 3;
			this.pictureBox1.TabStop = false;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(18, 78);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(400, 135);
			this.label2.TabIndex = 4;
			this.label2.Text = resources.GetString("label2.Text");
			// 
			// lblOsdReaderType
			// 
			this.lblOsdReaderType.AutoSize = true;
			this.lblOsdReaderType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblOsdReaderType.Location = new System.Drawing.Point(49, 45);
			this.lblOsdReaderType.Name = "lblOsdReaderType";
			this.lblOsdReaderType.Size = new System.Drawing.Size(65, 13);
			this.lblOsdReaderType.TabIndex = 5;
			this.lblOsdReaderType.Text = "KIWI-OSD";
			// 
			// frmOsdOcrCalibrationFailure
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(430, 260);
			this.Controls.Add(this.btnIgnore);
			this.Controls.Add(this.btnSendReport);
			this.Controls.Add(this.lblOsdReaderType);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmOsdOcrCalibrationFailure";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "OSD Reader Error";
			this.Load += new System.EventHandler(this.frmOsdOcrCalibrationFailure_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnSendReport;
		private System.Windows.Forms.Button btnIgnore;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblOsdReaderType;
	}
}
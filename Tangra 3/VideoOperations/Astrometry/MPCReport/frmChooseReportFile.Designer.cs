namespace Tangra.VideoOperations.Astrometry.MPCReport
{
	partial class frmChooseReportFile
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
            this.lbxAvailabeReports = new System.Windows.Forms.ListBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.rb2Days = new System.Windows.Forms.RadioButton();
            this.rbLast7Days = new System.Windows.Forms.RadioButton();
            this.rbLast30Days = new System.Windows.Forms.RadioButton();
            this.rbAllFiles = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDeleteAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbxAvailabeReports
            // 
            this.lbxAvailabeReports.FormattingEnabled = true;
            this.lbxAvailabeReports.Location = new System.Drawing.Point(8, 12);
            this.lbxAvailabeReports.Name = "lbxAvailabeReports";
            this.lbxAvailabeReports.Size = new System.Drawing.Size(338, 238);
            this.lbxAvailabeReports.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(271, 295);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(190, 295);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // rb2Days
            // 
            this.rb2Days.AutoSize = true;
            this.rb2Days.Location = new System.Drawing.Point(97, 261);
            this.rb2Days.Name = "rb2Days";
            this.rb2Days.Size = new System.Drawing.Size(56, 17);
            this.rb2Days.TabIndex = 3;
            this.rb2Days.Tag = "7";
            this.rb2Days.Text = "7 days";
            this.rb2Days.UseVisualStyleBackColor = true;
            this.rb2Days.CheckedChanged += new System.EventHandler(this.FilterReportFilesByLastModifiedDate);
            // 
            // rbLast7Days
            // 
            this.rbLast7Days.AutoSize = true;
            this.rbLast7Days.Checked = true;
            this.rbLast7Days.Location = new System.Drawing.Point(159, 261);
            this.rbLast7Days.Name = "rbLast7Days";
            this.rbLast7Days.Size = new System.Drawing.Size(62, 17);
            this.rbLast7Days.TabIndex = 4;
            this.rbLast7Days.TabStop = true;
            this.rbLast7Days.Tag = "30";
            this.rbLast7Days.Text = "30 days";
            this.rbLast7Days.UseVisualStyleBackColor = true;
            this.rbLast7Days.CheckedChanged += new System.EventHandler(this.FilterReportFilesByLastModifiedDate);
            // 
            // rbLast30Days
            // 
            this.rbLast30Days.AutoSize = true;
            this.rbLast30Days.Location = new System.Drawing.Point(221, 261);
            this.rbLast30Days.Name = "rbLast30Days";
            this.rbLast30Days.Size = new System.Drawing.Size(62, 17);
            this.rbLast30Days.TabIndex = 5;
            this.rbLast30Days.Tag = "90";
            this.rbLast30Days.Text = "90 days";
            this.rbLast30Days.UseVisualStyleBackColor = true;
            this.rbLast30Days.CheckedChanged += new System.EventHandler(this.FilterReportFilesByLastModifiedDate);
            // 
            // rbAllFiles
            // 
            this.rbAllFiles.AutoSize = true;
            this.rbAllFiles.Location = new System.Drawing.Point(289, 261);
            this.rbAllFiles.Name = "rbAllFiles";
            this.rbAllFiles.Size = new System.Drawing.Size(57, 17);
            this.rbAllFiles.TabIndex = 6;
            this.rbAllFiles.Tag = "12000";
            this.rbAllFiles.Text = "All files";
            this.rbAllFiles.UseVisualStyleBackColor = true;
            this.rbAllFiles.CheckedChanged += new System.EventHandler(this.FilterReportFilesByLastModifiedDate);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 262);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "List files from last";
            // 
            // btnDeleteAll
            // 
            this.btnDeleteAll.Location = new System.Drawing.Point(8, 295);
            this.btnDeleteAll.Name = "btnDeleteAll";
            this.btnDeleteAll.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteAll.TabIndex = 8;
            this.btnDeleteAll.Text = "Delete All";
            this.btnDeleteAll.UseVisualStyleBackColor = true;
            this.btnDeleteAll.Click += new System.EventHandler(this.btnDeleteAll_Click);
            // 
            // frmChooseReportFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 328);
            this.Controls.Add(this.btnDeleteAll);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rbAllFiles);
            this.Controls.Add(this.rbLast30Days);
            this.Controls.Add(this.rbLast7Days);
            this.Controls.Add(this.rb2Days);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lbxAvailabeReports);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmChooseReportFile";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Choose Report File";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox lbxAvailabeReports;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.RadioButton rb2Days;
        private System.Windows.Forms.RadioButton rbLast7Days;
        private System.Windows.Forms.RadioButton rbLast30Days;
        private System.Windows.Forms.RadioButton rbAllFiles;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDeleteAll;
	}
}
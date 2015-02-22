namespace Tangra.VideoOperations.Astrometry
{
	partial class frmIdentifyObjects
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.pbar = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlProgress = new System.Windows.Forms.Panel();
            this.pnlEnterTime = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.dtTime = new System.Windows.Forms.DateTimePicker();
            this.dtDate = new System.Windows.Forms.DateTimePicker();
            this.pnlProgress.SuspendLayout();
            this.pnlEnterTime.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(253, 1);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // pbar
            // 
            this.pbar.Location = new System.Drawing.Point(3, 3);
            this.pbar.Name = "pbar";
            this.pbar.Size = new System.Drawing.Size(237, 15);
            this.pbar.TabIndex = 1;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(0, 22);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(144, 13);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Checking known asteroids ...";
            // 
            // pnlProgress
            // 
            this.pnlProgress.Controls.Add(this.pbar);
            this.pnlProgress.Controls.Add(this.lblStatus);
            this.pnlProgress.Controls.Add(this.btnCancel);
            this.pnlProgress.Location = new System.Drawing.Point(3, 3);
            this.pnlProgress.Name = "pnlProgress";
            this.pnlProgress.Size = new System.Drawing.Size(336, 40);
            this.pnlProgress.TabIndex = 3;
            // 
            // pnlEnterTime
            // 
            this.pnlEnterTime.Controls.Add(this.label1);
            this.pnlEnterTime.Controls.Add(this.btnSearch);
            this.pnlEnterTime.Controls.Add(this.dtTime);
            this.pnlEnterTime.Controls.Add(this.dtDate);
            this.pnlEnterTime.Location = new System.Drawing.Point(4, 3);
            this.pnlEnterTime.Name = "pnlEnterTime";
            this.pnlEnterTime.Size = new System.Drawing.Size(336, 40);
            this.pnlEnterTime.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 13);
            this.label1.TabIndex = 73;
            this.label1.Text = "Current video frame time:";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(253, 14);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 72;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // dtTime
            // 
            this.dtTime.CustomFormat = "HH:mm:ss UT";
            this.dtTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtTime.Location = new System.Drawing.Point(99, 17);
            this.dtTime.Name = "dtTime";
            this.dtTime.Size = new System.Drawing.Size(89, 20);
            this.dtTime.TabIndex = 71;
            // 
            // dtDate
            // 
            this.dtDate.CustomFormat = "dd MMM yyyy";
            this.dtDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDate.Location = new System.Drawing.Point(4, 17);
            this.dtDate.Name = "dtDate";
            this.dtDate.Size = new System.Drawing.Size(89, 20);
            this.dtDate.TabIndex = 70;
            // 
            // frmIdentifyObjects
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(341, 46);
            this.ControlBox = false;
            this.Controls.Add(this.pnlEnterTime);
            this.Controls.Add(this.pnlProgress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmIdentifyObjects";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Identifying Objects...";
            this.Load += new System.EventHandler(this.frmIdentifyObjects_Load);
            this.pnlProgress.ResumeLayout(false);
            this.pnlProgress.PerformLayout();
            this.pnlEnterTime.ResumeLayout(false);
            this.pnlEnterTime.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.ProgressBar pbar;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.Panel pnlProgress;
		private System.Windows.Forms.Panel pnlEnterTime;
		private System.Windows.Forms.Button btnSearch;
		private System.Windows.Forms.DateTimePicker dtTime;
		private System.Windows.Forms.DateTimePicker dtDate;
		private System.Windows.Forms.Label label1;
	}
}
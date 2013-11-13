namespace OcrTester
{
	partial class frmMain
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
			this.tbxInputFolder = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btnPrev = new System.Windows.Forms.Button();
			this.btnNext = new System.Windows.Forms.Button();
			this.picField = new System.Windows.Forms.PictureBox();
			this.btnReload = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.picField)).BeginInit();
			this.SuspendLayout();
			// 
			// tbxInputFolder
			// 
			this.tbxInputFolder.Location = new System.Drawing.Point(13, 28);
			this.tbxInputFolder.Name = "tbxInputFolder";
			this.tbxInputFolder.Size = new System.Drawing.Size(352, 20);
			this.tbxInputFolder.TabIndex = 0;
			this.tbxInputFolder.Text = "D:\\Hristo\\Tangra3\\Test Data\\OcrTestImages";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(73, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Images Folder";
			// 
			// btnPrev
			// 
			this.btnPrev.Location = new System.Drawing.Point(233, 64);
			this.btnPrev.Name = "btnPrev";
			this.btnPrev.Size = new System.Drawing.Size(57, 23);
			this.btnPrev.TabIndex = 2;
			this.btnPrev.Text = "<";
			this.btnPrev.UseVisualStyleBackColor = true;
			this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
			// 
			// btnNext
			// 
			this.btnNext.Location = new System.Drawing.Point(308, 64);
			this.btnNext.Name = "btnNext";
			this.btnNext.Size = new System.Drawing.Size(57, 23);
			this.btnNext.TabIndex = 3;
			this.btnNext.Text = ">";
			this.btnNext.UseVisualStyleBackColor = true;
			this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
			// 
			// picField
			// 
			this.picField.Cursor = System.Windows.Forms.Cursors.Hand;
			this.picField.Location = new System.Drawing.Point(16, 113);
			this.picField.Name = "picField";
			this.picField.Size = new System.Drawing.Size(738, 101);
			this.picField.TabIndex = 4;
			this.picField.TabStop = false;
			this.picField.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picField_MouseDown);
			// 
			// btnReload
			// 
			this.btnReload.Location = new System.Drawing.Point(16, 64);
			this.btnReload.Name = "btnReload";
			this.btnReload.Size = new System.Drawing.Size(81, 23);
			this.btnReload.TabIndex = 5;
			this.btnReload.Text = "Reload";
			this.btnReload.UseVisualStyleBackColor = true;
			this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(765, 372);
			this.Controls.Add(this.btnReload);
			this.Controls.Add(this.picField);
			this.Controls.Add(this.btnNext);
			this.Controls.Add(this.btnPrev);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.tbxInputFolder);
			this.Name = "frmMain";
			this.Text = "OCR Tester";
			((System.ComponentModel.ISupportInitialize)(this.picField)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbxInputFolder;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnPrev;
		private System.Windows.Forms.Button btnNext;
		private System.Windows.Forms.PictureBox picField;
		private System.Windows.Forms.Button btnReload;
	}
}


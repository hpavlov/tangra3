namespace Tangra.Helpers
{
	partial class frmChooseFitsFolder
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnBrowseForFolder = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.openFitsSequenceDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.cbxFolderPath = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(299, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Path to the folder containing FITS exposures ordered by name";
            // 
            // btnBrowseForFolder
            // 
            this.btnBrowseForFolder.Location = new System.Drawing.Point(382, 31);
            this.btnBrowseForFolder.Name = "btnBrowseForFolder";
            this.btnBrowseForFolder.Size = new System.Drawing.Size(33, 23);
            this.btnBrowseForFolder.TabIndex = 2;
            this.btnBrowseForFolder.Text = "...";
            this.btnBrowseForFolder.UseVisualStyleBackColor = true;
            this.btnBrowseForFolder.Click += new System.EventHandler(this.btnBrowseForFolder_Click);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(127, 66);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(208, 66);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // openFitsSequenceDialog
            // 
            this.openFitsSequenceDialog.Description = "Choose a directory containing the FITS file sequence";
            this.openFitsSequenceDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.openFitsSequenceDialog.ShowNewFolderButton = false;
            // 
            // cbxFolderPath
            // 
            this.cbxFolderPath.FormattingEnabled = true;
            this.cbxFolderPath.Location = new System.Drawing.Point(12, 33);
            this.cbxFolderPath.Name = "cbxFolderPath";
            this.cbxFolderPath.Size = new System.Drawing.Size(362, 21);
            this.cbxFolderPath.TabIndex = 5;
            // 
            // frmChooseFitsFolder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(433, 101);
            this.Controls.Add(this.cbxFolderPath);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnBrowseForFolder);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmChooseFitsFolder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Open Sequence of FITS Files";
            this.Load += new System.EventHandler(this.frmChooseFitsFolder_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnBrowseForFolder;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.FolderBrowserDialog openFitsSequenceDialog;
        private System.Windows.Forms.ComboBox cbxFolderPath;
	}
}
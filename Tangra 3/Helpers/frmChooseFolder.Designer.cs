namespace Tangra.Helpers
{
    partial class frmChooseFolder
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnBrowseForFolder = new System.Windows.Forms.Button();
            this.lblSelectionTitle = new System.Windows.Forms.Label();
            this.browseFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.tbxFolderPath = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(248, 66);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(167, 66);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnBrowseForFolder
            // 
            this.btnBrowseForFolder.Location = new System.Drawing.Point(467, 33);
            this.btnBrowseForFolder.Name = "btnBrowseForFolder";
            this.btnBrowseForFolder.Size = new System.Drawing.Size(33, 23);
            this.btnBrowseForFolder.TabIndex = 7;
            this.btnBrowseForFolder.Text = "...";
            this.btnBrowseForFolder.UseVisualStyleBackColor = true;
            this.btnBrowseForFolder.Click += new System.EventHandler(this.btnBrowseForFolder_Click);
            // 
            // lblSelectionTitle
            // 
            this.lblSelectionTitle.AutoSize = true;
            this.lblSelectionTitle.Location = new System.Drawing.Point(13, 12);
            this.lblSelectionTitle.Name = "lblSelectionTitle";
            this.lblSelectionTitle.Size = new System.Drawing.Size(66, 13);
            this.lblSelectionTitle.TabIndex = 6;
            this.lblSelectionTitle.Text = "Select folder";
            // 
            // browseFolderDialog
            // 
            this.browseFolderDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // tbxFolderPath
            // 
            this.tbxFolderPath.Location = new System.Drawing.Point(16, 35);
            this.tbxFolderPath.Name = "tbxFolderPath";
            this.tbxFolderPath.Size = new System.Drawing.Size(445, 20);
            this.tbxFolderPath.TabIndex = 11;
            // 
            // frmChooseFolder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 103);
            this.Controls.Add(this.tbxFolderPath);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnBrowseForFolder);
            this.Controls.Add(this.lblSelectionTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmChooseFolder";
            this.Text = "Choose Folder";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnBrowseForFolder;
        private System.Windows.Forms.Label lblSelectionTitle;
        private System.Windows.Forms.FolderBrowserDialog browseFolderDialog;
        private System.Windows.Forms.TextBox tbxFolderPath;
    }
}
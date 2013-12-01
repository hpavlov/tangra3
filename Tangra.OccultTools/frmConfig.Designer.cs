namespace Tangra.OccultTools
{
    partial class frmConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConfig));
            this.btnOK = new System.Windows.Forms.Button();
            this.butLocateExec = new System.Windows.Forms.Button();
            this.tbxOccultPath = new System.Windows.Forms.TextBox();
            this.lblC2awPath = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(15, 127);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // butLocateExec
            // 
            this.butLocateExec.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.butLocateExec.Location = new System.Drawing.Point(245, 29);
            this.butLocateExec.Name = "butLocateExec";
            this.butLocateExec.Size = new System.Drawing.Size(28, 20);
            this.butLocateExec.TabIndex = 5;
            this.butLocateExec.Text = "...";
            this.butLocateExec.UseVisualStyleBackColor = true;
            this.butLocateExec.Click += new System.EventHandler(this.butLocateExec_Click);
            // 
            // tbxOccultPath
            // 
            this.tbxOccultPath.Location = new System.Drawing.Point(15, 29);
            this.tbxOccultPath.Name = "tbxOccultPath";
            this.tbxOccultPath.Size = new System.Drawing.Size(230, 20);
            this.tbxOccultPath.TabIndex = 3;
            // 
            // lblC2awPath
            // 
            this.lblC2awPath.Location = new System.Drawing.Point(12, 9);
            this.lblC2awPath.Name = "lblC2awPath";
            this.lblC2awPath.Size = new System.Drawing.Size(78, 17);
            this.lblC2awPath.TabIndex = 4;
            this.lblC2awPath.Text = "Occult4 Path:";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(96, 127);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(261, 51);
            this.label1.TabIndex = 7;
            this.label1.Text = "IMPORTANT: While the Occult Tools for Tangra addin is loaded you will not be able" +
    " to update Occult. You will need to stop Tangra before updating Occult.";
            // 
            // frmConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(290, 162);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.butLocateExec);
            this.Controls.Add(this.tbxOccultPath);
            this.Controls.Add(this.lblC2awPath);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Occult Tools for Tangra";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
		
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button butLocateExec;
        internal System.Windows.Forms.TextBox tbxOccultPath;
        private System.Windows.Forms.Label lblC2awPath;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label label1;
    }
}
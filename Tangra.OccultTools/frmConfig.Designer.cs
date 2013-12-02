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
			this.btnCancel = new System.Windows.Forms.Button();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.llOccult4 = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(15, 166);
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
			this.butLocateExec.Location = new System.Drawing.Point(268, 60);
			this.butLocateExec.Name = "butLocateExec";
			this.butLocateExec.Size = new System.Drawing.Size(28, 20);
			this.butLocateExec.TabIndex = 5;
			this.butLocateExec.Text = "...";
			this.butLocateExec.UseVisualStyleBackColor = true;
			this.butLocateExec.Click += new System.EventHandler(this.butLocateExec_Click);
			// 
			// tbxOccultPath
			// 
			this.tbxOccultPath.Location = new System.Drawing.Point(15, 60);
			this.tbxOccultPath.Name = "tbxOccultPath";
			this.tbxOccultPath.Size = new System.Drawing.Size(247, 20);
			this.tbxOccultPath.TabIndex = 3;
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(96, 166);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 101);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(284, 51);
			this.label1.TabIndex = 7;
			this.label1.Text = "IMPORTANT: While the Occult Tools for Tangra addin is loaded you will not be able" +
    " to update Occult. You will need to stop Tangra before updating Occult.";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(295, 31);
			this.label2.TabIndex = 8;
			this.label2.Text = "You need to have Occult4  installed to use this functionality. Please configure t" +
    "he Occult4 location below:";
			// 
			// llOccult4
			// 
			this.llOccult4.AutoSize = true;
			this.llOccult4.Location = new System.Drawing.Point(101, 9);
			this.llOccult4.Name = "llOccult4";
			this.llOccult4.Size = new System.Drawing.Size(44, 13);
			this.llOccult4.TabIndex = 9;
			this.llOccult4.TabStop = true;
			this.llOccult4.Text = "Occult4";
			this.llOccult4.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llOccult4_LinkClicked);
			// 
			// frmConfig
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(334, 204);
			this.Controls.Add(this.llOccult4);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.butLocateExec);
			this.Controls.Add(this.tbxOccultPath);
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
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.LinkLabel llOccult4;
    }
}
namespace Tangra
{
    partial class frmUnhandledException
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUnhandledException));
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.tbxUserComments = new System.Windows.Forms.TextBox();
            this.lblDetails = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(248, 246);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(391, 59);
            this.label1.TabIndex = 1;
            this.label1.Text = "    An unhandled error has occured. This is most likely caused by a bug in Tangra" +
                ". To help improve the program please submit this error report to Hristo Pavlov b" +
                "y pressing the \'Submit\' button below.";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(329, 246);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 23);
            this.btnSubmit.TabIndex = 2;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // tbxUserComments
            // 
            this.tbxUserComments.Location = new System.Drawing.Point(16, 109);
            this.tbxUserComments.Multiline = true;
            this.tbxUserComments.Name = "tbxUserComments";
            this.tbxUserComments.Size = new System.Drawing.Size(388, 127);
            this.tbxUserComments.TabIndex = 3;
            // 
            // lblDetails
            // 
            this.lblDetails.AutoSize = true;
            this.lblDetails.Location = new System.Drawing.Point(16, 251);
            this.lblDetails.Name = "lblDetails";
            this.lblDetails.Size = new System.Drawing.Size(176, 13);
            this.lblDetails.TabIndex = 4;
            this.lblDetails.TabStop = true;
            this.lblDetails.Text = "What does this error report contain?";
            this.lblDetails.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblDetails_LinkClicked);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(388, 41);
            this.label2.TabIndex = 5;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // frmUnhandledException
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 283);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblDetails);
            this.Controls.Add(this.tbxUserComments);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmUnhandledException";
            this.Text = "Unhandled Error";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmUnhandledException_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.TextBox tbxUserComments;
        private System.Windows.Forms.LinkLabel lblDetails;
        private System.Windows.Forms.Label label2;
    }
}
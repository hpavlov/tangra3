namespace Tangra.Config
{
	partial class frmInstDelayConfigChooser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmInstDelayConfigChooser));
            this.cbxCameras = new System.Windows.Forms.ComboBox();
            this.rbDontCorrect = new System.Windows.Forms.RadioButton();
            this.rbCorrect = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.llDelaysRef = new System.Windows.Forms.LinkLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbxCameras
            // 
            this.cbxCameras.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxCameras.Enabled = false;
            this.cbxCameras.FormattingEnabled = true;
            this.cbxCameras.Location = new System.Drawing.Point(38, 88);
            this.cbxCameras.Name = "cbxCameras";
            this.cbxCameras.Size = new System.Drawing.Size(240, 21);
            this.cbxCameras.TabIndex = 0;
            this.cbxCameras.SelectedIndexChanged += new System.EventHandler(this.cbxCameras_SelectedIndexChanged);
            // 
            // rbDontCorrect
            // 
            this.rbDontCorrect.AutoSize = true;
            this.rbDontCorrect.Checked = true;
            this.rbDontCorrect.Location = new System.Drawing.Point(20, 19);
            this.rbDontCorrect.Name = "rbDontCorrect";
            this.rbDontCorrect.Size = new System.Drawing.Size(255, 17);
            this.rbDontCorrect.TabIndex = 1;
            this.rbDontCorrect.TabStop = true;
            this.rbDontCorrect.Text = "Don\'t apply automatic corrections. I\'ll do it myself.";
            this.rbDontCorrect.UseVisualStyleBackColor = true;
            // 
            // rbCorrect
            // 
            this.rbCorrect.AutoSize = true;
            this.rbCorrect.Location = new System.Drawing.Point(20, 56);
            this.rbCorrect.Name = "rbCorrect";
            this.rbCorrect.Size = new System.Drawing.Size(124, 17);
            this.rbCorrect.TabIndex = 2;
            this.rbCorrect.Text = "Apply corrections for ";
            this.rbCorrect.UseVisualStyleBackColor = true;
            this.rbCorrect.CheckedChanged += new System.EventHandler(this.rbCorrect_CheckedChanged);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(158, 163);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(239, 163);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // llDelaysRef
            // 
            this.llDelaysRef.AutoSize = true;
            this.llDelaysRef.Location = new System.Drawing.Point(95, 112);
            this.llDelaysRef.Name = "llDelaysRef";
            this.llDelaysRef.Size = new System.Drawing.Size(183, 13);
            this.llDelaysRef.TabIndex = 5;
            this.llDelaysRef.TabStop = true;
            this.llDelaysRef.Text = "Instrumental delays by Gerhard Dangl";
            this.llDelaysRef.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbDontCorrect);
            this.groupBox1.Controls.Add(this.llDelaysRef);
            this.groupBox1.Controls.Add(this.cbxCameras);
            this.groupBox1.Controls.Add(this.rbCorrect);
            this.groupBox1.Location = new System.Drawing.Point(12, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(302, 141);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            // 
            // frmInstDelayConfigChooser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(327, 196);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmInstDelayConfigChooser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Correct for Instrumental Delay";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ComboBox cbxCameras;
		private System.Windows.Forms.RadioButton rbDontCorrect;
		private System.Windows.Forms.RadioButton rbCorrect;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.LinkLabel llDelaysRef;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}
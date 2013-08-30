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
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.rbDontCorrect = new System.Windows.Forms.RadioButton();
			this.rbCorrect = new System.Windows.Forms.RadioButton();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// comboBox1
			// 
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(44, 95);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(201, 21);
			this.comboBox1.TabIndex = 0;
			// 
			// rbDontCorrect
			// 
			this.rbDontCorrect.AutoSize = true;
			this.rbDontCorrect.Checked = true;
			this.rbDontCorrect.Location = new System.Drawing.Point(26, 26);
			this.rbDontCorrect.Name = "rbDontCorrect";
			this.rbDontCorrect.Size = new System.Drawing.Size(206, 17);
			this.rbDontCorrect.TabIndex = 1;
			this.rbDontCorrect.TabStop = true;
			this.rbDontCorrect.Text = "Don\'t apply corrections. I\'ll do it myself.";
			this.rbDontCorrect.UseVisualStyleBackColor = true;
			// 
			// rbCorrect
			// 
			this.rbCorrect.AutoSize = true;
			this.rbCorrect.Location = new System.Drawing.Point(26, 63);
			this.rbCorrect.Name = "rbCorrect";
			this.rbCorrect.Size = new System.Drawing.Size(124, 17);
			this.rbCorrect.TabIndex = 2;
			this.rbCorrect.Text = "Apply corrections for ";
			this.rbCorrect.UseVisualStyleBackColor = true;
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(89, 137);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(170, 137);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// frmInstDelayConfigChooser
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(269, 175);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.rbCorrect);
			this.Controls.Add(this.rbDontCorrect);
			this.Controls.Add(this.comboBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmInstDelayConfigChooser";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Correct for Instrumental Delay";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.RadioButton rbDontCorrect;
		private System.Windows.Forms.RadioButton rbCorrect;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
	}
}
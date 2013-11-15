namespace Tangra.VideoOperations
{
	partial class frmChooseOcrEngine
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChooseOcrEngine));
			this.cbxOcrEngine = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.cbxDontAsk = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// cbxOcrEngine
			// 
			this.cbxOcrEngine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxOcrEngine.Items.AddRange(new object[] {
            "IOTA-VTI Non TV-Safe",
            "IOTA-VTI TV-Safe"});
			this.cbxOcrEngine.Location = new System.Drawing.Point(13, 29);
			this.cbxOcrEngine.Name = "cbxOcrEngine";
			this.cbxOcrEngine.Size = new System.Drawing.Size(190, 21);
			this.cbxOcrEngine.TabIndex = 49;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(10, 13);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(135, 13);
			this.label3.TabIndex = 48;
			this.label3.Text = "Read OSD timestamp from:";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(10, 117);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 50;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(91, 117);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 51;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// cbxDontAsk
			// 
			this.cbxDontAsk.AutoSize = true;
			this.cbxDontAsk.Location = new System.Drawing.Point(12, 72);
			this.cbxDontAsk.Name = "cbxDontAsk";
			this.cbxDontAsk.Size = new System.Drawing.Size(139, 17);
			this.cbxDontAsk.TabIndex = 52;
			this.cbxDontAsk.Text = "Don\'t ask me every time";
			this.cbxDontAsk.UseVisualStyleBackColor = true;
			// 
			// frmChooseOcrEngine
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(225, 148);
			this.Controls.Add(this.cbxDontAsk);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.cbxOcrEngine);
			this.Controls.Add(this.label3);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmChooseOcrEngine";
			this.Text = "Select OSD Timestamp Reader";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox cbxOcrEngine;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.CheckBox cbxDontAsk;
	}
}
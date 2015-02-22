namespace Tangra.VideoOperations.Astrometry
{
	partial class ucFrameInterval
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lblSTNDRDTH = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.cbxEveryFrame = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// lblSTNDRDTH
			// 
			this.lblSTNDRDTH.AutoSize = true;
			this.lblSTNDRDTH.Location = new System.Drawing.Point(144, 7);
			this.lblSTNDRDTH.Name = "lblSTNDRDTH";
			this.lblSTNDRDTH.Size = new System.Drawing.Size(33, 13);
			this.lblSTNDRDTH.TabIndex = 35;
			this.lblSTNDRDTH.Text = "frame";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(4, 6);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(48, 13);
			this.label3.TabIndex = 34;
			this.label3.Text = "Measure";
			// 
			// cbxEveryFrame
			// 
			this.cbxEveryFrame.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxEveryFrame.FormattingEnabled = true;
			this.cbxEveryFrame.Items.AddRange(new object[] {
            "every",
            "every   2-nd",
            "every   4-th",
            "every   8-th",
            "every 16-th",
            "every 32-th",
            "every 64-th"});
			this.cbxEveryFrame.Location = new System.Drawing.Point(58, 3);
			this.cbxEveryFrame.Name = "cbxEveryFrame";
			this.cbxEveryFrame.Size = new System.Drawing.Size(80, 21);
			this.cbxEveryFrame.TabIndex = 36;
			this.cbxEveryFrame.SelectedIndexChanged += new System.EventHandler(this.cbxEveryFrame_SelectedIndexChanged);
			// 
			// ucFrameInterval
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.cbxEveryFrame);
			this.Controls.Add(this.lblSTNDRDTH);
			this.Controls.Add(this.label3);
			this.Name = "ucFrameInterval";
			this.Size = new System.Drawing.Size(180, 29);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblSTNDRDTH;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox cbxEveryFrame;
	}
}
